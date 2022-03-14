using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class DatSourceOrderLine
    {
        #region Aufbau Bestellzeile
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
        #endregion

        public DatSourceOrderLine()
        {
        }

        public DatSourceOrderLine(string OrderLineFromImportFile)
        {
            try
            {
                string[] arrZeile = OrderLineFromImportFile.Split(';');

                this.NFKennzeichen = arrZeile[0];
                this.BHMFilialNummer = arrZeile[1];
                this.BHMKundenNummer = arrZeile[2];
                this.BestellDatumTTMMJJ = arrZeile[3];
                this.LieferDatumTTMMJJ = arrZeile[4];
                this.BHMArtikelKey = arrZeile[5];
                this.BestellMenge = arrZeile[6];
                this.Preis = arrZeile[7];
                this.BHMArtikelNummer = arrZeile[8];
                this.Verpackungseinheit = arrZeile[9];
                this.AnzahlBestellPositionen = arrZeile[10];
            }
            catch (Exception ex)
            {
                //TODO Ausnahme anzeigen
                throw new Exception(ex.Message);
            }
        }

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
}
