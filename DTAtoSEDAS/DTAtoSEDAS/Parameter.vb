''' <summary>
''' Enthält alle wichtigen Parameterwerte.
''' </summary>
Public Class Parameter
    Private _Arguments As String()
    Private _SourceFileName As String
    Private _SourceFilePath As String
    Private _SourceFullPath As String
    Private _DestinationFileName As String
    Private _DestinationFilePath As String
    Private _DestinationFullPath As String
    Private _DeleteSourceFile As Boolean
    Private _IgnoreMessages As Boolean
    Private _Help As Boolean
    Private _AppendToSedas As Boolean
    Private _Counter As Integer

    'KONSTRUKTOR
    Sub New()
        Arguments = {}
        SourceFullPath = ""
        DestinationFullPath = ""
        DeleteSourceFile = False
        IgnoreMessages = False
        Append = False
    End Sub

    'PROPERTIES
    '----------
    Public Property Arguments() As String()
        Get
            Return _Arguments
        End Get
        Set(value As String())
            _Arguments = value
        End Set
    End Property

    Public Property SourceFileName() As String
        Get
            Return _SourceFileName
        End Get
        Set(value As String)
            _SourceFileName = value
        End Set
    End Property

    Public Property SourceFilePath() As String
        Get
            Return _SourceFilePath
        End Get
        Set(value As String)
            _SourceFilePath = value
        End Set
    End Property

    Public Property SourceFullPath() As String
        Get
            Return _SourceFullPath
        End Get
        Private Set(value As String)
            _SourceFullPath = value
        End Set
    End Property

    Public Property DestinationFileName() As String
        Get
            Return _DestinationFileName
        End Get
        Set(value As String)
            _DestinationFileName = value
        End Set
    End Property

    Public Property DestinationFilePath() As String
        Get
            Return _DestinationFilePath
        End Get
        Set(value As String)
            _DestinationFilePath = value
        End Set
    End Property

    Public Property DestinationFullPath() As String
        Get
            Return _DestinationFullPath
        End Get
        Private Set(value As String)
            _DestinationFullPath = value
        End Set
    End Property

    Public Property DeleteSourceFile() As Boolean
        Get
            Return _DeleteSourceFile
        End Get
        Set(value As Boolean)
            _DeleteSourceFile = value
        End Set
    End Property

    Public Property IgnoreMessages() As Boolean
        Get
            Return _IgnoreMessages
        End Get
        Set(value As Boolean)
            _IgnoreMessages = value
        End Set
    End Property

    Public Property Help() As Boolean
        Get
            Return _Help
        End Get
        Set(value As Boolean)
            _Help = value
        End Set
    End Property

    Public Property Append() As Boolean
        Get
            Return _AppendToSedas
        End Get
        Set(value As Boolean)
            _AppendToSedas = value
        End Set
    End Property

    Public Property Counter() As Integer
        Get
            Return _Counter
        End Get
        Set(value As Integer)
            _Counter = value
        End Set
    End Property


    'METHODEN
    '---------
    ''' <summary>
    ''' Volle Pfadangabe über Angabe des vollen Dateipfades. Gibt den kompletten Dateipfad zurück.
    ''' </summary>
    ''' <param name="SrcFullPath">Kompletter Dateipfad.</param>
    Public Sub SetSourceFullPath(SrcFullPath As String)
        If Not SrcFullPath = "" Then
            SourceFullPath = SrcFullPath
        End If
    End Sub

    ''' <summary>
    ''' Volle Pfadangabe über getrennte Angabe von Pfad und Dateiname. Gibt den kompletten Dateipfad zurück.
    ''' </summary>
    ''' <param name="SrcPath">Ordnerpfad, in der die Quelldatei liegt.</param>
    ''' <param name="SrcName">Dateiname der Quelldatei.</param>
    Public Sub SetSourceFullPath(SrcPath As String, SrcName As String)
        If Not SrcPath = "" Then
            If Mid(SrcPath, Len(SrcPath), 1) <> "\" Then
                SrcPath = SrcPath & "\"
            End If
        End If
        SourceFullPath = SourceFilePath & SourceFileName
    End Sub

    ''' <summary>
    ''' Setzt die volle Pfadangabe zur Zieldatei.
    ''' </summary>
    ''' <param name="DestFullPath">Dateipfad der Zieldatei.</param>
    Public Sub SetDestinationFullPath(DestFullPath As String)
        If Not DestFullPath = "" Then
            If InStr(DestFullPath, "\") <= 0 Then
                DestFullPath = DestFullPath & "\"
            End If
        End If
        DestinationFullPath = DestFullPath
    End Sub

    ''' <summary>
    ''' Setzt die volle Pfadangabe zur Zieldatei über getrennte Angabe von Pfad und Dateiname.
    ''' </summary>
    ''' <param name="DestPath">Ordnerpfad, in den die Zieldatei abgelegt werden soll.</param>
    ''' <param name="DestName">Dateiname der Zieldatei.</param>
    Public Sub SetDestinationFullPath(DestPath As String, DestName As String)
        If Not DestPath = "" Then
            If Mid(DestPath, Len(DestPath), 1) <> "\" Then
                DestPath = DestPath & "\"
            End If
        End If
        DestinationFullPath = DestinationFilePath & DestinationFileName
    End Sub

End Class
