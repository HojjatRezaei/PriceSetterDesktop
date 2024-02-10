namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using System.Windows;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Style.Base;

    public class XPathCollectionViewModel : BaseDialog
    {
        public XPathCollectionViewModel()
        {
        }
        public XPathCollectionViewModel(Provider providerSelection)
        {
            CurrentContainer.ProviderID = providerSelection.ID;
            UpdateList();
        }

        public Container SelectedContainer
        { 
            get => _selectedContainer; 
            set
            { 
                _selectedContainer = value;
                PropertyCall();
            }
        }
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
            APIDataStorage.PathManager.Add(CurrentXPathItem);
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
            APIDataStorage.PathManager.Update(CurrentXPathItem, PathItemSelection.ID);
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
            APIDataStorage.PathManager.Remove(PathItemSelection.ID);
            PropertyCall(nameof(SelectedContainer));
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void SubmitContainerCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentContainer.Path))
            {
                MessageBox.Show("آدرس/نوع ظرف خالی میباشد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (CurrentContainer.ProviderID == -1)
            {
                MessageBox.Show("کد تامین کننده ثبت نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            APIDataStorage.ContainerManager.Add(CurrentContainer);
            UpdateList();
            var seed = CurrentContainer.ProviderID;
            CurrentContainer = new() { ProviderID = seed };
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateContainerCommandHandler()
        {
            if (string.IsNullOrEmpty(CurrentContainer.Path))
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
            APIDataStorage.ContainerManager.Update(CurrentContainer, SelectedContainer.ID);
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
            APIDataStorage.PathManager.List.Where(x => x.ContainerID == SelectedContainer.ID).ToList().ForEach((pathItem) => 
            {
                APIDataStorage.PathManager.Remove(pathItem.ID);
            });
            APIDataStorage.ContainerManager.Remove(SelectedContainer.ID);
            //remove every related xpath to this container
            UpdateList();
            var seed = CurrentContainer.ProviderID;
            CurrentContainer = new() { ProviderID = seed };
            MessageBox.Show("عملیات با موفقیت انجام شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void UpdateList()
        {
            ContainerList = APIDataStorage.ContainerManager.List.Where(x => x.ProviderID == CurrentContainer.ProviderID).ToList();
        }

        private PathItem _currentXPathItem = new();
        private PathItem _pathItemSelection = new();
        private Container _selectedContainer = new();
        private Container _currentContainer = new();
        private ViewCollection<Container> _containerList = [];
        public override void FocusOnStartupObject()
        {
            //do nothing
        }
    }
}
