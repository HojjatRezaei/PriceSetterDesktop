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
            
        }

        private Article _currentArticle;
        public Article CurrentArticle
        { get => _currentArticle; set { _currentArticle = value; PropertyCall(); } }
        public ICommand GotoPrivdersPageCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.GotoPrivdersPageCommandHandler(); }, (object parameter) => { return true; });
        public ICommand SubmitArticleInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.SubmitArticleInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateArticleInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.UpdateArticleInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemoveArticleCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.RemoveArticleCommandHandler(); }, (object parameter) => { return true; });

        private void GotoPrivdersPageCommandHandler()
        {

        }
        private void SubmitArticleInfoCommandHandler()
        {

        }
        private void UpdateArticleInfoCommandHandler()
        {

        }
        private void RemoveArticleCommandHandler()
        {

        }
    }
}
