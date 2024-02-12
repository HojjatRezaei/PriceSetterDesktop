namespace PriceSetterDesktop.ViewModel
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using PriceSetterDesktop.Libraries.Engines;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Style.Base;

    public class ReportViewModel : BaseControl
    {
        public ReportViewModel()
        {
            //check for last inserted data
            ScrapItems = APIDataStorage.ScrapManager.List.ToList();
        }

        private ViewCollection<ScrapResult> _scrapItems = [];
        public ViewCollection<ScrapResult> ScrapItems
        { get => _scrapItems; set { _scrapItems = value; PropertyCall(); } }
        public ICommand ExcelOutputCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.ExcelOutputCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePricesCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.UpdatePricesCommandHandler(); }, (object parameter) => { return true; });
        public ICommand TryAgainCommand { get; set; } = new FastCommand
        ((object parameter) => 
        {
            ReportViewModel model = (ReportViewModel)((object[])parameter)[0];
            model.TryAgainCommandHandler((ScrapResult)((object[])parameter)[1]);
        }, (object parameter) => { return true; });
        private void TryAgainCommandHandler(ScrapResult retryScrapItem)
        {
            var articleObject = retryScrapItem.GetArticle();
            if (articleObject == null)
                return;
            var providerObject = retryScrapItem.GetProvider();
            if (providerObject == null)
                return;
            WebScrapEngine webScrap = new WebScrapEngine();
            var scrapList = new List<ScrapResult>();
            //remove every related provider and article
            ScrapItems.Remove(retryScrapItem);
            scrapList = [.. ScrapItems.Where(x => x != null)];
            webScrap.TurnOn();
            var scrapResult = webScrap.Scrap(providerObject, articleObject);
            if (scrapResult != null)
            {
                bool validScrap = false;
                foreach (var scrappedData in scrapResult)
                {
                    if (scrappedData.Equals(retryScrapItem))
                    {
                        scrapList.Add(scrappedData);
                        validScrap = true;
                    }
                }
                if (!validScrap)
                {
                    retryScrapItem.HaveMessage = true;
                    retryScrapItem.Messages = "مشکل در پیدا کردن منابع مربوطه ، لطفا اطلاعات را بررسی نمایید .";
                    scrapList.Add(retryScrapItem);
                }
            }
            webScrap.TurnOff();
            ScrapItems = scrapList.ToList();
        }
        private void ExcelOutputCommandHandler()
        {
            //UpdatePricesCommandHandler();
            //var excelIns = new ExcelFile();
        }
        private void UpdatePricesCommandHandler()
        {
            WebScrapEngine webScrap = new WebScrapEngine();
            var scrapList = new List<ScrapResult>();
            webScrap.TurnOn();
            foreach (var provider in APIDataStorage.ProviderManager.List)
            {
                var scrapResult = webScrap.Scrap(provider);
                if(scrapResult != null)
                {
                    foreach (var scrappedData in scrapResult)
                    {
                        scrapList.Add(scrappedData);
                    }
                }
            }
            webScrap.TurnOff();
            ScrapItems = scrapList;
        }

        public ICommand SendPriceToWebCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.SendPriceToWebCommandHandler(); }, (object parameter) => { return true; });
        private void SendPriceToWebCommandHandler()
        {
            var scrItems = ScrapItems.ToList();
            foreach (var scrapitem in scrItems)
            {
                if (scrapitem.ColorID == -1 || scrapitem.PriceID == -1 || !scrapitem.IsValidData())
                    continue;
                APIDataStorage.ScrapManager.Add(scrapitem);
            }
        }
    }
}