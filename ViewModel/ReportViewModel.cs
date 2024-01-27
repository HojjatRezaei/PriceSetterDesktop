namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Types;
    using System.Windows.Input;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Data.Types;
    using WPFCollection.Style.Base;
    using PriceSetterDesktop.Libraries.Statics;
    using WPFCollection.Data.List;

    public class ReportViewModel : BaseControl
    {
        public ReportViewModel()
        {
            _dataBase = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _articleTable = _dataBase.GetTable<Article>(nameof(Article));
            _providerTable = _dataBase.GetTable<Provider>(nameof(Provider));
            _urlTypeTable = _dataBase.GetTable<URLType>(nameof(URLType));
            UpdatePricesCommandHandler();
        }


        private ViewCollection<CollectionView> _priceList;
        public ViewCollection<CollectionView> PriceList
        { get => _priceList; set { _priceList = value; PropertyCall(); } }
        public ICommand ExcelOutputCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.ExcelOutputCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePricesCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.UpdatePricesCommandHandler(); }, (object parameter) => { return true; });

        private void ExcelOutputCommandHandler()
        {

        }
        private void UpdatePricesCommandHandler()
        {
            foreach (var item in _urlTypeTable.List)
            {
                var price = item.GetPriceFromWeb();
            }
        }
        private readonly XMLDataBase _dataBase;
        private readonly XMLTable<Article> _articleTable;
        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLTable<URLType> _urlTypeTable;
        public class CollectionView
        {
            public CollectionView()
            {
                
            }
            public string ArticleName { get; set; }
            public List<ProviderView> Providers { get; set; }
        }
        public class ProviderView
        {
            public string ProviderName { get; set; }
            public double ProviderPrice { get; set; }

        }
    }
}
