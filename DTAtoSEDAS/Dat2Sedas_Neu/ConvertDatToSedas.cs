using System;
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

        private List<DatBestellzeile> _DatContent;
        private string _SedasErstellDatumJJMMTT;
        private SedasFile _SedasFile;

        //KONSTRUKTOR
        public ConvertDatToSedas(string SourceFilePath, string DestinationFilePath)
        {
            this._SedasErstellDatumJJMMTT = ConvertToSedasDateJJMMTT(DateTime.Now);
            this._SourcePath = SourceFilePath;
            this._DestinationPath = DestinationFilePath;
        }

        //METHODEN
        private bool checkIfNFFileFormat()
        {
            string prefix = _SourceData[0].Substring(0, 2);
            if (prefix == "NF") return true;
            return false;
        }

        //TODO ReadOldDatDataFormat
        private List<DatBestellzeile> ReadOldDATDataFormat(List<string> arrSourceLines, string ErstelldatumTTMMJJ)
        {
            //bool flag = Operators.CompareString(arrSourceLines(0), "", false) = 0

            //    // The following expression was wrapped in a checked-statement
            List<DatBestellzeile> array = new List<DatBestellzeile>();
            //if (arrSourceLines[0] == "")
            //{
            //    array = null;
            //    array = array;
            //}
            //else
            //{
            //    try
            //    {
            //        int num = -1;
            //        int upperBound = arrSourceLines.GetUpperBound(0);
            //        for (int i = 0; i <= upperBound; i++)
            //        {
            //            bool flag2 = Operators.CompareString(arrSourceLines(i), "", false) <> 0;
            //            if (flag2)
            //                num += 1;
            //        }

            //        string[,] array2 = New String(num + 1 - 1, 10) { }; //Array dimensionieren
            //        int num2 = 0;
            //        int upperBound2 = arrSourceLines.GetUpperBound(0);
            //        for (int j = 0; j < upperBound2; j++)
            //        {
            //            string left = arrSourceLines(j);
            //            bool flag3 = Operators.CompareString(left, "", false) <> 0;
            //            if (flag3)
            //            {
            //                array2(num2, 2) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 9, 8));
            //                array2(num2, 3) = ErstelldatumTTMMJJ;
            //                array2(num2, 4) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 18, 13));
            //                array2(num2, 6) = this.Shorten(Strings.Mid(arrSourceLines(j), 73, 11), 7);
            //                array2(num2, 8) = this.Expand(Strings.Trim(Strings.Mid(arrSourceLines(j), 31, 42)), 10);
            //                num2 += 1;
            //            }
            //        }
            //        array = this.DeleteEntries1(array2);
            //    }
            //    catch (Exception ex)
            //    {
            //        array = null;
            //    }
            //}
            return array;
        }

        /// <summary>
        /// Konvertiert das neue NF-Format in eine Liste mit Bestellzeilen und wendet die Filter für
        /// Artikel- und Kundenlöschen, sowie Artikelaustausch auf die Liste an.
        /// </summary>
        /// <param name="NewNFDATData"></param>
        /// <param name="ErstelldatumTTMMJJ"></param>
        /// <returns></returns>
        private List<DatBestellzeile> ReadNewNFDATDataFormat(List<string> NewNFDATData)
        {
            /*
            Zeile aus neuer NF-DAT-Datei:            
            0 ;1   ;2   ;3     ;4     ;5;6   ;7;8 ;9;10
            NF;1050;1050;200924;200925;;20000;;209;;10
            0  NF               Kennzeichen neues Format
            1  FilNr            BHM Filialnummer der Filiale
            2  KdNr             BHM Kundennummer der Filiale
            3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
            4  Lieferdatum      Lieferdatum
            5  -leer-           BHM ArtikelKey
            6  20000            Menge*1000, echte Menge=Menge/1000> 20000=20
            7  -leer-           Preis (Bsp. 2.00), Dezimal = .
            8  ArtNummer        BHM ArtikelNummer
            9  -leer-           VPE
            10 10               Anzahl Bestellpositionen in Datei
            */

            List<DatBestellzeile> Bestellzeilen = new List<DatBestellzeile>();

            foreach (string eintrag in NewNFDATData)
            {
                string[] arrZeile = eintrag.Split(';');
                DatBestellzeile bestellzeile = new DatBestellzeile();
                bestellzeile.NFKennzeichen = arrZeile[0];
                bestellzeile.BHMFilialNummer = arrZeile[1];
                bestellzeile.BHMKundenNummer = arrZeile[2];
                bestellzeile.BestellDatumJJMMT = _SedasErstellDatumJJMMTT; // arrZeile[3];
                bestellzeile.LieferDatumJJMMTT = arrZeile[4];
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
            //**

            return Bestellzeilen;
        }


        //TODO Löschen und Ändern über Delegaten steuern lassen (Items löschen, welche kommt nach Auswahl).
        private List<DatBestellzeile> DeleteCustomers(List<DatBestellzeile> DatBestellzeilen)
        {
            #region über Delegaten steuern lassen
            string pathLoescheKunde = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            List<string> customersToDelete = Datenverarbeitung.LoadDeleteItemsList(pathLoescheKunde);
            #endregion

            foreach (string kundennummer in customersToDelete)
            {
                foreach (DatBestellzeile datBestellzeile in DatBestellzeilen)
                {
                    if (kundennummer == datBestellzeile.BHMKundenNummer)
                    {
                        DatBestellzeilen.Remove(datBestellzeile);
                    }
                }
            }
            return DatBestellzeilen;
        }

        private List<DatBestellzeile> DeleteArticles(List<DatBestellzeile> DatBestellzeilen)
        {
            string pathLoescheArtikel = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            List<string> articlesToDelete = Datenverarbeitung.LoadDeleteItemsList(pathLoescheArtikel);

            foreach (string kundennummer in articlesToDelete)
            {
                foreach (DatBestellzeile datBestellzeile in DatBestellzeilen)
                {
                    if (kundennummer == datBestellzeile.BHMKundenNummer)
                    {
                        DatBestellzeilen.Remove(datBestellzeile);
                    }
                }
            }
            return DatBestellzeilen;
        }

        private List<DatBestellzeile> ChangeArticleNumbers(List<DatBestellzeile> DatBestellzeilen)
        {
            if (DatBestellzeilen == null) { return null; }

            ////LogMessage.LogOnly("Austauschen von Artikelnummern laut tauscheArtikel.txt.");
            string pathTauscheArtikel = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";
            Dictionary<string, string> ArticlesDict = Datenverarbeitung.LoadChangeArticlesList(pathTauscheArtikel);

            foreach (DatBestellzeile datBestellzeile in DatBestellzeilen)
            {
                if (ArticlesDict.ContainsKey(datBestellzeile.BHMArtikelNummer))
                {
                    datBestellzeile.BHMKundenNummer = ArticlesDict[datBestellzeile.BHMKundenNummer];
                }
            }
            return DatBestellzeilen;
        }


        /// <summary>
        /// Gibt ein Datum als String zurück in der Form: 'JJMMTT'
        /// </summary>
        /// <param name="date">Datum</param>
        /// <returns>String: 'JJMMTT'</returns>
        private string ConvertToSedasDateJJMMTT(DateTime date)
        {
            string JJ = date.Year.ToString().Substring(2, 2);
            string MM = date.Month.ToString();
            string TT = date.Day.ToString();
            return JJ + MM + TT;
        }
        private string ConvertToSedasDateJJMMTT(string DateTTMMJJ)
        {
            string returnString = "";
            for (int i = 0; i < DateTTMMJJ.Length; i += 2)
            {
                returnString += DateTTMMJJ.Substring(i, 2);
            }
            return returnString;
        }


        /// <summary>
        /// Trimmt einen Text und entfernt eventuell führende Nullen.
        /// </summary>
        /// <param name="TrimText"></param>
        /// <returns></returns>
        private string MyTRIM(string TrimText)
        {
            if (TrimText != "")
            {
                TrimText = TrimText.Trim();
                while (TrimText.Substring(0, 1) == "0")
                {
                    TrimText.Remove(0, 1);
                }
            }
            return TrimText;
        }

        private string CutLeftStringSide(string input, int limit)
        {
            //Kürzt einen String vorne auf die angegebene Länge            
            string returnString = "";
            if (input.Length > limit)
            {
                returnString = input.Substring(input.Length - limit);
            }
            return returnString;
        }

        private string ExpandLeftStringSide(string input, int limit)
        {
            //Erweitert einen String links um "0" bis zur angegebenen Länge

            while (input.Length < limit)
            {
                input = "0" + input;
            }

            return input;
        }

        private bool ReadDatFileContent()
        {
            ////LogMessage.LogOnly("Beginn der Konvertierung...");

            _SourceData = Datenverarbeitung.ImportSourceFile(_SourcePath);
            if (_SourceData == null) return false;

            #region Als Delegate bauen ReadDatData
            if (checkIfNFFileFormat())
            {
                //////LogMessage.LogOnly("Einlesen neues Dateiformat...");
                this._DatContent = ReadNewNFDATDataFormat(_SourceData);
            }
            else
            {
                //////LogMessage.LogOnly("Einlesen neues Dateiformat...");
                this._DatContent = ReadOldDATDataFormat(_SourceData, _SedasErstellDatumJJMMTT);
            }
            #endregion

            //Sedas erstellen
            CreateSedasData();

            // ...
            // ////LogMessage.LogOnly("Konvertierung in SEDAS.DAT abgeschlossen.");
            // ////LogMessage.LogOnly("Fehler beim Konvertieren in Sedas.dat." & vbCrLf + ex.ToString());


            return false;
        }

        public void CreateSedasData()
        {
            //Bestellungen filtern
            string actualCustomer;
            int pointer1 = 0;
            int pointer2 = 0;

            _SedasFile = new SedasFile();
            while (pointer1 < _DatContent.Count())
            {
                //Kundenbestellung erzeugen/beginnen
                DatBestellzeile Bestellposition = _DatContent[pointer1];
                actualCustomer = Bestellposition.BHMKundenNummer;

                SedasOrder CustomerOrder = new SedasOrder(_SedasErstellDatumJJMMTT, Bestellposition.LieferDatumJJMMTT, Bestellposition.BHMKundenNummer);
                pointer2 = pointer1;
                while (_DatContent[pointer2].BHMKundenNummer == actualCustomer)
                {
                    Bestellposition = _DatContent[pointer2];
                    CustomerOrder.OrderLines.Add(new SedasOrderLine(Bestellposition.BHMArtikelNummer, Bestellposition.BestellMenge));
                    pointer2++;
                }
                pointer1 = pointer2; //next Customer Block
            }
        }

        public bool WriteSedasData()
        {
            ////LogMessage.LogOnly("Schreiben der Sedas.dat...");

            try
            {
                #region Zielverzeichnis erstellen, wenn nicht vorhanden
                bool isPath_NoFile = _DestinationPath.Contains("\\");
                if (isPath_NoFile)
                {
                    if (!File.Exists(_DestinationPath))
                    {
                        bool folderExists = Directory.Exists(Path.GetDirectoryName(_DestinationPath));
                        if (!folderExists)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(_DestinationPath));
                        }
                    }
                }
                else
                {
                    this._DestinationPath = Directory.GetCurrentDirectory() + "\\" + this._DestinationPath;
                }
                #endregion


                using (StreamWriter sw = new StreamWriter(this._DestinationPath, false))
                {
                    sw.Write(_SedasFile.Get());
                    sw.WriteLine("                                                                                    ");
                }
                return true;
            }
            catch (Exception ex)
            {
                ////LogMessage.LogOnly(ex.ToString());
                return false;
            }
        }
    }
}

