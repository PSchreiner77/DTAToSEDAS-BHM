using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatToSedas_CSharp
{
    class ConvertDatToSedas
    {
        private string _SourcePath;
        private string _SourceData;
        private string _DestinationPath;
        private string _DestinationData;
        private string _SedasHeader;
        private string _SedasFooter;
        private string _BlockHeader;
        private string[,] _DATContent;
        private int _DataSets;
        private int _Customers;
        private int _SummeGes;
        private List<string> _ListDelCustomer = new List<string>();
        private List<string> _ListDelArticle = new List<string>();
        private int _Counter;
        private string _ErstelldatumSedas;


        //KONSTRUKTOR
        public ConvertDatToSedas(string src, string dst, int cnt, List<string> dC = null, List<string> dA = null)
        {
            this._SourcePath = "";
            this._SourceData = "";
            this._DestinationPath = "";
            this._DestinationData = "";
            this._SedasHeader = "";
            this._SedasFooter = "";
            this._BlockHeader = "";
            this._DataSets = 0;
            this._Customers = 0;
            this._SummeGes = 0;
            this._Counter = 0;
            this._ErstelldatumSedas = "";
            this._SourcePath = src;
            this._DestinationPath = dst;
            this._Counter = cnt;
            this._ListDelArticle = dA;
            this._ListDelCustomer = dC;
        }

        public bool ConvertFile()
        {
            bool result = false;
            LogMessage.LogOnly("Beginn der Konvertierung...");
            this._ErstelldatumSedas = this.ReverseDate(Strings.Mid(DateAndTime.Now.ToString(), 1, 10).Replace(".", "").Remove(4, 2));

            try
            {
                if (!File.Exists(_SourcePath))
                {
                    return false; ;
                }
                LogMessage.LogOnly("Einlesen der Quelldatei...");

                using (StreamReader sr = new StreamReader(_SourcePath))
                {
                    this._SourceData = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            string[] array = Strings.Split(this._SourceData, vbCrLf, -1, CompareMethod.Binary);
            int num = 0;
            int upperBound = array.GetUpperBound(0);
            // The following expression was wrapped in a checked-statement
            for (int i = 0; i <= upperBound; i++)
            {
                if (array[i] == "")
                {
                    num += 1;
                }
            }

            string[] array2 = new string[num] { }; //TODO  ??
            Dim num2 As Integer = 0
            Dim upperBound2 As Integer = array.GetUpperBound(0)
            For j As Integer = 0 To upperBound2
                Dim flag3 As Boolean = Operators.CompareString(array(j), "", false) <> 0
                If flag3 Then
                    array2(num2) = array(j)
                    num2 += 1
                End If
            Next
        array = array2

            Try
            Dim flag4 As Boolean = Operators.CompareString(Strings.Mid(array(0), 1, 2), "NF", false) = 0


                If flag4 Then
                    LogMessage.LogOnly("Einlesen neues Dateiformat...")
                    this._DATContent = this.ReadNewDATData(array, this._ErstelldatumSedas)
                Else
                    LogMessage.LogOnly("Einlesen altes Dateiformat...")
                    this._DATContent = this.ReadDATData(array, this._ErstelldatumSedas)
                End If

                Dim flag5 As Boolean = Not Information.IsNothing(this._DATContent) And this.WriteSedasData()


                If flag5 Then
                    LogMessage.LogOnly("Konvertierung in SEDAS.DAT abgeschlossen.")
                    result = true
                End If


                Catch expr_1D8 As Exception
                ProjectData.SetProjectError(expr_1D8)
                Dim ex As Exception = expr_1D8
                LogMessage.LogOnly("Fehler beim Konvertieren in Sedas.dat." & vbCrLf + ex.ToString())
                result = false
                ProjectData.ClearProjectError()
            End Try

            return result;
        }

        private string[,] ReadDATData(arrSourceLines As String(), ErstelldatumTTMMJJ As String)
        {
            Dim flag As Boolean = Operators.CompareString(arrSourceLines(0), "", false) = 0
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
                            Dim flag2 As Boolean = Operators.CompareString(arrSourceLines(i), "", false) <> 0
                            If flag2 Then
                        num += 1
                            End If
                        Next
                Dim array2 As String(,) = New String(num + 1 - 1, 10) { }
            Dim num2 As Integer = 0
                        Dim upperBound2 As Integer = arrSourceLines.GetUpperBound(0)
                        For j As Integer = 0 To upperBound2
                            Dim left As String = arrSourceLines(j)
                            Dim flag3 As Boolean = Operators.CompareString(left, "", false) <> 0
                            If flag3 Then
                                array2(num2, 2) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 9, 8))
                                array2(num2, 3) = ErstelldatumTTMMJJ
                                array2(num2, 4) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 18, 13))
                                array2(num2, 6) = this.Shorten(Strings.Mid(arrSourceLines(j), 73, 11), 7)
                                array2(num2, 8) = this.Expand(Strings.Trim(Strings.Mid(arrSourceLines(j), 31, 42)), 10)
                                num2 += 1
                            End If
                        Next
                array = this.DeleteEntries1(array2)
                    Catch expr_139 As Exception
                        ProjectData.SetProjectError(expr_139)
                        array = Nothing
                        ProjectData.ClearProjectError()
                    End Try
                End If
    

                return array;
        }

        private string[,] ReadNewDATData(arrSourcelines As String(), ErstelldatumTTMMJJ As String)
        {
            ' The following expression was wrapped in a checked-statement
                Dim array3 As String(,)
                Try
            Dim num As Integer = -1
                    Dim upperBound As Integer = arrSourcelines.GetUpperBound(0)
                    For i As Integer = 0 To upperBound
                        Dim flag As Boolean = Operators.CompareString(arrSourcelines(i), "", false) <> 0
                        If flag Then
                    num += 1
                        End If
                    Next
            Dim array As String(,) = New String(num + 1 - 1, 10) { }
            Dim num2 As Integer = 0
                    Dim upperBound2 As Integer = arrSourcelines.GetUpperBound(0)
                    For j As Integer = 0 To upperBound2
                        Dim array2 As String() = Strings.Split(arrSourcelines(j), ";", -1, CompareMethod.Binary)
                        Dim flag2 As Boolean = Operators.CompareString(array2(0), "", false) <> 0
                        If flag2 Then
                    Dim flag3 As Boolean = Operators.CompareString(array(num2, 2), "1678", false) = 0 And Operators.CompareString(array(num2, 6), "222", false) = 0
                            If flag3 Then
                                Debugger.Break()
                            End If
                            array(num2, 2) = array2(2)
                            array(num2, 3) = ErstelldatumTTMMJJ
                            array(num2, 4) = array2(4)
                            array(num2, 6) = this.Expand(array2(6), 7)
                            array(num2, 8) = this.Expand(array2(8), 10)
                            num2 += 1
                        End If
                    Next
            array3 = this.DeleteEntries1(array)
                    array3 = this.ChangeArticleNumbers(array3)
                Catch expr_12D As Exception
                    ProjectData.SetProjectError(expr_12D)
                    array3 = Nothing
                    ProjectData.ClearProjectError()
                End Try
                array3 = array3
    

                return array3;
        }

        private string[,] DeleteEntries1(DATSource As String(,))
        {
            ' The following expression was wrapped in a checked-statement
                Dim array As String(,) = New String(DATSource.GetUpperBound(0) + 1 - 1, DATSource.GetUpperBound(1) + 1 - 1) { }
            Dim num As Integer = 0
                LogMessage.LogOnly("Löschen von nicht benötigten Kunden- und Artikeldaten laut loeschKunde.txt & loeschArtikel.txt.")
                Dim upperBound As Integer = DATSource.GetUpperBound(0)
                For i As Integer = 0 To upperBound
                    Dim flag As Boolean = false
                    Try
                Dim enumerator As List(Of String).Enumerator = Module1.ListDelCustomer.GetEnumerator()
                        While enumerator.MoveNext()
                            Dim current As String = enumerator.Current
                            Dim flag2 As Boolean = Operators.CompareString(this.MyTRIM(DATSource(i, 2)), this.MyTRIM(current), false) = 0
                            If flag2 Then
                        flag = true
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
                            Dim flag3 As Boolean = Operators.CompareString(this.MyTRIM(DATSource(i, 8)), this.MyTRIM(current2), false) = 0
                            If flag3 Then
                        flag = true
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
        Dim array2 As String(,) = New String(num - 1 + 1 - 1, array.GetUpperBound(1) + 1 - 1) { }
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
    

                return array2;
        }

        private string[,] ChangeArticleNumbers(DatSource As String(,))
        {
            LogMessage.LogOnly("Austauschen von Artikelnummern laut tauscheArtikel.txt.")
                Dim flag As Boolean = Not Information.IsNothing(Module1.ListChangeArticle)
                ' The following expression was wrapped in a checked-statement
                Dim result As String(,)
                If flag Then
            Dim array As String(,) = New String(Module1.ListChangeArticle.Count - 1 + 1 - 1, 1) { }
            Try
                Dim upperBound As Integer = array.GetUpperBound(0)
                        For i As Integer = 0 To upperBound
                            Dim array2 As String() = Strings.Split(Module1.ListChangeArticle(i), ";", -1, CompareMethod.Binary)
                            array(i, 0) = this.MyTRIM(array2(0))
                            array(i, 1) = this.MyTRIM(array2(1))
                        Next
                Dim upperBound2 As Integer = DatSource.GetUpperBound(0)
                        For j As Integer = 0 To upperBound2
                            Dim upperBound3 As Integer = array.GetUpperBound(0)
                            For k As Integer = 0 To upperBound3
                                Dim flag2 As Boolean = Operators.CompareString(DatSource(j, 8), this.Expand(array(k, 0), 10), false) = 0
                                If flag2 Then
                                    DatSource(j, 8) = this.Expand(array(k, 1), 10)
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
    

                return result;
        }

        private bool WriteSedasData()
        {
            LogMessage.LogOnly("Schreiben der Sedas.dat...")
                Dim list As List(Of String) = New List(Of String)()
                Dim result As Boolean = false
                this._SedasHeader = String.Concat(New String() { "010()000377777777777771", this._ErstelldatumSedas, ";,", Conversions.ToString(this._Counter), vbCrLf & ";)0240051310000002"})
        list.Add(this._SedasHeader)
                Dim i As Integer = 0
                Dim num As Integer = 0
                Dim upperBound As Integer = this._DATContent.GetUpperBound(0)
                ' The following expression was wrapped in a checked-statement
                Try
            While i <= this._DATContent.GetUpperBound(0)
                        list.Add(String.Concat(New String() { ";030,14,00000000000000000,", this._DATContent(i, 3), ",", this.ReverseDate(this._DATContent(i, 4)), ",,,,", this._DATContent(i, 2), "         ,,"}))
                this._Customers += 1
                        Dim text As String = "0"
                        While Operators.CompareString(this._DATContent(i, 2), this._DATContent(num, 2), false) = 0
                            this._DataSets += 1
                            list.Add(String.Concat(New String() { ";040000", this._DATContent(num, 8), ",4", this._DATContent(num, 6), ",,,,02 000000,,"}))
                    text = Conversions.ToString(Conversions.ToInteger(text) + Conversions.ToInteger(this._DATContent(num, 6)))
                            num += 1
                            Dim flag As Boolean = num > this._DATContent.GetUpperBound(0)
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
                        this._SummeGes += Conversions.ToInteger(text)
                    End While
                    this._SedasFooter = String.Concat(New String() { ";06", this.Expand(Conversions.ToString(this._Customers), 3), ",", this.Expand(Conversions.ToString(this._DataSets), 4), vbCrLf & ";07000000,00001,00001,000000,("})
            list.Add(this._SedasFooter)
                    Dim flag2 As Boolean = Strings.InStr(this._DestinationPath, "\", CompareMethod.Binary) > 0
            If flag2 Then
                Dim flag3 As Boolean = Not File.Exists(this._DestinationPath)
                If flag3 Then
                    Dim flag4 As Boolean = Not Directory.Exists(Strings.Mid(this._DestinationPath, 1, Strings.InStrRev(this._DestinationPath, "\", -1, CompareMethod.Binary)))
                    If flag4 Then
                        Directory.CreateDirectory(Strings.Mid(this._DestinationPath, 1, Strings.InStrRev(this._DestinationPath, "\", -1, CompareMethod.Binary)))
                    End If
                End If
            Else
                this._DestinationPath = Directory.GetCurrentDirectory() + "\" + this._DestinationPath
            End If
            Using streamWriter As StreamWriter = New StreamWriter(this._DestinationPath, false)
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
            result = true
        Catch expr_39B As Exception
            ProjectData.SetProjectError(expr_39B)
            Dim ex As Exception = expr_39B
            LogMessage.LogOnly(ex.ToString())
            Interaction.MsgBox(ex.ToString(), MsgBoxStyle.OkOnly, Nothing)
            result = false
            ProjectData.ClearProjectError()
        End Try
    

        return result;
        }

        private string ReverseDate(str As String)
        {
            Dim text As String = ""
                Dim flag As Boolean = Operators.CompareString(str, "", false) <> 0
                ' The following expression was wrapped in a checked-statement
                If flag Then
            str = Strings.Trim(str)
                    Dim num As Integer = Strings.Len(str) - 1
                    For i As Integer = num To 1 Step - 2
                        Dim str2 As String = Strings.Mid(str, i, 2)
                        text += str2
                    Next
        End If
    

                return text;
        }

        private string MyTRIM(MyText As String)
        {
            MyText = Strings.Trim(MyText)
                While Operators.CompareString(Strings.Mid(MyText, 1, 1), "0", false) = 0
                    MyText = Strings.Mid(MyText, 2)
                End While
                Return MyText
           }

        private string Shorten(input As String, limit As Integer)
        Dim num As Integer = Strings.Len(input)
        Dim flag As Boolean = num > limit
        Dim result As String
        If flag Then
            ' The following expression was wrapped in a checked-expression
            result = Strings.Mid(input, num - limit + 1)
        Else
            result = input
        End If
        
            return result;
    }

    private string Expand(input As String, limit As Integer)
    {
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
    

            return text;
    }
}


