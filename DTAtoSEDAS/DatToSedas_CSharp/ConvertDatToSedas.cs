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

            string[] array2 = new string[num] { }; //TODO  array mit festen dimensionen initialisieren
            int num2 = 0;
            int upperBound2 = array.GetUpperBound(0);
            for (int j = 0; j <= upperBound; j++)
            {
                if (array[j] == "")
                {
                    array2[num2] = array[j];
                    num2 += 1;
                }
            }
            array = array2;

            try
            {
                bool flag4 = Operators.CompareString(Strings.Mid(array(0), 1, 2), "NF", false) = 0;
                if (flag4)
                {
                    LogMessage.LogOnly("Einlesen neues Dateiformat...");
                    this._DATContent = this.ReadNewDATData(array, this._ErstelldatumSedas);
                }
                else
                {
                    LogMessage.LogOnly("Einlesen altes Dateiformat...");
                    this._DATContent = this.ReadDATData(array, this._ErstelldatumSedas);
                }

                bool flag5 = Not Information.IsNothing(this._DATContent) And this.WriteSedasData();
                if (flag5)
                {
                    LogMessage.LogOnly("Konvertierung in SEDAS.DAT abgeschlossen.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogMessage.LogOnly("Fehler beim Konvertieren in Sedas.dat." & vbCrLf + ex.ToString());

                return false;
            }
            return false;
        }


        private string[,] ReadDATData(string[] arrSourceLines, string ErstelldatumTTMMJJ)
        {
            bool flag = Operators.CompareString(arrSourceLines(0), "", false) = 0

                // The following expression was wrapped in a checked-statement
            string[,] array;
            if (arrSourceLines[0] == "")
            {
                array = null;
                array = array;
            }
            else
            {
                try
                {
                    int num = -1;
                    int upperBound = arrSourceLines.GetUpperBound(0);
                    for (int i = 0; i <= upperBound; i++)
                    {
                        bool flag2 = Operators.CompareString(arrSourceLines(i), "", false) <> 0;
                        if (flag2)
                            num += 1;
                    }

                    string[,] array2 = New String(num + 1 - 1, 10) { }; //Array dimensionieren
                    int num2 = 0;
                    int upperBound2 = arrSourceLines.GetUpperBound(0);
                    for (int j = 0; j < upperBound2; j++)
                    {
                        string left = arrSourceLines(j);
                        bool flag3 = Operators.CompareString(left, "", false) <> 0;
                        if (flag3)
                        {
                            array2(num2, 2) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 9, 8));
                            array2(num2, 3) = ErstelldatumTTMMJJ;
                            array2(num2, 4) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 18, 13));
                            array2(num2, 6) = this.Shorten(Strings.Mid(arrSourceLines(j), 73, 11), 7);
                            array2(num2, 8) = this.Expand(Strings.Trim(Strings.Mid(arrSourceLines(j), 31, 42)), 10);
                            num2 += 1;
                        }
                    }
                    array = this.DeleteEntries1(array2);
                }
                catch (Exception ex)
                {
                    array = null;
                }
            }
            return array;
        }


        private string[,] ReadNewDATData(string arrSourcelines, string ErstelldatumTTMMJJ)
        {
            // The following expression was wrapped in a checked-statement
            string[,] array3 = null;
            try
            {
                int num = -1;
                int upperBound = arrSourcelines.GetUpperBound(0);
                for (int i = 0; i <= upperBound; i++)
                {
                    bool flag = Operators.CompareString(arrSourcelines(i), "", false) <> 0;
                    if (flag)
                    {
                        num += 1;
                    }
                }

                string[,] array = new string[,] { };// (num + 1 - 1, 10) { };
                int num2 = 0;
                int upperBound2 = arrSourcelines.GetUpperBound(0);
                for (int j = 0; j <= upperBound2; j++)
                {
                    string[] array2 = Strings.Split(arrSourcelines(j), ";", -1, CompareMethod.Binary);
                    bool flag2 = Operators.CompareString(array2(0), "", false) <> 0;
                    if (flag2)
                    {
                        bool flag3 = Operators.CompareString(array(num2, 2), "1678", false) = 0 And Operators.CompareString(array(num2, 6), "222", false) = 0;
                        if (flag3)
                        {
                            Debugger.Break();
                        }
                        array(num2, 2) = array2(2);
                        array(num2, 3) = ErstelldatumTTMMJJ;
                        array(num2, 4) = array2(4);
                        array(num2, 6) = this.Expand(array2(6), 7);
                        array(num2, 8) = this.Expand(array2(8), 10);
                        num2 += 1;
                    }
                }
                array3 = this.DeleteEntries1(array);
                array3 = this.ChangeArticleNumbers(array3);
            }
            catch (Exception ex)
            {
            }
            return array3;
        }

        private string[,] DeleteEntries1(string[,] DATSource)
        {
            // The following expression was wrapped in a checked-statement
            string[,] array = New String(DATSource.GetUpperBound(0) + 1 - 1, DATSource.GetUpperBound(1) + 1 - 1) { };
            int num = 0;
            LogMessage.LogOnly("Löschen von nicht benötigten Kunden- und Artikeldaten laut loeschKunde.txt & loeschArtikel.txt.");
            int upperBound = DATSource.GetUpperBound(0);
            for ()// i As Integer = 0 To upperBound
            {
                bool flag = false;
                try
                {
                    Dim enumerator As List(Of String).Enumerator = Module1.ListDelCustomer.GetEnumerator();
                    while ()// enumerator.MoveNext()
                    {
                        string  current = enumerator.Current;
                        bool flag2 = Operators.CompareString(this.MyTRIM(DATSource(i, 2)), this.MyTRIM(current), false) = 0;
                        if (flag2)
                        {
                            flag = true;
                        }
                    }
                }
                finally
                {
                    Dim enumerator As List(Of String).Enumerator;
                    CType(enumerator, IDisposable).Dispose();
                }

                try
                {
                    Dim enumerator2 As List(Of String).Enumerator = Module1.ListDelArticle.GetEnumerator();
                    while ()// enumerator2.MoveNext()
                    {
                        string current2 enumerator2.Current;
                        bool flag3  = Operators.CompareString(this.MyTRIM(DATSource(i, 8)), this.MyTRIM(current2), false) = 0;
                        if (flag3)
                        {
                            flag = true;
                        }
                    }
                }
                finally
                {
                    Dim enumerator2 As List(Of String).Enumerator;
                    CType(enumerator2, IDisposable).Dispose();
                }

                bool flag4  = Not flag;
                if (flag4)
                {
                    int upperBound2 DATSource.GetUpperBound(1);
                    for ()// j As Integer = 0 To upperBound2
                    {
                        array(num, j) = DATSource(i, j);
                    }
                    num += 1;
                }
            }

            Dim array2 As String(,) = New String(num - 1 + 1 - 1, array.GetUpperBound(1) + 1 - 1) { };
            int num2 = 0;
            int upperBound3 = array.GetUpperBound(0);
            for ()// k As Integer = 0 To upperBound3
            {
                bool flag5 = Not Information.IsNothing(array(k, 2));
                if (flag5)
                {
                    int upperBound4 = array.GetUpperBound(1);
                    for ()// l As Integer = 0 To upperBound4
                    {
                        array2(num2, l) = array(k, l);
                    }
                    num2 += 1;
                }
            }
            return array2;
        }

        private string[,] ChangeArticleNumbers(DatSource As String(,))
        {
            LogMessage.LogOnly("Austauschen von Artikelnummern laut tauscheArtikel.txt.");
            bool flag = Not Information.IsNothing(Module1.ListChangeArticle);

            //The following expression was wrapped in a checked-statement
            string[,] result;
            if (flag)
            {
                string[,] array = New String(Module1.ListChangeArticle.Count - 1 + 1 - 1, 1) { };
                try
                {
                    int upperBound = array.GetUpperBound(0);
                    for (int i = 0; i <= upperBound; i++) // i As Integer = 0 To upperBound
                    {
                        Dim array2 As String() = Strings.Split(Module1.ListChangeArticle(i), ";", -1, CompareMethod.Binary);
                        array(i, 0) = this.MyTRIM(array2(0));
                        array(i, 1) = this.MyTRIM(array2(1));
                    }
                    int upperBound2 = DatSource.GetUpperBound(0);
                    for (int j = 0; j <= upperBound2; j++) // j As Integer = 0 To upperBound2
                    {
                        int upperBound3 = array.GetUpperBound(0);
                        for (int k = 0; k <= upperBound3; k++) // k As Integer = 0 To upperBound3
                        {
                            bool flag2 = Operators.CompareString(DatSource(j, 8), this.Expand(array(k, 0), 10), false) = 0;
                            if (flag2)
                            {
                                DatSource(j, 8) = this.Expand(array(k, 1), 10);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                result = DatSource;
            }

            result = DatSource;
        }





            return result;
        }

    private bool WriteSedasData()
    {
        LogMessage.LogOnly("Schreiben der Sedas.dat...");
        List<string> list = new List<string>();
        bool result = false;
        this._SedasHeader = String.Concat(New String() { "010()000377777777777771", this._ErstelldatumSedas, ";,", Conversions.ToString(this._Counter), vbCrLf & ";)0240051310000002"})  ;
        list.Add(this._SedasHeader);
        int i = 0;
        int j = 0;
        int upperBound = this._DATContent.GetUpperBound(0);


        // The following expression was wrapped in a checked-statement
        try
        {
            while (i <= this._DATContent.GetUpperBound(0))
            {
                list.Add(String.Concat(New String() { ";030,14,00000000000000000,", this._DATContent(i, 3), ",", this.ReverseDate(this._DATContent(i, 4)), ",,,,", this._DATContent(i, 2), "         ,,"}));
                this._Customers += 1;
                Dim text As String = "0";


                while (Operators.CompareString(this._DATContent(i, 2), this._DATContent(num, 2), false) = 0)
                {
                    this._DataSets += 1;
                    list.Add(String.Concat(New String() { ";040000", this._DATContent(num, 8), ",4", this._DATContent(num, 6), ",,,,02 000000,,"}));
                    text = Conversions.ToString(Conversions.ToInteger(text) + Conversions.ToInteger(this._DATContent(num, 6)));
                    num += 1;
                    Dim flag As Boolean = num > this._DATContent.GetUpperBound(0);

                    if (flag)
                    {
                        Exit While;
                    }
                }

                int num2 = 12 - Strings.Len(text);
                for (int j = 0; j <= num; j++) // j As Integer = 1 To num2
                {
                    text = "0" + text;
                }
                list.Add(";05" + text);
                i = num;
                this._SummeGes += Conversions.ToInteger(text);
            }

            this._SedasFooter = String.Concat(New String() { ";06", this.Expand(Conversions.ToString(this._Customers), 3), ",", this.Expand(Conversions.ToString(this._DataSets), 4), vbCrLf & ";07000000,00001,00001,000000,("}) ;
            list.Add(this._SedasFooter);
            Dim flag2 As Boolean = Strings.InStr(this._DestinationPath, "\\", CompareMethod.Binary) > 0;


            if (flag2)
            {
                Dim flag3 As Boolean = Not File.Exists(this._DestinationPath);
                if (flag3)
                {
                    bool flag4 = Not Directory.Exists(Strings.Mid(this._DestinationPath, 1, Strings.InStrRev(this._DestinationPath, "\\", -1, CompareMethod.Binary)));


                    if (flag4)
                    {
                        Directory.CreateDirectory(Strings.Mid(this._DestinationPath, 1, Strings.InStrRev(this._DestinationPath, "\\", -1, CompareMethod.Binary)));
                    }
                }
            }
            else
            {
                this._DestinationPath = Directory.GetCurrentDirectory() + "\\" + this._DestinationPath;
            }


            using (StreamWriter sw = new StreamWriter(this._DestinationPath, false))
            {
                try
                {
                    Dim enumerator As List(Of String).Enumerator = list.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Dim current As String = enumerator.Current;
                        streamWriter.WriteLine(current);
                    }
                }
                finally
                {
                    Dim enumerator As List(Of String).Enumerator;
                    CType(enumerator, IDisposable).Dispose();
                }

                sw.WriteLine("                                                                                    ");
            }
            result = true;


        }
        catch (Exception ex)
        {
            LogMessage.LogOnly(ex.ToString());
            result = false;
        }

        return result;
    }

    private string ReverseDate(string str)
    {
        string text = "";
        bool flag = Operators.CompareString(str, "", false) <> 0;

        //The following expression was wrapped in a checked-statement
        if (flag)
            str = Strings.Trim(str);
        int num = Strings.Len(str) - 1;
        for (int i = 0; i >= num; i = i - 2) //i As Integer = num To 1 Step - 2
        {
            Dim str2 As String = Strings.Mid(str, i, 2);
            text += str2;
        }
        return text;
    }

    private string MyTRIM(string MyText)
    {
        MyText = Strings.Trim(MyText);
        while (Operators.CompareString(Strings.Mid(MyText, 1, 1), "0", false) = 0)
        {
            MyText = Strings.Mid(MyText, 2);
        }
        return MyText;
    }

    private string Shorten(string input, int limit)
    {
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

    private string Expand(string input, int limit)
    {
        int num = input.Length;
        string text = "";
        bool flag = num < limit;

        // The following expression was wrapped in a checked-statement                
        if (flag)
        {
            int num2 = limit - num;
            for (int i = 0; i <= num2; i++)// i As Integer = 1 To num2
            {
                text += "0";
            }
            text += input;
        }
        else
        {
            text = input;
        }
        return text;
    }
}
}