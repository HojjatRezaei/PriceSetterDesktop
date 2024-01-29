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
    public class ProvidersViewModel : BaseDialog
    {
        public ProvidersViewModel()
        {
            _dataBase = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _providerTable = _dataBase.GetTable<Provider>(nameof(Provider));
            UpdateProviderList();
        }

        private void ProvidersViewModel_ChildExit(bool CloseState = true)
        {
            CompleteTheTask(null);
        }

        public Provider CurrentProvider
        { get => _currentProvider; set { _currentProvider = value; PropertyCall(); } }

        private Provider _selectedProvider;
        public Provider SelectedProvider
        { get => _selectedProvider; set { _selectedProvider = value; PropertyCall(); } }
        public ViewCollection<Provider> ProvidersList
        { get => _providersList; set { _providersList = value; PropertyCall(); } }

        public ICommand SubmitProviderInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ProvidersViewModel model = (ProvidersViewModel)parameter; model.SubmitProviderInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateProviderInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ProvidersViewModel model = (ProvidersViewModel)parameter; model.UpdateProviderInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemoveProviderInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ProvidersViewModel model = (ProvidersViewModel)parameter; model.RemoveProviderInfoCommandHandler(); }, (object parameter) => { return true; });

        private void SubmitProviderInfoCommandHandler()
        {
            //get provider name 
            var newItem = CurrentProvider;
            //check if exist in table 
            var searchResult = _providerTable.List.FirstOrDefault(x => x.Equals(newItem));
            if (searchResult!=null)
            {
                //promt user that provider name exist in database
                MessageBox.Show("کالا در لیست تامین کننده ها وجود دارد", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                //insert new value 
                _providerTable.Add(newItem);
                //promt user that adding new item is succesfull
                MessageBox.Show("کالا با موفقیت اضافه شد", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            //clear box
            CurrentProvider = new();
            //update list 
            UpdateProviderList();
        }
        private void UpdateProviderInfoCommandHandler()
        {
            if(SelectedProvider == null)
            {
                //promt user that no provider have been selected for updating operation 
                MessageBox.Show("تامین کننده ای جهت بروزرسانی انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //promt user for update
            var result = MessageBox.Show("ایا از بروزرسانی اطلاعات اطمینان دارید ؟", "خطا", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;
            var newItem = CurrentProvider;

            _providerTable.Update(SelectedProvider.ElementSeed, newItem);
            UpdateProviderList();
            CurrentProvider = new();
            MessageBox.Show("عملیات بروزرسانی با موفقیت انجام شد .", "موفق", MessageBoxButton.YesNo, MessageBoxImage.Information);
        }
        private void RemoveProviderInfoCommandHandler()
        {
            if (SelectedProvider == null)
            {
                //promt user that no provider have been selected for removing operation 
                MessageBox.Show("تامین کننده ای جهت حذف انتخاب نشده", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //if user said no , return
            //promt user for update
            var result = MessageBox.Show("ایا از حذف اطلاعات اطمینان دارید ؟", "سوال", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.No)
                return;
            _providerTable.Remove(SelectedProvider.ElementSeed);
            UpdateProviderList();
            MessageBox.Show("عملیات حذف با موفقیت انجام شد .", "موفق", MessageBoxButton.YesNo, MessageBoxImage.Information);
        }
        private void UpdateProviderList()
        {
            ProvidersList = _providerTable.List;
        }
        public override void FocusOnStartupObject()
        {
            //do nothing
        }
        private Provider _currentProvider = new();
        private ViewCollection<Provider> _providersList = [];

        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLDataBase _dataBase;

        
    }
}
