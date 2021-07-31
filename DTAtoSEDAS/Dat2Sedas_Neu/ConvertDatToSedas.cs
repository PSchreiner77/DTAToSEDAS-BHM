﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{
    class ConvertDatToSedas
    {
        private string _SourcePath;
        private List<string> _SourceData;
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
        public ConvertDatToSedas(string SourceFilePath, string DestinationFilePath, int CounterEntries, List<string> CustomersToDelete = null, List<string> ArticlesToDelete = null)
        {
            this._DestinationData = "";
            this._SedasHeader = "";
            this._SedasFooter = "";
            this._BlockHeader = "";
            this._DataSets = 0;
            this._Customers = 0;
            this._SummeGes = 0;
            this._ErstelldatumSedas = "";
            this._SourcePath = SourceFilePath;
            this._DestinationPath = DestinationFilePath;
            this._Counter = CounterEntries;
            this._ListDelArticle = ArticlesToDelete;
            this._ListDelCustomer = CustomersToDelete;
        }

        public bool ConvertFile()
        {
            LogMessage.LogOnly("Beginn der Konvertierung...");

            this._ErstelldatumSedas = ConvertToSedasDate(DateTime.Now);

            _SourceData = ImportSourceFile(_SourcePath);
            if (_SourceData == null) return false;

            #region Als Delegate bauen ReadDatData
            if (checkIfNewFileFormat())
            {
                //LogMessage.LogOnly("Einlesen neues Dateiformat...");
                this._DATContent = ReadNewNFDATDataFormat(array, this._ErstelldatumSedas);
            }
            else
            {
                //LogMessage.LogOnly("Einlesen neues Dateiformat...");
                this._DATContent = ReadOldDATDataFormat(array, this._ErstelldatumSedas);
            }
            #endregion

            //Sedas erstellen
            // ...
            // LogMessage.LogOnly("Konvertierung in SEDAS.DAT abgeschlossen.");
            // LogMessage.LogOnly("Fehler beim Konvertieren in Sedas.dat." & vbCrLf + ex.ToString());


            return false;
        }


        private List<string> ImportSourceFile(string SourceFilePath)
        {
            List<string> sourceDataList = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(_SourcePath))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != "") sourceDataList.Add(line);
                    }
                }

                //TODO Prüfen, ob neues Dateiformat "NF" und Delegate zuweisen
                //ReadDatData += ReadNewNFDATDataFormat
                //  ODER
                //ReadDatData += ReadOldDATDataFormat
            }
            catch (Exception ex)
            { //Fehlerausnahme auslösen und Fehler melden}                
            }

            return sourceDataList;
        }

        private bool checkIfNewFileFormat()
        {
            string prefix = _SourceData[0].Substring(0, 2);
            if (prefix == "NF") return true;
            return false;
        }

        private string[,] ReadOldDATDataFormat(string[] arrSourceLines, string ErstelldatumTTMMJJ)
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


        private List<Bestellzeile> ReadNewNFDATDataFormat(List<string> NewNFDATData, string ErstelldatumTTMMJJ)
        {
            /*
            Zeile aus neuer NF-DAT-Datei:            
            0 ;1   ;2   ;3     ;4     ;5;6   ;7;8 ;9;10
            NF;1050;1050;200924;200925;;20000;;209;;10
            0  NF               Kennzeichen neues Format
            1  FilNr            BHM Filialnummer der Filiale
            2  KdNr             BHM Kundennummer der Filiale
            3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen
            4  Lieferdatum      Lieferdatum
            5  -leer-           BHM ArtikelKey
            6  20000            Menge*1000, echte Menge=Menge/1000> 20000=20
            7  -leer-           Preis (Bsp. 2.00), Dezimal = .
            8  ArtNummer        BHM ArtikelNummer
            9  -leer-           VPE
            10 10               Anzahl Bestellpositionen in Datei
            */

            List<Bestellzeile> Bestellzeilen = new List<Bestellzeile>();

            foreach (string eintrag in NewNFDATData)
            {
                string[] arrZeile = eintrag.Split(';');
                Bestellzeile bestellzeile = new Bestellzeile();
                bestellzeile.NFKennzeichen = arrZeile[0];
                bestellzeile.BHMFilialNummer = arrZeile[1];
                bestellzeile.BHMKundenNummer = arrZeile[2];
                bestellzeile.BestellDatum = arrZeile[3];
                bestellzeile.LieferDatum = arrZeile[4];
                bestellzeile.BHMArtikelKey = arrZeile[5];
                bestellzeile.BestellMenge = arrZeile[6];
                bestellzeile.Preis = arrZeile[7];
                bestellzeile.BHMArtikelNummer = arrZeile[8];
                bestellzeile.Verpackungseinheit = arrZeile[9];
                bestellzeile.AnzahlBestellPositionen = arrZeile[10];

                Bestellzeilen.Add(bestellzeile);
            }

            Bestellzeilen = DeleteCustomers(Bestellzeilen);        //Zu löschende Einträge entfernen
            Bestellzeilen = DeleteArticles(Bestellzeilen);
            Bestellzeilen = ChangeArticleNumbers(Bestellzeilen); //zu tauschende Artikelnummern tauschen

            return Bestellzeilen;
        }

        private List<Bestellzeile> DeleteCustomers(List<Bestellzeile> bestellzeilen)
        {
            bool flag = false;
            try
            {
                Dim enumerator As List(Of String).Enumerator = Module1.ListDelCustomer.GetEnumerator();
                while ()// enumerator.MoveNext()
                {
                    string current = enumerator.Current;
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

            return bestellzeilen;
        }

        private List<Bestellzeile> DeleteArticles(List<Bestellzeile> bestellzeilen )
        {
            try
            {
                Dim enumerator2 As List(Of String).Enumerator = Module1.ListDelArticle.GetEnumerator();
                while ()// enumerator2.MoveNext()
                {
                    string current2 enumerator2.Current;
                    bool flag3 = Operators.CompareString(this.MyTRIM(DATSource(i, 8)), this.MyTRIM(current2), false) = 0;
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

            return bestellzeilen;

        }

        private List<Bestellzeile> ChangeArticleNumbers(List<Bestellzeile> bestellzeile)
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

            return result;
        }

        private List<string> LoadDeleteArticlesList(string path)
        {
            List<string> delArticles = new List<string>();
            try
            {
                delArticles = File.ReadAllText("Pfad").Split('\n').ToList<string>();
            }
            catch (Exception ex)
            { }
            return delArticles;
        }

        private void LoadDeleteCustomersList()
        {

        }

        private List<string> LoadDeleteItemsList(string path)
        {
            List<string> delItems = new List<string>();
            try
            {
                delItems = File.ReadAllText("Pfad").Split('\n').ToList<string>();
            }
            catch (Exception ex)
            { }
            return delItems;
        }

        private void LoadChangeArticlesList()
        {

        }

        //private string[,] DeleteArticlesAndCustomers(string[,] DATSource)
        //{
        //    // The following expression was wrapped in a checked-statement
        //    string[,] arrDatenarray = New String(DATSource.GetUpperBound(0) + 1 - 1, DATSource.GetUpperBound(1) + 1 - 1) { };

        //    int num = 0;
        //    LogMessage.LogOnly("Löschen von nicht benötigten Kunden- und Artikeldaten laut loeschKunde.txt & loeschArtikel.txt.");

        //    // ## Customer löschen
        //    //int upperBound = DATSource.GetUpperBound(0);
        //    //for ()// i As Integer = 0 To upperBound
        //    //{
        //    //    bool flag = false;
        //    //    try
        //    //    {
        //    //        Dim enumerator As List(Of String).Enumerator = Module1.ListDelCustomer.GetEnumerator();
        //    //        while ()// enumerator.MoveNext()
        //    //        {
        //    //            string current = enumerator.Current;
        //    //            bool flag2 = Operators.CompareString(this.MyTRIM(DATSource(i, 2)), this.MyTRIM(current), false) = 0;
        //    //            if (flag2)
        //    //            {
        //    //                flag = true;
        //    //            }
        //    //        }
        //    //    }
        //    //    finally
        //    //    {
        //    //        Dim enumerator As List(Of String).Enumerator;
        //    //        CType(enumerator, IDisposable).Dispose();
        //    //    }


        //    //Artikel löschen
        //    //try
        //    //{
        //    //    Dim enumerator2 As List(Of String).Enumerator = Module1.ListDelArticle.GetEnumerator();
        //    //    while ()// enumerator2.MoveNext()
        //    //    {
        //    //        string current2 enumerator2.Current;
        //    //        bool flag3 = Operators.CompareString(this.MyTRIM(DATSource(i, 8)), this.MyTRIM(current2), false) = 0;
        //    //        if (flag3)
        //    //        {
        //    //            flag = true;
        //    //        }
        //    //    }
        //    //}
        //    //finally
        //    //{
        //    //    Dim enumerator2 As List(Of String).Enumerator;
        //    //    CType(enumerator2, IDisposable).Dispose();
        //    //}


        //    //Daten übernehmen in neues Array
        //        bool flag4 = Not flag;
        //        if (flag4)
        //        {
        //            int upperBound2 DATSource.GetUpperBound(1);
        //            for ()// j As Integer = 0 To upperBound2
        //            {
        //                arrDatenarray(num, j) = DATSource(i, j);
        //            }
        //            num += 1;
        //        }
        //    //}


        ////Daten  übernehmen in Ausgabearray
        //    Dim arrReturn As String(,) = New String(num - 1 + 1 - 1, arrDatenarray.GetUpperBound(1) + 1 - 1) { };
        //    int num2 = 0;
        //    int upperBound3 = arrDatenarray.GetUpperBound(0);
        //    for ()// k As Integer = 0 To upperBound3
        //    {
        //        bool flag5 = Not Information.IsNothing(arrDatenarray(k, 2));
        //        if (flag5)
        //        {
        //            int upperBound4 = arrDatenarray.GetUpperBound(1);
        //            for ()// l As Integer = 0 To upperBound4
        //            {
        //                array2(num2, l) = arrDatenarray(k, l);
        //            }
        //            num2 += 1;
        //        }
        //    }

        //    return array2;
        //}




       

        //private string ReverseDate(string str)
        //{
        //    string text = "";
        //    bool flag = Operators.CompareString(str, "", false) <> 0;

        //    //The following expression was wrapped in a checked-statement
        //    if (flag)
        //        str = Strings.Trim(str);
        //    int num = Strings.Len(str) - 1;
        //    for (int i = 0; i >= num; i = i - 2) //i As Integer = num To 1 Step - 2
        //    {
        //        Dim str2 As String = Strings.Mid(str, i, 2);
        //        text += str2;
        //    }
        //    return text;
        //}

        /// <summary>
        /// Gibt ein Datum als String zurück in der Form: 'JJMMTT'
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>String: 'JJMMTT'</returns>
        private string ConvertToSedasDate(DateTime date)
        {
            string JJ = date.Year.ToString().Substring(2, 2);
            string MM = date.Month.ToString();
            string TT = date.Day.ToString();
            return JJ + MM + TT;
        }

        private string ConvertToSedasDate(string DateTTMMJJ)
        {
            string returnString = "";
            for (int i = DateTTMMJJ.Length - 1; i >= 0; i -= 2) //i As Integer = num To 1 Step - 2
            {
                returnString += DateTTMMJJ.Substring(i - 1, 2);
            }
            return returnString;
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
            //Kürzt einen String auf die angegebene Länge
            //Dim num As Integer = Strings.Len(input)
            //                    Dim flag As Boolean = num > limit
            //                    Dim result As String
            //                    If flag Then
            //                        ' The following expression was wrapped in a checked-expression
            //                        result = Strings.Mid(input, num - limit + 1)
            //                    Else
            //result = input
            //                    End If








            return result;
        }

        private string Expand(string input, int limit)
        {
            //Erweitert einen String auf die angegebene Länge
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

    class Bestellzeile
    {
        #region Aufbau Bestellzeile
        /*
        Zeile aus neuer NF-DAT-Datei:            
        0 ;1   ;2   ;3     ;4     ;5;6   ;7;8 ;9;10
        NF;1050;1050;200924;200925;;20000;;209;;10
        0  NF               Kennzeichen neues Format
        1  KdNr             BHM Filialnummer
        2  KdNr             BHM Kundennummer der Filiale
        3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen
        4  Lieferdatum      Lieferdatum
        5  -leer-           BHM ArtikelKey
        6  20000            Menge*1000, echte Menge=Menge/1000> 20000=20
        7  -leer-           Preis (Bsp. 2.00), Dezimal = .
        8  ArtNummer        BHM ArtikelNummer
        9  -leer-           VPE
        10 10               Anzahl Bestellpositionen in Datei
        */
        #endregion

        public string NFKennzeichen { get; set; }
        public string BHMFilialNummer { get; set; }
        public string BHMKundenNummer { get; set; }
        public string BestellDatum { get; set; }
        public string LieferDatum { get; set; }
        public string BHMArtikelKey { get; set; }
        public string BestellMenge { get; set; }
        public string Preis { get; set; }
        public string BHMArtikelNummer { get; set; }
        public string Verpackungseinheit { get; set; }
        public string AnzahlBestellPositionen { get; set; }
    }



}

