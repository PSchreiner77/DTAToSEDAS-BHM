using System;
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

        private List<string> _SourceDataList;
        private List<InputFileOrderLineNF> _ListeDatBestellzeilen;


        //****************
        private Logger log = Logger.GetInstance();
        private string _SedasErstellDatumJJMMTT;
        private string _SourcePath;
        private string _DestinationPath;
        private int _counter;
        private SedasFile _SedasFile;  //Objekt mit allen SEDAS-Einträgen, fertig zur Erstellung einer Datei (.ToString()).

        InputFileNF inputFile = new InputFileNF();

        //****************

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
            if (prefix == "NF")
                return true;
            return false;
        }

        //TODO ? ReadOldDatDataFormat
        //private List<InputFileOrderLineNF> ReadOldDATDataFormat(List<string> OldDATData, string ErstelldatumTTMMJJ)
        //{

        //    List<InputFileOrderLineNF> datBestellzeilen = new List<InputFileOrderLineNF>();

        //    foreach (string eintrag in OldDATData)
        //    {
        //        string[] arrZeile = eintrag.Split(';');
        //        InputFileOrderLineNF datBestellzeile = new InputFileOrderLineNF();
        //        datBestellzeile.NFKennzeichen = arrZeile[0];
        //        datBestellzeile.BHMFilialNummer = arrZeile[1];
        //        datBestellzeile.BHMKundenNummer = arrZeile[2];
        //        datBestellzeile.BestellDatumJJMMT = _SedasErstellDatumJJMMTT; // arrZeile[3];
        //        datBestellzeile.LieferDatumJJMMTT = arrZeile[4];
        //        datBestellzeile.BHMArtikelKey = arrZeile[5];
        //        datBestellzeile.BestellMenge = arrZeile[6];
        //        datBestellzeile.Preis = arrZeile[7];
        //        datBestellzeile.BHMArtikelNummer = arrZeile[8];
        //        datBestellzeile.Verpackungseinheit = arrZeile[9];
        //        datBestellzeile.AnzahlBestellPositionen = arrZeile[10];

        //        datBestellzeilen.Add(datBestellzeile);
        //    }

        //    return datBestellzeilen;

        //    //bool flag = Operators.CompareString(arrSourceLines(0), "", false) = 0

        //    //    // The following expression was wrapped in a checked-statement
        //    //List<DatBestellzeile> array = new List<DatBestellzeile>();
        //    //if (arrSourceLines[0] == "")
        //    //{
        //    //    array = null;
        //    //    array = array;
        //    //}
        //    //else
        //    //{
        //    //    try
        //    //    {
        //    //        int num = -1;
        //    //        int upperBound = arrSourceLines.GetUpperBound(0);
        //    //        for (int i = 0; i <= upperBound; i++)
        //    //        {
        //    //            bool flag2 = Operators.CompareString(arrSourceLines(i), "", false) <> 0;
        //    //            if (flag2)
        //    //                num += 1;
        //    //        }

        //    //        string[,] array2 = New String(num + 1 - 1, 10) { }; //Array dimensionieren
        //    //        int num2 = 0;
        //    //        int upperBound2 = arrSourceLines.GetUpperBound(0);
        //    //        for (int j = 0; j < upperBound2; j++)
        //    //        {
        //    //            string left = arrSourceLines(j);
        //    //            bool flag3 = Operators.CompareString(left, "", false) <> 0;
        //    //            if (flag3)
        //    //            {
        //    //                array2(num2, 2) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 9, 8));
        //    //                array2(num2, 3) = ErstelldatumTTMMJJ;
        //    //                array2(num2, 4) = this.MyTRIM(Strings.Mid(arrSourceLines(j), 18, 13));
        //    //                array2(num2, 6) = this.Shorten(Strings.Mid(arrSourceLines(j), 73, 11), 7);
        //    //                array2(num2, 8) = this.Expand(Strings.Trim(Strings.Mid(arrSourceLines(j), 31, 42)), 10);
        //    //                num2 += 1;
        //    //            }
        //    //        }
        //    //        array = this.DeleteEntries1(array2);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        array = null;
        //    //    }
        //    //}
        //}

        ///// <summary>
        ///// Konvertiert das neue NF-Format in eine Liste mit Bestellzeilen und wendet die Filter für
        ///// Artikel- und Kundenlöschen, sowie Artikelaustausch auf die Liste an.
        ///// </summary>
        ///// <param name="NewNFDATData"></param>
        ///// <param name="ErstelldatumTTMMJJ"></param>
        ///// <returns></returns>
        //private List<InputFileOrderLineNF> ReadNewNFDATDataFormat(List<string> NewNFDATData)
        //{
        //    /*
        //    Zeile aus neuer NF-DAT-Datei:            
        //    0 ;1   ;2   ;3     ;4     ;5   ;6    ;7;8  ;9    ;10
        //    NF;1050;1050;200924;200925;    ;20000; ;209;     ;10    (Aldi-Datei Beispiel)
        //    NF; 180;3785;091121;101121;1111; 9000;0;  2;1.000;1     (BHM-Datei Beispiel)

        //    (!* Originaldateien enthalten keine Leerzeichen in den Feldern*!)

        //    0  NF               Kennzeichen neues Format
        //    1  FilNr            BHM Filialnummer der Filiale / Aldi-Filial/Kundennummer
        //    2  KdNr             BHM Kundennummer der Filiale / Aldi-Filial/Kundennummer
        //    3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
        //    4  Lieferdatum      Lieferdatum
        //    5  Artikelkey       BHM ArtikelKey
        //    6  Menge            Menge*1000, echte Menge=Menge/1000> 20000=20
        //    7  Preis            Preis (Bsp. 2.000), Dezimal = .
        //    8  ArtNummer        BHM ArtikelNummer
        //    9  VPE              Verpackungseinheit
        //    10 10               Anzahl Bestellpositionen in Datei
        //    */

        //    List<InputFileOrderLineNF> ListeDatBestellzeilen = new List<InputFileOrderLineNF>();

        //    foreach (string eintrag in NewNFDATData)
        //    {
        //        string[] arrZeile = eintrag.Split(';');
        //        InputFileOrderLineNF datBestellzeile = new InputFileOrderLineNF();
        //        datBestellzeile.NFKennzeichen = arrZeile[0];
        //        datBestellzeile.BHMFilialNummer = arrZeile[1];
        //        datBestellzeile.BHMKundenNummer = arrZeile[2];
        //        //TODO Bestelldatum nativ übernehmen und erst in SEDAS-Objekt umwandeln!
        //        datBestellzeile.BestellDatumJJMMT = _SedasErstellDatumJJMMTT; // arrZeile[3]
        //        datBestellzeile.LieferDatumJJMMTT = ConvertToSedasDateJJMMTT(arrZeile[4]);
        //        datBestellzeile.BHMArtikelKey = arrZeile[5];
        //        datBestellzeile.BestellMenge = arrZeile[6];
        //        datBestellzeile.Preis = arrZeile[7];
        //        datBestellzeile.BHMArtikelNummer = arrZeile[8];
        //        datBestellzeile.Verpackungseinheit = arrZeile[9];
        //        datBestellzeile.AnzahlBestellPositionen = arrZeile[10];

        //        ListeDatBestellzeilen.Add(datBestellzeile);
        //    }

        //    return ListeDatBestellzeilen;
        //}


        /// <summary>
        ///Bereinigt die Ordnerzeilen von ungewünschten Kunden und Artikeln und tauscht Artikelnummern aus.
        /// </summary>
        /// <returns></returns>
        private List<InputFileOrderLineNF> CleanupOrders(List<InputFileOrderLineNF> DatBestellzeilen)
        {
            DatBestellzeilen = DeleteCustomers(DatBestellzeilen);
            DatBestellzeilen = DeleteArticles(DatBestellzeilen);
            DatBestellzeilen = ChangeArticleNumbers(DatBestellzeilen);

            return DatBestellzeilen;
        }

        #region Löschen und ändern
        //TODO Löschen und Ändern über Delegaten steuern lassen (Items löschen, welche kommt nach Auswahl).
        //TODO GGf alles in eigene Klasse(n) auslagern.
        private List<InputFileOrderLineNF> DeleteCustomers(List<InputFileOrderLineNF> ListeDatBestellzeilen)
        {
            string messageTitle = "Kundennummern löschen";

            #region über Delegaten steuern lassen
            string pathLoescheKunde = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
            List<string> customersToDelete = Datenverarbeitung.LoadDeleteItemsList(pathLoescheKunde);
            List<InputFileOrderLineNF> deletedCustomers = new List<InputFileOrderLineNF>();
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
                foreach (InputFileOrderLineNF datBestellzeile in ListeDatBestellzeilen)
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
            if (nothingChanged)
                log.Log("...keine Kundennummern gelöscht.");

            //"Entfernen" der Löschliste von der Hauptliste.
            ListeDatBestellzeilen = ListeDatBestellzeilen.Except(deletedCustomers).ToList();
            return ListeDatBestellzeilen;
        }

        private List<InputFileOrderLineNF> DeleteArticles(List<InputFileOrderLineNF> DatBestellzeilen)
        {
            string messageTitle = "Artikelnummern löschen";
            string pathLoescheArtikel = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
            List<string> articlesToDelete = Datenverarbeitung.LoadDeleteItemsList(pathLoescheArtikel);
            List<InputFileOrderLineNF> deletedArticles = new List<InputFileOrderLineNF>();

            bool nothingChanged = true;
            log.Log("Löschen von Artikelnummern aus der Bestellung...", messageTitle, Logger.MsgType.Message);
            foreach (string artikelnummer in articlesToDelete)
            {
                bool articleDeleted = false;
                foreach (InputFileOrderLineNF datBestellzeile in DatBestellzeilen)
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
            if (nothingChanged)
                log.Log("...keine Artikelnummern gelöscht", messageTitle, Logger.MsgType.Message);
            DatBestellzeilen = DatBestellzeilen.Except(deletedArticles).ToList();
            return DatBestellzeilen;
        }

        private List<InputFileOrderLineNF> ChangeArticleNumbers(List<InputFileOrderLineNF> DatBestellzeilen)
        {
            string messageTitle = "Artikelnummern tauschen";
            if (DatBestellzeilen == null)
            { return null; }

            log.Log("Austauschen von Artikelnummern laut tauscheArtikel.txt...", messageTitle, Logger.MsgType.Message);
            string pathTauscheArtikel = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";
            Dictionary<string, string> ArticlesDict = Datenverarbeitung.LoadChangeArticlesList(pathTauscheArtikel);

            bool nothingChanged = true;
            foreach (KeyValuePair<string, string> dictEntry in ArticlesDict)
            {
                bool articleChanged = false;
                foreach (InputFileOrderLineNF datBestellzeile in DatBestellzeilen)
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

            if (nothingChanged)
                log.Log("...keine Artikelnummern ausgetauscht.", messageTitle, Logger.MsgType.Message);
            return DatBestellzeilen;
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

        public void CreateSedasDatFile()
        {
            if (inputFile.Count() > 0)
            {
                //Kundennummern ermitteln
                var listOfCustomerNumbers = this.inputFile.Select(orderLine => orderLine.BHMKundenNummer).Distinct();

                log.Log("Konvertieren in Sedas-Format", "Sedas-Konvertierung", Logger.MsgType.Message);
                this._SedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, this._counter);
                //Für jede Kundennummer eine Sedas-Order erstellen und dem Sedas-File hinzufügen.
                foreach (string actualCustomerNumber in listOfCustomerNumbers)
                {
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
                    sw.Write(_SedasFile.ToString());
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

    class SedasOrder :IEnumerable<SedasOrderLine>
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


        //private void ConvertInputFileData(List<string> inputFileData)
        //{
        //    /*
        //   Zeile aus neuer NF-DAT-Datei:            
        //   0 ;1   ;2   ;3     ;4     ;5   ;6    ;7;8  ;9    ;10
        //   NF;1050;1050;200924;200925;    ;20000; ;209;     ;10    (Aldi-Datei Beispiel)
        //   NF; 180;3785;091121;101121;1111; 9000;0;  2;1.000;1     (BHM-Datei Beispiel)

        //   (!* Originaldateien enthalten keine Leerzeichen in den Feldern*!)

        //   0  NF               Kennzeichen neues Format
        //   1  FilNr            BHM Filialnummer der Filiale / Aldi-Filial/Kundennummer
        //   2  KdNr             BHM Kundennummer der Filiale / Aldi-Filial/Kundennummer
        //   3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
        //   4  Lieferdatum      Lieferdatum
        //   5  Artikelkey       BHM ArtikelKey
        //   6  Menge            Menge*1000, echte Menge=Menge/1000> 20000=20
        //   7  Preis            Preis (Bsp. 2.000), Dezimal = .
        //   8  ArtNummer        BHM ArtikelNummer
        //   9  VPE              Verpackungseinheit
        //   10 10               Anzahl Bestellpositionen in Datei
        //   */

        //    try
        //    {
        //        foreach (string eintrag in inputFileData)
        //        {
        //            ConvertInputFileData(eintrag);
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        //TODO Ausnahme anzeigen
        //        throw new Exception(ex.Message);
        //    }
        //}

        //DELETE
        //private void ConvertInputFileData(string inputFileOrderLine)
        //{
        //    /*
        //  Zeile aus neuer NF-DAT-Datei:            
        //  0 ;1   ;2   ;3     ;4     ;5   ;6    ;7;8  ;9    ;10
        //  NF;1050;1050;200924;200925;    ;20000; ;209;     ;10    (Aldi-Datei Beispiel)
        //  NF; 180;3785;091121;101121;1111; 9000;0;  2;1.000;1     (BHM-Datei Beispiel)

        //  (!* Originaldateien enthalten keine Leerzeichen in den Feldern*!)

        //  0  NF               Kennzeichen neues Format
        //  1  FilNr            BHM Filialnummer der Filiale / Aldi-Filial/Kundennummer
        //  2  KdNr             BHM Kundennummer der Filiale / Aldi-Filial/Kundennummer
        //  3  Bestelldatum     Bestelldatum = Tagesdatum beim Einlesen = Erstelldatum
        //  4  Lieferdatum      Lieferdatum
        //  5  Artikelkey       BHM ArtikelKey
        //  6  Menge            Menge*1000, echte Menge=Menge/1000> 20000=20
        //  7  Preis            Preis (Bsp. 2.000), Dezimal = .
        //  8  ArtNummer        BHM ArtikelNummer
        //  9  VPE              Verpackungseinheit
        //  10 10               Anzahl Bestellpositionen in Datei
        //  */

        //    try
        //    {
        //        string[] arrZeile = inputFileOrderLine.Split(';');

        //        this.InputFileOrderLines.Add(new InputFileOrderLineNF()
        //        {
        //            NFKennzeichen = arrZeile[0],
        //            BHMFilialNummer = arrZeile[1],
        //            BHMKundenNummer = arrZeile[2],
        //            BestellDatumTTMMJJ = arrZeile[3],
        //            LieferDatumTTMMJJ = arrZeile[4],
        //            BHMArtikelKey = arrZeile[5],
        //            BestellMenge = arrZeile[6],
        //            Preis = arrZeile[7],
        //            BHMArtikelNummer = arrZeile[8],
        //            Verpackungseinheit = arrZeile[9],
        //            AnzahlBestellPositionen = arrZeile[10]
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO Ausnahme anzeigen
        //        throw new Exception(ex.Message);
        //    }
        //}


        IEnumerator IEnumerable.GetEnumerator()
        {
            return InputFileOrderLines.GetEnumerator();
        }

        public IEnumerator<InputFileOrderLineNF> GetEnumerator()
        {
            return InputFileOrderLines.GetEnumerator();
        }

        //DELETE
        //public void AddNewNFOrderLine(string orderLineNFFormat)
        //{
        //    ConvertInputFileData(orderLineNFFormat);
        //}
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
}