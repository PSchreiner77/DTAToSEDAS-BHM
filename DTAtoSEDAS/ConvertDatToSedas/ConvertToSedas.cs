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
        /// <param name="DatFilePath">Einzulesende Datei mit Bestelldaten.</param>
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
        public DatFile ImportDatFileContent(List<string> datFileLines)
        {
            if (datFileLines.Count() > 0)
            {
                List<DatOrderLine> newOrderLines = GenerateDatOrderLines(datFileLines);
                List<DatOrder> newOrders = GenerateDatOrders(newOrderLines);

                DatFile newDatFile = new DatFile();
                newDatFile.AddList(newOrders);
                return newDatFile;
            }
            return null;
        }


        public SedasFile ToSedas(DatFile datOrders, int Counter)
        {
            if (datOrders.Count() > 0)
            {
                SedasFile newSedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, Counter);

                foreach (DatOrder sdatOrder in datOrders)
                {
                    string sedasLieferDatumJJMMT = Tools.ConvertToSedasDateJJMMTT(sdatOrder.LieferDatumTTMMJJ);
                    SedasOrder newSedasOrder = new SedasOrder(this._SedasErstellDatumJJMMTT,
                                                              sedasLieferDatumJJMMT,
                                                              sdatOrder.BHMKundennummer);

                    foreach (DatOrderLine datOrderLine in sdatOrder)
                    {
                        SedasOrderLine newSedasOrderLine = ConvertToSedasOrderLine(datOrderLine);
                        newSedasOrder.Add(newSedasOrderLine);
                    }

                    newSedasFile.AddOrder(newSedasOrder);
                }

                return newSedasFile;
            }
            return null;
        }


        private List<DatOrderLine> GenerateDatOrderLines(List<string> datFileLines)
        {
            List<DatOrderLine> newOrderLines = new List<DatOrderLine>();
            foreach (string line in datFileLines)
            {
                newOrderLines.Add(new DatOrderLine(line));
            }
            return newOrderLines;
        }

        private List<DatOrder> GenerateDatOrders(List<DatOrderLine> datOrderLines)
        {
            List<DatOrder> newOrders = new List<DatOrder>();

            var customerGroupedOrderLines = datOrderLines.GroupBy(o => o.BHMKundenNummer);

            foreach (var customerOrders in customerGroupedOrderLines)
            {
                DatOrder newOrder = new DatOrder(customerOrders.First().BHMKundenNummer);
                newOrder.AddList(customerOrders.ToList());
                newOrders.Add(newOrder);
            }
            return newOrders;
        }

        private SedasOrderLine ConvertToSedasOrderLine(DatOrderLine datOrderLine)
        {
            SedasOrderLine sol = new SedasOrderLine(datOrderLine.BHMArtikelNummer, datOrderLine.BestellMenge);
            return sol;
        }

    }
}