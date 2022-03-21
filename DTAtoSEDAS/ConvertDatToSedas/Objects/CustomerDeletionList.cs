using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class CustomerDeletionList : IEnumerable
    {
        private List<string> _customerNumbers;

        public CustomerDeletionList(List<string> list)
        {
            _customerNumbers = list;

            for (int i = 0; i < _customerNumbers.Count(); i++)
            {
                _customerNumbers[i] = _customerNumbers[i].Trim();
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _customerNumbers.GetEnumerator();
        }
    }
}
