using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class DatSource : IEnumerable<DatSourceOrderLine>
    {
        public List<DatSourceOrderLine> InputFileOrderLines = new List<DatSourceOrderLine>();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InputFileOrderLines.GetEnumerator();
        }

        public IEnumerator<DatSourceOrderLine> GetEnumerator()
        {
            return InputFileOrderLines.GetEnumerator();
        }

    }
}
