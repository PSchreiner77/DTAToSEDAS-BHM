using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class ArticleChangeList : IEnumerable
    {
        private List<ArticleChangePair> _articlesToChange;

        public ArticleChangeList()
        {
            _articlesToChange = new List<ArticleChangePair>();
        }

        public void Add(ArticleChangePair articleExchangePair)
        {
            _articlesToChange.Add(articleExchangePair);
        }

        public void Remove(ArticleChangePair articleExcangePair)
        {
            ArticleChangePair result = _articlesToChange.First(pair => (pair.OriginalNumber == articleExcangePair.OriginalNumber) & pair.NewNumber == articleExcangePair.NewNumber);

            if (result != null)
            {
                _articlesToChange.Remove(result);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _articlesToChange.GetEnumerator();
        }
    }
}
