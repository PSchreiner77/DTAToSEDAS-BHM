using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dat2Sedas_Neu
{

    //TODO TEST: Dateien vom Wochenende mit 9000er Nummern.
    //TODO Gruppieren der Daten nach Kundennummer. Es soll vermieden werden, dass in der Sedas.dat 
    //     Kundennummern wiederholt auftauchen. Sie sollen en-block gelistet werden (sortiert nach Art.Nr).
    class ConvertDatToSedas
    {
        private Logger log = Logger.GetInstance();

        private string _SourcePath;
        private List<string> _SourceDataList;
        private string _DestinationPath;

        private List<DatBestellzeile> _ListeDatBestellzeilen;
        private string _SedasErstellDatumJJMMTT;
        private int _counter;
        private SedasFile _SedasFile;  //Objekt mit allen SEDAS-Einträgen, fertig zur Erstellung einer Datei (.ToString()).

        //KONSTRUKTOR
        public ConvertDatToSedas(string SourceFilePath, string DestinationFilePath, int Counter)
        {
            this._SedasErstellDatumJJMMTT = ConvertToSedasDateJJMMTT(DateTime.Now);
            this._SourcePath = SourceFilePath;
            this._DestinationPath = DestinationFilePath;
            this._counter = Counter;
        }

        //METHODEN
       
        private bool checkIfNFFileFormat()
        {
            string prefix = _SourceDataList[0].Substring(0, 2);
            if (prefix == "NF") return true;
            return false;
        }

        //TODO ReadOldDatDataFormat
        private List<DatBestellzeile> ReadOldDATDataFormat(List<string> OldDATData, string ErstelldatumTTMMJJ)
        {

            List<DatBestellzeile> datBestellzeilen = new List<DatBestellzeile>();

            foreach (string eintrag in OldDATData)
            {
                string[] arrZeile = eintrag.Split(';');
                DatBestellzeile datBestellzeile = new DatBestellzeile();
                datBestellzeile.NFKennzeichen = arrZeile[0];
                datBestellzeile.BHMFilialNummer = arrZeile[1];
                datBestellzeile.BHMKundenNummer = arrZeile[2];
                datBestellzeile.BestellDatumJJMMT = _SedasErstellDatumJJMMTT; // arrZeile[3];
                datBestellzeile.LieferDatumJJMMTT = arrZeile[4];
                datBestellzeile.BHMArtikelKey = arrZeile[5];
                datBestellzeile.BestellMenge = arrZeile[6];
                datBestellzeile.Preis = arrZeile[7];
                datBestellzeile.BHMArtikelNummer = arrZeile[8];
                datBestellzeile.Verpackungseinheit = arrZeile[9];
                datBestellzeile.AnzahlBestellPositionen = arrZeile[10];

                datBestellzeilen.Add(datBestellzeile);
            }

            return datBestellzeilen;

            //bool flag = Operators.CompareString(arrSourceLines(0), "", false) = 0

            //    // The following expression was wrapped in a checked-statement
            //List<DatBestellzeile> array = new List<DatBestellzeile>();
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
            0 ;1   ;2   ;3     ;4     ;5   ;6    ;7;8  ;9    ;10
            NF;1050;1050;200924;200925;    ;20000; ;209;     ;10    (Aldi-Datei Beispiel)
            NF; 180;3785;091121;101121;1111; 9000;0;  2;1.000;1     (BHM-Datei Beispiel)
            
            (!* Originaldateien enthalten keine Leerzeichen in den Feldern*!)

            0  NF               Kennzeichen neues Format
            1  FilNr            BHM Filialnummer der Filiale / Aldi-Filial/Kundennummer
            2  KdNr             BHM Kundennummer der Filiale / Aldi-Filial/Kundennummer
            3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
            4  Lieferdatum      Lieferdatum
            5  Artikelkey       BHM ArtikelKey
            6  Menge            Menge*1000, echte Menge=Menge/1000> 20000=20
            7  Preis            Preis (Bsp. 2.000), Dezimal = .
            8  ArtNummer        BHM ArtikelNummer
            9  VPE              Verpackungseinheit
            10 10               Anzahl Bestellpositionen in Datei
            */

            List<DatBestellzeile> ListeDatBestellzeilen = new List<DatBestellzeile>();

            foreach (string eintrag in NewNFDATData)
            {
                string[] arrZeile = eintrag.Split(';');
                DatBestellzeile datBestellzeile = new DatBestellzeile();
                datBestellzeile.NFKennzeichen = arrZeile[0];
                datBestellzeile.BHMFilialNummer = arrZeile[1];
                datBestellzeile.BHMKundenNummer = arrZeile[2];
                //TODO Bestelldatum nativ übernehmen und erst in SEDAS-Objekt umwandeln!
                datBestellzeile.BestellDatumJJMMT = _SedasErstellDatumJJMMTT; // arrZeile[3]
                datBestellzeile.LieferDatumJJMMTT = ConvertToSedasDateJJMMTT(arrZeile[4]);
                datBestellzeile.BHMArtikelKey = arrZeile[5];
                datBestellzeile.BestellMenge = arrZeile[6];
                datBestellzeile.Preis = arrZeile[7];
                datBestellzeile.BHMArtikelNummer = arrZeile[8];
                datBestellzeile.Verpackungseinheit = arrZeile[9];
                datBestellzeile.AnzahlBestellPositionen = arrZeile[10];

                ListeDatBestellzeilen.Add(datBestellzeile);
            }

            return ListeDatBestellzeilen;
        }

        /// <summary>
        ///Bereinigt die Ordnerzeilen von ungewünschten Kunden und Artikeln und tauscht Artikelnummern aus.
        /// </summary>
        /// <returns></returns>
        private List<DatBestellzeile> CleanupOrders(List<DatBestellzeile> DatBestellzeilen)
        {
            DatBestellzeilen = DeleteCustomers(DatBestellzeilen);
            DatBestellzeilen = DeleteArticles(DatBestellzeilen);
            DatBestellzeilen = ChangeArticleNumbers(DatBestellzeilen);

            return DatBestellzeilen;
        }

        #region Löschen und ändern
        //TODO Löschen und Ändern über Delegaten steuern lassen (Items löschen, welche kommt nach Auswahl).
        //TODO GGf alles in eigene Klasse(n) auslagern.
        private List<DatBestellzeile> DeleteCustomers(List<DatBestellzeile> ListeDatBestellzeilen)
        {
            string messageTitle = "Kundennummern löschen";

            #region über Delegaten steuern lassen
            string pathLoescheKunde = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            List<string> customersToDelete = Datenverarbeitung.LoadDeleteItemsList(pathLoescheKunde);
            List<DatBestellzeile> deletedCustomers = new List<DatBestellzeile>();
            #endregion
            bool nothingChanged = true;
            log.Log("Kundennummern aus Bestellung löschen...", messageTitle, Logger.MsgType.Message);

            /*Einträge mit Kundennummer werden gesucht und in eine eigene Liste für
             * zu löschende Einträge geschrieben.
             * Abschließend wird die Liste mit den zu löschenden Einträgen von der
             * Hauptliste "entfernt".
             */
            foreach (string kundennummer in customersToDelete)
            {
                bool customerDeleted = false;
                foreach (DatBestellzeile datBestellzeile in ListeDatBestellzeilen)
                {
                    if (kundennummer == datBestellzeile.BHMKundenNummer)
                    {
                        deletedCustomers.Add(datBestellzeile);
                        customerDeleted = true;
                    }
                }
                if (customerDeleted)
                {
                    log.Log($" => Kundennummer {kundennummer} aus Bestellzeilen entfernt.", messageTitle, Logger.MsgType.Message);
                    customerDeleted = false;
                    nothingChanged = false;
                }
            }
            if (nothingChanged) log.Log("...keine Kundennummern gelöscht.");

            //"Entfernen" der Löschliste von der Hauptliste.
            ListeDatBestellzeilen = ListeDatBestellzeilen.Except(deletedCustomers).ToList();
            return ListeDatBestellzeilen;
        }

        private List<DatBestellzeile> DeleteArticles(List<DatBestellzeile> DatBestellzeilen)
        {
            string messageTitle = "Artikelnummern löschen";
            string pathLoescheArtikel = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
            List<string> articlesToDelete = Datenverarbeitung.LoadDeleteItemsList(pathLoescheArtikel);
            List<DatBestellzeile> deletedArticles = new List<DatBestellzeile>();

            bool nothingChanged = true;
            log.Log("Löschen von Artikelnummern aus der Bestellung...", messageTitle, Logger.MsgType.Message);
            foreach (string artikelnummer in articlesToDelete)
            {
                bool articleDeleted = false;
                foreach (DatBestellzeile datBestellzeile in DatBestellzeilen)
                {
                    if (artikelnummer == datBestellzeile.BHMArtikelNummer)
                    {
                        deletedArticles.Add(datBestellzeile);
                        articleDeleted = true;
                    }
                }
                if (articleDeleted)
                {
                    log.Log($" => Artikelnummer {artikelnummer} aus Bestellung gelöscht.", messageTitle, Logger.MsgType.Message);
                    articleDeleted = false;
                    nothingChanged = false;
                }
            }
            if (nothingChanged) log.Log("...keine Artikelnummern gelöscht", messageTitle, Logger.MsgType.Message);
            DatBestellzeilen = DatBestellzeilen.Except(deletedArticles).ToList();
            return DatBestellzeilen;
        }

        private List<DatBestellzeile> ChangeArticleNumbers(List<DatBestellzeile> DatBestellzeilen)
        {
            string messageTitle = "Artikelnummern tauschen";
            if (DatBestellzeilen == null) { return null; }

            log.Log("Austauschen von Artikelnummern laut tauscheArtikel.txt...", messageTitle, Logger.MsgType.Message);
            string pathTauscheArtikel = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";
            Dictionary<string, string> ArticlesDict = Datenverarbeitung.LoadChangeArticlesList(pathTauscheArtikel);

            bool nothingChanged = true;
            foreach (KeyValuePair<string, string> dictEntry in ArticlesDict)
            {
                bool articleChanged = false;
                foreach (DatBestellzeile datBestellzeile in DatBestellzeilen)
                {
                    if (dictEntry.Key == datBestellzeile.BHMArtikelNummer)
                    {
                        datBestellzeile.BHMArtikelNummer = dictEntry.Value;
                        articleChanged = true;
                    }
                }
                if (articleChanged)
                {
                    log.Log($" => Artikelnummer ausgetauscht: {dictEntry.Key} => {dictEntry.Value}", messageTitle, Logger.MsgType.Message);
                    nothingChanged = false;
                    articleChanged = true;
                }
            }

            if (nothingChanged) log.Log("...keine Artikelnummern ausgetauscht.", messageTitle, Logger.MsgType.Message);
            return DatBestellzeilen;
        }

        #endregion


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
            if (MM.Length < 2) MM = "0" + MM;
            if (TT.Length < 2) TT = "0" + TT;
            return JJ + MM + TT;
        }

        /// <summary>
        /// Dreht das Quelldatei-Datumsformat um in das Sedas-Datumsformat: TTMMJJ => JJMMTT
        /// </summary>
        /// <param name="DateTTMMJJ">Quelldatei-Datumsformat: TTMMJJ</param>
        /// <returns></returns>
        private string ConvertToSedasDateJJMMTT(string DateTTMMJJ)
        {
            string returnString = "";
            for (int i = DateTTMMJJ.Length-2; i >=0; i -= 2)
            {
                returnString += DateTTMMJJ.Substring(i, 2);
            }
            return returnString;
        }

        /// <summary>
        /// Liest die Quell-Dat-Datei ein. Möglich sind das neue Format (NF) und das alte Format. Rückgabewert ist eine Liste mit Bestellzeilen-Objekten.
        /// </summary>
        /// <returns></returns>
        private bool ReadDatFileContent()
        {
            log.Log("Beginn der Konvertierung...", "Konvertierung der Daten", Logger.MsgType.Message);

            _SourceDataList = Datenverarbeitung.ImportSourceFileToList(_SourcePath);
            if (_SourceDataList == null)
                return false;

            //TODO Als Delegate bauen: ReadDatData auf den dann die passende (neu/alt) Einlesemethode gemappt wird.
            #region 
            if (checkIfNFFileFormat())
            {
                log.Log("Einlesen neues Dateiformat...", "Einlesen der Bestelldaten", Logger.MsgType.Message);
                this._ListeDatBestellzeilen = ReadNewNFDATDataFormat(_SourceDataList);
            }
            else
            {
                log.Log("Einlesen altes Dateiformat...", "Einlesen der Bestelldaten", Logger.MsgType.Message);
                this._ListeDatBestellzeilen = ReadOldDATDataFormat(_SourceDataList, _SedasErstellDatumJJMMTT);
            }
            #endregion

            _ListeDatBestellzeilen = CleanupOrders(_ListeDatBestellzeilen);
            return false;
        }


        public void CreateSedasData()
        {
            //Bestellungen filtern
            string actualCustomer;
            int pointer1 = 0;  // Zeiger für einzelne Bestellung, Findet Bestell-Header
            int pointer2 = 0; // Zeiger für einzelne Bestellposition in Bestellung

            //TODO Einlesen der Bestelldatei auslagern
            log.Log("Einlesen der Bestelldatei...", "Einlesen", Logger.MsgType.Message);
            ReadDatFileContent();

            log.Log("Konvertieren in Sedas-Format", "Sedas-Konvertierung", Logger.MsgType.Message);
            
            //TODO alle SEDAS-eigenenen Eigenschaften in Sedas-Objekt vereinen und ggf. erzeugen/konvertieren
            _SedasFile = new SedasFile(_SedasErstellDatumJJMMTT, _counter);

            //Alle Bestellzeilen durchgehen und bei jeder Kundennummer eine neue SEDAS-Bestellung erzeugen
            while (pointer1 < _ListeDatBestellzeilen.Count())
            {
                //TODO Ggf. die Ermittlung der Bestellung und Bestellpositionen über LINQ abfragen/filtern und anschließend erstellen lassen.
                //Kundenbestellung erzeugen/beginnen
                DatBestellzeile Bestellposition = _ListeDatBestellzeilen[pointer1];
                actualCustomer = Bestellposition.BHMKundenNummer;

                //SedasOrder-Objekte (einzelne Bestellung) aus den Bestellzeilen erstellen.
                SedasOrder CustomerOrder = new SedasOrder(_SedasErstellDatumJJMMTT, Bestellposition.LieferDatumJJMMTT, Bestellposition.BHMKundenNummer);

                pointer2 = pointer1;
                //Solange die Kundennummer gleich bleibt, und die Liste nich zu Ende ist, aus jeder Zeile eine SEDAS-Bestellposition machen.
                while (pointer2 < _ListeDatBestellzeilen.Count() && _ListeDatBestellzeilen[pointer2].BHMKundenNummer == actualCustomer)
                {
                    Bestellposition = _ListeDatBestellzeilen[pointer2];
                    CustomerOrder.OrderLines.Add(new SedasOrderLine(Bestellposition.BHMArtikelNummer, Bestellposition.BestellMenge));
                    pointer2++;
                }
                _SedasFile.SedasOrdersList.Add(CustomerOrder);
                pointer1 = pointer2; //next Customer Block
            }
        }


        public bool WriteSedasData()
        {
            log.Log("Schreiben der Sedas.dat...", "Schreiben der Sedas.dat Datei", Logger.MsgType.Message);

            try
            {
                #region Zielverzeichnis erstellen, wenn nicht vorhanden
                //TODO Zielverzeichnis sollte schon bei Programmstart geprüft sein. Prüfung überflüssig.
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

                //Eigentliches Schreiben der SEDAS-Informationen in eine Datei.
                using (StreamWriter sw = new StreamWriter(this._DestinationPath, false))
                {
                    sw.Write(_SedasFile.GetSedasFileString());
                    sw.WriteLine("                                                                                    ");
                }
                return true;
            }
            catch (Exception ex)
            {
                string message = $"Fehler beim Schreiben der Sedas.dat:\n{ex.ToString()}";
                log.Log(message, "Fehler", Logger.MsgType.Message);
                return false;
            }
        }
    }
}

