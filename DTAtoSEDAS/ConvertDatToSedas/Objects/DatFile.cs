using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class DatFile : IEnumerable<DatOrder>
    {
        private List<DatOrder> DatOrders = new List<DatOrder>();

        public void Add(DatOrder datOrder)
        {
            DatOrders.Add(datOrder);
        }

        public void AddList(List<DatOrder> datOrderList)
        {
            foreach (DatOrder datOrder in datOrderList)
            {
                this.Add(datOrder);
            }
        }

        public void Remove( DatOrder datOrder)
        {
            DatOrders.Remove(datOrder);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<DatOrder> GetEnumerator()
        {
            return DatOrders.GetEnumerator();
        }

    }
}
