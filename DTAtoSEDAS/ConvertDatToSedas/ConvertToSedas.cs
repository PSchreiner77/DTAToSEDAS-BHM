using System;
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
        private int _counter = 0;
        private SedasFile _sedasFile;

        private string _pathDeleteCustomer = Directory.GetCurrentDirectory() + @"\loescheKunde.txt";
        private string _pathDeleteArticle = Directory.GetCurrentDirectory() + @"\loescheArtikel.txt";
        private string _pathChangeArticle = Directory.GetCurrentDirectory() + @"\tauscheArtikel.txt";

        private CustomerDeletionList _customersToDelete;
        private ArticleDeletionList _articlesToDelete;
        private ArticleChangeList articlesToChange = new ArticleChangeList();

        public SedasFile GetSedas { get { return this._sedasFile; } }


        //KONSTRUKTOR

        public ConvertToSedas()
        { }

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

            this._customersToDelete = customersToDelete;
            this._articlesToDelete = articlesToDelete;
            this.articlesToChange = articlesToChangeList;
        }


        //METHODEN
        public SourceFile ImportDatFileContent(List<string> sourceFileLines)
        {
            if (sourceFileLines.Count() > 0)
            {
                List<SourceOrderLine> newOrderLines = GenerateSourceOrderLines(sourceFileLines);
                List<SourceOrder> newOrders = GenerateSourceOrders(newOrderLines);

                SourceFile newSourceFile = new SourceFile();
                newSourceFile.AddList(newOrders);
                return newSourceFile;
            }
            return null;
        }


        public SedasFile ToSedas(SourceFile sourceOrders, int Counter)
        {
            if (sourceOrders.Count() > 0)
            {
                SedasFile newSedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, Counter);

                foreach (SourceOrder sourceOrder in sourceOrders)
                {
                    string sedasLieferDatumJJMMT = Tools.ConvertToSedasDateJJMMTT(sourceOrder.LieferDatumTTMMJJ);
                    SedasOrder newSedasOrder = new SedasOrder(this._SedasErstellDatumJJMMTT,
                                                              sedasLieferDatumJJMMT,
                                                              sourceOrder.BHMKundennummer);

                    foreach (SourceOrderLine sourceOrderLine in sourceOrder)
                    {
                        SedasOrderLine newSedasOrderLine = ConvertToSedasOrderLine(sourceOrderLine);
                        newSedasOrder.Add(newSedasOrderLine);
                    }

                    newSedasFile.AddOrder(newSedasOrder);
                }

                return newSedasFile;
            }
            return null;
        }


        private List<SourceOrderLine> GenerateSourceOrderLines(List<string> sourceFileLines)
        {
            List<SourceOrderLine> newOrderLines = new List<SourceOrderLine>();
            foreach (string line in sourceFileLines)
            {
                newOrderLines.Add(new SourceOrderLine(line));
            }
            return newOrderLines;
        }

        private List<SourceOrder> GenerateSourceOrders(List<SourceOrderLine> sourceOrderLines)
        {
            List<SourceOrder> newOrders = new List<SourceOrder>();

            var customerGroupedOrderLines = sourceOrderLines.GroupBy(o => o.BHMKundenNummer);

            foreach (var customerOrders in customerGroupedOrderLines)
            {
                SourceOrder newOrder = new SourceOrder(customerOrders.First().BHMKundenNummer);
                newOrder.AddList(customerOrders.ToList());
                newOrders.Add(newOrder);
            }
            return newOrders;
        }

        private SedasOrderLine ConvertToSedasOrderLine(SourceOrderLine sourceOrderLine)
        {
            SedasOrderLine sol = new SedasOrderLine(sourceOrderLine.BHMArtikelNummer, sourceOrderLine.BestellMenge);
            return sol;
        }



        #region Löschen und ändern



        //public bool DeleteCustomers()
        //{
        //    bool customersDeleted = false;
        //    string messageTitle = "Kundennummern löschen";
        //    //log.Log("Kundennummern aus Bestellung löschen...", messageTitle, Logger.MsgType.Message);

        //    foreach (string customerNumber in _customersToDelete)
        //    {
        //        if (SourceDatFile.InputFileOrderLines.FirstOrDefault(orderLine => orderLine.BHMKundenNummer == customerNumber) != null)
        //        {
        //            SourceDatFile.InputFileOrderLines = SourceDatFile.InputFileOrderLines.Where(orderLine => orderLine.BHMKundenNummer != customerNumber).Select(orderLine => orderLine).ToList();
        //            //log.Log($" => Kundennummer {customerNumber} aus Bestellungen gelöscht.", messageTitle, Logger.MsgType.Message);
        //            customersDeleted = true;
        //        }
        //    }

        //    return customersDeleted;
        //}

        //public bool DeleteArticle()
        //{
        //    bool articleDeleted = false;
        //    string messageTitle = "Artikelnummern löschen";
        //    //log.Log("Löschen von Artikelnummern aus der Bestellung...", messageTitle, Logger.MsgType.Message);

        //    foreach (string articleNumber in _articlesToDelete)
        //    {
        //        if (SourceDatFile.InputFileOrderLines.FirstOrDefault(line => line.BHMArtikelNummer == articleNumber) != null)
        //        {
        //            SourceDatFile.InputFileOrderLines = SourceDatFile.InputFileOrderLines.Where(line => line.BHMArtikelNummer != articleNumber).ToList();
        //            //log.Log($" => Artikelnummer {articleNumber} aus Bestellung gelöscht.", messageTitle, Logger.MsgType.Message);
        //            articleDeleted = true;
        //        }
        //    }

        //    return articleDeleted;
        //}

        //private bool ChangeArticleNumbers()
        //{
        //    bool articleChanged = false;
        //    string messageTitle = "Artikelnummern tauschen";
        //    //log.Log("Austauschen von Artikelnummern laut tauscheArtikel.txt...", messageTitle, Logger.MsgType.Message);

        //    foreach (ArticleChangePair pair in articlesToChange)
        //    {
        //        bool currentArticleChanged = false;
        //        var linesToChange = SourceDatFile.InputFileOrderLines.Where(line => line.BHMArtikelNummer == pair.OriginalNumber).ToList();
        //        foreach (SourceOrderLine orderLine in linesToChange)
        //        {
        //            orderLine.BHMArtikelNummer = pair.NewNumber;
        //            currentArticleChanged = true;
        //            articleChanged = true;
        //        }

        //        //if (currentArticleChanged)
        //        //log.Log($" => Artikelnummer {pair.OriginalNumber} getauscht gegen {pair.NewNumber}.", messageTitle, Logger.MsgType.Message);
        //    }
        //    return articleChanged;
        //}
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




}