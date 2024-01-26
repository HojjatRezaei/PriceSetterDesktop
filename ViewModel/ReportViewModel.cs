namespace PriceSetterDesktop.ViewModel
{
    using System.Windows.Input;
    using WPFCollection.Style.Base;
    public class ReportViewModel : BaseControl
    {
        public ReportViewModel()
        {
            
        }

        public ICommand ExcelOutputCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.ExcelOutputCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePricesCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.UpdatePricesCommandHandler(); }, (object parameter) => { return true; });

        private void ExcelOutputCommandHandler()
        {

        }
        private void UpdatePricesCommandHandler()
        {

        }
    }
}
