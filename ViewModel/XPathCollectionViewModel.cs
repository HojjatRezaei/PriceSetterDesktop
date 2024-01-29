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

    public class XPathCollectionViewModel : BaseDialog
    {
        public XPathCollectionViewModel()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _xPathTable = db.GetTable<XPathItem>(nameof(XPathItem));
        }
        public XPathCollectionViewModel(Article articleSelection , ProviderView providerSelection)
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _xPathTable = db.GetTable<XPathItem>(nameof(XPathItem));
            CurrentXPath.ArticleID = articleSelection.ElementSeed;
            CurrentXPath.ProviderID = providerSelection.ElementSeed;
            UpdateList();
        }

        public XPathItem XPathSelection
        { get => _xPathSelection; set { _xPathSelection = value; PropertyCall(); } }
        public XPathItem CurrentXPath
        { get => _currentXPath; set { _currentXPath = value; PropertyCall(); } }
        public ViewCollection<XPathItem> XPathCollectionList
        { get => _xPathCollectionList; set { _xPathCollectionList = value; PropertyCall(); } }

        public ICommand SubmitCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.SubmitCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.UpdateCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemoveCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.RemoveCommandHandler(); }, (object parameter) => { return true; });

        private void SubmitCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentXPath.XPath) || string.IsNullOrEmpty(CurrentXPath.XpathType))
            {
                MessageBox.Show("آدرس/نوع منبع خالی میباشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CurrentXPath.ArticleID == -1 || CurrentXPath.ProviderID == -1)
            {
                MessageBox.Show("کد کالا/تامین کننده منبع خالی میباشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<XPathItem>(nameof(XPathItem));
            tb.Add(CurrentXPath);
            UpdateList();
            CurrentXPath.XpathType = string.Empty;
            CurrentXPath.XPath = string.Empty;
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentXPath.XPath) || string.IsNullOrEmpty(CurrentXPath.XpathType))
            {
                MessageBox.Show("آدرس/نوع منبع خالی میباشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(XPathSelection == null)
            {
                MessageBox.Show("منبعی جهت بروزرسانی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<XPathItem>(nameof(XPathItem));
            tb.Update(XPathSelection.ElementSeed,CurrentXPath);
            CurrentXPath.XpathType = string.Empty;
            CurrentXPath.XPath = string.Empty;
            UpdateList();
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RemoveCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentXPath.XPath))
            {
                MessageBox.Show("آدرسی برای حذف انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<XPathItem>(nameof(XPathItem));
            tb.Remove(XPathSelection.ElementSeed);
            UpdateList();
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateList()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<XPathItem>(nameof(XPathItem));
            XPathCollectionList = tb.List.Where(x=> x.ArticleID == CurrentXPath.ArticleID).ToList();
        }

        private XPathItem _xPathSelection=new();
        private XPathItem _currentXPath=new();
        private ViewCollection<XPathItem> _xPathCollectionList = [];
        private readonly XMLTable<XPathItem> _xPathTable;
        public override void FocusOnStartupObject()
        {
            //do nothing
        }
    }
}