class SedasFile
{
    private string _ErstellDatumSedas;
    private int _IniSedasRunThroughCounter;

    public string Header { get { return CreateHeader(); } }
    public string Footer { get { return CreateFooter(); } }
    public List<SedasOrder> Orders = new List<SedasOrder>();

    public int CustomerOrdersCount { get { return Orders.Count; } }
    public int OverallOrderLineEntriesCount { get { return GetTotalNumberOfOrderLines(); } }

    //KONSTRUKTOR
    public SedasFile() { }

    public SedasFile(string Erstelldatum, int IniSedasRunThroughCounter)
    {
        _ErstellDatumSedas = Erstelldatum;
        _IniSedasRunThroughCounter = IniSedasRunThroughCounter;
    }

    //METHODEN
    private int GetTotalNumberOfOrderLines()
    {
        int count = 0;
        foreach (SedasOrder order in Orders)
        {
            count += order.OrderLines.Count;
        }
        return count; ;
    }

    private string CreateHeader()
    {
        return $"010()000377777777777771{_ErstellDatumSedas};,{_IniSedasRunThroughCounter}\n\r;)0240051310000002";
    }

    private string CreateFooter()
    {
        #region Aufbau FooterLine
        //--FOOTER der Zieldatei
        //; 06100,1178
        //; 07000000,00001,00001,000000,(
        //
        //;06108,1178 
        //  ;06    = Kennung Zusammenfassung Einträge
        //  108,   = Anzahl Kunden in Datei (Blöcke)
        //  1178   = Anzahl einzelner Datensätze/Artikelzeilen
        #endregion
        string FooterLine1 = $":06{CustomerOrdersCount};{OverallOrderLineEntriesCount}";
        string FooterLine2 = $";07000000,00001,00001,000000,(                                                      ";

        return FooterLine1 + "\n\r" + FooterLine2;
    }

