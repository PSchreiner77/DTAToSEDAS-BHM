using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class SedasOrderLine
    {
        /* ;0400000000000317,40002000,,,,02 000000,,
         *      ;040000          = Kennung Zeile BestellPosition
         *      0000000317       = zehn Stellen Artikelnummer
         *      ,4               = fix
         *      00002000         = Bestellenge (Wert/1000)
         *      ,,,,02 000000,,  = fix
        */

        public string BHMArtikelNummer { get; set; }
        public string Bestellmenge { get; set; }


        //KONSTRUKTOR
        public SedasOrderLine(string BHMArtikelNummer, string Bestellmenge)
        {
            this.BHMArtikelNummer = BHMArtikelNummer;
            this.Bestellmenge = Bestellmenge;
        }

        public SedasOrderLine(string BHMArtikelNummer, int Bestellmenge)
        {

        }

        //METHODEN
        public override string ToString()
        {
            return $";040000{SedasTools.ExpandLeftStringSide(BHMArtikelNummer, 10)},4{SedasTools.ExpandLeftStringSide(Bestellmenge, 7)},,,,02 000000,,";
        }
    }
}
