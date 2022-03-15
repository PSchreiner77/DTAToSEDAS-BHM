using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class SourceOrder : IEnumerable<SourceOrderLine>
    {
        List<SourceOrderLine> sourceOrderLines = new List<SourceOrderLine>();

        public string BestellDatumTTMMJJ
        {
            get
            {
                if (this.sourceOrderLines.Count() > 0)
                    return this.sourceOrderLines[0].BestellDatumTTMMJJ;
                return "";
            }
        }

        public string LieferDatumTTMMJJ
        {
            get
            {
                if (this.sourceOrderLines.Count() > 0)
                    return this.sourceOrderLines[0].LieferDatumTTMMJJ;
                return "";
            }
        }


        public string BHMKundennummer { get; set; }

        public int Count { get { return sourceOrderLines.Count(); } }

        public SourceOrder(string customerNumber)
        { this.BHMKundennummer = customerNumber; }

        public void Add(SourceOrderLine sourceOrderLine)
        {
            this.sourceOrderLines.Add(sourceOrderLine);
        }

        public void AddList(List<SourceOrderLine> sourceOrderLines)
        {
            foreach (SourceOrderLine line in sourceOrderLines)
            {
                this.Add(line);
            }
        }

        public IEnumerator<SourceOrderLine> GetEnumerator()
        {
            return sourceOrderLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
