using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConvertDatToSedas
{
    public enum LogMessageLevel
    {
        Information,
        Warning,
        Critical,
        Error
    }

    public delegate void LogMessageEventHandler(object sender, string message, LogMessageLevel level);

    //TODO TEST: Dateien vom Wochenende mit 9000er Nummern.
    //TODO Gruppieren der Daten nach Kundennummer. Es soll vermieden werden, dass in der Sedas.dat 
    //     Kundennummern wiederholt auftauchen. Sie sollen en-block gelistet werden (sortiert nach Art.Nr).
    public class ConvertToSedas
    {
        #region # Properties
        public event LogMessageEventHandler LogEventHandler;
        public ArticleChangeList sedasArticleChangeList { get; private set; }
        public ArticleDeletionList sedasArticleDeletionList { get; private set; }
        public CustomerDeletionList sedasCustomerDeletionList { get; private set; }

        private string _SedasErstellDatumJJMMTT { get; set; }  //Datum der Dateierstellung / des Programmlaufs    
        #endregion


        #region # KONSTRUKTOR

        /// <summary>
        /// Erstellt ein Objekt zum Erzeugen einer SEDAS.DAT Datei aus einer Bestell.dat Datei.
        /// </summary>
        /// <param name="DatFilePath">Einzulesende Datei mit Bestelldaten.</param>
        /// <param name="DestinationFilePath">Dateipfad für SEDAS.DAT-Ausgabe.</param>
        /// <param name="actualCounter">Neue Zählerposition für SEDAS.DAT Datei.</param>
        public ConvertToSedas()
        {
            this._SedasErstellDatumJJMMTT = SedasTools.ConvertToSedasDateJJMMTT(DateTime.Now);
        }
        #endregion

        #region # METHODEN
        /// <summary>
        /// Importiert eine *.dat-Datei zum Konvertieren in eine CSB-Sedas-Datei.
        /// </summary>
        /// <param name="datFileLines">Zeilen der *.dat-Datei.</param>
        /// <returns></returns>
        public DatFile ImportDatFileContent(List<string> datFileLines)
        {
            Log(this,"Einlesen der Importdateien...",LogMessageLevel.Information);

            if (datFileLines.Count() <= 0)
                return null;

            List<DatOrderLine> newOrderLines = GenerateDatOrderLines(datFileLines);
            List<DatOrder> newOrders = GenerateDatOrders(newOrderLines);

            DatFile newDatFile = new DatFile();
            newDatFile.AddList(newOrders);
            return newDatFile;

        }

        /// <summary>
        /// Erzeug aus einem DatFile-Objekt ein SedasFile-Objekt.
        /// </summary>
        /// <param name="DatFileObject">DatFile-Objekt mit den importierten Daten.</param>
        /// <param name="Counter">Fortlaufender Zähler.</param>
        /// <returns></returns>
        public SedasFile ToSedas(DatFile DatFileObject, int Counter)
        {
            if (DatFileObject.Count() > 0)
            {
                SedasFile newSedasFile = new SedasFile(this._SedasErstellDatumJJMMTT, Counter);

                foreach (DatOrder sdatOrder in DatFileObject)
                {
                    string sedasLieferDatumJJMMT = SedasTools.ConvertToSedasDateJJMMTT(sdatOrder.LieferDatumTTMMJJ);
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

        private void Log(object sender, string message, LogMessageLevel level)
        {
            LogEventHandler?.Invoke(this.ToString(), message, level);
        }


        private List<DatOrderLine> GenerateDatOrderLines(List<string> datFileLines)
        {
            Log(this, "Einlesen der Bestellzeilen...", LogMessageLevel.Information);
            List<DatOrderLine> newOrderLines = new List<DatOrderLine>();
            foreach (string line in datFileLines)
            {
                if (line != "")
                {
                    newOrderLines.Add(new DatOrderLine(line));
                }
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
        #endregion
    }
}