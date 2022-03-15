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
        private List<SedasOrderLine> SedasOrderLines = new List<SedasOrderLine>();
        public string BHMKundennummer { get; private set; }
        
        /// <summary>
        /// Gibt die Anzahl der Bestellpostitionen zurück.
        /// </summary>
        public int OrderPositionsCount { get { return SedasOrderLines.Count(); } }

        private string _ErstellDatumJJMMTT;
        private string _LieferDatumJJMMTT;

        //KONSTRUKTOR
        public SedasOrder(string ErstellDatumJJMMTT, string LieferDatumJJMMTT, string BHMKundennummer)
        {
            _ErstellDatumJJMMTT = ErstellDatumJJMMTT;
            _LieferDatumJJMMTT = LieferDatumJJMMTT;
            this.BHMKundennummer = BHMKundennummer;
        }


        //METHODEN
        public int GetOrderedArticleQuantity()
        {
            int count = 0;
            foreach (SedasOrderLine orderLine in SedasOrderLines)
            {
                count += Convert.ToInt32(orderLine.Bestellmenge.Substring(0, orderLine.Bestellmenge.Length - 3));
            }

            return count;
        }

        public void Add(SedasOrderLine sedasOrderLine)
        {
            //TODO Prüfen, ob Zeile/Artikel schon existiert. Wenn ja, zusammenführen.
            this.SedasOrderLines.Add(sedasOrderLine);
        }

        public void AddList(IList<SedasOrderLine> liste)
        {
            foreach (SedasOrderLine line in liste)
            {
                this.Add(line);
            }
        }

        public void RemoveArticles(ArticleDeletionList articlesToDelete)
        {
            foreach (string articleNumber in articlesToDelete)
            {
                this.RemoveArticle(articleNumber);
            }
        }

        public void RemoveArticle(string articleNumber)
        {
            SedasOrderLines = SedasOrderLines.Where(ol => ol.BHMArtikelNummer != articleNumber).ToList();
        }

        public void RemoveOrderPosition(SedasOrderLine sedasOrderLine)
        {
            SedasOrderLines.Remove(sedasOrderLine);
        }

        public void ChangeArticle(ArticleChangePair articleChangePair)
        {
            foreach (SedasOrderLine orderLine in SedasOrderLines)
            {
                if (orderLine.BHMArtikelNummer == articleChangePair.OriginalNumber)
                {
                    orderLine.BHMArtikelNummer = articleChangePair.NewNumber;
                }
            }
        }

        public string Header()
        {
            /* ;030,14,00000000000000000,180705,180706,,,,3789         ,,
            */
            return $";030,14,00000000000000000,{_ErstellDatumJJMMTT},{_LieferDatumJJMMTT},,,,{BHMKundennummer}         ,,";
        }

        public string Footer()
        {
            //;05000000039000
            //;05               Kennung Footer
            //   000000039      neun Stellen für Summe bestellter Artikelmengen
            //            0000  1000er Stelle für Artikelmenge

            string articleQuantity = Tools.ExpandLeftStringSide(this.GetOrderedArticleQuantity().ToString(), 9);
            return $";05{articleQuantity}000";
        }


        public override string ToString()
        {
            string cr = "\r\n";

            string returnString = Header() + cr;
            foreach (SedasOrderLine orderLine in this.SedasOrderLines)
            {
                returnString += orderLine.ToString() + cr;
            }
            returnString += Footer() + cr;

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
