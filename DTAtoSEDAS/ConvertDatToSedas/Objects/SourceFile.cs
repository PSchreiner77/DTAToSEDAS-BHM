using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class SourceFile : IEnumerable<SourceOrder>
    {
        public List<SourceOrder> SourceOrders = new List<SourceOrder>();


        public void Add(SourceOrder sourceOrder)
        {
            SourceOrders.Add(sourceOrder);
        }

        public void AddList(List<SourceOrder> sourceOrderList)
        {
            foreach (SourceOrder sourceOrder in sourceOrderList)
            {
                this.Add(sourceOrder);
            }
        }

        public void Remove( SourceOrder sourceOrder)
        {
            SourceOrders.Remove(sourceOrder);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<SourceOrder> GetEnumerator()
        {
            return SourceOrders.GetEnumerator();
        }

    }
}
