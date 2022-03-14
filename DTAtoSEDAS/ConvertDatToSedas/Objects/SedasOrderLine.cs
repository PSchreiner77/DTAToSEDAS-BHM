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
}
