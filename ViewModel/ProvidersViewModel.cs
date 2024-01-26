namespace PriceSetterDesktop.ViewModel
{
    using System.Windows.Input;
    using WPFCollection.Style.Base;
    public class ProvidersViewModel : BaseControl
    {
        public ProvidersViewModel()
        {
            
        }

        public ICommand SubmitProviderInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ProvidersViewModel model = (ProvidersViewModel)parameter; model.SubmitProviderInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdateProviderInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ProvidersViewModel model = (ProvidersViewModel)parameter; model.UpdateProviderInfoCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RemoveProviderInfoCommand { get; set; } = new FastCommand
            ((object parameter) => { ProvidersViewModel model = (ProvidersViewModel)parameter; model.RemoveProviderInfoCommandHandler(); }, (object parameter) => { return true; });

        private void SubmitProviderInfoCommandHandler()
        {

        }
        private void UpdateProviderInfoCommandHandler()
        {

        }
        private void RemoveProviderInfoCommandHandler()
        {

        }
    }
}