class SedasFile
{
    /*
     *    CSB-SEDAS Datei-Beispiel:
     *    
        010()000377777777777771180705;,241
        ;)0240051310000002
        ;030,14,00000000000000000,180705,180706,,,,3789         ,,
        ;0400000000001360,40002000,,,,02 000000,,
        ...
        ...
        ...
        ;0400000000004023,40001000,,,,02 000000,,
        ;05000000039000           
        ;06100,1178
        ;07000000,00001,00001,000000,(

        Erläuterung:
        010...  = Datei-Header
        030...  = Order-Header
        040...  = Bestellposition(en)
        050...  = Order-Footer
        06....  = DateiFooter (Zusammenfassung)
        070...  = DateiFooter (Dateiende)
     */

    private string _ErstellDatumSedas;
    private int _IniSedasRunThroughCounter; //TODO Was bedeutet der Zähler?

    public string Header { get { return GetSedasFileHeaderString(); } }
    public string Footer { get { return GetSedasFileFooterString(); } }
    public List<SedasOrder> SedasOrdersList = new List<SedasOrder>();

    public int CustomerOrdersCount { get { return SedasOrdersList.Count; } }
    public int OverallOrderLineEntriesCount { get { return GetTotalNumberOfOrderLines(); } }

    //KONSTRUKTOR
    //public SedasFile() { }

