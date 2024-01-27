namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types;
    using System.Windows;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Data.Types;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Style.Base;

    public class ArticleViewModel : BasePage
    {
        public ArticleViewModel()
        {
            _dataBase = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _articleTable = _dataBase.GetTable<Article>(nameof(Article));
            _providerTable = _dataBase.GetTable<Provider>(nameof(Provider));
            _urlTypeTable = _dataBase.GetTable<URLType>(nameof(URLType));
            UpdateArticleList();
            UpdateProviderList();
        }

        public Article CurrentArticle
        { get => _currentArticle; set { _currentArticle = value; PropertyCall(); } }
        public URLType CurrentURL
        { get => _currentURL; set { _currentURL = value; PropertyCall(); } }

        public Provider SelectedProvider
        { get => _selectedProvider; set { _selectedProvider = value; PropertyCall(); } }
        public Article SelectedArticle
        { get => _selectedArticle; set { _selectedArticle = value; PropertyCall(); } }
        public ViewCollection<Provider> ListOfProviders
        { get => _listOfProviders; set { _listOfProviders = value; PropertyCall(); } }
        public ViewCollection<Article> ListofArticles
        { get => _listofArticles; set { _listofArticles = value; PropertyCall(); } }
        public ICommand SubmitArticleInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.SubmitArticleInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand GotoPrivdersPageCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.GotoProvidersPageCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateArticleInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.UpdateArticleInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemoveArticleCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.RemoveArticleCommandHandler(); }, (object parameter) => { return true; });
        /// <summary>
        /// add new article to the list
        /// </summary>
        private void SubmitArticleInfoCommandHandler()
        {
            var newItem = CurrentArticle;
            //check for name in table
            if(!_articleTable.Exist(newItem))
                _articleTable.Add(newItem);
            UpdateArticleList();
            CurrentArticle = new();
        }
        /// <summary>
        /// 
        /// </summary>
        private async void GotoProvidersPageCommandHandler()
        {
            var dialog = new ProvidersViewModel();
            _ = await dialog.LoadDialog(dialog);
            UpdateProviderList();
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateArticleInfoCommandHandler()
        {
            if (SelectedArticle == null)
            {
                //promt user that no article is selected and exit function
                MessageBox.Show("کالایی جهت بروز رسانی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                return;
            }
            //check if any provider have been selected
            var selectedProvider = ListOfProviders.CurrentItemWithType;
            if(selectedProvider == null)
            {
                //promt user that no provider is selected and exit function
                MessageBox.Show("تامین کننده ای انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                return;
            }
            //promt user to confirm update action
            //if user said no , exit function
            string message = "آیا از بروزرسانی اطلاعات اطمینان دارید ؟";
            if (MessageBox.Show(message,"اطلاعات", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                return;

            var newItem = CurrentArticle;
            var oldItem = _articleTable.List.FirstOrDefault(x=> x.ElementSeed == SelectedArticle.ElementSeed);
            //if item cannot be find in the list , create new article
            if(oldItem != null)
            {
                //compare entered name and update the value in table
                if (!oldItem.Equals(newItem))
                {
                    _articleTable.Update(SelectedArticle.ElementSeed, newItem);
                }
                //update values in other tables 
                //update url and xpath
                //check if any details have been entered in database related to article and provider
                var urlSearchResult = _urlTypeTable.List.FirstOrDefault(x => x.ProviderID == ListOfProviders.CurrentItemWithType.ElementSeed && x.ArticleID == ListofArticles.CurrentItemWithType.ElementSeed);
                if(urlSearchResult != null)
                {
                    //update information for xpath and url in table
                    var newURL = CurrentURL;
                    newURL.ProviderID = ListOfProviders.CurrentItemWithType.ElementSeed;
                    newURL.ArticleID = ListofArticles.CurrentItemWithType.ElementSeed;
                    _urlTypeTable.Update(urlSearchResult.ElementSeed,newURL);
                }
                else
                {
                    //add information for xpath and url to the table
                    var newURL = CurrentURL;
                    newURL.ProviderID = ListOfProviders.CurrentItemWithType.ElementSeed;
                    newURL.ArticleID = ListofArticles.CurrentItemWithType.ElementSeed;
                    _urlTypeTable.Add(newURL);
                }
            }
            //check if any value have changed
            _articleTable.Update(SelectedArticle.ElementSeed, newItem);
            UpdateArticleList();
        }
        /// <summary>
        /// 
        /// </summary>
        private void RemoveArticleCommandHandler()
        {
            //check if any article is selected
            if(SelectedArticle == null)
            {
                //promt user that no article have been selected for remove action
                string msg = "هیچ کالایی جهت حذف انتخاب نشده";
                MessageBox.Show(msg, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //promt user to confirm remove action
            string message = "آیا از حذف اطلاعات اطمینان دارید ؟";
            if (MessageBox.Show(message, "Info", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                return;
            //search for data in 2 tables
            //remove every related info to selectedArticle From Articles
            //remove every related Info to selectedArticle From URLType
            //remove article based on selectedarticle elementSeed
            _articleTable.Remove(SelectedArticle.ElementSeed);
            //get a list of removable article
            var removeList = _urlTypeTable.List.Where(x => x.ArticleID == SelectedArticle.ElementSeed).ToList();
            //loop through each item and remove references
            foreach (var item in removeList)
            {
                _urlTypeTable.Remove(item.ElementSeed);
            }
            UpdateArticleList();
        }
        /// <summary>
        /// Update <see cref="ListofArticles"/>
        /// </summary>
        private void UpdateArticleList()
        {
            ListofArticles = _articleTable.List;
        }
        /// <summary>
        /// Update <see cref="ListOfProviders"/>
        /// </summary>
        private void UpdateProviderList()
        {
            ListOfProviders = _providerTable.List;
        }
        private Article _currentArticle=new();
        private URLType _currentURL=new();
        private Provider _selectedProvider;
        private Article _selectedArticle;
        private ViewCollection<Provider> _listOfProviders;
        private ViewCollection<Article> _listofArticles;
        private readonly XMLDataBase _dataBase;
        private readonly XMLTable<Article> _articleTable;
        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLTable<URLType> _urlTypeTable;
    }
}
