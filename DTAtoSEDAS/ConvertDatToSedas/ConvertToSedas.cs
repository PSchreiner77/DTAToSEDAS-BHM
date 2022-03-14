using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConvertDatToSedas
{

    //TODO TEST: Dateien vom Wochenende mit 9000er Nummern.
    //TODO Gruppieren der Daten nach Kundennummer. Es soll vermieden werden, dass in der Sedas.dat 
    //     Kundennummern wiederholt auftauchen. Sie sollen en-block gelistet werden (sortiert nach Art.Nr).
    public class ConvertToSedas
    {
        private string _SedasErstellDatumJJMMTT;  //Datum der Dateierstellung / des Programmlaufs
        private int _counter;

        private string _pathDeleteCustomer = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
        private string _pathDeleteArticle = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
        private string _pathChangeArticle = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";

        private CustomerDeletionList _customersToDelete;
        private ArticleDeletionList _articlesToDelete;
        private ArticleChangeList articlesToChange = new ArticleChangeList();

        //private Logger log = Logger.GetInstance();
        //private string _SourcePath;
        //private string _DestinationPath;
        //public SedasFile SedasFile;  //Objekt mit allen SEDAS-Einträgen, fertig zur Erstellung einer Datei (.ToString()).
        //public DatSource SourceDatFile = new DatSource();

        //KONSTRUKTOR
        /// <summary>
        /// Erstellt ein Objekt zum Erzeugen einer SEDAS.DAT Datei aus einer Bestell.dat Datei.
        /// </summary>
        /// <param name="SourceFilePath">Einzulesende Datei mit Bestelldaten.</param>
        /// <param name="DestinationFilePath">Dateipfad für SEDAS.DAT-Ausgabe.</param>
        /// <param name="actualCounter">Neue Zählerposition für SEDAS.DAT Datei.</param>
        public ConvertToSedas(int actualCounter, ArticleChangeList articlesToChangeList, ArticleDeletionList articlesToDelete, CustomerDeletionList customersToDelete)
        {
            this._SedasErstellDatumJJMMTT = Tools.ConvertToSedasDateJJMMTT(DateTime.Now);
            this._counter = actualCounter;

            //this._SourcePath = SourceFilePath;
            //this._DestinationPath = DestinationFilePath;
            //Importieren der Lösch- und Änderungslisten.
            //this._customersToDelete = DataProcessing.GetDeleteCustomersList(_pathDeleteCustomer);
            //this._articlesToDelete = DataProcessing.GetDeleteArticlesList(_pathDeleteArticle);
            //this.articlesToChange = DataProcessing.LoadChangeArticlesList(_pathChangeArticle);
            
            this._customersToDelete = customersToDelete;
            this._articlesToDelete = articlesToDelete;
            this.articlesToChange = articlesToChange;
        }


        //METHODEN
        private SedasOrder CreateSedasOrder(IEnumerable<DatSourceOrderLine> singleCustomerOrderLines)
        {
            string customerNumber = singleCustomerOrderLines.First().BHMKundenNummer;
            string sedasLieferDatumJJMMTT = Tools.ConvertToSedasDateJJMMTT(singleCustomerOrderLines.First().LieferDatumTTMMJJ);
            // SEDAS-Order erstellen
            SedasOrder singleCustomerOrder = new SedasOrder(this._SedasErstellDatumJJMMTT, sedasLieferDatumJJMMTT, customerNumber);

            foreach (DatSourceOrderLine orderLine in singleCustomerOrderLines)
            {
                //Alle Einträge einer Kundennummer als Sedas-OrderLines der Sedas-Order hinzufügen.
                singleCustomerOrder.SedasOrderLines.Add(new SedasOrderLine(orderLine.BHMArtikelNummer, orderLine.BestellMenge));
            }

            return singleCustomerOrder;
        }


        public SedasFile ConvertDatToSedas(DatFile ImportedDatFile, string SedasOutputPath)
        {
            return null;
        }



        public DatSource ImportSourceDatFile(string filePath)
        {
            List<string> sourceFileLines = DataProcessing.GetFileContent(filePath);
            return ConvertSourceDatFileToObject(sourceFileLines);
        }

        public DatSource ConvertSourceDatFileToObject(List<string> importedFileLines)
        {
            DatSource newInputFile = new DatSource();

            foreach (string line in importedFileLines)
            {
                DatSourceOrderLine newOrderLine = new DatSourceOrderLine(line);
                newInputFile.InputFileOrderLines.Add(newOrderLine);
            }
            return newInputFile;
        }


        public SedasFile CreateSedasFile(DatSource DatSourceObject)
        {
            SedasFile sedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, this._counter);

            if (DatSourceObject.Count() > 0)
            {
                //Daten aus Inputfile löschen und verändern.
                this.DeleteCustomers();
                this.DeleteArticle();
                this.ChangeArticleNumbers();

                //log.Log("Konvertieren in Sedas-Format", "Sedas-Konvertierung", Logger.MsgType.Message);
                //Sedas-Datei-Objekt erstellen
                sedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, this._counter);

                //Kundennummern aus Input-Daten ermitteln und
                List<string> listOfCustomerNumbers = (List<string>)DatSourceObject.Select(orderLine => orderLine.BHMKundenNummer).Distinct().ToList();
                //für jede Kundennummer eine Sedas-Order erstellen und dem Sedas-File hinzufügen.
                foreach (string actualCustomerNumber in listOfCustomerNumbers)
                {
                    ////Sedas-Order erstllen und alle Zeilen einer Kundennummer hinzufügen.
                    var singleCustomerOrderLines = DatSourceObject.Where(orderLine => orderLine.BHMKundenNummer == actualCustomerNumber).Select(orderLine => orderLine);
                    SedasOrder singleCustomerSedasOrder = CreateSedasOrder(singleCustomerOrderLines);
                    sedasFile.SedasOrdersList.Add(singleCustomerSedasOrder);
                }


                return sedasFile;
            }

            return null;
        }

        public bool WriteSedasData()
        {
            //log.Log("Schreiben der Sedas.dat...", "Schreiben der Sedas.dat Datei", Logger.MsgType.Message);

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
            if (DataProcessing.WriteToFile(this.SedasFile.ToString(), _DestinationPath))
            { return true; }

            string message = $"Fehler beim Schreiben der Zieldatei: {_DestinationPath}";
            //log.Log(message, "Fehler", Logger.MsgType.Message);
            return false;
        }
        #region Löschen und ändern
        public bool DeleteCustomers()
        {
            bool customersDeleted = false;
            string messageTitle = "Kundennummern löschen";
            //log.Log("Kundennummern aus Bestellung löschen...", messageTitle, Logger.MsgType.Message);

            foreach (string customerNumber in _customersToDelete)
            {
                if (SourceDatFile.InputFileOrderLines.FirstOrDefault(orderLine => orderLine.BHMKundenNummer == customerNumber) != null)
                {
                    SourceDatFile.InputFileOrderLines = SourceDatFile.InputFileOrderLines.Where(orderLine => orderLine.BHMKundenNummer != customerNumber).Select(orderLine => orderLine).ToList();
                    //log.Log($" => Kundennummer {customerNumber} aus Bestellungen gelöscht.", messageTitle, Logger.MsgType.Message);
                    customersDeleted = true;
                }
            }

            return customersDeleted;
        }

        public bool DeleteArticle()
        {
            bool articleDeleted = false;
            string messageTitle = "Artikelnummern löschen";
            //log.Log("Löschen von Artikelnummern aus der Bestellung...", messageTitle, Logger.MsgType.Message);

            foreach (string articleNumber in _articlesToDelete)
            {
                if (SourceDatFile.InputFileOrderLines.FirstOrDefault(line => line.BHMArtikelNummer == articleNumber) != null)
                {
                    SourceDatFile.InputFileOrderLines = SourceDatFile.InputFileOrderLines.Where(line => line.BHMArtikelNummer != articleNumber).ToList();
                    //log.Log($" => Artikelnummer {articleNumber} aus Bestellung gelöscht.", messageTitle, Logger.MsgType.Message);
                    articleDeleted = true;
                }
            }

            return articleDeleted;
        }

        private bool ChangeArticleNumbers()
        {
            bool articleChanged = false;
            string messageTitle = "Artikelnummern tauschen";
            //log.Log("Austauschen von Artikelnummern laut tauscheArtikel.txt...", messageTitle, Logger.MsgType.Message);

            foreach (ArticleChangePair pair in articlesToChange)
            {
                bool currentArticleChanged = false;
                var linesToChange = SourceDatFile.InputFileOrderLines.Where(line => line.BHMArtikelNummer == pair.OriginalNumber).ToList();
                foreach (DatSourceOrderLine orderLine in linesToChange)
                {
                    orderLine.BHMArtikelNummer = pair.NewNumber;
                    currentArticleChanged = true;
                    articleChanged = true;
                }

                //if (currentArticleChanged)
                //log.Log($" => Artikelnummer {pair.OriginalNumber} getauscht gegen {pair.NewNumber}.", messageTitle, Logger.MsgType.Message);
            }
            return articleChanged;
        }
        #endregion

    }

        // public ConvertToSedas(string SourceFilePath, string DestinationFilePath, int actualCounter)
        //{
        //    this._SedasErstellDatumJJMMTT = Tools.ConvertToSedasDateJJMMTT(DateTime.Now);
        //    this._SourcePath = SourceFilePath;
        //    this._DestinationPath = DestinationFilePath;
        //    this._counter = actualCounter;

        //    //Importieren der Lösch- und Änderungslisten.
        //    string pathLoescheKunde = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
        //    string pathLoescheArtikel = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
        //    string pathTauscheArtikel = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";
        //    this._customersToDelete = new CustomerDeletionList(DataProcessing.LoadDeleteItemsList(pathLoescheKunde));
        //    this._articlesToDelete = new ArticleDeletionList(DataProcessing.LoadDeleteItemsList(pathLoescheArtikel));
        //    this.articlesToChange = DataProcessing.LoadChangeArticlesList(pathTauscheArtikel);
        //}

        //private List<string> ReadInputFile(string sourcePathNFDatFile)
        //{
        //    List<string> _sourceDataList = new List<string>();
        //    try
        //    {
        //        using (StreamReader sr = new StreamReader(sourcePathNFDatFile))
        //        {
        //            while (!sr.EndOfStream)
        //            {
        //                string line = sr.ReadLine();
        //                if (line != "")
        //                    _sourceDataList.Add(line);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO Fehlerausnahme auslösen und Fehler melden
        //        throw new Exception(ex.Message);
        //    }
        //    return _sourceDataList;
        //}

        //private InputFileOrderLineNF ConvertInputFileData(string inputFileOrderLine)
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

        //        InputFileOrderLineNF newLine = new InputFileOrderLineNF()
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
        //        };

        //        return newLine;
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO Ausnahme anzeigen
        //        throw new Exception(ex.Message);
        //    }
        //}
}