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

        public int CustomerOrdersCount { get { return SedasOrdersList.Count; } }
        public int OrderLinesCount { get { return SedasOrdersList.Sum(o => o.SedasOrderLines.Count()); } }

        //KONSTRUKTOR
        public SedasFile(string Erstelldatum, int Counter)
        {
            _ErstellDatumSedasJJMMTT = Erstelldatum;
            _Counter = Counter;
        }

        //METHODEN
        public void Add(SedasOrder sedasOrder)
        {
            SedasOrdersList.Add(sedasOrder);
        }

        public void AddList(List<SedasOrder> sedasOrderList)
        {
            foreach (SedasOrder order in sedasOrderList)
            {
                SedasOrdersList.Add(order);
            }
        }

        public void Remove(SedasOrder sedasOrder)
        {
            throw new NotImplementedException();
        }

        public void RemoveCustomer(CustomerDeletionList customersToDelete)
        {
            foreach (string customer in customersToDelete)
            {
                RemoveCustomer(customer);
            }
        }
        public void RemoveCustomer(string customerNumber)
        {
            //var order = SedasOrdersList.Where(o => o);
        }

        public void RemoveArticle(string articleNumber)
        {
            throw new NotImplementedException();
        }

        public void ChangeArticle(ArticleChangePair articlePair)
        {
            throw new NotImplementedException();
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
            string FooterLine1 = $":06{Tools.ExpandLeftStringSide(CustomerOrdersCount.ToString(), 3)};{OrderLinesCount}";
            string FooterLine2 = $";07000000,00001,00001,000000,(                                                      ";
            string FooterLine3 = "                                                                                    ";
            return FooterLine1 + "\r\n" + FooterLine2 + "\r\n" + FooterLine3;
        }

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
