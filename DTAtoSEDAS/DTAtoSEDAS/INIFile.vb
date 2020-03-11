Public Class INIFile
    'Klasse zum Schreiben und Lesen von Daten aus INI-Dateien
    'Inkl. Sektion, Parameter und Wert
    '
    'Eine Sektion darf nur einmal in der INI-Datei vorkommen.
    'Parameter dürfen in einer Sektion nur einmal auftauchen,
    ' jedoch in mehreren Sektionen separat verwendet werden.

    Dim Path As String = "" '= System.IO.Directory.GetCurrentDirectory & "\Settings.ini" 'Pfad der INI-Datei

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Path">Pfad zu Ini Datei.</param>
    Sub New(Path As String)
        INIPath = Path
    End Sub

    ''' <summary>
    ''' Pfad zur INI-Datei
    ''' </summary>
    ''' <returns>Gibt den Pfad zur INI Datei zurück. Existiert die Datei nicht, wird leer zurückgeggeben.</returns>
    Property INIPath() As String
        Get
            Return Path
        End Get

        Set(value As String)
            If Not System.IO.File.Exists(value) Then
                Path = ""
            Else
                Path = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Liest einen Wert aus einer INI-Datei. Gibt den Wert des Parameters als String zurück.
    ''' Wir der Parameter nicht gefunden, wird ein leerer String zurückgegeben
    ''' </summary>
    ''' <param name="Section">Name der Sektion, innerhalb welcher sich der gewünschte Parameter befindet.</param>
    ''' <param name="Parameter">Name des Parameters, dessen Wert ausgelesen und zurückgegeben werden soll.</param>
    ''' <returns></returns>
    Public Function Read(Section As String, Parameter As String) As String
        'Liest einen Wert eines Parameters einer Sektion aus der INI-Datei aus und gibt diesen zurück.

        Dim srLine As String
        Dim strSection As String = ""
        Dim strParameter As String = ""
        Dim LineNr As Integer = 0
        Dim Check As Integer = 0

        'Datei Zeilenweise durchgehen und auf Sektion prüfen.
        Section = "[" & Section & "]"

        Using sr As New System.IO.StreamReader(Path)
            While Not sr.EndOfStream
                srLine = sr.ReadLine
                LineNr = LineNr + 1
                If Mid(srLine, 1, 1) = "[" Then
                    If Trim(srLine.ToUpper) = Section.ToUpper Then
                        Check = 1

                        While Not sr.EndOfStream
                            srLine = sr.ReadLine
                            If Mid(srLine, 1, 1) = "[" Then Exit While

                            If Trim(Mid(srLine, 1, Len(Parameter)).ToUpper) = Parameter.ToUpper Then
                                Read = Trim(Mid(srLine, InStr(srLine, "=") + 1))
                                Exit Function
                            End If
                        End While
                        Exit While

                    End If
                End If
            End While
        End Using

        Read = ""

    End Function

    ''' <summary>
    ''' Schreibt einen Wert in eine INI-Datei.
    ''' Existiert die INI-Datei, eine Sektion oder ein Parameter nicht, wird sie/er mit dem angegebenen Wert angelegt. 
    ''' Bei Fehler wird FALSE zurückgegeben, sonst TRUE.
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="parameter"></param>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    Public Function Write(Section As String, Parameter As String, value As String) As Boolean
        Write = True
        Dim arrINI As String()
        Dim Check As Integer = 0

        Section = "[" & Section & "]"

        Try
            'Config.ini in Array einlesen und Parameter finden
            Using sr As New IO.StreamReader(Path)
                arrINI = Split(sr.ReadToEnd, vbCrLf)
            End Using

            'Sektion und Parameter in Array finden und neuen Wert eintragen.
            For i = 0 To arrINI.GetUpperBound(0)
                If Mid(arrINI(i), 1, 1) = "[" Then
                    If Trim(arrINI(i).ToUpper) = Section.ToUpper Then
                        Dim j = i + 1
                        Do Until Mid(arrINI(j), 1, 1) = "[" Or Check = 1
                            Dim Param As String
                            If Trim(Mid(arrINI(j), 1, Len(Parameter)).ToUpper) = Parameter.ToUpper Then
                                Param = Trim(Mid(arrINI(j), 1, InStr(arrINI(j), "=")))
                                arrINI(j) = Param & Trim(value)
                                Check = 1
                            End If
                        Loop
                    End If
                End If
                If Check = 1 Then Exit For
            Next

            If Check = 0 Then
                Write = False
                Return Write
            End If

            'Neue Config.ini mit geänderten Werten schreiben.
            Using sw As New IO.StreamWriter(Path, False)
                For i = 0 To arrINI.GetUpperBound(0)
                    sw.WriteLine(arrINI(i))
                Next
            End Using

        Catch ex As Exception
            Write = False
        End Try

        Return Write
    End Function

End Class
