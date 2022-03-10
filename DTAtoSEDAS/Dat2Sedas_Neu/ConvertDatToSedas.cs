﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dat2Sedas_Neu
{

    //TODO TEST: Dateien vom Wochenende mit 9000er Nummern.
    //TODO Gruppieren der Daten nach Kundennummer. Es soll vermieden werden, dass in der Sedas.dat 
    //     Kundennummern wiederholt auftauchen. Sie sollen en-block gelistet werden (sortiert nach Art.Nr).
    class ConvertDatToSedas
    {
        private Logger log = Logger.GetInstance();
        private string _SedasErstellDatumJJMMTT;
        private string _SourcePath;
        private string _DestinationPath;
        private int _counter;
        private SedasFile _SedasFile;  //Objekt mit allen SEDAS-Einträgen, fertig zur Erstellung einer Datei (.ToString()).

        InputFileNF inputFile = new InputFileNF();
        CustomerDeletionList _customersToDelete;
        ArticleDeletionList _articlesToDelete;
        ArticleChangeList articlesToChange;


        //KONSTRUKTOR
        public ConvertDatToSedas(string SourceFilePath, string DestinationFilePath, int Counter)
        {
            this._SedasErstellDatumJJMMTT = ConvertToSedasDateJJMMTT(DateTime.Now);
            this._SourcePath = SourceFilePath;
            this._DestinationPath = DestinationFilePath;
            this._counter = Counter;

            //Importieren der Lösch- und Änderungslisten.
            string pathLoescheKunde = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            string pathLoescheArtikel = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
            string pathTauscheArtikel = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";
            this._customersToDelete = new CustomerDeletionList(Datenverarbeitung.LoadDeleteItemsList(pathLoescheKunde));
            this._articlesToDelete = new ArticleDeletionList(Datenverarbeitung.LoadDeleteItemsList(pathLoescheArtikel));
            this.articlesToChange = Datenverarbeitung.LoadChangeArticlesList(pathTauscheArtikel);
        }


        //METHODEN
        #region Löschen und ändern
        public bool DeleteCustomers()
        {
            bool customersDeleted = false;
            string messageTitle = "Kundennummern löschen";
            log.Log("Kundennummern aus Bestellung löschen...", messageTitle, Logger.MsgType.Message);

            foreach (string customerNumber in _customersToDelete)
            {
                if (inputFile.InputFileOrderLines.FirstOrDefault(orderLine => orderLine.BHMKundenNummer == customerNumber) != null)
                {
                    inputFile.InputFileOrderLines = inputFile.InputFileOrderLines.Where(orderLine => orderLine.BHMKundenNummer != customerNumber).Select(orderLine => orderLine).ToList();
                    log.Log($" => Kundennummer {customerNumber} aus Bestellungen gelöscht.", messageTitle, Logger.MsgType.Message);
                    customersDeleted = true;
                }
            }

            return customersDeleted;
        }
 
        public bool DeleteArticle()
        {
            bool articleDeleted = false;
            string messageTitle = "Artikelnummern löschen";
            log.Log("Löschen von Artikelnummern aus der Bestellung...", messageTitle, Logger.MsgType.Message);

            foreach (string articleNumber in _articlesToDelete)
            {
                if (inputFile.InputFileOrderLines.FirstOrDefault(line => line.BHMArtikelNummer == articleNumber) != null)
                {
                    inputFile.InputFileOrderLines = inputFile.InputFileOrderLines.Where(line => line.BHMArtikelNummer != articleNumber).ToList();
                    log.Log($" => Artikelnummer {articleNumber} aus Bestellung gelöscht.", messageTitle, Logger.MsgType.Message);
                    articleDeleted = true;
                }
            }

            return articleDeleted;
        }
   
        private bool ChangeArticleNumbers()
        {
            bool articleChanged = false;
            string messageTitle = "Artikelnummern tauschen";
            log.Log("Austauschen von Artikelnummern laut tauscheArtikel.txt...", messageTitle, Logger.MsgType.Message);

            foreach (ArticleChangePair pair in articlesToChange)
            {
                bool currentArticleChanged = false;
                var linesToChange = inputFile.InputFileOrderLines.Where(line => line.BHMArtikelNummer == pair.OriginalNumber).ToList();
                foreach (InputFileOrderLineNF orderLine in linesToChange)
                {
                    orderLine.BHMArtikelNummer = pair.NewNumber;
                    currentArticleChanged = true;
                    articleChanged = true;
                }
                if (currentArticleChanged)
                    log.Log($" => Artikelnummer {pair.OriginalNumber} getauscht gegen {pair.NewNumber}.", messageTitle, Logger.MsgType.Message);
            }
            return articleChanged;
        }
        #endregion

        private List<string> ReadInputFile(string sourcePathNFDatFile)
        {
            List<string> _sourceDataList = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(sourcePathNFDatFile))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (line != "")
                            _sourceDataList.Add(line);
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO Fehlerausnahme auslösen und Fehler melden
                throw new Exception(ex.Message);
            }
            return _sourceDataList;
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
            if (MM.Length < 2)
                MM = "0" + MM;
            if (TT.Length < 2)
                TT = "0" + TT;
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
            for (int i = DateTTMMJJ.Length - 2; i >= 0; i -= 2)
            {
                returnString += DateTTMMJJ.Substring(i, 2);
            }
            return returnString;
        }

        private SedasOrder CreateSedasOrder(IEnumerable<InputFileOrderLineNF> singleCustomerOrderLines)
        {
            string customerNumber = singleCustomerOrderLines.First().BHMKundenNummer;
            string sedasLieferDatumJJMMTT = ConvertToSedasDateJJMMTT(singleCustomerOrderLines.First().LieferDatumTTMMJJ);
            // SEDAS-Order erstellen
            SedasOrder singleCustomerOrder = new SedasOrder(this._SedasErstellDatumJJMMTT, sedasLieferDatumJJMMTT, customerNumber);

            foreach (InputFileOrderLineNF orderLine in singleCustomerOrderLines)
            {
                //Alle Einträge einer Kundennummer als Sedas-OrderLines der Sedas-Order hinzufügen.
                singleCustomerOrder.SedasOrderLines.Add(new SedasOrderLine(orderLine.BHMArtikelNummer, orderLine.BestellMenge));
            }

            return singleCustomerOrder;
        }

        private InputFileOrderLineNF ConvertInputFileData(string inputFileOrderLine)
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

            try
            {
                string[] arrZeile = inputFileOrderLine.Split(';');

                InputFileOrderLineNF newLine = new InputFileOrderLineNF()
                {
                    NFKennzeichen = arrZeile[0],
                    BHMFilialNummer = arrZeile[1],
                    BHMKundenNummer = arrZeile[2],
                    BestellDatumTTMMJJ = arrZeile[3],
                    LieferDatumTTMMJJ = arrZeile[4],
                    BHMArtikelKey = arrZeile[5],
                    BestellMenge = arrZeile[6],
                    Preis = arrZeile[7],
                    BHMArtikelNummer = arrZeile[8],
                    Verpackungseinheit = arrZeile[9],
                    AnzahlBestellPositionen = arrZeile[10]
                };

                return newLine;
            }
            catch (Exception ex)
            {
                //TODO Ausnahme anzeigen
                throw new Exception(ex.Message);
            }
        }




        public void ImportNFDatFile(string filePath)
        {
            List<string> orderFileLines = ReadInputFile(filePath);
            foreach (string line in orderFileLines)
            {
                this.inputFile.InputFileOrderLines.Add(ConvertInputFileData(line));
            }
        }

        public void CreateSedasDatFileFromInputFile()
        {
            if (inputFile.Count() > 0)
            {
                //Daten aus Inputfile löschen und verändern.
                DeleteCustomers();
                DeleteArticle();
                ChangeArticleNumbers();

                log.Log("Konvertieren in Sedas-Format", "Sedas-Konvertierung", Logger.MsgType.Message);
                //Sedas-Datei-Objekt erstellen
                this._SedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, this._counter);

                //Kundennummern aus Input-Daten ermitteln und
                //für jede Kundennummer eine Sedas-Order erstellen und dem Sedas-File hinzufügen.
                List<string> listOfCustomerNumbers = (List<string>)this.inputFile.Select(orderLine => orderLine.BHMKundenNummer).Distinct().ToList();
                foreach (string actualCustomerNumber in listOfCustomerNumbers)
                {
                    ////Sedas-Order erstllen und alle Zeilen einer Kundennummer hinzufügen.
                    var singleCustomerOrderLines = this.inputFile.Where(orderLine => orderLine.BHMKundenNummer == actualCustomerNumber).Select(orderLine => orderLine);
                    this._SedasFile.SedasOrdersList.Add(CreateSedasOrder(singleCustomerOrderLines));
                }
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
                    sw.Write(this._SedasFile.ToString());
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




    class SedasFile : IEnumerable<SedasOrder>
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

        private string _ErstellDatumSedasJJMMTT;
        private int _IniSedasRunThroughCounter; //TODO Was bedeutet der Zähler?
        public List<SedasOrder> SedasOrdersList = new List<SedasOrder>();


        public int CustomerOrdersCount { get { return SedasOrdersList.Count; } }
        public int OverallOrderLineEntriesCount { get { return GetTotalNumberOfOrderLines(); } }

        //KONSTRUKTOR
        public SedasFile(string Erstelldatum, int IniSedasRunThroughCounter)
        {
            _ErstellDatumSedasJJMMTT = Erstelldatum;
            _IniSedasRunThroughCounter = IniSedasRunThroughCounter;
        }

        //METHODEN

        //TODO Propery erstellen
        private int GetTotalNumberOfOrderLines()
        {
            int count = 0;
            foreach (SedasOrder order in SedasOrdersList)
            {
                count += order.SedasOrderLines.Count;
            }
            return count;
            ;
        }

        private string GetSedasFileHeaderString()
        {
            return $"010()000377777777777771{_ErstellDatumSedasJJMMTT};,{_IniSedasRunThroughCounter}\r\n;)0240051310000002";
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


        public override string ToString()
        {
            string cr = "\r\n";

            string returnString = GetSedasFileHeaderString() + cr;
            foreach (SedasOrder order in SedasOrdersList)
            {
                returnString += order.ToString();
            }
            returnString += GetSedasFileFooterString() + cr;
            return returnString;
        }


        public IEnumerator<SedasOrder> GetEnumerator()
        {
            return SedasOrdersList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class SedasOrder : IEnumerable<SedasOrderLine>
    {
        private string _ErstellDatumJJMMTT;
        private string _LieferDatumJJMMTT;
        private string _BHMKundennummer;

        public string Header
        {
            get { return GetSedasOrderHeaderString(); }
        }
        public string Footer { get { return GetSedasOrderFooterString(); } }
        public int OrderArticleQuantity { get { return GetSedasOrderArticleQuantity(); } }
        public List<SedasOrderLine> SedasOrderLines = new List<SedasOrderLine>();

        //KONSTRUKTOR
        public SedasOrder(string ErstellDatumJJMMTT, string LieferDatumJJMMTT, string BHMKundennummer)
        {
            _ErstellDatumJJMMTT = ErstellDatumJJMMTT;
            _LieferDatumJJMMTT = LieferDatumJJMMTT;
            _BHMKundennummer = BHMKundennummer;
        }

        //METHODEN
        private string GetSedasOrderHeaderString()
        {
            return $";030,14,00000000000000000,{_ErstellDatumJJMMTT},{_LieferDatumJJMMTT},,,,{_BHMKundennummer}         ,,";
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
            foreach (SedasOrderLine orderLine in SedasOrderLines)
            {
                count += Convert.ToInt32(orderLine.ArtikelMenge.Substring(0, orderLine.ArtikelMenge.Length - 3));
            }

            return count;
        }

        public override string ToString()
        {
            string cr = "\r\n";

            string returnString = this.Header + cr;
            foreach (SedasOrderLine orderLine in this.SedasOrderLines)
            {
                returnString += orderLine.ToString() + cr;
            }
            returnString += this.Footer + cr;

            return returnString;
        }

        public IEnumerator<SedasOrderLine> GetEnumerator()
        {
            return SedasOrderLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        public string BHMArtikelNummer { get; set; }
        public string ArtikelMenge { get; set; }


        //KONSTRUKTOR
        public SedasOrderLine(string BHMArtikelNummer, string ArtikelMenge)
        {
            this.BHMArtikelNummer = BHMArtikelNummer;
            this.ArtikelMenge = ArtikelMenge;
        }

        //METHODEN

        public override string ToString()
        {
            return $";040000{Tools.ExpandLeftStringSide(BHMArtikelNummer, 10)},4{Tools.ExpandLeftStringSide(ArtikelMenge, 7)},,,,02 000000,,";
        }
    }




    class InputFileNF : IEnumerable<InputFileOrderLineNF>
    {
        public List<InputFileOrderLineNF> InputFileOrderLines = new List<InputFileOrderLineNF>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InputFileOrderLines.GetEnumerator();
        }

        public IEnumerator<InputFileOrderLineNF> GetEnumerator()
        {
            return InputFileOrderLines.GetEnumerator();
        }

    }

    class InputFileOrderLineNF
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
        public string BestellDatumTTMMJJ { get; set; }
        public string LieferDatumTTMMJJ { get; set; }
        public string BHMArtikelKey { get; set; }
        public string BestellMenge { get; set; }
        public string Preis { get; set; }
        public string BHMArtikelNummer { get; set; }
        public string Verpackungseinheit { get; set; }
        public string AnzahlBestellPositionen { get; set; }
    }




    public class ArticleDeletionList : IEnumerable
    {
        private List<string> _articlesToDelete;

        public ArticleDeletionList(List<string> articleList)
        {
            _articlesToDelete = articleList;
            for(int i=0;i<_articlesToDelete.Count();i++)
            {
                _articlesToDelete[i] = _articlesToDelete[i].Trim();
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _articlesToDelete.GetEnumerator();
        }
    }

    public class CustomerDeletionList : IEnumerable
    {
        private List<string> _customerNumbers;

        public CustomerDeletionList(List<string> list)
        {
            _customerNumbers = list;

            for (int i=0;i<_customerNumbers.Count(); i++)
            {
                _customerNumbers[i] = _customerNumbers[i].Trim();
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _customerNumbers.GetEnumerator();
        }
    }

    public class ArticleChangeList : IEnumerable
    {
        private List<ArticleChangePair> _articlesToChange;

        public ArticleChangeList()
        {
            _articlesToChange = new List<ArticleChangePair>();
        }

        public void Add(ArticleChangePair articleExchangePair)
        {
            _articlesToChange.Add(articleExchangePair);
        }

        public void Remove(ArticleChangePair articleExcangePair)
        {
            ArticleChangePair result = _articlesToChange.First(pair => (pair.OriginalNumber == articleExcangePair.OriginalNumber) & pair.NewNumber == articleExcangePair.NewNumber);

            if (result != null)
            {
                _articlesToChange.Remove(result);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _articlesToChange.GetEnumerator();
        }
    }

    public class ArticleChangePair
    {
        public string OriginalNumber { get; set; }
        public string NewNumber { get; set; }
        public string Description { get; set; }

        public ArticleChangePair(string OriginalNumber, string NewNumber)
        {
            this.OriginalNumber = OriginalNumber.Trim();
            this.NewNumber = NewNumber.Trim();
        }
        public ArticleChangePair(string OriginalNumber, string NewNumber, string Description)
        {
            this.OriginalNumber = OriginalNumber.Trim();
            this.NewNumber = NewNumber.Trim();
            this.Description = Description.Trim();
        }
    }



    static class Datenverarbeitung
    {
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

        public static ArticleChangeList LoadChangeArticlesList(string Path)
        {
            List<string> changeArticleFileContent = new List<string>();
            try
            {
                changeArticleFileContent = File.ReadAllText(Path).Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList<string>();
            }
            catch (Exception ex)
            { }

            ArticleChangeList articleChangeList = new ArticleChangeList();
            foreach (string line in changeArticleFileContent)
            {
                if (line != "")
                {
                    string[] elements = line.Split(';');
                    ArticleChangePair newPair = new ArticleChangePair(elements[0].Trim(), elements[1].Trim());
                    articleChangeList.Add(newPair);
                };

            }
            return articleChangeList;
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
}