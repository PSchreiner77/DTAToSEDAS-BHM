using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class SedasFile : IEnumerable<SedasOrder>
    {
        /*
         *    CSB-SEDAS Datei-Beispiel:
         *    
            010()000377777777777771180705;,241
            ;)0240051310000002
            ;030,14,00000000000000000,180705,180706,,,,3789         ,,
            ;0400000000001360,40002000,,,,02 000000,,
            ...
            ...
            ...
            ;0400000000004023,40001000,,,,02 000000,,
            ;05000000039000   
            ...
            ;06100,1178
            ;07000000,00001,00001,000000,(

            Erläuterung:
            010...  = Datei-Header
            030...  = Order-Header
            040...  = Bestellposition(en)
            050...  = Order-Footer
            ...
            06....  = DateiFooter (Zusammenfassung)
            070...  = DateiFooter (Dateiende)
         */

        private string _ErstellDatumSedasJJMMTT;
        private int _Counter; //Laufende Nummer 
        public List<SedasOrder> SedasOrdersList = new List<SedasOrder>();

        /// <summary>
        /// Gibt die Anzahl aller Kundenbestellungen zurück.
        /// </summary>
        public int CustomerOrdersCount { get { return SedasOrdersList.Count(); } }

        /// <summary>
        /// Gibt die Anzahl aller Bestellzeilen zurück.
        /// </summary>
        public int OrderLinesCount { get { return SedasOrdersList.Sum(o => o.OrderPositionsCount); } }

        //KONSTRUKTOR
        public SedasFile(string Erstelldatum, int Counter)
        {
            _ErstellDatumSedasJJMMTT = Erstelldatum;
            _Counter = Counter;
        }

        //METHODEN
        public void AddOrder(SedasOrder sedasOrder)
        {
            SedasOrdersList.Add(sedasOrder);
        }

        public void AddListOfOrders(List<SedasOrder> sedasOrderList)
        {
            foreach (SedasOrder order in sedasOrderList)
            {
                SedasOrdersList.Add(order);
            }
        }

        public void RemoveOrder(SedasOrder sedasOrder)
        {
            SedasOrdersList.Remove(sedasOrder);
        }

        public void RemoveCustomers(CustomerDeletionList customersToDelete)
        {
            if (customersToDelete != null)
            {
                foreach (string customer in customersToDelete)
                {
                    RemoveCustomer(customer);
                }
            }
        }

        public void RemoveCustomer(string customerNumber)
        {
            SedasOrdersList = SedasOrdersList.Where(o => o.BHMKundennummer != customerNumber).ToList();
        }

        public void RemoveArticles(ArticleDeletionList articlestoDelete)
        {
            if (articlestoDelete != null)
            {
                foreach (string articleNumber in articlestoDelete)
                {
                    this.RemoveArticle(articleNumber);
                }
            }
        }

        public void RemoveArticle(string articleNumberToDelete)
        {
            foreach (SedasOrder order in SedasOrdersList)
            {
                order.RemoveArticle(articleNumberToDelete);
            }
        }

        public void ChangeArticles(ArticleChangeList articlesToChange)
        {
            if (articlesToChange != null)
            {
                foreach (ArticleChangePair articlePair in articlesToChange)
                {
                    this.ChangeArticle(articlePair);
                }
            }
        }
        
        public void ChangeArticle(ArticleChangePair articlePair)
        {
            foreach (SedasOrder order in SedasOrdersList)
            {
                order.ChangeArticle(articlePair);
            }
        }

        public string Header()
        {
            return $"010()000377777777777771{_ErstellDatumSedasJJMMTT};,{_Counter}\r\n;)0240051310000002";
        }

        public string Footer()
        {
            #region Aufbau FooterLine
            //--FOOTER der Zieldatei
            //; 06100,1178
            //; 07000000,00001,00001,000000,(
            //
            //;06108,1178 
            //  ;06    = Kennung Zusammenfassung Einträge
            //  108,   = Anzahl Kunden in Datei (Blöcke)
            //  1178   = Anzahl einzelner Datensätze/Artikelzeilen
            #endregion
            string FooterLine1 = $":06{SedasTools.ExpandLeftStringSide(this.CustomerOrdersCount.ToString(), 3)};{this.OrderLinesCount}";
            string FooterLine2 = $";07000000,00001,00001,000000,(                                                      ";
            string FooterLine3 = "                                                                                    ";

            return FooterLine1 + "\r\n" + FooterLine2 + "\r\n" + FooterLine3;
        }

        /// <summary>
        /// Erzeugt den Textinhalt für eine Sedas-Datei.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string cr = "\r\n";

            string returnString = Header() + cr;
            foreach (SedasOrder order in SedasOrdersList)
            {
                returnString += order.ToString();
            }
            returnString += Footer() + cr;
            return returnString;
        }


        public IEnumerator<SedasOrder> GetEnumerator()
        {
            return SedasOrdersList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
