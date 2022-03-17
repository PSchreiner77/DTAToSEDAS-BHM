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
       
        //KONSTRUKTOR

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
              

        //METHODEN
        /// <summary>
        /// Importiert eine *.dat-Datei zum Konvertieren in eine CSB-Sedas-Datei.
        /// </summary>
        /// <param name="datFileLines">Zeilen der *.dat-Datei.</param>
        /// <returns></returns>
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