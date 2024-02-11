namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Style.Base;

    public class ArticleViewModel : BasePage
    {
        public ArticleViewModel()
        {
            UpdateArticleList();
            UpdateProviderList(null);
        }

        public string CurrentXPath
        { get => _currentXPath; set { _currentXPath = value; PropertyCall(); } }
        public ViewCollection<Provider> UnSetterProvider
        { get => _unSetterProvider; set { _unSetterProvider = value; PropertyCall(); } }
        public Url CurrentURL
        { get => _currentURL; set { _currentURL = value; PropertyCall(); } }
        public Provider SelectedProvider
        { get => _selectedProvider; set { _selectedProvider = value; PropertyCall(); } }
        public ReadableArticle SelectedArticle
        { get => _selectedArticle; set { _selectedArticle = value; PropertyCall(); } }
        public ViewCollection<Provider> ListOfProviders
        { get => _listOfProviders; set { _listOfProviders = value; PropertyCall(); } }
        public ViewCollection<ReadableArticle> ListofArticles
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

            if (SelectedProvider == null)
            {
                //promt user that no provider is selected and exit function
                MessageBox.Show("تامین کننده ای انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
                return;
            }
            //update values in other tables 
            //update url and xpath
            //check if any details have been entered in database related to article and provider
            var urlSearchResult = APIDataStorage.UrlManager.List.FirstOrDefault(x => x.ProviderID == SelectedProvider.ID && x.ArticleID == SelectedArticle.ID);
            if (urlSearchResult != null)
            {
                //update information for xpath and url in table
                var newURL = CurrentURL;
                newURL.ProviderID = SelectedProvider.ID;
                newURL.ArticleID = SelectedArticle.ID;
                APIDataStorage.UrlManager.Update(newURL, urlSearchResult.ID);
            }
            else
            {
                if (CurrentURL.URL != string.Empty)
                {
                    //add information for xpath and url to the table
                    var newURL = CurrentURL;
                    newURL.ProviderID = SelectedProvider.ID;
                    newURL.ArticleID = SelectedArticle.ID;
                    APIDataStorage.UrlManager.Add(newURL);
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
            DataHolder.UpdateReadableArticleList();
            ListofArticles = DataHolder.ReadableArticleList.ToList();
        }
        private void UpdateProviderList(ReadableArticle? art = null)
        {
            ReadableArticle ArticleSelection;
            if (art != null)
            {
                ArticleSelection = art;
            }
            else if (SelectedArticle != null)
            {
                ArticleSelection = SelectedArticle;
            }
            else
            {
                ListOfProviders = APIDataStorage.ProviderManager.List;
                return;
            }
            var urlList = APIDataStorage.UrlManager.List.Where(x => x.ArticleID == ArticleSelection.ID);
            var providerList = APIDataStorage.ProviderManager.List;
            foreach (var provider in providerList)
            {
                provider.HaveURL = urlList.FirstOrDefault(x => x.ProviderID == provider.ID) != null;
            }
            ListOfProviders = providerList.ToList();
            UnSetterProvider = providerList.Where(x => !x.HaveURL).ToList();
        }
        private void ArticleSelectionChangedHandler(SelectionChangedEventArgs e)
        {
            if (e.OriginalSource.GetType().GetProperty("DataContext") is PropertyInfo prop && prop.GetValue(e.OriginalSource) is ArticleViewModel castedModel && e.AddedItems.Count != 0 && e.AddedItems[0] is ReadableArticle selectedItem)
            {
                castedModel.UpdateProviderList(selectedItem);
                if (castedModel.SelectedProvider != null)
                    castedModel.UpdateURLInfo();
            }
        }
        private void ProviderSelectionChangedHandler(SelectionChangedEventArgs e)
        {
            if (e.OriginalSource.GetType().GetProperty("DataContext") is PropertyInfo prop && prop.GetValue(e.OriginalSource) is ArticleViewModel castedModel && e.AddedItems.Count != 0 && e.AddedItems[0] is Provider selectedItem && castedModel.SelectedArticle != null)
            {
                castedModel.UpdateURLInfo(selectedItem);
                e.Handled = true;
            }
        }
        private void UpdateURLInfo(Provider? providerSelection = null)
        {
            Provider selection = providerSelection == null ? SelectedProvider : providerSelection;
            var searchResult = APIDataStorage.UrlManager.List.FirstOrDefault(x => x.ArticleID == SelectedArticle.ID && x.ProviderID == selection.ID);
            CurrentURL = searchResult != null
                ? new Url()
                {
                    ArticleID = SelectedArticle.ID,
                    ProviderID = selection.ID,
                    URL = searchResult.URL,
                    ID = searchResult.ID
                }
                : new();
        }

        private string _currentXPath = string.Empty;
        private Url _currentURL = new();
        private Provider _selectedProvider;
        private ReadableArticle _selectedArticle;
        private ViewCollection<Provider> _listOfProviders;
        private ViewCollection<ReadableArticle> _listofArticles;
        private ViewCollection<Provider> _unSetterProvider;
    }
}
