using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class SedasOrder : IEnumerable<SedasOrderLine>
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
}
