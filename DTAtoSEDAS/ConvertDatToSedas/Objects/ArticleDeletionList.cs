using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class ArticleDeletionList : IEnumerable
    {
        private List<string> _articlesToDelete;

        public ArticleDeletionList(List<string> articleList)
        {
            _articlesToDelete = articleList;
            for (int i = 0; i < _articlesToDelete.Count(); i++)
            {
                _articlesToDelete[i] = _articlesToDelete[i].Trim();
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _articlesToDelete.GetEnumerator();
        }
    }
}
