namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Net.Http.Headers;
    using System.Windows.Input;
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types;
    using WPFCollection.Style.Base;

    public class DashboardViewModel : BasePage
    {
        public DashboardViewModel()
        {
            var db = DataHolder.XMLData.CreateDataBase(DataHolder.XMLDataBaseName);
            db.CreateTable<Article>(nameof(Article));
            db.CreateTable<ContainerXPath>(nameof(ContainerXPath));
            db.CreateTable<Prices>(nameof(Prices));
            db.CreateTable<Provider>(nameof(Provider));
            db.CreateTable<URLType>(nameof(URLType));
            db.CreateTable<XPathItem>(nameof(XPathItem));
            CurrentContent = new ArticleViewModel();
            //create test list 
            var list = new List<ArticleView>();
            for (int x = 0; x < 10; x++)
            {
                var articleView = new ArticleView()
                {
                    Name = DataGenerator.GenerateRandomStringValue(),
                    ArticleDetails = [],
                };
                for (int y = 0; y < 5; y++)
                {
                    var tagList = new List<TagView>();
                    for (int i = 0; i < 6; i++)
                    {
                        var tagView = new TagView()
                        {
                            TagName = DataGenerator.GenerateRandomStringValue(),
                            TagValue = DataGenerator.GenerateRandomStringValue(),
                        };
                        tagList.Add(tagView);
                    }
                    var articleDetails = new ArticleDetails()
                    {
                        Color = DataGenerator.GenerateRandomStringValue(),
                        Tags = tagList

                    };
                    articleView.ArticleDetails.Add(articleDetails);
                }
                list.Add(articleView);
            }
            //write generated list into excel file and save it
            var ef = new ExcelFile();
            ef.WriteFile(list);
        }
        private object _currentContent;
        
        public object CurrentContent
        {
            get { return _currentContent; }
            set { _currentContent = value; PropertyCall(); }
        }


        public ICommand GotoReportPage { get; set; } = new FastCommand
            ((object parameter) => { DashboardViewModel model = (DashboardViewModel)parameter; model.GotoReportPageHandler(); }, (object parameter) => { return true; });
        public ICommand GotoCreateArticlePage { get; set; } = new FastCommand
            ((object parameter) => { DashboardViewModel model = (DashboardViewModel)parameter; model.GotoCreateArticlePageHandler(); }, (object parameter) => { return true; });

        private void GotoReportPageHandler()
        {
            CurrentContent = new ReportViewModel();
        }
        private void GotoCreateArticlePageHandler()
        {
            CurrentContent = new ArticleViewModel();
        }
    }
}
