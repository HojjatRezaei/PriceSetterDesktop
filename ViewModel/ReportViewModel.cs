namespace PriceSetterDesktop.ViewModel
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.APIManager;
    using PriceSetterDesktop.Libraries.Engines;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Diagnostics;
    using System.DirectoryServices;
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
            var lastDateUpdate = defList.Max(x => x.Date);
            var restest1 = APIDataStorage.ArticleManager.List.Select((articleItem) => 
            {
                var maxList = defList.Where(x => x.ArticleID == articleItem.ID && x.ColorID == articleItem.ColorID);
                if (!maxList.Any())
                {
                    return default;
                }
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
            PersianDate pd = PersianDate.Now;
            string time = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}";
            var scrItems = ScrapItems.Where(x => x.ValidData).ToList();
            //update buy pcies
            foreach (var scrapitem in scrItems)
            {
                scrapitem.Time = time;
                scrapitem.Date = pd;
                //send prices to scrappedPrices Table in hojjatdb DataBase
                //APIDataStorage.ScrapManager.Add(scrapitem);
            }
            //Group Articles and Loop Through it
            scrItems.GroupBy(x => x.ArticleID).ToList().ForEach((srcItem) => 
            {
                //try to find 1 article object based on Grouped Key
                var articleObject = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == srcItem.Key);
                
                if (articleObject == null)
                    return;
                //Check if Article Have Any Variables , Like Color . Size . . . etc
                if (articleObject.HaveVariable)
                {
                    var articleColors = APIDataStorage.ArticleManager.List.Where(x => x.ID == srcItem.Key).ToList();
                    foreach (var color in articleColors)
                    {
                        color.ColorStockStatus = "outofstock";
                    }
                    //save json value into variable for future editing . . .
                    var jObj = articleObject.OptionValueJson;
                    //Group Article Colors and loop through it
                    scrItems.GroupBy(x => x.ColorID).ToList().ForEach((colorItem) =>
                    {
                        //Extract Article Object Based On Color Key
                        var articleWithColorObject = articleColors.FirstOrDefault(x => x.ID == srcItem.Key && x.ColorID == colorItem.Key);
                        if (articleWithColorObject == null)
                            return;
                        var minPrice = colorItem.Min(x => x.Price);
                        //validate extracted price
                        if (minPrice < 10000)
                            return;
                        articleWithColorObject.Price = (minPrice * 1.07).RoundUp(4);
                        articleWithColorObject.RegularPrice = (minPrice * 1.07).RoundUp(4);
                        articleWithColorObject.ColorStockStatus = "instock";
                        articleWithColorObject.RequestType = 0;
                        //try to find articleWithColor
                        var itemIndex = articleColors.IndexOf(articleWithColorObject);
                        articleColors[itemIndex] = articleWithColorObject;
                    });
                    //extract article color list
                    //filter instock colors
                    var outofStockColors = articleColors.Where(x => x.ColorStockStatus == "outofstock");
                    var inStockColors = articleColors.Where(x => x.ColorStockStatus == "instock");

                    var mininStockPrice = inStockColors.Min(x => x.Price);
                    foreach (var outStockColor in outofStockColors)
                    {
                        if(outStockColor.Price < mininStockPrice)
                        {
                            // 1 is price
                            jObj.Last.First["price"][$"{outStockColor.ColorID}"] = mininStockPrice.ToString();
                            // 2 is regular_price
                            jObj.Last.First["regular_price"][$"{outStockColor.ColorID}"] = mininStockPrice.ToString();
                            // 3 is regular sale_price
                            jObj.Last.First["sale_price"][$"{outStockColor.ColorID}"] = mininStockPrice.ToString();
                            outStockColor.Price = mininStockPrice;
                            outStockColor.RegularPrice = mininStockPrice;
                        }
                        outStockColor.RequestType = 0;
                        var res = APIDataStorage.ArticleManager.Add(outStockColor);
                    }
                    Article? singleStockObject = null;
                    foreach (var inStockColor in inStockColors)
                    {
                        // 1 is price
                        jObj.Last.First["price"][$"{inStockColor.ColorID}"] = inStockColor.Price.ToString();
                        // 2 is regular_price
                        jObj.Last.First["regular_price"][$"{inStockColor.ColorID}"] = inStockColor.Price.ToString();
                        // 3 is regular sale_price
                        jObj.Last.First["sale_price"][$"{inStockColor.ColorID}"] = inStockColor.Price.ToString();
                        inStockColor.RequestType = 0;
                        var res = APIDataStorage.ArticleManager.Add(inStockColor);
                        singleStockObject = inStockColor;
                    }
                    if (singleStockObject != null)
                    {
                        singleStockObject.RequestType = 2;
                        var res = APIDataStorage.ArticleManager.Add(singleStockObject);
                    }
                    //find lowest submited price 
                    if (jObj != null)
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
            //scrItems
            //detach unset value from site // set it to outofstock
            var unsetList = APIDataStorage.ArticleManager.List.Where(x => scrItems.FirstOrDefault(y => y.ArticleID != x.ID) != default);
        }
    }
}