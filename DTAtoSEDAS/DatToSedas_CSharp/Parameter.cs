using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    class Parameter
    {
        private string _Arguments;
        private string _SourceFileName;
        private string _SourceFilePath;
        private string _SourceFullPath;
        private string _DestinationFileName;
        private string _DestinationFilePath;
        private string _DestinationFullPath;
        private bool _DeleteSourceFile;
        private bool _IgnoreMessages;
        private bool _Help;
        private bool _AppendToSedas;
        private int _Counter;

        public string[] Arguments { get; set; }
        public string SourceFileName { get; set; }
        public string SourceFilePath { get; set; }
        public string SourceFullPath { get; set; }
        public string DestinationFileName { get; set; }
        public string DestinationFilePath { get; set; }
        public string DestinationFullPath { get; set; }
        public bool DeleteSourceFile { get; set; }
        public bool IgnoreMessages { get; set; }
        public bool Help { get; set; }
        public bool AppendToSedas { get; set; }
        public int Counter { get; set; }

        public void New()
        {
            Arguments = new string[] { };
            SourceFullPath = "";
            DestinationFullPath = "";
            DeleteSourceFile = false;
            IgnoreMessages = false;
            AppendToSedas = false;
        }

        public void SetSourceFullPath(string SrcFullPath)
        {
            if (SrcFullPath != "")
            {
                SourceFullPath = SrcFullPath;
            }
        }

        public void SetSourceFullPath(string SrcPath, string SrcName)
        {
            if (SrcPath != "")
            {
                if (SrcPath.Substring(SrcPath.Length, 1) != "\\")
                {
                    SrcPath += "\\";
                }
            }
            SourceFullPath = SourceFilePath + SourceFileName;
        }

        public void SetDestinationFullPath(string DestFullPath)
        {
            if (DestFullPath != "")
            {

                if (DestFullPath.Substring(DestFullPath.Length, 1) != "\\")
                {
                    DestFullPath += "\\";
                }
            }
            DestinationFullPath = DestFullPath;
        }

        public void SetDestinationFullPath(string DestPath, string DestName)
        {
            if (DestPath != "")
            {
                if (DestPath.Substring(DestPath.Length, 1) != "\\")
                {
                    DestPath += "\\";
                }
            }
            DestinationFullPath = DestinationFilePath + DestinationFileName;
        }
    }
}

//Imports Microsoft.VisualBasic.CompilerServices

//public Class Parameter
//    private _Arguments As String()

//    private _SourceFileName As String

//    private _SourceFilePath As String

//    private _SourceFullPath As String

//    private _DestinationFileName As String

//    private _DestinationFilePath As String

//    private _DestinationFullPath As String

//    private _DeleteSourceFile As Boolean

//    private _IgnoreMessages As Boolean

//    private _Help As Boolean

//    private _AppendToSedas As Boolean

//    private _Counter As Integer

//    public Property Arguments() As String()
//        get
//            return Me._Arguments
//        End get
//        set(value As String())
//            Me._Arguments = value
//        End set
//    End Property

//    public Property SourceFileName() As String
//        get
//            return Me._SourceFileName
//        End get
//        set(value As String)
//            Me._SourceFileName = value
//        End set
//    End Property

//    public Property SourceFilePath() As String
//        get
//            return Me._SourceFilePath
//        End get
//        set(value As String)
//            Me._SourceFilePath = value
//        End set
//    End Property

//    public Property SourceFullPath() As String
//        get
//            return Me._SourceFullPath
//        End get
//        private set(value As String)
//            Me._SourceFullPath = value
//        End set
//    End Property

//    public Property DestinationFileName() As String
//        get
//            return Me._DestinationFileName
//        End get
//        set(value As String)
//            Me._DestinationFileName = value
//        End set
//    End Property

//    public Property DestinationFilePath() As String
//        get
//            return Me._DestinationFilePath
//        End get
//        set(value As String)
//            Me._DestinationFilePath = value
//        End set
//    End Property

//    public Property DestinationFullPath() As String
//        get
//            return Me._DestinationFullPath
//        End get
//        private set(value As String)
//            Me._DestinationFullPath = value
//        End set
//    End Property

//    public Property DeleteSourceFile() As Boolean
//        get
//            return Me._DeleteSourceFile
//        End get
//        set(value As Boolean)
//            Me._DeleteSourceFile = value
//        End set
//    End Property

//    public Property IgnoreMessages() As Boolean
//        get
//            return Me._IgnoreMessages
//        End get
//        set(value As Boolean)
//            Me._IgnoreMessages = value
//        End set
//    End Property

//    public Property Help() As Boolean
//        get
//            return Me._Help
//        End get
//        set(value As Boolean)
//            Me._Help = value
//        End set
//    End Property

//    public Property Append() As Boolean
//        get
//            return Me._AppendToSedas
//        End get
//        set(value As Boolean)
//            Me._AppendToSedas = value
//        End set
//    End Property

//    public Property Counter() As Integer
//        get
//            return Me._Counter
//        End get
//        set(value As Integer)
//            Me._Counter = value
//        End set
//    End Property

//    public Sub New()
//        Me.Arguments = New String(-1) { }
//Me.SourceFullPath = ""
//        Me.DestinationFullPath = ""
//        Me.DeleteSourceFile = False
//        Me.IgnoreMessages = False
//        Me.Append = False
//    End Sub

//    public Sub SetSourceFullPath(SrcFullPath As String)
//        Dim flag As Boolean = Operators.CompareString(SrcFullPath, "", False) <> 0
//        If flag Then
//            Me.SourceFullPath = SrcFullPath
//        End If
//    End Sub

//    public Sub SetSourceFullPath(SrcPath As String, SrcName As String)
//        Dim flag As Boolean = Operators.CompareString(SrcPath, "", False) <> 0
//        If flag Then
//            Dim flag2 As Boolean = Operators.CompareString(Strings.Mid(SrcPath, Strings.Len(SrcPath), 1), "\", False) <> 0
//            If flag2 Then
//                SrcPath += "\"
//            End If
//        End If
//        Me.SourceFullPath = Me.SourceFilePath + Me.SourceFileName
//    End Sub

//    public Sub SetDestinationFullPath(DestFullPath As String)
//        Dim flag As Boolean = Operators.CompareString(DestFullPath, "", False) <> 0
//        If flag Then
//            Dim flag2 As Boolean = Strings.InStr(DestFullPath, "\", CompareMethod.Binary) <= 0
//            If flag2 Then
//                DestFullPath += "\"
//            End If
//        End If
//        Me.DestinationFullPath = DestFullPath
//    End Sub

//    public Sub SetDestinationFullPath(DestPath As String, DestName As String)
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
