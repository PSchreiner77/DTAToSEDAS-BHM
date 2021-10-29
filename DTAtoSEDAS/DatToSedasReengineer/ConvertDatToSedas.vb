Imports System.IO
Imports Microsoft.VisualBasic.CompilerServices

Public Class ConvertDATtoSEDAS
    Private _SourcePath As String

    Private _SourceData As String

    Private _DestinationPath As String

    Private _DestinationData As String

    Private _SedasHeader As String

    Private _SedasFooter As String

    Private _BlockHeader As String

    Private _DATContent As String(,)

    Private _DataSets As Integer

    Private _Customers As Integer

    Private _SummeGes As Integer

    Private _ListDelCustomer As List(Of String)

    Private _ListDelArticle As List(Of String)

    Private _Counter As Integer

    Private _ErstelldatumSedas As String

    Public Sub New(src As String, dst As String, cnt As Integer, Optional dC As List(Of String) = Nothing, Optional dA As List(Of String) = Nothing)
        Me._SourcePath = ""
        Me._SourceData = ""
        Me._DestinationPath = ""
        Me._DestinationData = ""
        Me._SedasHeader = ""
        Me._SedasFooter = ""
        Me._BlockHeader = ""
        Me._DataSets = 0
        Me._Customers = 0
        Me._SummeGes = 0
        Me._Counter = 0
        Me._ErstelldatumSedas = ""
        Me._SourcePath = src
        Me._DestinationPath = dst
        Me._Counter = cnt
        Me._ListDelArticle = dA
        Me._ListDelCustomer = dC
    End Sub

    Public Function ConvertFile() As Boolean
        Dim result As Boolean = False
        LogMessage.LogOnly("Beginn der Konvertierung...")
        Me._ErstelldatumSedas = Me.ReverseDate(Strings.Mid(DateAndTime.Now.ToString(), 1, 10).Replace(".", "").Remove(4, 2))
        Try
            Dim flag As Boolean = File.Exists(Me._SourcePath)
            If Not flag Then
                result = False
                Return result
            End If
            LogMessage.LogOnly("Einlesen der Quelldatei...")
            Using streamReader As StreamReader = New StreamReader(Me._SourcePath)
                Me._SourceData = streamReader.ReadToEnd()
            End Using
        Catch expr_9B As Exception
            ProjectData.SetProjectError(expr_9B)
            result = False
            ProjectData.ClearProjectError()
            Return result
        End Try
        Dim array As String() = Strings.Split(Me._SourceData, vbCrLf, -1, CompareMethod.Binary)
        Dim num As Integer = 0
        Dim upperBound As Integer = array.GetUpperBound(0)
        ' The following expression was wrapped in a checked-statement
        For i As Integer = 0 To upperBound
            Dim flag2 As Boolean = Operators.CompareString(array(i), "", False) <> 0
            If flag2 Then
                num += 1
            End If
        Next
        Dim array2 As String() = New String(num + 1 - 1) {}
        Dim num2 As Integer = 0
        Dim upperBound2 As Integer = array.GetUpperBound(0)
        For j As Integer = 0 To upperBound2
            Dim flag3 As Boolean = Operators.CompareString(array(j), "", False) <> 0
            If flag3 Then
                array2(num2) = array(j)
                num2 += 1
            End If
        Next
        array = array2
        Try
            Dim flag4 As Boolean = Operators.CompareString(Strings.Mid(array(0), 1, 2), "NF", False) = 0
            If flag4 Then
                LogMessage.LogOnly("Einlesen neues Dateiformat...")
                Me._DATContent = Me.ReadNewDATData(array, Me._ErstelldatumSedas)
            Else
                LogMessage.LogOnly("Einlesen altes Dateiformat...")
                Me._DATContent = Me.ReadDATData(array, Me._ErstelldatumSedas)
            End If
            Dim flag5 As Boolean = Not Information.IsNothing(Me._DATContent) And Me.WriteSedasData()
            If flag5 Then
                LogMessage.LogOnly("Konvertierung in SEDAS.DAT abgeschlossen.")
                result = True
            End If
        Catch expr_1D8 As Exception
            ProjectData.SetProjectError(expr_1D8)
            Dim ex As Exception = expr_1D8
            LogMessage.LogOnly("Fehler beim Konvertieren in Sedas.dat." & vbCrLf + ex.ToString())
            result = False
            ProjectData.ClearProjectError()
        End Try
        Return result
    End Function

    Private Function ReadDATData(arrSourceLines As String(), ErstelldatumTTMMJJ As String) As String(,)
        Dim flag As Boolean = Operators.CompareString(arrSourceLines(0), "", False) = 0
        ' The following expression was wrapped in a checked-statement
        Dim array As String(,)
        If flag Then
            array = Nothing
            array = array
        Else
            Try
                Dim num As Integer = -1
                Dim upperBound As Integer = arrSourceLines.GetUpperBound(0)
                For i As Integer = 0 To upperBound
                    Dim flag2 As Boolean = Operators.CompareString(arrSourceLines(i), "", False) <> 0
                    If flag2 Then
                        num += 1
                    End If
                Next
                Dim array2 As String(,) = New String(num + 1 - 1, 10) {}
                Dim num2 As Integer = 0
                Dim upperBound2 As Integer = arrSourceLines.GetUpperBound(0)
                For j As Integer = 0 To upperBound2
                    Dim left As String = arrSourceLines(j)
                    Dim flag3 As Boolean = Operators.CompareString(left, "", False) <> 0
                    If flag3 Then
                        array2(num2, 2) = Me.MyTRIM(Strings.Mid(arrSourceLines(j), 9, 8))
                        array2(num2, 3) = ErstelldatumTTMMJJ
                        array2(num2, 4) = Me.MyTRIM(Strings.Mid(arrSourceLines(j), 18, 13))
                        array2(num2, 6) = Me.Shorten(Strings.Mid(arrSourceLines(j), 73, 11), 7)
                        array2(num2, 8) = Me.Expand(Strings.Trim(Strings.Mid(arrSourceLines(j), 31, 42)), 10)
                        num2 += 1
                    End If
                Next
                array = Me.DeleteEntries1(array2)
            Catch expr_139 As Exception
                ProjectData.SetProjectError(expr_139)
                array = Nothing
                ProjectData.ClearProjectError()
            End Try
        End If
        Return array
    End Function

    Private Function ReadNewDATData(arrSourcelines As String(), ErstelldatumTTMMJJ As String) As String(,)
        ' The following expression was wrapped in a checked-statement
        Dim array3 As String(,)
        Try
            Dim num As Integer = -1
            Dim upperBound As Integer = arrSourcelines.GetUpperBound(0)
            For i As Integer = 0 To upperBound
                Dim flag As Boolean = Operators.CompareString(arrSourcelines(i), "", False) <> 0
                If flag Then
                    num += 1
                End If
            Next
            Dim array As String(,) = New String(num + 1 - 1, 10) {}
            Dim num2 As Integer = 0
            Dim upperBound2 As Integer = arrSourcelines.GetUpperBound(0)
            For j As Integer = 0 To upperBound2
                Dim array2 As String() = Strings.Split(arrSourcelines(j), ";", -1, CompareMethod.Binary)
                Dim flag2 As Boolean = Operators.CompareString(array2(0), "", False) <> 0
                If flag2 Then
                    Dim flag3 As Boolean = Operators.CompareString(array(num2, 2), "1678", False) = 0 And Operators.CompareString(array(num2, 6), "222", False) = 0
                    If flag3 Then
                        Debugger.Break()
                    End If
                    array(num2, 2) = array2(2)
                    array(num2, 3) = ErstelldatumTTMMJJ
                    array(num2, 4) = array2(4)
                    array(num2, 6) = Me.Expand(array2(6), 7)
                    array(num2, 8) = Me.Expand(array2(8), 10)
                    num2 += 1
                End If
            Next
            array3 = Me.DeleteEntries1(array)
            array3 = Me.ChangeArticleNumbers(array3)
        Catch expr_12D As Exception
            ProjectData.SetProjectError(expr_12D)
            array3 = Nothing
            ProjectData.ClearProjectError()
        End Try
        array3 = array3
        Return array3
    End Function

    Private Function DeleteEntries1(DATSource As String(,)) As String(,)
        ' The following expression was wrapped in a checked-statement
        Dim array As String(,) = New String(DATSource.GetUpperBound(0) + 1 - 1, DATSource.GetUpperBound(1) + 1 - 1) {}
        Dim num As Integer = 0
        LogMessage.LogOnly("Löschen von nicht benötigten Kunden- und Artikeldaten laut loeschKunde.txt & loeschArtikel.txt.")
        Dim upperBound As Integer = DATSource.GetUpperBound(0)
        For i As Integer = 0 To upperBound
            Dim flag As Boolean = False
            Try
                Dim enumerator As List(Of String).Enumerator = Module1.ListDelCustomer.GetEnumerator()
                While enumerator.MoveNext()
                    Dim current As String = enumerator.Current
                    Dim flag2 As Boolean = Operators.CompareString(Me.MyTRIM(DATSource(i, 2)), Me.MyTRIM(current), False) = 0
                    If flag2 Then
                        flag = True
                    End If
                End While
            Finally
                Dim enumerator As List(Of String).Enumerator
                CType(enumerator, IDisposable).Dispose()
            End Try
            Try
                Dim enumerator2 As List(Of String).Enumerator = Module1.ListDelArticle.GetEnumerator()
                While enumerator2.MoveNext()
                    Dim current2 As String = enumerator2.Current
                    Dim flag3 As Boolean = Operators.CompareString(Me.MyTRIM(DATSource(i, 8)), Me.MyTRIM(current2), False) = 0
                    If flag3 Then
                        flag = True
                    End If
                End While
            Finally
                Dim enumerator2 As List(Of String).Enumerator
                CType(enumerator2, IDisposable).Dispose()
            End Try
            Dim flag4 As Boolean = Not flag
            If flag4 Then
                Dim upperBound2 As Integer = DATSource.GetUpperBound(1)
                For j As Integer = 0 To upperBound2
                    array(num, j) = DATSource(i, j)
                Next
                num += 1
            End If
        Next
        Dim array2 As String(,) = New String(num - 1 + 1 - 1, array.GetUpperBound(1) + 1 - 1) {}
        Dim num2 As Integer = 0
        Dim upperBound3 As Integer = array.GetUpperBound(0)
        For k As Integer = 0 To upperBound3
            Dim flag5 As Boolean = Not Information.IsNothing(array(k, 2))
            If flag5 Then
                Dim upperBound4 As Integer = array.GetUpperBound(1)
                For l As Integer = 0 To upperBound4
                    array2(num2, l) = array(k, l)
                Next
                num2 += 1
            End If
        Next
        Return array2
    End Function

    Private Function ChangeArticleNumbers(DatSource As String(,)) As String(,)
        LogMessage.LogOnly("Austauschen von Artikelnummern laut tauscheArtikel.txt.")
        Dim flag As Boolean = Not Information.IsNothing(Module1.ListChangeArticle)
        ' The following expression was wrapped in a checked-statement
        Dim result As String(,)
        If flag Then
            Dim array As String(,) = New String(Module1.ListChangeArticle.Count - 1 + 1 - 1, 1) {}
            Try
                Dim upperBound As Integer = array.GetUpperBound(0)
                For i As Integer = 0 To upperBound
                    Dim array2 As String() = Strings.Split(Module1.ListChangeArticle(i), ";", -1, CompareMethod.Binary)
                    array(i, 0) = Me.MyTRIM(array2(0))
                    array(i, 1) = Me.MyTRIM(array2(1))
                Next
                Dim upperBound2 As Integer = DatSource.GetUpperBound(0)
                For j As Integer = 0 To upperBound2
                    Dim upperBound3 As Integer = array.GetUpperBound(0)
                    For k As Integer = 0 To upperBound3
                        Dim flag2 As Boolean = Operators.CompareString(DatSource(j, 8), Me.Expand(array(k, 0), 10), False) = 0
                        If flag2 Then
                            DatSource(j, 8) = Me.Expand(array(k, 1), 10)
                        End If
                    Next
                Next
            Catch expr_111 As Exception
                ProjectData.SetProjectError(expr_111)
                ProjectData.ClearProjectError()
            End Try
            result = DatSource
        Else
            result = DatSource
        End If
        Return result
    End Function

    Private Function WriteSedasData() As Boolean
        LogMessage.LogOnly("Schreiben der Sedas.dat...")
        Dim list As List(Of String) = New List(Of String)()
        Dim result As Boolean = False
        Me._SedasHeader = String.Concat(New String() {"010()000377777777777771", Me._ErstelldatumSedas, ";,", Conversions.ToString(Me._Counter), vbCrLf & ";)0240051310000002"})
        list.Add(Me._SedasHeader)
        Dim i As Integer = 0
        Dim num As Integer = 0
        Dim upperBound As Integer = Me._DATContent.GetUpperBound(0)
        ' The following expression was wrapped in a checked-statement
        Try
            While i <= Me._DATContent.GetUpperBound(0)
                list.Add(String.Concat(New String() {";030,14,00000000000000000,", Me._DATContent(i, 3), ",", Me.ReverseDate(Me._DATContent(i, 4)), ",,,,", Me._DATContent(i, 2), "         ,,"}))
                Me._Customers += 1
                Dim text As String = "0"
                While Operators.CompareString(Me._DATContent(i, 2), Me._DATContent(num, 2), False) = 0
                    Me._DataSets += 1
                    list.Add(String.Concat(New String() {";040000", Me._DATContent(num, 8), ",4", Me._DATContent(num, 6), ",,,,02 000000,,"}))
                    text = Conversions.ToString(Conversions.ToInteger(text) + Conversions.ToInteger(Me._DATContent(num, 6)))
                    num += 1
                    Dim flag As Boolean = num > Me._DATContent.GetUpperBound(0)
                    If flag Then
                        Exit While
                    End If
                End While
                Dim num2 As Integer = 12 - Strings.Len(text)
                For j As Integer = 1 To num2
                    text = "0" + text
                Next
                list.Add(";05" + text)
                i = num
                Me._SummeGes += Conversions.ToInteger(text)
            End While
            Me._SedasFooter = String.Concat(New String() {";06", Me.Expand(Conversions.ToString(Me._Customers), 3), ",", Me.Expand(Conversions.ToString(Me._DataSets), 4), vbCrLf & ";07000000,00001,00001,000000,("})
            list.Add(Me._SedasFooter)
            Dim flag2 As Boolean = Strings.InStr(Me._DestinationPath, "\", CompareMethod.Binary) > 0
            If flag2 Then
                Dim flag3 As Boolean = Not File.Exists(Me._DestinationPath)
                If flag3 Then
                    Dim flag4 As Boolean = Not Directory.Exists(Strings.Mid(Me._DestinationPath, 1, Strings.InStrRev(Me._DestinationPath, "\", -1, CompareMethod.Binary)))
                    If flag4 Then
                        Directory.CreateDirectory(Strings.Mid(Me._DestinationPath, 1, Strings.InStrRev(Me._DestinationPath, "\", -1, CompareMethod.Binary)))
                    End If
                End If
            Else
                Me._DestinationPath = Directory.GetCurrentDirectory() + "\" + Me._DestinationPath
            End If
            Using streamWriter As StreamWriter = New StreamWriter(Me._DestinationPath, False)
                Try
                    Dim enumerator As List(Of String).Enumerator = list.GetEnumerator()
                    While enumerator.MoveNext()
                        Dim current As String = enumerator.Current
                        streamWriter.WriteLine(current)
                    End While
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                streamWriter.WriteLine("                                                                                    ")
            End Using
            result = True
        Catch expr_39B As Exception
            ProjectData.SetProjectError(expr_39B)
            Dim ex As Exception = expr_39B
            LogMessage.LogOnly(ex.ToString())
            Interaction.MsgBox(ex.ToString(), MsgBoxStyle.OkOnly, Nothing)
            result = False
            ProjectData.ClearProjectError()
        End Try
        Return result
    End Function

    Private Function ReverseDate(str As String) As String
        Dim text As String = ""
        Dim flag As Boolean = Operators.CompareString(str, "", False) <> 0
        ' The following expression was wrapped in a checked-statement
        If flag Then
            str = Strings.Trim(str)
            Dim num As Integer = Strings.Len(str) - 1
            For i As Integer = num To 1 Step -2
                Dim str2 As String = Strings.Mid(str, i, 2)
                text += str2
            Next
        End If
        Return text
    End Function

    Private Function MyTRIM(MyText As String) As String
        MyText = Strings.Trim(MyText)
        While Operators.CompareString(Strings.Mid(MyText, 1, 1), "0", False) = 0
            MyText = Strings.Mid(MyText, 2)
        End While
        Return MyText
    End Function

    Private Function Shorten(input As String, limit As Integer) As String
        Dim num As Integer = Strings.Len(input)
        Dim flag As Boolean = num > limit
        Dim result As String
        If flag Then
            ' The following expression was wrapped in a checked-expression
            result = Strings.Mid(input, num - limit + 1)
        Else
            result = input
        End If
        Return result
    End Function

    Private Function Expand(input As String, limit As Integer) As String
        Dim num As Integer = Strings.Len(input)
        Dim text As String = ""
        Dim flag As Boolean = num < limit
        ' The following expression was wrapped in a checked-statement
        If flag Then
            Dim num2 As Integer = limit - num
            For i As Integer = 1 To num2
                text += "0"
            Next
            text += input
        Else
            text = input
        End If
        Return text
    End Function
End Class