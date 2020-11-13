using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    class Parameter
    {
    }
}

//Imports Microsoft.VisualBasic.CompilerServices

//Public Class Parameter
//    Private _Arguments As String()

//    Private _SourceFileName As String

//    Private _SourceFilePath As String

//    Private _SourceFullPath As String

//    Private _DestinationFileName As String

//    Private _DestinationFilePath As String

//    Private _DestinationFullPath As String

//    Private _DeleteSourceFile As Boolean

//    Private _IgnoreMessages As Boolean

//    Private _Help As Boolean

//    Private _AppendToSedas As Boolean

//    Private _Counter As Integer

//    Public Property Arguments() As String()
//        Get
//            Return Me._Arguments
//        End Get
//        Set(value As String())
//            Me._Arguments = value
//        End Set
//    End Property

//    Public Property SourceFileName() As String
//        Get
//            Return Me._SourceFileName
//        End Get
//        Set(value As String)
//            Me._SourceFileName = value
//        End Set
//    End Property

//    Public Property SourceFilePath() As String
//        Get
//            Return Me._SourceFilePath
//        End Get
//        Set(value As String)
//            Me._SourceFilePath = value
//        End Set
//    End Property

//    Public Property SourceFullPath() As String
//        Get
//            Return Me._SourceFullPath
//        End Get
//        Private Set(value As String)
//            Me._SourceFullPath = value
//        End Set
//    End Property

//    Public Property DestinationFileName() As String
//        Get
//            Return Me._DestinationFileName
//        End Get
//        Set(value As String)
//            Me._DestinationFileName = value
//        End Set
//    End Property

//    Public Property DestinationFilePath() As String
//        Get
//            Return Me._DestinationFilePath
//        End Get
//        Set(value As String)
//            Me._DestinationFilePath = value
//        End Set
//    End Property

//    Public Property DestinationFullPath() As String
//        Get
//            Return Me._DestinationFullPath
//        End Get
//        Private Set(value As String)
//            Me._DestinationFullPath = value
//        End Set
//    End Property

//    Public Property DeleteSourceFile() As Boolean
//        Get
//            Return Me._DeleteSourceFile
//        End Get
//        Set(value As Boolean)
//            Me._DeleteSourceFile = value
//        End Set
//    End Property

//    Public Property IgnoreMessages() As Boolean
//        Get
//            Return Me._IgnoreMessages
//        End Get
//        Set(value As Boolean)
//            Me._IgnoreMessages = value
//        End Set
//    End Property

//    Public Property Help() As Boolean
//        Get
//            Return Me._Help
//        End Get
//        Set(value As Boolean)
//            Me._Help = value
//        End Set
//    End Property

//    Public Property Append() As Boolean
//        Get
//            Return Me._AppendToSedas
//        End Get
//        Set(value As Boolean)
//            Me._AppendToSedas = value
//        End Set
//    End Property

//    Public Property Counter() As Integer
//        Get
//            Return Me._Counter
//        End Get
//        Set(value As Integer)
//            Me._Counter = value
//        End Set
//    End Property

//    Public Sub New()
//        Me.Arguments = New String(-1) { }
//Me.SourceFullPath = ""
//        Me.DestinationFullPath = ""
//        Me.DeleteSourceFile = False
//        Me.IgnoreMessages = False
//        Me.Append = False
//    End Sub

//    Public Sub SetSourceFullPath(SrcFullPath As String)
//        Dim flag As Boolean = Operators.CompareString(SrcFullPath, "", False) <> 0
//        If flag Then
//            Me.SourceFullPath = SrcFullPath
//        End If
//    End Sub

//    Public Sub SetSourceFullPath(SrcPath As String, SrcName As String)
//        Dim flag As Boolean = Operators.CompareString(SrcPath, "", False) <> 0
//        If flag Then
//            Dim flag2 As Boolean = Operators.CompareString(Strings.Mid(SrcPath, Strings.Len(SrcPath), 1), "\", False) <> 0
//            If flag2 Then
//                SrcPath += "\"
//            End If
//        End If
//        Me.SourceFullPath = Me.SourceFilePath + Me.SourceFileName
//    End Sub

//    Public Sub SetDestinationFullPath(DestFullPath As String)
//        Dim flag As Boolean = Operators.CompareString(DestFullPath, "", False) <> 0
//        If flag Then
//            Dim flag2 As Boolean = Strings.InStr(DestFullPath, "\", CompareMethod.Binary) <= 0
//            If flag2 Then
//                DestFullPath += "\"
//            End If
//        End If
//        Me.DestinationFullPath = DestFullPath
//    End Sub

//    Public Sub SetDestinationFullPath(DestPath As String, DestName As String)
//        Dim flag As Boolean = Operators.CompareString(DestPath, "", False) <> 0
//        If flag Then
//            Dim flag2 As Boolean = Operators.CompareString(Strings.Mid(DestPath, Strings.Len(DestPath), 1), "\", False) <> 0
//            If flag2 Then
//                DestPath += "\"
//            End If
//        End If
//        Me.DestinationFullPath = Me.DestinationFilePath + Me.DestinationFileName
//    End Sub
//End Class
