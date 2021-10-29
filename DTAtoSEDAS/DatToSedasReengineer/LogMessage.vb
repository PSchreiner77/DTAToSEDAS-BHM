Imports System.IO
Imports DatToSedasReengineer.My
Imports Microsoft.VisualBasic.CompilerServices

Public Class LogMessage
    Private Shared _GlobalLog As Boolean = True

    Private Shared _GlobalOutputToConsole As Boolean = False

    Private Shared MessagePrefix As String = "Meldung"

    Private Shared WarningPrefix As String = "[Warnung]"

    Private Shared CriticalPrefix As String = "[**FEHLER**]"

    Private Shared _Path As String = Directory.GetCurrentDirectory() + "\DatToSedas.log"

    Public Enum MsgType
        Message
        Warning
        Critical
    End Enum

    Public Enum Output
        Console = 1
    End Enum

    Public Shared Property SetGlobalLog() As Boolean
        Get
            Return LogMessage._GlobalLog
        End Get
        Set(value As Boolean)
            LogMessage._GlobalLog = value
        End Set
    End Property

    Public Shared Property SetGlobalOutputToConsole() As Boolean
        Get
            Return LogMessage._GlobalOutputToConsole
        End Get
        Set(value As Boolean)
            LogMessage._GlobalOutputToConsole = value
        End Set
    End Property

    Public Shared Sub Show(msg As String)
        LogMessage.ShowMessage(msg, "", MsgType.Message, 1)
    End Sub

    Public Shared Sub Show(msg As String, type As MsgType)
        LogMessage.ShowMessage(msg, "", type, 0)
    End Sub

    Public Shared Sub Show(msg As String, output As Output)
        LogMessage.ShowMessage(msg, "", MsgType.Message, 1)
    End Sub

    Public Shared Sub Show(msg As String, type As MsgType, output As Output)
        LogMessage.ShowMessage(msg, "", type, 1)
    End Sub

    Public Shared Sub Show(msg As String, title As String)
        LogMessage.ShowMessage(msg, title, MsgType.Message, 1)
    End Sub

    Public Shared Sub Show(msg As String, title As String, type As MsgType)
        LogMessage.ShowMessage(msg, title, type, 1)
    End Sub

    Public Shared Sub Show(msg As String, title As String, output As Output)
        LogMessage.ShowMessage(msg, title, MsgType.Message, 1)
    End Sub

    Public Shared Sub Show(msg As String, title As String, type As MsgType, output As Output)
        LogMessage.ShowMessage(msg, title, type, 1)
    End Sub

    Public Shared Sub LogOnly(msg As String)
        LogMessage.WriteToLogfile(msg, MsgType.Message)
    End Sub

    Public Shared Sub LogOnly(msg As String, type As MsgType)
        LogMessage.LogOnly(msg, type)
    End Sub

    Private Shared Sub ShowMessage(msg As String, title As String, type As MsgType, output As Integer)
        Dim prefix As String = ""
        Select Case type
            Case MsgType.Message
                Dim flag As Boolean = Operators.CompareString(title, "", False) = 0
                If flag Then
                    title = "Nachricht"
                End If
                prefix = LogMessage.MessagePrefix
            Case MsgType.Warning
                Dim flag2 As Boolean = Operators.CompareString(title, "", False) = 0
                If flag2 Then
                    title = "Warnung"
                End If
                prefix = LogMessage.WarningPrefix
            Case MsgType.Critical
                Dim flag3 As Boolean = Operators.CompareString(title, "", False) = 0
                If flag3 Then
                    title = "Kritischer Fehler!"
                End If
                prefix = LogMessage.CriticalPrefix
        End Select
        Dim flag4 As Boolean = Not LogMessage._GlobalOutputToConsole And output = 0
        If Not flag4 Then
            Dim flag5 As Boolean = type = MsgType.Critical
            If flag5 Then
                LogMessage.WriteToConsole(prefix, msg)
                Console.Write("Bitte ENTER drücken...")
                Console.ReadLine()
            Else
                LogMessage.WriteToConsole(prefix, msg)
            End If
        End If
        Dim globalLog As Boolean = LogMessage._GlobalLog
        If globalLog Then
            LogMessage.WriteToLogfile(msg, type)
        End If
    End Sub

    Private Shared Sub WriteToConsole(prefix As String, msg As String)
        Dim array As String() = Strings.Split(msg, vbCrLf, -1, CompareMethod.Binary)
        Dim list As List(Of String) = New List(Of String)()
        Dim num As Integer = 80
        Dim array2 As String() = array
        ' The following expression was wrapped in a checked-statement
        For i As Integer = 0 To array2.Length - 1
            Dim text As String = array2(i)
            While Strings.Len(text) > num
                list.Add(Strings.Mid(text, 1, num))
                text = Strings.Mid(text, num + 1)
            End While
            list.Add(text)
        Next
        Dim num2 As Integer = list.Count - 1
        For j As Integer = 0 To num2
            Dim flag As Boolean = j = 0
            If flag Then
                Console.WriteLine(String.Format("{0,-12}: {1}", prefix, list(j)))
            Else
                Console.WriteLine("{0,-12}: {1}", "", list(j))
            End If
        Next
    End Sub

    Private Shared Sub WriteToLogfile(msg As String, type As MsgType)
        Dim array As String() = Strings.Split(msg, vbCrLf, -1, CompareMethod.Binary)
        Dim list As List(Of String) = New List(Of String)()
        Dim arg As String = ""
        Dim num As Integer = Strings.Len(DateAndTime.Now.ToString())
        Select Case type
            Case MsgType.Message
                arg = LogMessage.MessagePrefix
            Case MsgType.Warning
                arg = LogMessage.WarningPrefix
            Case MsgType.Critical
                arg = LogMessage.CriticalPrefix
        End Select
        msg = msg.Replace(vbCrLf, " ")
        Try
            File.AppendAllText(LogMessage._Path, String.Format("{0};{1,15} ;{2}", DateAndTime.Now.ToString(), arg, msg) + vbCrLf)
        Catch expr_AC As Exception
            ProjectData.SetProjectError(expr_AC)
            ProjectData.ClearProjectError()
        End Try
    End Sub

    Public Shared Function CheckLogFile(maxSizeKB As Integer) As Boolean
        Dim text As String = ""
        Dim list As List(Of String) = New List(Of String)()
        Dim text2 As String = ""
        Dim flag As Boolean = Not File.Exists(LogMessage._Path)
        Dim flag2 As Boolean
        If flag Then
            flag2 = flag2
        Else
            Dim num As Long = MyProject.Computer.FileSystem.GetFileInfo(LogMessage._Path).Length
            Dim num2 As Long = num
            Dim flag3 As Boolean = num2 > 1000L
            If flag3 Then
                num /= 1000L
                text2 = "KB"
            End If
            Dim flag4 As Boolean = num > CLng(maxSizeKB)
            ' The following expression was wrapped in a checked-statement
            If flag4 Then
                LogMessage.LogOnly(String.Concat(New String() {"Verkleinern der Logdatei von ", Conversions.ToString(num), text2, " auf ca. ", Conversions.ToString(num / 2L), text2}))
                Using streamReader As StreamReader = New StreamReader(LogMessage._Path)
                    text = streamReader.ReadToEnd()
                End Using
                Dim array As String() = Strings.Split(text, vbCrLf, -1, CompareMethod.Binary)
                Dim num3 As Integer = array.Count / 2
                Dim upperBound As Integer = array.GetUpperBound(0)
                For i As Integer = num3 To upperBound
                    list.Add(array(i))
                Next
                File.Delete(LogMessage._Path)
                text = ""
                LogMessage.LogOnly("######## Logdatei verkleinert ##########")
                Try
                    Dim enumerator As List(Of String).Enumerator = list.GetEnumerator()
                    While enumerator.MoveNext()
                        Dim current As String = enumerator.Current
                        text = text + current + vbCrLf
                    End While
                Finally
                    Dim enumerator As List(Of String).Enumerator
                    CType(enumerator, IDisposable).Dispose()
                End Try
                File.AppendAllText(LogMessage._Path, text)
            End If
        End If
        Return flag2
    End Function
End Class