    public string Get()
    {
        string returnString = "";
        string cr = "\n\r";

        returnString += CreateHeader() + cr;
        foreach (SedasOrder order in Orders)
        {
            returnString += order.Header + cr;
            foreach (SedasOrderLine orderLine in order.OrderLines)
            {
                returnString += orderLine.Get() + cr;
            }
            returnString += order.Footer + cr;
        }
        returnString += CreateFooter() + cr;
        return returnString;
    }
}

class SedasOrder
{
    private string _ErstellDatum;
    private string _LieferDatum;
    private string _BHMKundennummer;

    public string Header
    {
        get { return CreateHeader(); }
    }
    public string Footer { get { return CreateFooter(); } }
    public int OrderArticleQuantity { get { return GetArticleQuantity(); } }
    public List<SedasOrderLine> OrderLines = new List<SedasOrderLine>();

    //KONSTRUKTOR
    public SedasOrder(string ErstellDatumJJMMTT, string LieferDatumJJMMTT, string BHMKundennummer)
    {
        _ErstellDatum = ErstellDatumJJMMTT;
        _LieferDatum = LieferDatumJJMMTT;
        _BHMKundennummer = BHMKundennummer;
    }

    //METHODEN
    private string CreateHeader()
    {
        return $";030,14,00000000000000000,{_ErstellDatum},{_LieferDatum},,,,{_BHMKundennummer}         ,,";
    }

