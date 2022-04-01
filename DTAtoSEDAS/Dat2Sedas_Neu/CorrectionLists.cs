using ConvertDatToSedas;

namespace Dat2Sedas_Neu
{
    internal class CorrectionLists
    {
        public ArticleChangeList sedasArticleChangeList { get; private set; }
        public ArticleDeletionList sedasArticleDeletionList { get; private set; }
        public CustomerDeletionList sedasCustomerDeletionList { get; private set; }
        public Parameters Param;

        public void Generate()
        {
            this.sedasArticleChangeList = DataProcessing.GetChangeArticlesList(Param.PathChangeArticlesList);
            this.sedasArticleDeletionList = DataProcessing.GetDeleteArticlesList(Param.PathDeleteArticleList);
            this.sedasCustomerDeletionList = DataProcessing.GetDeleteCustomersList(Param.PathDeleteCustomerList);
        }
    }
}
