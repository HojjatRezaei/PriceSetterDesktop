namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types;
    using WPFCollection.Style.Base;

    public class DashboardViewModel : BasePage
    {
        public DashboardViewModel()
        {
            CurrentContent = new ArticleViewModel();
            var db = DataHolder.XMLData.CreateDataBase(DataHolder.XMLDataBaseName);
            db.CreateTable<Article>(nameof(Article));
            db.CreateTable<URLScrap>(nameof(URLScrap));
            db.CreateTable<Provider>(nameof(Provider));

        }
        private object _currentContent;

        public object CurrentContent
        {
            get { return _currentContent; }
            set { _currentContent = value; PropertyCall(); }
        }

    }
}
