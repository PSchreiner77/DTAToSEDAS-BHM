using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    public class SedasHeader
    {
        public string Headerline { get; private set; }

        public SedasHeader(string Erstelldatum, string Counter)
        {
            Headerline = $"010()000377777777777771{Erstelldatum};,{Counter}\n\r;)0240051310000002";
        }
    }

    public class SedasFooter
    {
        public string FooterLine { get; set; }

        public SedasFooter(string Customers, string DataSets)
        {
            //SedasFooter = String.Concat(New String() { ";06", this.Expand(Conversions.ToString(this._Customers), 3), ",", this.Expand(Conversions.ToString(this._DataSets), 4), vbCrLf & ";07000000,00001,00001,000000,("}) ;
            FooterLine = $"; 06{Customers expand to 3},{DataSets expand to 4}\n\r; 07000000,00001,00001,000000,(";
        }
    }

    public class SedasOrderHeader
    {
        public string OrderHeaderLine { get; private set; }

        public SedasOrderHeader()
        {
            //list.Add(String.Concat(New String() { ";030,14,00000000000000000,", this._DATContent(i, 3), ",", this.ReverseDate(this._DATContent(i, 4)), ",,,,", this._DATContent(i, 2), "         ,,"}));
            OrderHeaderLine = "";
        }
    }

    public class SedasOrderFooter
    {
        public string FooterLine { get; private set; }

        public SedasOrderFooter()
        {
            //list.Add(String.Concat(New String() { ";030,14,00000000000000000,", this._DATContent(i, 3), ",", this.ReverseDate(this._DATContent(i, 4)), ",,,,", this._DATContent(i, 2), "         ,,"}));
            int num2 = 12 - Strings.Len(text);
            for (int j = 0; j <= num; j++) // j As Integer = 1 To num2
            {
                text = "0" + text;
            }

            list.Add(";05" + text);
            i = num;
            this._SummeGes += Conversions.ToInteger(text);
            FooterLine = "";
        }
    }

    public class SedasOrderLine
    {
        public string OrderLine { get; private set; }

        public SedasOrderLine()
        {
            //list.Add(String.Concat(New String() { ";030,14,00000000000000000,", this._DATContent(i, 3), ",", this.ReverseDate(this._DATContent(i, 4)), ",,,,", this._DATContent(i, 2), "         ,,"}));
            string fix1 = ";040000";
            string fix2 = ",4";
            string fix3 = ",,,,02 000000,,";

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
            OrderLine = "";
        }
    }

    class SedasFile
    {
        public string SedasFileContent { get; set; }

        public string Erstelldatum { get; set; }
        public string Counter { get; set; }

        public string Customers { get; set; }
        public string DataSets { get; set; }


        public SedasFile(List<Bestellzeile> Bestellzeilen, string Erstelldatum, string Counter)
        {
            this.Erstelldatum = Erstelldatum;
            this.Counter = Counter;
        }

        private bool WriteSedasData()
        {
            LogMessage.LogOnly("Schreiben der Sedas.dat...");

            List<string> list = new List<string>();

            //bool result = false;
            //this._SedasHeader = String.Concat(New String() { "010()000377777777777771", this._ErstelldatumSedas, ";,", Conversions.ToString(this._Counter), vbCrLf & ";)0240051310000002"})  ;

            list.Add(new SedasHeader(Erstelldatum, Counter).Headerline);

            int i = 0;
            int j = 0;
            int upperBound = this._DATContent.GetUpperBound(0);


            // The following expression was wrapped in a checked-statement
            try
            {
                while (i <= this._DATContent.GetUpperBound(0))
                {
                    #region OrderHeader 
                    list.Add(String.Concat(New String() { ";030,14,00000000000000000,", this._DATContent(i, 3), ",", this.ReverseDate(this._DATContent(i, 4)), ",,,,", this._DATContent(i, 2), "         ,,"}));
                    list.Add(new SedasOrderHeader().OrderHeaderLine);
                    #endregion

                    this.Customers += 1;

                    string fix1 = ";040000";
                    string fix2 = ",4";
                    string fix3 = ",,,,02 000000,,";

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

                    #region OrderFooterLine
                    int num2 = 12 - Strings.Len(text);
                    for (int j = 0; j <= num; j++) // j As Integer = 1 To num2
                    {
                        text = "0" + text;
                    }
                    list.Add(";05" + text);
                    i = num;
                    this._SummeGes += Conversions.ToInteger(text);

                    list.Add(new SedasOrderFooter().FooterLine);
                    #endregion
                }


                //this._SedasFooter = String.Concat(New String() { ";06", this.Expand(Conversions.ToString(this._Customers), 3), ",", this.Expand(Conversions.ToString(this._DataSets), 4), vbCrLf & ";07000000,00001,00001,000000,("}) ;
                list.Add(new SedasFooter(Customers, DataSets).FooterLine);



                #region Zielverzeichnis erstellen, wenn nicht vorhanden
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
                #endregion

                #region Sedas-Datei schreiben
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
                #endregion

                result = true;


            }
            catch (Exception ex)
            {
                LogMessage.LogOnly(ex.ToString());
                result = false;
            }

            return result;
        }

    }
}
