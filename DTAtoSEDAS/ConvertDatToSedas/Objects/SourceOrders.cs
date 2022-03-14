using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class SourceOrders : IEnumerable<SourceOrderLine>
    {
        public List<SourceOrderLine> SourceOrderLines = new List<SourceOrderLine>();


        public void Add(SourceOrderLine sourceOrderLine)
        {
            SourceOrderLines.Add(sourceOrderLine);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SourceOrderLines.GetEnumerator();
        }

        public IEnumerator<SourceOrderLine> GetEnumerator()
        {
            return SourceOrderLines.GetEnumerator();
        }

    }
}
