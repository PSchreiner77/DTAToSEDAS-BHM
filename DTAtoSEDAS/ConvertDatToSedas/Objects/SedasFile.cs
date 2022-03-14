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
            ;06100,1178
            ;07000000,00001,00001,000000,(

            Erläuterung:
            010...  = Datei-Header
            030...  = Order-Header
            040...  = Bestellposition(en)
            050...  = Order-Footer
            06....  = DateiFooter (Zusammenfassung)
            070...  = DateiFooter (Dateiende)
         */

        private string _ErstellDatumSedasJJMMTT;
        private int _IniSedasRunThroughCounter; //TODO Was bedeutet der Zähler?
        public List<SedasOrder> SedasOrdersList = new List<SedasOrder>();


        public int CustomerOrdersCount { get { return SedasOrdersList.Count; } }
        public int OverallOrderLineEntriesCount { get { return GetTotalNumberOfOrderLines(); } }

        //KONSTRUKTOR
        public SedasFile(string Erstelldatum, int IniSedasRunThroughCounter)
        {
            _ErstellDatumSedasJJMMTT = Erstelldatum;
            _IniSedasRunThroughCounter = IniSedasRunThroughCounter;
        }

        //METHODEN

        //TODO Propery erstellen
        private int GetTotalNumberOfOrderLines()
        {
            int count = 0;
            foreach (SedasOrder order in SedasOrdersList)
            {
                count += order.SedasOrderLines.Count;
            }
            return count;
            ;
        }

        private string GetSedasFileHeaderString()
        {
            return $"010()000377777777777771{_ErstellDatumSedasJJMMTT};,{_IniSedasRunThroughCounter}\r\n;)0240051310000002";
        }

        private string GetSedasFileFooterString()
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
            string FooterLine1 = $":06{Tools.ExpandLeftStringSide(CustomerOrdersCount.ToString(), 3)};{OverallOrderLineEntriesCount}";
            string FooterLine2 = $";07000000,00001,00001,000000,(                                                      ";
            string FooterLine3 = "                                                                                    ";
            return FooterLine1 + "\r\n" + FooterLine2 + "\r\n" + FooterLine3;
        }


        public override string ToString()
        {
            string cr = "\r\n";

            string returnString = GetSedasFileHeaderString() + cr;
            foreach (SedasOrder order in SedasOrdersList)
            {
                returnString += order.ToString();
            }
            returnString += GetSedasFileFooterString() + cr;
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
