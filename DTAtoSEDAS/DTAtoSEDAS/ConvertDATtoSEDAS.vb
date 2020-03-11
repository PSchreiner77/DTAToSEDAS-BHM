Public Class ConvertDATtoSEDAS
    Dim _Source As String = ""
    Dim _Destination As String = ""
    Dim _Header As String = ""
    Dim _Footer As String = ""
    Dim _DTAContent As String(,)
    Dim _FileDate As String
    Dim _DataSets As Integer = 0
    Dim _Customers As Integer = 0
    Dim _SummeGes As Integer = 0
    Dim _Counter As Integer

    'TODO Fehlermeldungen mit zus. Informationen anreichern
    'TODO 

    'CONSTRUCTOR
    ''' <summary>
    ''' Erstellt das Objekt
    ''' </summary>
    ''' <param name="s">Quelldateipfad</param>
    ''' <param name="d">Zieldateipfad</param>
    Public Sub New(s As String, d As String, c As Integer)
        'Properties setzen
        Source = s
        Destination = d
        Counter = c
    End Sub

    'PROPERTIES
    Public Property Source() As String
        Get
            Return _Source
        End Get
        Set(value As String)
            _Source = value
        End Set
    End Property

    Public Property Destination() As String
        Get
            Return _Destination
        End Get
        Set(value As String)
            _Destination = value
        End Set
    End Property

    Public Property Header() As String
        Get
            Return _Header
        End Get
        Set(value As String)
            _Header = value
        End Set
    End Property

    Public Property Footer() As String
        Get
            Return _Footer
        End Get
        Set(value As String)
            _Footer = value
        End Set
    End Property

    Private Property DTAContent() As String(,)
        Get
            Return _DTAContent
        End Get
        Set(value As String(,))
            _DTAContent = value
        End Set
    End Property

    Private Property Counter() As Integer
        Get
            Return _Counter
        End Get
        Set(value As Integer)
            _Counter = value
        End Set
    End Property

    'FUNCTIONS
    ''' <summary>
    ''' Konvertiert die Daten einer DTA-Datei in eine SEDAS.DAT Datei.
    ''' </summary>
    ''' <returns></returns>
    Public Function ConvertFile() As Boolean
        'Daten konvertieren und speichern.
        ConvertFile = False

        Try
            '-------------------------------------------------------------------------
            'DTA einlesen
            DTAContent = ReadDTAData(Source)

            '-------------------------------------------------------------------------
            'SEDAS erstellen
            If Not IsNothing(DTAContent) And WriteSedasData(DTAContent, Destination) Then
                ConvertFile = True
            End If

        Catch
            ConvertFile = False
        End Try

    End Function

    ''' <summary>
    ''' Liest die DTA-Datei ein und gibt die enthaltenen Werte als 2-dimensionales Array zurück.
    ''' </summary>
    ''' <param name="s">Pfad zur DTA-Datei, die eingelesen werden soll.</param>
    ''' <returns></returns>
    Function ReadDTAData(s As String) As String(,)

        'Quelle
        '1030051800009175N       300518                                     1428200000001000+
        '1030051800009175N       300518                                     1501100000001000+
        '1030051800009175N       300518                                     1511100000001000+
        '---------------------------------
        '10,300518,00009175,N,       ,300518,                                     ,14282,00000001,000+
        '? ,LiDat ,KdNr    ,?,       ,Liefedatum,                                 ,ArtNr,Menge,?


        'Prüfen, ob Datei existiert
        If Not System.IO.File.Exists(Source) Then
            ReadDTAData = Nothing
            Return ReadDTAData
        End If

        '-------------------------------------------------------------------------
        Try
            '-- QUELL-DATEI EINLESEN
            _FileDate = System.IO.File.GetCreationTime(Source).ToString
            _FileDate = Mid(_FileDate, 1, 10)
            _FileDate = _FileDate.Replace(".", "")
            _FileDate = _FileDate.Remove(4, 2)
            Dim arrSourceLines() As String  'Array mit Zeilen der Quelldatei
            Dim arrSourceData(,) As String  'Array mit zerlegten Zeilen der Quelldatei in Einzelinformationen.


            Using srSource As IO.StreamReader = New IO.StreamReader(Source)
                'Alle Zeilen der Quelldatei in Array einlesen
                arrSourceLines = Split(srSource.ReadToEnd, vbCrLf)

                'arrSourceData neu Dimensionieren auf die Anzal eingelesener (nicht leerer) Zeilen.
                Dim redimVal As Integer = -1
                For j = 0 To arrSourceLines.GetUpperBound(0)
                    If Not arrSourceLines(j) = "" Then redimVal = redimVal + 1
                Next
                ReDim arrSourceData(redimVal, 10)

                'Einzelinformationen aus LINE-Array in Datenarray übernehmen.
                Dim i As Integer = 0
                Dim z As Integer = 0
                Do Until i > arrSourceLines.GetUpperBound(0) 'Or arrSourceLines(i) = ""
                    Dim text As String = arrSourceLines(i)
                    If text <> "" Then
                        'String zerlegen:
                        arrSourceData(z, 1) = MyTRIM(Mid(arrSourceLines(i), 1, 2))   'Führende Ziffer 10
                        arrSourceData(z, 2) = MyTRIM(Mid(arrSourceLines(i), 3, 6))   'Datum ttmmjj      300518
                        arrSourceData(z, 3) = MyTRIM(Mid(arrSourceLines(i), 9, 8))   'Kundennummer      00009175
                        arrSourceData(z, 4) = MyTRIM(Mid(arrSourceLines(i), 18, 13)) 'Datum ttmmjj, wie Pos2 (?)    300518
                        arrSourceData(z, 5) = Trim(Mid(arrSourceLines(i), 31, 42))   'Artikelnummer     14282
                        arrSourceData(z, 6) = Trim(Mid(arrSourceLines(i), 73, 11))    'Menge            00000001000
                        z += 1
                    End If
                    i = i + 1
                Loop
            End Using
            ReadDTAData = arrSourceData

        Catch
            ReadDTAData = Nothing
        End Try

    End Function

    ''' <summary>
    ''' Schreibt die ausgelesenen Daten aus einem Array in eine Sedas.dat.
    ''' </summary>
    ''' <param name="arr">Auszulesendes DTA-Array zur Ausgabe in die Datei.</param>
    ''' <param name="d">Zielpfad zur SEDAS.DAT Datei.</param>
    ''' <returns></returns>
    Function WriteSedasData(arr As String(,), d As String) As Boolean
        'Ziel
        ';030,14,00000000000000000,180529,180530,,,,9175         ,,                          
        ';0400000000014282,40001000,,,,02 000000,,                                           
        ';0400000000015011,40001000,,,,02 000000,,                                           
        ';0400000000015111,40001000,,,,02 000000,,                                           
        ';05000000003000    

        '--ZIELDATEI ERSTELLEN
        Dim ListBodyText As New List(Of String) 'Nimmt zeilenweise den Zieltext auf
        WriteSedasData = False

        '-------------------------------------------------------------------------
        '-- Header
        Dim Spaces As String = ""
        For i = 0 To 53 - Len(Counter)
            Spaces = Spaces & " "
        Next

        Header = vbLf & "010()000377777777777771" & ReverseDate(_FileDate) & ";," & Counter & Spaces & vbCrLf &
                        ";)0240051310000002                                                                  "

        ListBodyText.Add(Header)

        '-------------------------------------------------------------------------
        Dim KdNr As String = ""
        Dim SummeMenge As String
        Dim Pos1 As Integer = 0
        Dim Pos2 As Integer = 0
        Dim arrBound As Integer = arr.GetUpperBound(0)

        Try
            While Pos1 <= arr.GetUpperBound(0)
                'If Pos1 = 1129 Then Stop
                'Begin Kundenblock
                ';030,14,00000000000000000,180529,180530,,,,9175         ,,      
                ListBodyText.Add(";030,14,00000000000000000," & ReverseDate(_FileDate) & "," & ReverseDate(DTAContent(Pos1, 2)) & ",,,," & DTAContent(Pos1, 3) & "         ,," &
                                 "                          ")
                'DEBUG
                _Customers = _Customers + 1

                '-------------------------------------------------------------------------
                'Positionszeilen
                SummeMenge = "0"
                While DTAContent(Pos1, 3) = DTAContent(Pos2, 3)
                    'DEBUG
                    _DataSets = _DataSets + 1
                    'Ziel                      
                    ';0400000000014282,40001000,,,,02 000000,,                                           
                    ';0400000000015011,40001000,,,,02 000000,,                                           
                    ';0400000000015111,40001000,,,,02 000000,,       
                    ListBodyText.Add(";04000000000" & DTAContent(Pos2, 5) & ",4" & Mid(DTAContent(Pos2, 6), 5) & ",,,,02 000000,," &
                                 "                                           ")
                    SummeMenge = CStr(CInt(SummeMenge) + CInt(DTAContent(Pos2, 6)))
                    Pos2 = Pos2 + 1
                    If Pos2 > DTAContent.GetUpperBound(0) Then Exit While
                End While


                For i = 1 To 12 - Len(SummeMenge)
                    SummeMenge = "0" & SummeMenge
                Next
                '-------------------------------------------------------------------------
                'Fusszeile
                ListBodyText.Add(";05" & SummeMenge & "                                                                     ")
                Pos1 = Pos2
                _SummeGes = _SummeGes + CInt(SummeMenge)
            End While

            '-------------------------------------------------------------------------
            '--FOOTER der Zieldatei
            ';06108,1143 
            ';06    = Zusammenfassung Einträge
            '108,   = Anzahl Kunden in Datei (Blöcke)
            '1143   = Anzahl einzelner Datensätze/Artikelzeilen

            'Falls kein Footer angegeben wurde, diese Standards übernehmen.
            Footer = ";06" & _Customers & "," & _DataSets
            For i = 1 To 84 - Len(Footer)
                Footer = Footer & " "
            Next
            Footer = Footer & vbCrLf & ";07000000,00001,00001,000000,(                                                      "

            ListBodyText.Add(Footer)

            '-------------------------------------------------------------------------
            'Zieldatei ablegen
            If InStr(Destination, "\") > 0 Then
                If Not System.IO.File.Exists(Destination) Then
                    If Not System.IO.Directory.Exists(Mid(Destination, 1, InStrRev(Destination, "\"))) Then
                        System.IO.Directory.CreateDirectory(Mid(Destination, 1, InStrRev(Destination, "\")))
                    End If
                End If
            Else
                Destination = IO.Directory.GetCurrentDirectory & "\" & Destination
            End If

            Using srDestFile As New System.IO.StreamWriter(Destination, False)
                For Each entry In ListBodyText
                    srDestFile.WriteLine(entry)
                    'srDestFile.Write("")
                Next
                srDestFile.WriteLine("                                                                                    ")
            End Using

            WriteSedasData = True
        Catch ex As Exception
            WriteSedasData = False
        End Try

    End Function

    ''' <summary>
    ''' Spiegelt den übergebenen Text. Alle Zeichen werden in umgekehrter Reihenfolge zurückgegeben.
    ''' </summary>
    ''' <param name="str">Zu spiegelnder Text.</param>
    ''' <returns></returns>
    Function ReverseDate(str As String) As String
        ReverseDate = ""
        If Not str = "" Then
            str = Trim(str)
            For i = Len(str) - 1 To 1 Step -2
                Dim Part As String = Mid(str, i, 2)
                ReverseDate = ReverseDate & Part
            Next
        End If

    End Function

    ''' <summary>
    ''' Entfernt Leerzeichen vom Anfang und Ende eines Strings, sowie eventuell führende Nullen.
    ''' </summary>
    ''' <param name="MyText">Zu verarbeitender Text.</param>
    ''' <returns></returns>
    Function MyTRIM(MyText As String) As String
        MyText = Trim(MyText)

        Do While Mid(MyText, 1, 1) = "0"
            MyText = Mid(MyText, 2)
        Loop
        MyTRIM = MyText
    End Function


End Class
