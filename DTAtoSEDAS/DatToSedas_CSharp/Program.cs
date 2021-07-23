using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    class Program
    {

        private string[] Arguments = new Environment.GetCommandLineArgs(); //CommandlineArgs besorgen

        public string FileDate = "";

        private INIFile INI;

        public Parameter Param = new Parameter();

        public List<string> ListDelCustomer;

        public List<string> ListDelArticle;

        public List<string> ListChangeArticle;

        //TODO ??
        //< STAThread() >
        public void Main()
        {
            LogMessage.GlobalLog = true;
            LogMessage.GlobalOutputToConsole = true;
            LogMessage.LogOnly("**********************************");
            LogMessage.LogOnly("--------- PROGRAMMSTART ----------");
            LogMessage.CheckLogFile(100);
            Dim flag As Boolean = Not Module1.InitProgram();
            If flag Then
                LogMessage.LogOnly("Initialisierung des Programms fehlgeschlagen.");
            Module1.ExitProgram();
            End If
            LogMessage.LogOnly("Initialisierung des Programms erfolgreich.");
            Dim flag2 As Boolean = Not Module1.CheckParameters();
            If flag2 Then
                LogMessage.LogOnly("Prüfung der Parameter fehlgeschlagen.");
            Module1.ExitProgram();
            End If
            LogMessage.LogOnly("Prüfung der Parameter erfolgreich.");
            Dim flag3 As Boolean = Not Module1.CheckSource();
            If flag3 Then
                LogMessage.LogOnly("Prüfung des Quellpfades fehlgeschlagen.");
            Module1.ExitProgram();
            End If
            LogMessage.LogOnly("Prüfung des Quellpfades erfolgreich.");
            
            //Module1.Param.SetDestinationFullPath(Module1.Param.DestinationFilePath, Module1.Param.DestinationFileName)  ;
            // The following expression was wrapped in a checked-expression
            Module1.Param.Counter = Module1.Param.Counter + 1
            If Param.Counter > 999 Then Param.Counter = 1
            LogMessage.LogOnly("Start der Konvertierung der Bestelldatei in eine Sedas.dat...")
            Dim convertDATtoSEDAS As ConvertDATtoSEDAS = New ConvertDATtoSEDAS(Module1.Param.SourceFullPath, Module1.Param.DestinationFullPath, Module1.Param.Counter, Module1.ListDelCustomer, Module1.ListDelArticle)
            Dim flag4 As Boolean = convertDATtoSEDAS.ConvertFile()
            If flag4 Then
                LogMessage.Show("Datei " + Module1.Param.SourceFullPath + " erstellt.")
            Else
                LogMessage.Show("Fehler beim Konvertieren/Erstellen der Sedas.dat.", LogMessage.MsgType.Critical)
                Module1.ExitProgram()
            End If
            LogMessage.LogOnly("Counter in Config.ini zurückschreiben...")
            Dim flag5 As Boolean = Not Module1.INI.Write("Setup", "Counter", Conversions.ToString(Module1.Param.Counter))
            If flag5 Then
                LogMessage.Show("Zähler konnte nicht in Config.ini zurückgeschrieben werden!" & vbCrLf & "Weitere Programmausführung nicht möglich!", LogMessage.MsgType.Critical)
                Module1.ExitProgram()
            End If
            Dim deleteSourceFile As Boolean = Module1.Param.DeleteSourceFile
            If deleteSourceFile Then
                LogMessage.LogOnly("Quelldatei wird gelöscht.")
                File.Delete(Module1.Param.SourceFullPath)
            End If
            LogMessage.LogOnly("--- Programm normal beendet. ---")
            LogMessage.LogOnly("********************************")
            LogMessage.LogOnly("");

        }


        public bool InitProgram()
        {
            Dim flag As Boolean = true
            LogMessage.LogOnly("Initialisierung des Programms...")
            LogMessage.LogOnly("Einlesen der Config.ini Datei...")

            Dim flag2 As Boolean = File.Exists(Directory.GetCurrentDirectory() + "\config.ini")
            If flag2 Then
                Module1.INI = New INIFile(Directory.GetCurrentDirectory() + "\Config.ini")
                Dim flag3 As Boolean = Not Module1.ReadIniValues()
                If flag3 Then
                    LogMessage.Show("Fehler beim Auslesen der Config.ini. Bitte Einstellungen prüfen. Programm wird beendet.", LogMessage.MsgType.Critical)
                    flag = Not flag
                Else
                    LogMessage.LogOnly("Einlesen der Config.ini Datei erfolgreich.")
                    LogMessage.LogOnly("Einlesen der Datei loeschKunde.txt...")
                    Dim flag4 As Boolean = File.Exists(Directory.GetCurrentDirectory() + "\loeschKunde.txt")
                    If flag4 Then
                        Module1.ListDelCustomer = New List(Of String)()
                        Try
                            Using streamReader As StreamReader = New StreamReader(Directory.GetCurrentDirectory() + "\loeschKunde.txt")
                                While true
                                    Dim endOfStream As Boolean = streamReader.EndOfStream
                                    If endOfStream Then
                                        Exit While
                                    End If
                                    Dim text As String = streamReader.ReadLine()
                                    Dim flag5 As Boolean = Operators.CompareString(text, "", false) <> 0
                                    If flag5 Then
                                        Module1.ListDelCustomer.Add(text)
                                    End If
                                End While
                            End Using
                        Catch expr_157 As Exception
                            ProjectData.SetProjectError(expr_157)
                            Dim ex As Exception = expr_157
                            LogMessage.LogOnly(ex.ToString())
                            flag = Not flag
                            ProjectData.ClearProjectError()
                            Return flag
                        End Try
                    Else
                        Try
                            File.CreateText(Directory.GetCurrentDirectory() + "\loeschKunde.txt")
                        Catch expr_199 As Exception
                            ProjectData.SetProjectError(expr_199)
                            Dim ex2 As Exception = expr_199
                            LogMessage.Show("Die Datei 'loeschKunde.txt' hat nicht existiert und eine Neuanlage ist fehlgeschlagen." & vbCrLf & "Das Programm wird beendet", LogMessage.MsgType.Critical)
                            LogMessage.LogOnly(ex2.ToString())
                            flag = Not flag
                            ProjectData.ClearProjectError()
                            Return flag
                        End Try
                    End If
                    LogMessage.LogOnly("Einlesen der Datei loeschKunde.txt erfolgreich.")
                    LogMessage.LogOnly("Einlesen der Datei loeschArtikel.txt...")
                    Dim flag6 As Boolean = File.Exists(Directory.GetCurrentDirectory() + "\loeschArtikel.txt")
                    If flag6 Then
                        Module1.ListDelArticle = New List(Of String)()
                        Try
                            Using streamReader2 As StreamReader = New StreamReader(Directory.GetCurrentDirectory() + "\loeschArtikel.txt")
                                While true
                                    Dim endOfStream2 As Boolean = streamReader2.EndOfStream
                                    If endOfStream2 Then
                                        Exit While
                                    End If
                                    Dim text2 As String = streamReader2.ReadLine()
                                    Dim flag7 As Boolean = Operators.CompareString(text2, "", false) <> 0
                                    If flag7 Then
                                        Module1.ListDelArticle.Add(text2)
                                    End If
                                End While
                            End Using
                        Catch expr_26F As Exception
                            ProjectData.SetProjectError(expr_26F)
                            Dim ex3 As Exception = expr_26F
                            LogMessage.LogOnly(ex3.ToString())
                            flag = Not flag
                            ProjectData.ClearProjectError()
                            Return flag
                        End Try
                    Else
                        Try
                            File.CreateText(Directory.GetCurrentDirectory() + "\loeschArtikel.txt")
                        Catch expr_2B1 As Exception
                            ProjectData.SetProjectError(expr_2B1)
                            Dim ex4 As Exception = expr_2B1
                            LogMessage.Show("Die Datei 'loeschArtikel.txt' hat nicht existiert und eine Neuanlage ist fehlgeschlagen." & vbCrLf & "Das Programm wird beendet", LogMessage.MsgType.Critical)
                            LogMessage.LogOnly(ex4.ToString())
                            flag = Not flag
                            ProjectData.ClearProjectError()
                            Return flag
                        End Try
                    End If
                    LogMessage.LogOnly("Einlesen der Datei loeschArtikel.txt erfolgreich.")
                    LogMessage.LogOnly("Einlesen der Datei tauscheArtikel.txt...")
                    Dim flag8 As Boolean = File.Exists(Directory.GetCurrentDirectory() + "\tauscheArtikel.txt")
                    If flag8 Then
                        Module1.ListChangeArticle = New List(Of String)()
                        Try
                            Using streamReader3 As StreamReader = New StreamReader(Directory.GetCurrentDirectory() + "\tauscheArtikel.txt")
                                While true
                                    Dim endOfStream3 As Boolean = streamReader3.EndOfStream
                                    If endOfStream3 Then
                                        Exit While
                                    End If
                                    Dim text3 As String = streamReader3.ReadLine()
                                    Dim flag9 As Boolean = Operators.CompareString(text3, "", false) <> 0
                                    If flag9 Then
                                        Module1.ListChangeArticle.Add(text3)
                                    End If
                                End While
                            End Using
                        Catch expr_387 As Exception
                            ProjectData.SetProjectError(expr_387)
                            Dim ex5 As Exception = expr_387
                            LogMessage.Show("Fehler beim Einlesen der tauscheArtikel.txt." & vbCrLf & "Programm wird beendet.", LogMessage.MsgType.Critical)
                            LogMessage.LogOnly(ex5.ToString())
                            flag = Not flag
                            ProjectData.ClearProjectError()
                            Return flag
                        End Try
                    Else
                        Try
                            File.CreateText(Directory.GetCurrentDirectory() + "\tauscheArtikel.txt")
                        Catch expr_3D2 As Exception
                            ProjectData.SetProjectError(expr_3D2)
                            Dim ex6 As Exception = expr_3D2
                            LogMessage.Show("Die Datei 'tauscheArtikel.txt' hat nicht existiert und eine Neuanlage ist fehlgeschlagen." & vbCrLf & "Das Programm wird beendet", LogMessage.MsgType.Critical)
                            LogMessage.LogOnly(ex6.ToString())
                            flag = Not flag
                            ProjectData.ClearProjectError()
                            Return flag
                        End Try
                    End If
                    LogMessage.LogOnly("Einlesen der Datei tauscheArtikel.txt erfolgreich.")
                End If
            Else
                LogMessage.Show("! Es wurde keine Config.ini Datei gefunden. Es wird eine neue Config.ini mit " & vbCrLf & "  Standardeinstellungen erstellt.", LogMessage.MsgType.Warning)
                Dim flag10 As Boolean = Not Module1.CreateNewConfigIni()
                If flag10 Then
                    LogMessage.Show("Eine neue Config.ini konnte nicht erstellt werden. Programm wird beendet.", LogMessage.MsgType.Critical)
                Else
                    LogMessage.Show("Die Datei 'Config.ini' hat nicht existiert und wurde neu angelegt." & vbCrLf & "  Bitte die Einstellungen kontrollieren, bevor das Programm erneut ausgeführt wird.", LogMessage.MsgType.Critical)
                End If
                flag = Not flag
            End If
            return flag;
        }


        public bool ReadIniValues()
        {
            Dim result As Boolean = true
            Try
                Module1.Param.SourceFileName = Module1.INI.Read("Setup", "Quelldateiname")
                Module1.Param.SourceFilePath = Module1.INI.Read("Setup", "Quelldateipfad")
                Dim flag As Boolean = Operators.CompareString(Module1.Param.SourceFilePath, "", false) <> 0
                If flag Then
                    Dim flag2 As Boolean = Operators.CompareString(Strings.Mid(Module1.Param.SourceFilePath, Strings.Len(Module1.Param.SourceFilePath), 1), "\", false) <> 0
                    If flag2 Then
                        Module1.Param.SourceFilePath = Module1.Param.SourceFilePath + "\"
                    End If
                Else
                    Module1.Param.SourceFilePath = Directory.GetCurrentDirectory() + "\"
                End If
                Module1.Param.DestinationFileName = Module1.INI.Read("Setup", "Zieldateiname")
                Module1.Param.DestinationFilePath = Module1.INI.Read("Setup", "Zieldateipfad")
                Dim flag3 As Boolean = Operators.CompareString(Module1.Param.DestinationFilePath, "", false) <> 0
                If flag3 Then
                    Dim flag4 As Boolean = Operators.CompareString(Strings.Mid(Module1.Param.DestinationFilePath, Strings.Len(Module1.Param.DestinationFilePath), 1), "\", false) <> 0
                    If flag4 Then
                        Module1.Param.DestinationFilePath = Module1.Param.DestinationFilePath + "\"
                    End If
                Else
                    Module1.Param.DestinationFilePath = Directory.GetCurrentDirectory() + "\"
                End If
                Dim flag5 As Boolean = Operators.CompareString(Module1.Param.DestinationFileName, "", false) = 0 And Operators.CompareString(Module1.Param.DestinationFilePath, "", false) <> 0
                If flag5 Then
                    Module1.Param.DestinationFileName = "SEDAS.DAT"
                End If
                Module1.Param.DeleteSourceFile = Conversions.ToBoolean(Module1.INI.Read("Setup", "QuelleLöschen"))
                Module1.Param.IgnoreMessages = Conversions.ToBoolean(Module1.INI.Read("Setup", "IgnoriereMeldungen"))
                Module1.Param.Counter = Conversions.ToInteger(Module1.INI.Read("Setup", "Counter"))
            Catch expr_24D As Exception
                ProjectData.SetProjectError(expr_24D)
                Dim ex As Exception = expr_24D
                LogMessage.LogOnly("Fehler beim Einlesen der Config.ini: " & vbCrLf + ex.ToString())
                result = false
                ProjectData.ClearProjectError()
            End Try
            return result;
        }

        public bool CreateNewConfigIni()
        {
            Dim flag As Boolean = true
            LogMessage.LogOnly("Erstellen einer neuen leeren Config.ini...")
            Dim value As String = "-----------------------" & vbCrLf & "DATtoSEDAS Config-Datei" & vbCrLf & "-----------------------" & vbCrLf & "Quell- und Zielpfad müssen mit Laufwerksbuchstabe angegeben werden (vollständig), jedoch ohne Dateiname." & vbCrLf & "Der Dateiname der Quell- und Zieldatei wird separat eingetragen." & vbCrLf & "Werden Quell- und Zieldateiname beim Programmstart per Schalter übergeben (/Q=, /Z=), werden die Einträge" & vbCrLf & "in der Config.ini übergangen." & vbCrLf & "Dies gilt auch für alle weiteren Schalter (z.B. QuelleLöschen, /D)" & vbCrLf & vbCrLf & "[Setup]" & vbCrLf & "Counter=" & vbCrLf & "Quelldateipfad=" & vbCrLf & "Quelldateiname=1.txt" & vbCrLf & "Zieldateipfad=C:\Temp" & vbCrLf & "Zieldateiname=Sedas.dat" & vbCrLf & vbCrLf & "QuelleLöschen=0" & vbCrLf & "IgnoriereMeldungen=0" & vbCrLf & "DatenAnhängen=0"
            Try
                Using streamWriter As StreamWriter = New StreamWriter(Directory.GetCurrentDirectory() + "\Config.ini")
                    streamWriter.WriteLine(value)
                End Using
            Catch expr_49 As Exception
                ProjectData.SetProjectError(expr_49)
                Dim ex As Exception = expr_49
                LogMessage.LogOnly("Erstellen einer neuen leeren Config.ini fehlgeschlagen: " & vbCrLf + ex.ToString())
                flag = false
                ProjectData.ClearProjectError()
            End Try
            flag = flag
            return flag;
        }

        public bool CheckParameters()
        {
            CheckParameters = true

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
                    Param.Help = true
                    ShowHelp()
                    CheckParameters = false
                    Return CheckParameters
                End If
            End If

            '-----------------------------------------------------------------------------------------
            'Prüfen, ob mehr als 1 Argument übergeben und im Array /? vorkommt
            'Wenn ja, dann Parameter falsch.
            If Arguments.GetUpperBound(0) > 1 And Array.Exists(Arguments, Function(element)
                                                                              Return element.Contains("/?")
                                                                          End Function) Then
                ShowMessage("FEHLER! Falsche Startparameter angegeben. '/?' darf nur alleine verwendet werden.", false)
                CheckParameters = false
                Return CheckParameters
            End If

            '-----------------------------------------------------------------------------------------
            '## Parameter auswerten.
            Dim erg As Integer = 0
            For i = 1 To Arguments.GetUpperBound(0)
                Dim Switch As String = Arguments(i)

                'Parameter ermitteln
                If InStr(Arguments(i), "=") > 0 Then 'Prüfen, ob der Parameter ein = enthält
                    Switch = Mid(Arguments(i), 1, InStr(Arguments(i), "="))
                Else
                    Switch = Arguments(i)
                End If

                'Parameter prüfen und Parameterwert setzen
                Select Case Switch.ToUpper

                    Case = "/Q="
                        'Prüfen, ob Eintrag aus Pfad und/oder Dateiname besteht
                        If InStr(Mid(Arguments(i), InStr(Arguments(i), "=") +1), "\") > 0 Then
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
                        If InStr(Mid(Arguments(i), InStr(Arguments(i), "=") +1), "\") > 0 Then
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
                        Param.DeleteSourceFile = true
                        'erg = erg + 1

                    Case = "/I"
                        'Fehlermeldungen unterdrücken
                        Param.IgnoreMessages = true

                    Case = "/A"
                        'Fehlermeldungen unterdrücken
                        Param.Append = true

                    Case Else
                        ShowMessage("FEHLER! Falsche Startparameter angegeben.", false)
                        CheckParameters = false
                        Return CheckParameters
                End Select
            Next
            return CheckParameters;
        }

        public bool CheckSource()
        {
            Dim flag As Boolean = true;
            if ()// Param.SourceFullPath = "" Then
            {
                Module1.Param.SetSourceFullPath(Module1.Param.SourceFilePath, Module1.Param.SourceFileName);
            }

            LogMessage.LogOnly("Prüfen des Quellpfades: " + Module1.Param.SourceFullPath);
            Dim flag2 As Boolean = Operators.CompareString(Module1.Param.SourceFullPath, "", false) = 0;
            if ()// flag2 Then
            {
                flag = false;
                LogMessage.Show("Es wurde keine Quelldatei angegeben.", LogMessage.MsgType.Critical);
                flag = flag;
            }
            else
            {
                Dim flag3 As Boolean = Not File.Exists(Module1.Param.SourceFullPath);
                if ()// flag3 Then
                {
                    flag = false;
                    LogMessage.Show("Die Quelldatei existiert nicht oder ist nicht erreichbar.", LogMessage.MsgType.Critical);
                    flag = flag;
                }
                else
                {
                    flag = flag;
                }
            }
            return flag;
        }

        public void ShowMessage(string Message, bool Ignorable = true, bool Pause = false)
        {
            bool flag = Not Module1.Param.IgnoreMessages Or Ignorable;
            if (flag)
            {
                Console.WriteLine(Message);
                if (Pause)
                {
                    Console.Write("<Enter> drücken...");
                    Console.ReadLine();
                }
            }
        }

        public void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("DATtoSEDAS-Converter, Version " + MyProject.Application.Info.Version.ToString());
            Console.WriteLine("Wandelt eine Bestell.dat in eine SEDAS.dat um, für den Import in CSB.");
            Console.WriteLine("Wird das Programm ohne Parameter gestartet, werden Pfad und Dateiinformationen");
            Console.WriteLine("aus der Datei Config.ini übernommen.");
            Console.WriteLine("Parameter, die mit Schaltern übergeben werden überschreiben Einstellungen ");
            Console.WriteLine("der Config.ini");
            Console.WriteLine();
            Console.WriteLine("DATtoSEDAS.exe [/Q=][Laufwerk:][Pfad][Dateiname] [/Z=][Laufwerk:][Pfad][Dateiname] [/D] [/I]");
            Console.WriteLine("               [/A] [/?]");
            Console.WriteLine("[Laufwerk:]  Laufwerksbuchstabe, z.B. C:");
            Console.WriteLine("[Pfad]       Verzeichnispfad");
            Console.WriteLine("[Dateiname]  Dateiname");
            Console.WriteLine();
            Console.WriteLine(" /Q=         Laufwerk/Pfad/Dateiname der Quelldatei.");
            Console.WriteLine("                Wird nur ein Dateiname angegeben, wird dieser im Programm-");
            Console.WriteLine("                Verzeichnis gesucht.");
            Console.WriteLine(" /Z=         Laufwerk/Pfad/Dateiname der Zieldatei." & vbCrLf & "                Wird kein Zielpfad angegeben,wird die " & vbCrLf & "                Zieldatei im Programmverzeichnis abgelegt." & vbCrLf & "              ! Existiert eine gleichnamige Zieldatei bereits, wird");
            Console.WriteLine("                sie ohne Rückfrage überschrieben!");
            Console.WriteLine(" /D          Quelldatei wird nach der Konvertierung gelöscht.");
            Console.WriteLine(" /I          Statusmeldungen werden nicht angezeigt (jedoch Fehlermeldungen!).");
            Console.WriteLine(" /?          Ruft diese Hilfeseite auf.");
            Console.WriteLine();
            Console.WriteLine("Beispiel:");
            Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /Z=D:\Import\Sedas.dat /D /I");
            Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen, die Quelldatei wird anschließend gelöscht.");
            Console.WriteLine("   Statusmeldungen werden nicht ausgegeben.");
            Console.WriteLine();
            Console.WriteLine(">> DATtoSEDAS.exe /Q=C:\Daten\Bestell.dat /I");
            Console.WriteLine(" - Quelldatei wird in Zieldatei eingelesen. Statusmeldungen werden nicht ausgegeben.");
            Console.WriteLine();
            Console.Write("<Enter> drücken...");
            Console.ReadLine();
        }

        public void ExitProgram()
        {
            LogMessage.Show("Programm wird nach Fehler beendet.")
            Environment.[Exit](0);
        }
    }
}
