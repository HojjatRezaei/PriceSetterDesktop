namespace PriceSetterDesktop.ViewModel
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Engines;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
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
                APIDataStorage.ScrapManager.Add(scrapitem);
            }
            //Group Articles and Loop Through it
            scrItems.GroupBy(x => x.ArticleID).ToList().ForEach((srcItem) =>
            {
                //try to find 1 article object based on Grouped Key
                var articleObject = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == srcItem.Key);
                if (articleObject == null)
                    return;
                if (articleObject.ValidForProcess)
                    return;
                //Check if Article Have Any Variables , Like Color . Size . . . etc
                if (articleObject.HaveVariable)
                {
                    var articleColors = APIDataStorage.ArticleManager.List.Where(x => x.ID == articleObject.ID).ToList();
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
                        //articleWithColorObject.Price = (minPrice * 1.07).RoundUp(4);
                        //articleWithColorObject.RegularPrice = (minPrice * 1.07).RoundUp(4);
                        articleWithColorObject.Price = CalculatePrice(minPrice);
                        articleWithColorObject.RegularPrice = CalculatePrice(minPrice);
                        articleWithColorObject.ColorStockStatus = "instock";
                        articleWithColorObject.RequestType = 0;
                        articleWithColorObject.DateValue = pd.ToString();
                        //try to find articleWithColor
                        var itemIndex = articleColors.IndexOf(articleWithColorObject);
                        articleColors[itemIndex] = articleWithColorObject;
                    });
                    //extract article color list
                    //filter instock colors
                    var inStockColors = articleColors.Where(x => x.ColorStockStatus == "instock");
                    //calculate minimum existing stock price 
                    var minStockPrice = inStockColors.Min(x => x.Price);
                    //filter out outofstock colors
                    var outofStockColors = articleColors.Where(x => x.ColorStockStatus == "outofstock").ToList();
                    //filter out not used colors
                    var unsetList = articleColors.Where(x => !inStockColors.Any(y => y.ColorID == x.ColorID) && !outofStockColors.Any(y => y.ColorID == x.ColorID)).ToList();
                    //Append Unset Color List to OutofStock Colors List
                    outofStockColors.AddRange(unsetList);
                    //Loop through each OutofStock Color object and Calculate Minimum Price
                    foreach (var outStockColor in outofStockColors)
                    {
                        if (outStockColor.Price < minStockPrice)
                        {
                            outStockColor.Price = minStockPrice;
                            outStockColor.RegularPrice = minStockPrice;
                        }
                        else
                        {
                            outStockColor.Price = outStockColor.Price;
                            outStockColor.RegularPrice = outStockColor.Price;
                        }
                        outStockColor.RequestType = 0;
                        var res = APIDataStorage.ArticleManager.Add(outStockColor);
                    }
                    //Create an Object to indicate whenever a color exist in current article scope 
                    Article? singleStockObject = null;
                    //loop through each InStock Colors
                    foreach (var inStockColor in inStockColors)
                    {
                        //set request type 
                        inStockColor.RequestType = 0;
                        //send API POST Request for updating value in Wordpress DataBase
                        var res = APIDataStorage.ArticleManager.Add(inStockColor);
                        //if there's a stock color in current article scope , instock parent in Wordpress DataBase
                        singleStockObject ??= inStockColor;
                    }
                    var colorObjectList = new List<Article>();
                    colorObjectList.AddRange(outofStockColors);
                    colorObjectList.AddRange(inStockColors);
                    if (singleStockObject != null)
                    {
                        singleStockObject.RequestType = 2;
                        var res = APIDataStorage.ArticleManager.Add(singleStockObject);
                    }
                    else if (articleObject.ParentStockID != -1 && articleObject.ParentStockStatus != "outofstock")
                    {
                        articleObject.RequestType = 2;
                        articleObject.ParentStockStatus = "outofstock";
                        var res = APIDataStorage.ArticleManager.Add(articleObject);
                    }
                    //find lowest submited price 
                    var jobjectResult = WriteJsonOject(jObj, colorObjectList);
                    if (jobjectResult != null)
                    {
                        articleObject.OptionValue = jobjectResult.ToString(Formatting.None);
                        articleObject.RequestType = 1;
                        //send POST Request for updating wp_options
                        var res = APIDataStorage.ArticleManager.Add(articleObject);
                    }
                }
                else
                {
                    //not implemented yet
                }
            });
            //set unscrapped article stock to outofstock
            //filter unscrapped articles
            var unscrappedArticles = APIDataStorage.ArticleManager.List.Where(x => !scrItems.Any(y => y.ArticleID == x.ID)).ToList();
            unscrappedArticles.GroupBy(x => x.ID).ToList().ForEach((articleKey) =>
            {
                var childList = APIDataStorage.ArticleManager.List.Where(x => x.ID == articleKey.Key);
                //unstock childs
                foreach (var child in childList)
                {
                    if (child.ColorStockStatus != "outofstock")
                    {
                        child.RequestType = 3;
                        child.ColorStockStatus = "outofstock";
                        var res = APIDataStorage.ArticleManager.Add(child);
                    }
                }
                var parent = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == articleKey.Key);
                //unstock parent 
                if (parent != null && parent.ParentStockStatus != "outofstock")
                {
                    parent.RequestType = 3;
                    parent.ParentStockStatus = "outofstock";
                    var res = APIDataStorage.ArticleManager.Add(parent);
                }
            });
        }
        private JObject? WriteJsonOject(JObject? jObj, List<Article> colorList)
        {
            if (jObj == null) return null;
            Dictionary<string, object>? dictionary = jObj.ToObject<Dictionary<string, object>>();
            if (dictionary == null)
                return null;
            var colorJObject = new JObject();
            foreach (var colorItem in colorList)
            {
                colorJObject.Add($"{colorItem.ColorID}", $"{colorItem.Price}");
            }
            var newJObject = new JObject
            {
                //add version token
                { "version", $"{dictionary["version"]}" },
                //add Hash Token
                { dictionary.Last().Key, new JObject()
                    {
                        { "price",colorJObject},
                        { "regular_price" ,colorJObject},
                        { "sale_price",colorJObject},
                    }
                }
            };
            return newJObject;
        }
        private double CalculatePrice(double price)
        {
            //if buy price is greater than 10,000,000 Rial , Multiple Value by 7% ==> 1.07
            if (price > 10000000)
            {
                return (price * 1.07).RoundUp(4);
            }
            //if buy price is lesser than 10,000,000 Rial , Multiple Value By 8% ==> 1.08
            else
            {
                return (price * 1.08).RoundUp(4);
            }
        }
    }
}