    private string CreateFooter()
    {
        //;05000000039000

        string text = "";
        int maxNumberOfCharacters = 9;
        int zerosToAdd = maxNumberOfCharacters - GetArticleQuantity();
        for (int i = 0; i < zerosToAdd; i++)
        {
            text = "0" + text;
        }
        return $";5{text}0000";
    }

    public int GetArticleQuantity()
    {
        int count = 0;
        foreach (SedasOrderLine orderLine in OrderLines)
        {
            count += Convert.ToInt32(orderLine.ArtikelMenge);
        }

        return count;
    }
}

class SedasOrderLine
{
    /* ;0400000000000317,40002000,,,,02 000000,,
     *      ;040000          = Kennung Zeile BestellPosition
     *      0000000317       = Artikelnummer
     *      ,4               = fix
     *      0002000          = Menge (Wert/1000)
     *      ,,,,02 000000,,  = fix
    */

    public string BHMArtikelNummer { get; private set; }
    public string ArtikelMenge { get; private set; }


    //KONSTRUKTOR
    public SedasOrderLine()
    { }

    public SedasOrderLine(string BHMArtikelNummer, string ArtikelMenge)
    {
        this.BHMArtikelNummer = BHMArtikelNummer;
        this.ArtikelMenge = ArtikelMenge;
    }

    //METHODEN
    public string Get()
    {
        return $";040000{BHMArtikelNummer},4{ArtikelMenge},,,,02 000000,,";
    }
}

class DatBestellzeile
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
    public string BestellDatumJJMMT { get; set; }
    public string LieferDatumJJMMTT { get; set; }
    public string BHMArtikelKey { get; set; }
    public string BestellMenge { get; set; }
    public string Preis { get; set; }
    public string BHMArtikelNummer { get; set; }
    public string Verpackungseinheit { get; set; }
    public string AnzahlBestellPositionen { get; set; }
}





static class Datenverarbeitung
{
    /// <summary>
    /// Liest die Dat-Quelldatei ein ohne Leerzeilen und gibt sie als List<string> zurück.</string>
    /// </summary>
    /// <param name="SourceFilePath"></param>
    /// <returns></returns>
    public static List<string> ImportSourceFile(string SourcePath)
    {
        List<string> data = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(SourcePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != "") data.Add(line);
                }
            }
        }
        catch (Exception ex)
        { //Fehlerausnahme auslösen und Fehler melden}                
            return null;
        }
        return data;
    }


    public static List<string> LoadDeleteItemsList(string Path)
    {
        List<string> delItems = new List<string>();
        try
        {
            delItems = File.ReadAllText(Path).Split('\n').ToList<string>();
        }
        catch (Exception ex)
        { }

        foreach (string entry in delItems)
        {
            entry.Trim();
        }

        return delItems;
    }

    public static Dictionary<string, string> LoadChangeArticlesList(string Path)
    {
        Dictionary<string, string> DictChangeArticles = new Dictionary<string, string>();
        List<string> changeArticleFileContent = new List<string>();
        try
        {
            changeArticleFileContent = File.ReadAllText(Path).Split('\n').ToList<string>();
        }
        catch (Exception ex)
        { }

        foreach (string line in changeArticleFileContent)
        {
            string[] elements = line.Split(';');
            DictChangeArticles.Add(elements[0].Trim(), elements[1].Trim());
        }
        return DictChangeArticles;
    }
}


