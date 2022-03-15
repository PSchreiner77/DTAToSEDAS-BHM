using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class DatOrder : IEnumerable<DatOrderLine>
    {
        private List<DatOrderLine> datOrderLines = new List<DatOrderLine>();

        public string BestellDatumTTMMJJ
        {
            get
            {
                if (this.datOrderLines.Count() > 0)
                    return this.datOrderLines[0].BestellDatumTTMMJJ;
                return "";
            }
        }

        public string LieferDatumTTMMJJ
        {
            get
            {
                if (this.datOrderLines.Count() > 0)
                    return this.datOrderLines[0].LieferDatumTTMMJJ;
                return "";
            }
        }


        public string BHMKundennummer { get; set; }

        public int Count { get { return datOrderLines.Count(); } }

        public DatOrder(string customerNumber)
        { this.BHMKundennummer = customerNumber; }

        public void Add(DatOrderLine datOrderLine)
        {
            this.datOrderLines.Add(datOrderLine);
        }

        public void AddList(List<DatOrderLine> datOrderLines)
        {
            foreach (DatOrderLine line in datOrderLines)
            {
                this.Add(line);
            }
        }

        public IEnumerator<DatOrderLine> GetEnumerator()
        {
            return datOrderLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
