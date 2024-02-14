namespace PriceSetterDesktop.ViewModel
{
    using Newtonsoft.Json;
    using PriceSetterDesktop.Libraries.Statics;
    using System.Windows.Input;
    using WPFCollection.Style.Base;

    public class DashboardViewModel : BasePage
    {
        public DashboardViewModel()
        {
            DebugTest();
            CurrentContent = new ArticleViewModel();
            var articleList = APIDataStorage.ArticleManager.List.ToList();
            //foreach (var item in articleList)
            //{
            //    if(item.ColorMetaID != -1)
            //    {
            //        var getResult = APIDataStorage.ScrapManager.GetSingle(new Libraries.Types.Interaction.ScrapResult() { ArticleID = item.ID , ColorID = item.ColorMetaID});

            //    }
            //}
        }
        private void DebugTest()
        {
        }
        private object? _currentContent=null;
        public object? CurrentContent
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
