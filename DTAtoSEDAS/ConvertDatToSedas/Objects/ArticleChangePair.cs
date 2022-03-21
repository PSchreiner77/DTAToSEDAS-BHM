using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDatToSedas
{
    public class ArticleChangePair
    {
        public string OriginalNumber { get; set; }
        public string NewNumber { get; set; }
        public string Description { get; set; }

        public ArticleChangePair(string OriginalNumber, string NewNumber)
        {
            this.OriginalNumber = OriginalNumber.Trim();
            this.NewNumber = NewNumber.Trim();
        }
        public ArticleChangePair(string OriginalNumber, string NewNumber, string Description)
        {
            this.OriginalNumber = OriginalNumber.Trim();
            this.NewNumber = NewNumber.Trim();
            this.Description = Description.Trim();
        }
    }
}
