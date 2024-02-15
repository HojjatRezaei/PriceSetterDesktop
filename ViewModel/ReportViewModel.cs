namespace PriceSetterDesktop.ViewModel
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.APIManager;
    using PriceSetterDesktop.Libraries.Engines;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Diagnostics;
    using System.Windows.Input;
    using WPFCollection.Data.List;
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types;
    using WPFCollection.Style.Base;

    public class ReportViewModel : BaseControl
    {
        public ReportViewModel()
        {
            var defList = APIDataStorage.ScrapManager.List;
            //check for last inserted data

            //min price
            //ScrapItems =
            var restest1 = APIDataStorage.ArticleManager.List.Select((articleItem) => 
            {
                var maxList = defList.Where(x => x.ArticleID == articleItem.ID && x.ColorID == articleItem.ColorID);
                if (!maxList.Any())
                {
                    return default;
                }
                var lastDateUpdate = maxList.Max(x => x.Date);
                var lastTimeUpdate = maxList.Max(x => x.Time);
                var submitedPrcies = defList.Where(x => x.ArticleID == articleItem.ID && x.ColorID == articleItem.ColorID && x.Date == lastDateUpdate);
                var minPrice = submitedPrcies.Any() ? submitedPrcies.Min(x => x.Price) : 0;
                return defList.FirstOrDefault(x => x.ArticleID == articleItem.ID && x.ColorID == articleItem.ColorID && x.Date == lastDateUpdate && x.Price == minPrice);
            });
            List<ScrapResult> finalList = [];
            foreach (var item in restest1)
            {
                if (item != null)
                    finalList.Add(item);
            }
            finalList.Sort();
            ScrapItems = finalList;
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
                    if (scrappedData.ArticleID == retryScrapItem.ArticleID && scrappedData.ProviderID == retryScrapItem.ProviderID && (retryScrapItem.ColorID == -1 || retryScrapItem.ColorID == scrappedData.ColorID))
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
                if (scrapResult != null)
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
            //set time
            //set date
            PersianDate pd = PersianDate.Now;
            string time = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
            var scrItems = ScrapItems.Where(x => x.ValidData).ToList();
            //update buy pcies
            foreach (var scrapitem in scrItems)
            {
                scrapitem.Time = time;
                scrapitem.Date = pd;
                APIDataStorage.ScrapManager.Add(scrapitem);
            }
            scrItems.GroupBy(x => x.ArticleID).ToList().ForEach((srcItem) => 
            {
                //try to find 1 item
                //find article object
                var articleObject = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == srcItem.Key);
                if (articleObject == null)
                    return;
                //iphone 13 , black , white , pink , blue , green
                //check if article have variables
                if (articleObject.HaveVariable)
                {
                    var jObj = articleObject.OptionValueJson;
                    //group and loop through colors
                    scrItems.GroupBy(x => x.ColorID).ToList().ForEach((colorItem) =>
                    {
                        //extract article with color object
                        var articleWithColorObject = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == srcItem.Key && x.ColorID == colorItem.Key);
                        if (articleWithColorObject == null)
                            return;
                        //1==>iphone 13 , black 
                        var minPrice = colorItem.Min(x => x.Price);
                        articleWithColorObject.Price = (minPrice * 1.07).RoundUp(4);
                        articleWithColorObject.RegularPrice = (minPrice * 1.07).RoundUp(4);
                        //send POST Request for updating wp_postmeta
                        articleWithColorObject.RequestType = 0;
                        APIDataStorage.ArticleManager.Add(articleWithColorObject);
                        if (jObj != null)
                        {
                            // 1 is price
                            jObj.Last.First["price"][$"{colorItem.Key}"] = (minPrice * 1.07).RoundUp(4).ToString();
                            // 2 is regular_price
                            jObj.Last.First["regular_price"][$"{colorItem.Key}"] = (minPrice * 1.07).RoundUp(4).ToString();
                            // 3 is regular sale_price
                            jObj.Last.First["sale_price"][$"{colorItem.Key}"] = (minPrice * 1.07).RoundUp(4).ToString();
                        }
                    });
                    if(jObj != null)
                    {
                        articleObject.OptionValue = jObj.ToString();
                        articleObject.RequestType = 1;
                        //send POST Request for updating wp_options
                        APIDataStorage.ArticleManager.Add(articleObject);
                    }
                }
                else
                {
                    //not implemented yet
                }

            });
            
            
            //loop trought each scrapped article
            ScrapItems.GroupBy(x => x.ArticleID).ToList().ForEach((groupItem)=>
            {
                var scrItem = groupItem.FirstOrDefault();
                if (scrItem == null)
                    return;
                var art = scrItem.GetArticle();
                if (art == null)
                    return;
                
                //loop through each scrapped color
                //group by color and loop trought it 
                groupItem.GroupBy(x => x.ColorID).ToList().ForEach((colorItem)=>
                {
                    
                    //extract minimun price for each color
                    var price = colorItem.Min(x => x.Price);
                    //set article price
                    art.Price = price;
                    //set article regular price
                    art.RegularPrice = price;
                    //detect if item have any defined color
                    
                    if (art.HaveVariable)
                    {
                        var jObject = art.OptionValueJson;
                        //update json file based on color id 

                    }
                });

                //send new data to API
            });
        }
    }
}