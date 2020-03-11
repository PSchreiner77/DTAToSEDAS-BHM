Module Module1
    Dim Arguments() As String = Environment.GetCommandLineArgs
    'Dim SourceFileName As String = ""
    'Dim SourceFilePath As String = ""
    'Dim SourceFullPath As String = ""
    'Dim DestinationFileName As String = ""
    'Dim DestinationFilePath As String = ""
    'Dim DestinationFullPath As String = ""
    'Dim DeleteSourceFile As Boolean = False
    'Dim ShowErrorMessage As Boolean = True
    Dim FileDate As String = ""
    Dim INI As INIFile
    Dim Param As New Parameter  'Objekt mit Parameterwerten

    Sub Main()
        Dim ArgList As String = ""
        Dim test As Integer = 0

        '----------------------------------------------------------------------
        '-- Programm initialisieren und Config-Daten übernehmen.
        If Not InitProgram() Then
            Environment.Exit(0)
        End If

        '----------------------------------------------------------------------
        'Parameter prüfen
        If Not CheckParameters() Then
            Environment.Exit(0)
        End If

        '----------------------------------------------------------------------
        '-- Prüfen, ob Quellpfad und -dateiname vorhanden sind. Prüfen, ob Zielpfad und -dateiname prüfen.
        If Not CheckSource() Then
            Environment.Exit(0)
        End If

        Param.SetDestinationFullPath(Param.DestinationFilePath, Param.DestinationFileName)

        '----------------------------------------------------------------------
        '-- Start der Konvertierung (Klasse): Einlesen und auswerten der Quelldatei und schreiben in die SEDAS.DAT
        'Counter um 1 erhöhen
        Param.Counter = Param.Counter + 1

        'Datei konvertieren
        Dim conv As New ConvertDATtoSEDAS(Param.SourceFullPath, Param.DestinationFullPath, Param.Counter)
        If conv.ConvertFile() Then
            ShowMessage("Sedas.dat erstellt.")
        Else
            ShowMessage("FEHLER! Fehler beim Konvertieren/Erstellen der Sedas.dat.", True)
            Environment.Exit(0)
        End If

        'Counter in INI zurückspeichern
        'TODO Wert in Ini zurückschreiben
        If INI.Write("Setup", "Counter", CStr(Param.Counter)) = False Then
            ShowMessage("FEHLER! Zähler konnte nicht in Config.ini zurückgeschrieben werden!" & vbCrLf &
                        "Weitere Programmausführung nicht möglich!", False, True)
            Environment.Exit(0)
        End If

        '----------------------------------------------------------------------
        '-- Quelldatei gegebenenfalls löschen
        If Param.DeleteSourceFile Then
            System.IO.File.Delete(Param.SourceFullPath)
        End If

    End Sub



    ''' <summary>
    ''' Prüfen ob Config.ini existiert und einlesen der INI-Werte. Wenn nicht existiert, neu anlegen. Gibt TRE zurück, wenn Auswertung ok.
    ''' </summary>
    ''' <returns></returns>
    Public Function InitProgram() As Boolean
        Dim ExitFunction As Boolean = False
        'Dim Message As String = ""
        InitProgram = True

        '----------------------------------------------------------------------
        'Prüfenund einlesen der Config.ini Datei.
        If System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory & "\config.ini") Then
            'CONFIG.INI existiert und wird ausgelesen.
            INI = New INIFile(IO.Directory.GetCurrentDirectory & "\Config.ini")
            'Werte aus INI übernehmen.
            If Not ReadIniValues() Then
                'wenn fehler beim Auslesen, Abbruch
                ShowMessage("Fehler beim Auslesen der Config.ini. Bitte Einstellungen prüfen. Programm wird beendet.", False, True)
                InitProgram = False
            End If
        Else
            '----------------------------------------------------------------------
            'Standard Config.ini erstellen
            ShowMessage("! Es wurde keine Config.ini Datei gefunden. Es wird eine neue Config.ini mit " & vbCrLf &
                        "  Standardeinstellungen erstellt.", True, True)
            If Not CreateNewConfigIni() Then
                ShowMessage("FEHLER! Eine neue Config.ini konnte nicht erstellt werden. Programm wird beendet.", False, True)
            Else
                'Hinweis auf Konfiguration geben
                ShowMessage("! Die Datei 'Config.ini' hat nicht existiert und wurde neu angelegt." & vbCrLf &
                            "  Bitte die Einstellungen kontrollieren, bevor das Programm erneut ausgeführt wird.", False, True)
            End If
            InitProgram = False
        End If

    End Function

    ''' <summary>
    ''' Liest die Werte der Ini-Datei in eine Parameterklasse ein.
    ''' </summary>
    ''' <returns></returns>
    Function ReadIniValues() As Boolean

        ReadIniValues = True
        Try
            Param.SourceFileName = INI.Read("Setup", "Quelldateiname")
            Param.SourceFilePath = INI.Read("Setup", "Quelldateipfad")
            If Param.SourceFilePath <> "" Then
                If Mid(Param.SourceFilePath, Len(Param.SourceFilePath), 1) <> "\" Then Param.SourceFilePath = Param.SourceFilePath & "\"
            Else
                Param.SourceFilePath = IO.Directory.GetCurrentDirectory & "\"
            End If
            'Param.SetSourceFullPath(SourceFilePath, SourceFileName)

            Param.DestinationFileName = INI.Read("Setup", "Zieldateiname")
            Param.DestinationFilePath = INI.Read("Setup", "Zieldateipfad")
            If Param.DestinationFilePath <> "" Then
                If Mid(Param.DestinationFilePath, Len(Param.DestinationFilePath), 1) <> "\" Then Param.DestinationFilePath = Param.DestinationFilePath & "\"
            Else
                Param.DestinationFilePath = System.IO.Directory.GetCurrentDirectory & "\"
            End If
            If Param.DestinationFileName = "" And Param.DestinationFilePath <> "" Then Param.DestinationFileName = "SEDAS.DAT"
            'Param.SetDestinationFullPath(DestinationFilePath, DestinationFileName)


            Param.DeleteSourceFile = CBool(INI.Read("Setup", "QuelleLöschen"))
            Param.IgnoreMessages = CBool(INI.Read("Setup", "IgnoriereMeldungen"))
            Param.Counter = CInt(INI.Read("Setup", "Counter"))
            'TODO Param.Append = CBool(INI.Read("Setup", "DatenAnhängen"))

        Catch ex As Exception
            ReadIniValues = False
        End Try

    End Function

    ''' <summary>
    ''' Erstellt eine neue Config.ini mit Standardeinstellungen.
    ''' </summary>
    ''' <returns></returns>
    Function CreateNewConfigIni() As Boolean
        CreateNewConfigIni = True
        '----------------------------------------------------------------------
        'Standard Config.ini erstellen.
        Dim ConfigText As String = ""
        ConfigText = "-----------------------" & vbCrLf &
                     "DATtoSEDAS Config-Datei" & vbCrLf &
                     "-----------------------" & vbCrLf &
                     "Quell- und Zielpfad müssen mit Laufwerksbuchstabe angegeben werden (vollständig), jedoch ohne Dateiname." & vbCrLf &
                     "Der Dateiname der Quell- und Zieldatei wird separat eingetragen." & vbCrLf &
                     "Werden Quell- und Zieldateiname beim Programmstart per Schalter übergeben (/Q=, /Z=), werden die Einträge" & vbCrLf &
                     "in der Config.ini übergangen." & vbCrLf &
                     "Dies gilt auch für alle weiteren Schalter (z.B. QuelleLöschen, /D)" & vbCrLf &
                     "" & vbCrLf &
                     "[Setup]" & vbCrLf &
                     "Counter=" & vbCrLf &
                     "Quelldateipfad=" & vbCrLf &
                     "Quelldateiname=1.txt" & vbCrLf &
                     "Zieldateipfad=C:\Temp" & vbCrLf &
                     "Zieldateiname=Sedas.dat" & vbCrLf &
                     "" & vbCrLf &
                     "QuelleLöschen=0" & vbCrLf &
                     "IgnoriereMeldungen=0" & vbCrLf &
                     "DatenAnhängen=0"

        '----------------------------------------------------------------------
        Try

            Using sw As New System.IO.StreamWriter(System.IO.Directory.GetCurrentDirectory & "\Config.ini")
                sw.WriteLine(ConfigText)
            End Using
        Catch ex As Exception
            CreateNewConfigIni = False
        End Try

        Return CreateNewConfigIni

    End Function

    ''' <summary>
    ''' Prüft, ob keine unerlaubten Parameter übergeben wurden. Rückgabewerte: -1=Keine Parameter, 0=Parameter fehlerhaft, 1=Parameter OK. 
    ''' Wird ein /? als Parameter mitgegeben, 
    ''' </summary>
    ''' <returns>Integer: -1=Keine Parameter, 0=Parameter fehlerhaft, 1=Parameter OK</returns>
    Public Function CheckParameters() As Boolean
        CheckParameters = True

        '-----------------------------------------------------------------------------------------
        'Keine Parameter übergeben. Funktion Ende.
        If Arguments.GetUpperBound(0) < 1 Then
            'Keine Parameter übergeben
            Return CheckParameters
        End If

        '-----------------------------------------------------------------------------------------
        'Wenn nur ein Element übergeben wurde, prüfen, ob es /? ist.
        If Arguments.GetUpperBound(0) = 1 Then
            If InStr(Arguments(1), "/?") > 0 Then
                Param.Help = True
                ShowHelp()
                CheckParameters = False
                Return CheckParameters
            End If
        End If

        '-----------------------------------------------------------------------------------------
        'Prüfen, ob mehr als 1 Argument übergeben und im Array /? vorkommt
        'Wenn ja, dann Parameter falsch.
        If Arguments.GetUpperBound(0) > 1 And Array.Exists(Arguments, Function(element)
                                                                          Return element.Contains("/?")
                                                                      End Function) Then
            ShowMessage("FEHLER! Falsche Startparameter angegeben. '/?' darf nur alleine verwendet werden.", False)
            CheckParameters = False
            Return CheckParameters
        End If

        '-----------------------------------------------------------------------------------------
        '## Parameter auswerten.
        Dim erg As Integer = 0
        For i = 1 To Arguments.GetUpperBound(0)
            Dim Switch As String = Arguments(i)

            'Parameter ermitteln
            If InStr(Arguments(i), "=") > 0 Then        'Prüfen, ob der Parameter ein = enthält
                Switch = Mid(Arguments(i), 1, InStr(Arguments(i), "="))
            Else
                Switch = Arguments(i)
            End If

            'Parameter prüfen und Parameterwert setzen
            Select Case Switch.ToUpper

                Case = "/Q="
                    'Prüfen, ob Eintrag aus Pfad und/oder Dateiname besteht
                    If InStr(Mid(Arguments(i), InStr(Arguments(i), "=") + 1), "\") > 0 Then
                        'Pfad enthalten.
                        If InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\") + 1), ".") > 0 Then
                            'Dateiname enthalten: Eintrag als kompletten Pfad übernehmen.
                            Param.SetSourceFullPath(Mid(Arguments(i), InStr(Arguments(i), "=") + 1))
                        Else
                            'Kein Dateiname enthalten: Eintrag als nur Pfad übernehmen.
                            Param.SourceFilePath = Mid(Arguments(i), InStr(Arguments(i), "=") + 1)
                        End If
                    Else
                        'Kein Pfad enthalten
                        If InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\") + 1), ".") > 0 Then
                            'Dateiname enthalten: Eintrag als Dateiname übernehmen.
                            Param.SourceFileName = Mid(Arguments(i), InStr(Arguments(i), "=") + 1)
                        End If
                    End If

                Case = "/Z="
                    If InStr(Mid(Arguments(i), InStr(Arguments(i), "=") + 1), "\") > 0 Then
                        'Pfad enthalten.
                        If InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\") + 1), ".") > 0 Then
                            'Dateiname enthalten: Eintrag als kompletten Pfad übernehmen.
                            Param.SetDestinationFullPath(Mid(Arguments(i), InStr(Arguments(i), "=") + 1))
                        Else
                            'Kein Dateiname enthalten: Eintrag als nur Pfad übernehmen.
                            Param.DestinationFilePath = Mid(Arguments(i), InStr(Arguments(i), "=") + 1)
                        End If
                    Else
                        'Kein Pfad enthalten
                        If InStr(Mid(Arguments(i), InStrRev(Arguments(i), "\") + 1), ".") > 0 Then
                            'Dateiname enthalten: Eintrag als Dateiname übernehmen.
                            Param.DestinationFileName = Mid(Arguments(i), InStr(Arguments(i), "=") + 1)
                        End If
                    End If


                Case = "/D"
                        Param.DeleteSourceFile = True
                    'erg = erg + 1

                Case = "/I"
                    'Fehlermeldungen unterdrücken
                    Param.IgnoreMessages = True

                Case = "/A"
                    'Fehlermeldungen unterdrücken
                    Param.Append = True

                Case Else
                    ShowMessage("FEHLER! Falsche Startparameter angegeben.", False)
                    CheckParameters = False
                    Return CheckParameters
            End Select
        Next
        Return CheckParameters

    End Function

    ''' <summary>
    ''' Prüft, ob ein Quell- und/oder Zielpfad angegeben wurden.
    ''' </summary>
    ''' <returns></returns>
    Function CheckSource() As Boolean
        CheckSource = True

        '----------------------------------------------------------------------
        'Quellpfad prüfen
        Param.SetSourceFullPath(Param.SourceFilePath, Param.SourceFileName)
        If Param.SourceFullPath = "" Then
            CheckSource = False
            ShowMessage("FEHLER! Es wurde keine Quelldatei angegeben.", False)
            Return CheckSource
        Else
            If Not IO.File.Exists(Param.SourceFullPath) Then
                CheckSource = False
                ShowMessage("FEHLER! Die Quelldatei existiert nicht oder ist nicht erreichbar.", False)
                Return CheckSource
            End If
        End If

        Return CheckSource
    End Function

    ''' <summary>
    ''' Lässt eine Konsolennachricht erscheinen und wartet ggf. auf einen Tastendruck.
    ''' </summary>
    ''' <param name="Message">Anzuzeigenden Nachricht.</param>
    ''' <param name="Ignorable">WENN TRUE kann die Anzeige der Nachricht bei entsprechendem Startparameter /I unterdrückt werden. 
    ''' FALSE zeigt die Nachricht in jedem Fall an. Standard: TRUE.</param>
    ''' <param name="Pause">Wenn TRUE, wird auf Tastendruck des Users gewartet. Standard: FALSE</param>
    Public Sub ShowMessage(Message As String, Optional Ignorable As Boolean = True, Optional Pause As Boolean = False)

        If Not Param.IgnoreMessages Or Ignorable Then
            Console.WriteLine(Message)
            If Pause Then
                Console.Write("<Enter> drücken...")
                Console.ReadLine()
            End If
        End If

    End Sub


    ''' <summary>
    ''' Zeigt die Hilfeseite des Programms auf der Konsole an.
    ''' </summary>
    Public Sub ShowHelp()
        Console.WriteLine()
        Console.WriteLine("DATtoSEDAS-Converter, Version " & My.Application.Info.Version.ToString)
        Console.WriteLine("Wandelt eine Bestell.dat in eine SEDAS.dat um, für den Import in CSB.")
        Console.WriteLine("Wird das Programm ohne Parameter gestartet, werden Pfad und Dateiinformationen")
        Console.WriteLine("aus der Datei Config.ini übernommen.")
        Console.WriteLine("Parameter, die mit Schaltern übergeben werden überschreiben Einstellungen ")
        Console.WriteLine("der Config.ini")
        Console.WriteLine()
        Console.WriteLine("DATtoSEDAS.exe [/Q=][Laufwerk:][Pfad][Dateiname] [/Z=][Laufwerk:][Pfad][Dateiname] [/D] [/I]")
        Console.WriteLine("               [/A] [/?]")
        Console.WriteLine("[Laufwerk:]  Laufwerksbuchstabe, z.B. C:")
        Console.WriteLine("[Pfad]       Verzeichnispfad")
        Console.WriteLine("[Dateiname]  Dateiname")
        Console.WriteLine()
        Console.WriteLine(" /Q=         Laufwerk/Pfad/Dateiname der Quelldatei.")
        Console.WriteLine("                Wird nur ein Dateiname angegeben, wird dieser im Programm-")
        Console.WriteLine("                Verzeichnis gesucht.")
        Console.WriteLine(" /Z=         Laufwerk/Pfad/Dateiname der Zieldatei." & vbCrLf &
                          "                Wird kein Zielpfad angegeben,wird die " & vbCrLf &
                          "                Zieldatei im Programmverzeichnis abgelegt." & vbCrLf &
                          "              ! Existiert eine gleichnamige Zieldatei bereits, wird")
        Console.WriteLine("                sie ohne Rückfrage überschrieben!")
        Console.WriteLine(" /D          Quelldatei wird nach der Konvertierung gelöscht.")
        Console.WriteLine(" /I          Statusmeldungen werden nicht angezeigt (jedoch Fehlermeldungen!).")
        'Console.WriteLine(" /A          Daten der Quelldatei an bestehende Zieldatei anhängen.")
        Console.WriteLine(" /?          Ruft diese Hilfeseite auf.")
        Console.WriteLine()
        Console.WriteLine("Beispiel:")
        Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /Z=D:\Import\Sedas.dat /D /I")
        Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen, die Quelldatei wird anschließend gelöscht.")
        Console.WriteLine("   Statusmeldungen werden nicht ausgegeben.")
        Console.WriteLine()
        Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /I")
        Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen. Statusmeldungen werden nicht ausgegeben.")
        Console.WriteLine()
        Console.Write("<Enter> drücken...")
        Console.ReadLine()

    End Sub
End Module
