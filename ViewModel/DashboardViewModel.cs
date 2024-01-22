namespace PriceSetterDesktop.ViewModel
{
    using WPFCollection.Style.Base;

    public class DashboardViewModel : BasePage
    {
        public DashboardViewModel()
        {
            CurrentContent = new ArticleViewModel();
        }
        private object _currentContent;

        public object CurrentContent
        {
            get { return _currentContent; }
            set { _currentContent = value; PropertyCall(); }
        }

    }
}
