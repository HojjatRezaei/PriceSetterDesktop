namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Diagnostics.Eventing.Reader;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using WPFCollection.Data.List;
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Style.Base;

    public class ArticleViewModel : BasePage
    {
        public ArticleViewModel()
        {
            _dataBase = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _providerTable = _dataBase.GetTable<Provider>(nameof(Provider));
            _urlTypeTable = _dataBase.GetTable<URLType>(nameof(URLType));
            UpdateArticleList();
            UpdateProviderList(null);
        }


        public string CurrentXPath
        { get => _currentXPath; set { _currentXPath = value; PropertyCall(); } }
        public ViewCollection<Provider> UnSetterProvider
        { get => _unSetterProvider; set { _unSetterProvider = value; PropertyCall(); } }
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
        public ICommand GotoPrivdersPageCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.GotoProvidersPageCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateArticleInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)parameter; model.UpdateArticleInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand ProviderSelectionChanged { get; set; } = new FastCommand
            ((object parameter) => { ArticleViewModel model = (ArticleViewModel)((object[])parameter)[1]; model.ProviderSelectionChangedHandler((SelectionChangedEventArgs)((object[])parameter)[0]); }, (object parameter) => { return true; });
        public ICommand ArticleSelectionChanged { get; set; } = new FastCommand
            ((object parameter) => 
            { 
                ArticleViewModel model = (ArticleViewModel)((object[])parameter)[1];
                model.ArticleSelectionChangedHandler((SelectionChangedEventArgs)((object[])parameter)[0]);
            }, (object parameter) => { return true; });

        private async void GotoProvidersPageCommandHandler()
        {
            var dialog = new ProvidersViewModel();
            _ = await dialog.LoadDialog(dialog);
            UpdateProviderList();
        }
        private void UpdateArticleInfoCommandHandler()
        {
            if (SelectedArticle == null)
            {
                //promt user that no article is selected and exit function
                MessageBox.Show("کالایی جهت بروز رسانی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                return;
            }
            //check if any provider have been selected
            
            if(SelectedProvider == null)
            {
                //promt user that no provider is selected and exit function
                MessageBox.Show("تامین کننده ای انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                return;
            }
            //update values in other tables 
            //update url and xpath
            //check if any details have been entered in database related to article and provider
            var urlSearchResult = _urlTypeTable.List.FirstOrDefault(x => x.ProviderID == SelectedProvider.ElementSeed && x.ArticleID == SelectedArticle.ArticleID);
            if(urlSearchResult != null)
            {
                //update information for xpath and url in table
                var newURL = CurrentURL;
                newURL.ProviderID = SelectedProvider.ElementSeed;
                newURL.ArticleID = SelectedArticle.ArticleID;
                _urlTypeTable.Update(urlSearchResult.ElementSeed,newURL);
            }
            else
            {
                if(CurrentURL.URL != string.Empty)
                {
                    //add information for xpath and url to the table
                    var newURL = CurrentURL;
                    newURL.ProviderID = SelectedProvider.ElementSeed;
                    newURL.ArticleID = SelectedArticle.ArticleID;
                    _urlTypeTable.Add(newURL);
                }
            }
            //check if any value have changed
            CurrentURL = new();
            UpdateProviderList(SelectedArticle);
            UpdateArticleList();
            MessageBox.Show("عملیات با موفقت انجام شد", "اطلاعات", MessageBoxButton.OK, MessageBoxImage.Information);
            

        }
        private void UpdateArticleList()
        {
            ListofArticles = DataHolder.Articles;
        }
        private void UpdateProviderList(Article? art = null)
        {
            Article ArticleSelection;
            if(art != null)
            {
                ArticleSelection = art;
            }
            else if(SelectedArticle != null)
            {
                ArticleSelection = SelectedArticle;
            }
            else
            {
                ListOfProviders = _providerTable.List;
                return;
            }
            var urlList = _urlTypeTable.List.Where(x => x.ArticleID == ArticleSelection.ArticleID);
            var providerList = _providerTable.List;
            foreach (var provider in providerList)
            {
                provider.HaveURL = urlList.FirstOrDefault(x => x.ProviderID == provider.ElementSeed) != null;
            }
            ListOfProviders = providerList.ToList();
            UnSetterProvider = providerList.Where(x => !x.HaveURL).ToList();
        }
        private void ArticleSelectionChangedHandler(SelectionChangedEventArgs e)
        {
            if (e.OriginalSource.GetType().GetProperty("DataContext") is PropertyInfo prop && prop.GetValue(e.OriginalSource) is ArticleViewModel castedModel && e.AddedItems.Count != 0 && e.AddedItems[0] is Article selectedItem)
            {
                castedModel.UpdateProviderList(selectedItem);
                if (castedModel.SelectedProvider != null)
                    castedModel.UpdateURLInfo();
            }
        }
        private void ProviderSelectionChangedHandler(SelectionChangedEventArgs e)
        {
            if (e.OriginalSource.GetType().GetProperty("DataContext") is PropertyInfo prop && prop.GetValue(e.OriginalSource) is ArticleViewModel castedModel && e.AddedItems.Count != 0 &&e.AddedItems[0] is Provider selectedItem && castedModel.SelectedArticle != null)
            {
                castedModel.UpdateURLInfo(selectedItem);
                e.Handled = true;
            }
        }
        private void UpdateURLInfo(Provider? providerSelection = null)
        {
            Provider selection;
            if (providerSelection == null)
            {
                selection = SelectedProvider;
            }
            else
            {
                selection = providerSelection;
            }
            
            var searchResult = _urlTypeTable.List.FirstOrDefault(x => x.ArticleID == SelectedArticle.ArticleID && x.ProviderID == selection.ElementSeed);
            if (searchResult != null)
            {
                CurrentURL = new URLType()
                {
                    ArticleID = SelectedArticle.ArticleID,
                    ProviderID = selection.ElementSeed,
                    URL = searchResult.URL,
                    ElementSeed = searchResult.ElementSeed
                };
            }
            else
            {
                CurrentURL = new();
            }
        }

        private string _currentXPath=string.Empty;
        private URLType _currentURL=new();
        private Provider _selectedProvider;
        private Article _selectedArticle;
        private ViewCollection<Provider> _listOfProviders;
        private ViewCollection<Article> _listofArticles;
        private ViewCollection<Provider> _unSetterProvider;
        private readonly XMLDataBase _dataBase;
        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLTable<URLType> _urlTypeTable;
    }
}
