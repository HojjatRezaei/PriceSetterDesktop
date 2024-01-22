namespace PriceSetterDesktop.ViewModel
{
    using WPFCollection.Style.Base;

    public class ArticleViewModel : BasePage
    {
        public ArticleViewModel()
        {
            
        }
        private string _currentArticleName="";

        public string CurrentArticleName
        {
            get { return _currentArticleName; }
            set { _currentArticleName = value; PropertyCall(); }
        }

    }
}
