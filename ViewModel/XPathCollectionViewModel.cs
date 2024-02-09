namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using System.Windows;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Style.Base;

    public class XPathCollectionViewModel : BaseDialog
    {
        public XPathCollectionViewModel()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _xPathTable = db.GetTable<PathItem>(nameof(PathItem));
            _containerXPathTable = db.GetTable<Container>(nameof(Container));
        }
        public XPathCollectionViewModel(Provider providerSelection)
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _xPathTable = db.GetTable<PathItem>(nameof(PathItem));
            _containerXPathTable = db.GetTable<Container>(nameof(Container));
            CurrentContainer.ProviderID = providerSelection.ID;
            UpdateList();
        }

        public Container SelectedContainer
        { get => _selectedContainer; set { _selectedContainer = value; PropertyCall(); } }
        public Container CurrentContainer
        { get => _currentContainer; set { _currentContainer = value; PropertyCall(); } }
        public PathItem CurrentXPathItem
        { get => _currentXPathItem; set { _currentXPathItem = value; PropertyCall(); } }
        public PathItem PathItemSelection
        { get => _pathItemSelection; set { _pathItemSelection = value; PropertyCall(); } }
        public ViewCollection<Container> ContainerList
        { get => _containerList; set { _containerList = value; PropertyCall(); } }

        public ICommand SubmitContainerCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.SubmitContainerCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateContainerCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.UpdateContainerCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemoveContainerCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.RemoveContainerCommandHandler(); }, (object parameter) => { return true; });
        public ICommand SubmitPathItemCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.SubmitPathItemCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePathItemCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.UpdatePathItemCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemovePathItemCommand { get; set; } = new FastCommand
            ((object parameter) => { XPathCollectionViewModel model = (XPathCollectionViewModel)parameter; model.RemovePathItemCommandHandler(); }, (object parameter) => { return true; });

        private void SubmitPathItemCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentXPathItem.Path) || string.IsNullOrEmpty(CurrentXPathItem.PathTag))
            {
                MessageBox.Show("آدرس منبع/تگ مشخص نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SelectedContainer == null)
            {
                MessageBox.Show("ظرفی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentXPathItem.ContainerID = SelectedContainer.ID;
            _xPathTable.Add(CurrentXPathItem);
            CurrentXPathItem = new();
            PropertyCall(nameof(SelectedContainer));
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdatePathItemCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentXPathItem.Path) || string.IsNullOrEmpty(CurrentXPathItem.PathTag))
            {
                MessageBox.Show("آدرس منبع/تگ مشخص نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (PathItemSelection == null || SelectedContainer == null)
            {
                MessageBox.Show("منبع/ظرفی جهت بروزرسانی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            CurrentXPathItem.ContainerID = SelectedContainer.ID;
            _xPathTable.Update(PathItemSelection.ID, CurrentXPathItem);
            CurrentXPathItem = new();
            PropertyCall(nameof(SelectedContainer));
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RemovePathItemCommandHandler()
        {
            if (PathItemSelection == null)
            {
                MessageBox.Show("آدرس منبعی جهت حذف انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _xPathTable.Remove(PathItemSelection.ID);
            PropertyCall(nameof(SelectedContainer));
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void SubmitContainerCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentContainer.ContainerPath))
            {
                MessageBox.Show("آدرس/نوع ظرف خالی میباشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CurrentContainer.ProviderID == -1)
            {
                MessageBox.Show("کد تامین کننده ثبت نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _containerXPathTable.Add(CurrentContainer);
            UpdateList();
            var seed = CurrentContainer.ProviderID;
            CurrentContainer = new() { ProviderID = seed };
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateContainerCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentContainer.ContainerPath))
            {
                MessageBox.Show("آدرس/نوع ظرف خالی میباشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CurrentContainer.ProviderID == -1)
            {
                MessageBox.Show("کد تامین کننده ثبت نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SelectedContainer == null)
            {
                MessageBox.Show("ظرفی جهت بروزرسانی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _containerXPathTable.Update(SelectedContainer.ID, CurrentContainer);
            UpdateList();
            var seed = CurrentContainer.ProviderID;
            CurrentContainer = new() { ProviderID = seed };
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void RemoveContainerCommandHandler()
        {
            if (SelectedContainer == null)
            {
                MessageBox.Show("ظرفی جهت حذف انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = _xPathTable.List.Where(x => x.ContainerID == SelectedContainer.ID);
            _xPathTable.Remove(result);
            _containerXPathTable.Remove(SelectedContainer.ID);
            //remove every related xpath to this container
            UpdateList();
            var seed = CurrentContainer.ProviderID;
            CurrentContainer = new() { ProviderID = seed };
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateList()
        {
            ContainerList = _containerXPathTable.List.Where(x => x.ProviderID == CurrentContainer.ProviderID).ToList();
        }

        private PathItem _currentXPathItem = new();
        private PathItem _pathItemSelection = new();
        private Container _selectedContainer = new();
        private Container _currentContainer = new();
        private ViewCollection<Container> _containerList = [];
        private readonly XMLTable<PathItem> _xPathTable;
        private readonly XMLTable<Container> _containerXPathTable;
        public override void FocusOnStartupObject()
        {
            //do nothing
        }
    }
}