    public SedasFile(string Erstelldatum, int IniSedasRunThroughCounter)
    {
        _ErstellDatumSedas = Erstelldatum;
        _IniSedasRunThroughCounter = IniSedasRunThroughCounter;
    }

    //METHODEN

    //TODO Propery erstellen
    private int GetTotalNumberOfOrderLines()
    {
        int count = 0;
        foreach (SedasOrder order in SedasOrdersList)
        {
            count += order.OrderLines.Count;
        }
        return count; ;
    }

    private string GetSedasFileHeaderString()
    {
        return $"010()000377777777777771{_ErstellDatumSedas};,{_IniSedasRunThroughCounter}\r\n;)0240051310000002";
    }

    private string GetSedasFileFooterString()
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
        string FooterLine1 = $":06{Tools.ExpandLeftStringSide(CustomerOrdersCount.ToString(), 3)};{OverallOrderLineEntriesCount}";
        string FooterLine2 = $";07000000,00001,00001,000000,(                                                      ";

        return FooterLine1 + "\r\n" + FooterLine2;
    }

    public string GetSedasFileString()
    {
        string returnString = "";
        string cr = "\r\n";

        returnString += GetSedasFileHeaderString() + cr;
        foreach (SedasOrder order in SedasOrdersList)
        {
            returnString += order.Header + cr;
            foreach (SedasOrderLine orderLine in order.OrderLines)
            {
                returnString += orderLine.GetSedasOrderLineString() + cr;
            }
            returnString += order.Footer + cr;
        }
        returnString += GetSedasFileFooterString() + cr;
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
        get { return GetSedasOrderHeaderString(); }
    }
    public string Footer { get { return GetSedasOrderFooterString(); } }
    public int OrderArticleQuantity { get { return GetSedasOrderArticleQuantity(); } }
    public List<SedasOrderLine> OrderLines = new List<SedasOrderLine>();

    //KONSTRUKTOR
    public SedasOrder(string ErstellDatumJJMMTT, string LieferDatumJJMMTT, string BHMKundennummer)
    {
        _ErstellDatum = ErstellDatumJJMMTT;
        _LieferDatum = LieferDatumJJMMTT;
        _BHMKundennummer = BHMKundennummer;
    }

    //METHODEN
    private string GetSedasOrderHeaderString()
    {
        return $";030,14,00000000000000000,{_ErstellDatum},{_LieferDatum},,,,{_BHMKundennummer}         ,,";
    }

    private string GetSedasOrderFooterString()
    {
        //;05000000039000
        //;05               Kennung Footer
        //   000000039      9 Stellen für Summe bestellter Artikelmengen
        //            0000  1000er Stelle für Artikelmenge

        string articleQuantity = Tools.ExpandLeftStringSide(OrderArticleQuantity.ToString(), 9);
        return $";05{articleQuantity}000";
    }

    public int GetSedasOrderArticleQuantity()
    {
        int count = 0;
        foreach (SedasOrderLine orderLine in OrderLines)
        {
            count += Convert.ToInt32(orderLine.ArtikelMenge.Substring(0, orderLine.ArtikelMenge.Length - 3));
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
     *      00002000         = Menge (Wert/1000)
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
    public string GetSedasOrderLineString()
    {
        return $";040000{Tools.ExpandLeftStringSide(BHMArtikelNummer, 10)},4{Tools.ExpandLeftStringSide(ArtikelMenge, 7)},,,,02 000000,,";
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
    public static List<string> ImportSourceFileToList(string SourcePath)
    {
        List<string> _sourceDataList = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(SourcePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != "") _sourceDataList.Add(line);
                }
            }
        }
        catch (Exception ex)
        { //Fehlerausnahme auslösen und Fehler melden}                
            return null;
        }
        return _sourceDataList;
    }


    public static List<string> LoadDeleteItemsList(string Path)
    {
        List<string> delItems = new List<string>();
        try
        {
            List<string> allLines = File.ReadAllText(Path).Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();
            foreach (string element in allLines)
            {
                delItems.Add(element.Split(';')[0]);
            }
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
            changeArticleFileContent = File.ReadAllText(Path).Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();
        }
        catch (Exception ex)
        { }

        foreach (string line in changeArticleFileContent)
        {
            if (line != "")
            {
                string[] elements = line.Split(';');
                DictChangeArticles.Add(elements[0].Trim(), elements[1].Trim());
            }
        }
        return DictChangeArticles;
    }
}

static class Tools
{
    public static string CutLeftStringSide(string Input, int MaxLength)
    {
        //Kürzt einen String vorne auf die angegebene Länge            
        string returnString = "";
        if (Input.Length > MaxLength)
        {
            returnString = Input.Substring(Input.Length - MaxLength);
        }
        return returnString;
    }

    public static string ExpandLeftStringSide(string InputString, int MaxLength)
    {
        //Erweitert einen String links um "0" bis zur angegebenen Länge

        while (InputString.Length < MaxLength)
        {
            InputString = "0" + InputString;
        }

        return InputString;
    }
}
