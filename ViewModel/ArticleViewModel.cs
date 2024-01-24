namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types;
    using System.Diagnostics;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Style.Base;

    public class ArticleViewModel : BasePage
    {
        public ArticleViewModel()
        {
            UpdateListOfArticles();
            UpdateListofProviders();
            _currentScrap = new();
        }

        public ViewCollection<Provider> ListofProviders
        {
            get { return _listOfProviders; }
            set { _listOfProviders = value; PropertyCall(); }
        }
        public ViewCollection<Article> ListofArticles
        {
            get { return _listofArticles; }
            set { _listofArticles = value; PropertyCall(); }
        }
        public string CurrentArticleName 
        {
            get { return _currentArticleName; }
            set 
            {
                _currentArticleName = value;
                PropertyCall();
            }
        }
        public ICommand CreateArticle { get; set; } = new FastCommand(
    (object parameter) => 
    {
        ArticleViewModel ins = (ArticleViewModel)parameter;
        ins.CreateArticleCommand();
    },
    (object parameter) => { return true; });
        public ICommand CreateProvider { get; set; } = new FastCommand(
(object parameter) =>
{
ArticleViewModel ins = (ArticleViewModel)parameter;
ins.CreateProviderCommand();
},
(object parameter) => { return true; });
        public URLScrap CurrentScrap
        { 
            get { return _currentScrap; } 
            set { _currentScrap = value; PropertyCall(); }
        }


        private void CreateArticleCommand()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Article>(nameof(Article));
            var newItem = new Article()
            {
                Name = CurrentArticleName
            };
            tb.Add(newItem);
            UpdateListOfArticles();
        }
        private void CreateProviderCommand()
        {
            //open a textbox for entering new provider name
            UpdateListofProviders();
        }
        private void UpdateListOfArticles()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Article>(nameof(Article));
            ListofArticles = tb.List;
        }
        private void UpdateListofProviders()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Provider>(nameof(Provider));
            ListofProviders = tb.List;
        }

        private ViewCollection<Provider> _listOfProviders;
        private ViewCollection<Article> _listofArticles;
        private URLScrap _currentScrap;
        private string _currentArticleName="";
    }
}
