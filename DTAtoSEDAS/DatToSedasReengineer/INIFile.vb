Imports System.IO
Imports Microsoft.VisualBasic.CompilerServices

Public Class INIFile
    Private Path As String

    Public Property INIPath() As String
        Get
            Return Me.Path
        End Get
        Set(value As String)
            Dim flag As Boolean = Not File.Exists(value)
            If flag Then
                Me.Path = ""
            Else
                Me.Path = value
            End If
        End Set
    End Property

    Public Sub New(Path As String)
        Me.Path = ""
        Me.INIPath = Path
    End Sub

    Public Function Read(Section As String, Parameter As String) As String
        Dim num As Integer = 0
        Section = "[" + Section + "]"
        ' The following expression was wrapped in a checked-statement
        Dim result As String
        Using streamReader As StreamReader = New StreamReader(Me.Path)
            While Not streamReader.EndOfStream
                Dim text As String = streamReader.ReadLine()
                num += 1
                Dim flag As Boolean = Operators.CompareString(Strings.Mid(text, 1, 1), "[", False) = 0
                If flag Then
                    Dim flag2 As Boolean = Operators.CompareString(Strings.Trim(text.ToUpper()), Section.ToUpper(), False) = 0
                    If flag2 Then
                        While Not streamReader.EndOfStream
                            text = streamReader.ReadLine()
                            Dim flag3 As Boolean = Operators.CompareString(Strings.Mid(text, 1, 1), "[", False) = 0
                            If flag3 Then
                                Exit While
                            End If
                            Dim flag4 As Boolean = Operators.CompareString(Strings.Trim(Strings.Mid(text, 1, Strings.Len(Parameter)).ToUpper()), Parameter.ToUpper(), False) = 0
                            If flag4 Then
                                result = Strings.Trim(Strings.Mid(text, Strings.InStr(text, "=", CompareMethod.Binary) + 1))
                                Return result
                            End If
                        End While
                        Exit While
                    End If
                End If
            End While
        End Using
        result = ""
        Return result
    End Function

    Public Function Write(Section As String, Parameter As String, value As String) As Boolean
        Dim flag As Boolean = True
        Dim num As Integer = 0
        Section = "[" + Section + "]"
        ' The following expression was wrapped in a checked-statement
        Try
            Dim array As String()
            Using streamReader As StreamReader = New StreamReader(Me.Path)
                array = Strings.Split(streamReader.ReadToEnd(), vbCrLf, -1, CompareMethod.Binary)
            End Using
            Dim upperBound As Integer = array.GetUpperBound(0)
            For i As Integer = 0 To upperBound
                Dim flag2 As Boolean = Operators.CompareString(Strings.Mid(array(i), 1, 1), "[", False) = 0
                If flag2 Then
                    Dim flag3 As Boolean = Operators.CompareString(Strings.Trim(array(i).ToUpper()), Section.ToUpper(), False) = 0
                    If flag3 Then
                        Dim num2 As Integer = i + 1
                        While True
                            Dim flag4 As Boolean = Operators.CompareString(Strings.Mid(array(num2), 1, 1), "[", False) = 0 Or num2 > array.GetUpperBound(0)
                            If flag4 Then
                                Exit While
                            End If
                            Dim flag5 As Boolean = Operators.CompareString(Strings.Trim(Strings.Mid(array(num2), 1, Strings.Len(Parameter)).ToUpper()), Parameter.ToUpper(), False) = 0
                            If flag5 Then
                                GoTo Block_6
                            End If
                            num2 += 1
                        End While
                        GoTo IL_142
Block_6:
                        Dim str As String = Strings.Trim(Strings.Mid(array(num2), 1, Strings.InStr(array(num2), "=", CompareMethod.Binary)))
                        array(num2) = str + Strings.Trim(value)
                        num = 1
                    End If
IL_142:
                End If
                Dim flag6 As Boolean = num = 1
                If flag6 Then
                    Exit For
                End If
            Next
            Dim flag7 As Boolean = num = 0
            If flag7 Then
                flag = False
                flag = flag
                Return flag
            End If
            Using streamWriter As StreamWriter = New StreamWriter(Me.Path, False)
                Dim upperBound2 As Integer = array.GetUpperBound(0)
                For j As Integer = 0 To upperBound2
                    Dim flag8 As Boolean = j < array.GetUpperBound(0)
                    If flag8 Then
                        streamWriter.WriteLine(array(j))
                    Else
                        streamWriter.Write(array(j))
                    End If
                Next
            End Using
        Catch expr_1DD As Exception
            ProjectData.SetProjectError(expr_1DD)
            flag = False
            ProjectData.ClearProjectError()
        End Try
        flag = flag
        Return flag
    End Function
End Class
