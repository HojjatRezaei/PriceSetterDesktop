namespace PriceSetterDesktop.Libraries.Engines
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Collections.ObjectModel;
    using WPFCollection.Data.Statics;

    public class WebScrapEngine : IDisposable
    {
        #region Scope Properties
        private List<ScrapResult> _scrapResult = [];

        #endregion

        #region WebScrap From URL
        public void TurnOn()
        {
            //setup chrome startup option
            ChromeOptions chromeOption = new();
            //add maximized parameter to the chromeoption instance
            chromeOption.AddArgument("--start-maximized");
            chromeOption.AddArgument("--disable-popup-blocking");
            //create new instance of chrome driver
            _drive = new ChromeDriver(chromeOption);

            //create instance for executing javascript commands
            _scripter = _drive;
            //create new list container for saving scrapped result
            _scrapResult = [];
        }
        public void TurnOff()
        {
            Dispose();
        }
        private IWebElement? FindSingleElement(string path)
        {
            //try the following , if error occured , return null,
            try
            {
                //if xpath argument is null or empty , return null
                if (string.IsNullOrEmpty(path)) return null;
                //return single value
                //validate path by trying to find element
                var isValid = _driveWaiter.Until((x) => { return _scripter.ExecuteScript($"return document.evaluate(\"{path}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null; });
                //if element founded
                if (isValid)
                {
                    return _drive.FindElement(By.XPath(path));
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        private ReadOnlyCollection<IWebElement>? FindChildren(string path)
        {
            //try the following , if error occured , return null,
            try
            {
                //if xpath argument is null or empty , return null
                if (string.IsNullOrEmpty(path)) return null;
                //return single value
                //validate path by trying to find element
                var isValid = _driveWaiter.Until((x) => { return _scripter.ExecuteScript($"return document.evaluate(\"{path}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null; });
                //if element founded
                if (isValid)
                {
                    return _drive.FindElements(By.XPath(path + "/*"));
                }
                else
                {
                    return null;
                }

            }
            catch (Exception)
            {
                return null;
            }

        }
        private IEnumerable<ScrapResult>? ScrapFromWeb(Provider providerObject, Article? articleObject = null)
        {
            //detect if is a single scrap or a list scrap
            if (articleObject == null)
            {
                var scrapList = new List<ScrapResult>();
                //list scrap
                //get a list of urls
                //check for articleList nullable
                foreach (var url in providerObject.UrlList)
                {
                    if (url != null)
                    {
                        var result = ScrapSingleURL(url);
                        if (result == null)
                            continue;
                        foreach (var res in result)
                        {
                            if (res == null)
                                continue;
                            scrapList.Add(res);
                        }
                    }
                }
                return scrapList;
            }
            else
            {
                //extract related url
                var url = providerObject.UrlList.FirstOrDefault(x => x.ArticleID == articleObject.ID);
                if (url != null)
                {
                    //single scrap
                    return ScrapSingleURL(url);
                }
                else
                {
                    return null;
                }
            }
        }
        private IEnumerable<ScrapResult>? ScrapSingleURL(Url urlObject)
        {
            //try to get providerObject
            var providerObject = urlObject.GetProvider();
            if (providerObject == null)
                return [new ScrapResult() { HaveMessage = true, Messages = "Cannot Find Provider Object" }];
            //try to extract containers
            var articleObject = urlObject.GetArticle();
            if (articleObject == null)
                return [new ScrapResult() { HaveMessage = true, Messages = "Cannot Find Article Object", ProviderID = providerObject.ID, Source = providerObject.Name }];
            //create new instance for dirve waiter
            _driveWaiter = new(_drive, new(0, 0, 10));
            //time out for waiting a page to load
            _drive.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 20);
            //time out for searching for elements
            _drive.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 2);
            //time out for executing javascript commands
            _drive.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(0, 0, 2);
            //nagivate to url and for page to load completely 
            //catch navigation time out error
            try
            {
                _drive.Navigate().GoToUrl(urlObject.URL);
            }
            catch (Exception)
            {
                return [new ScrapResult() { HaveMessage = true, Messages = "TimeOut", ProviderID = providerObject.ID, ArticleID = articleObject.ID, Source = providerObject.Name }];
            }
            /*Create Task for checking ad popup and try to close it*/

            /*Log in Part*/

            /* Scrap Part*/

            var clickAndExtractContainers = providerObject.Containers.Where(x => x.Type == Types.Enum.ContainerType.ClickAndExtract);
            var listExtractContainer = providerObject.Containers.Where(x => x.Type == Types.Enum.ContainerType.List);
            var clickAndContinueContainers = providerObject.Containers.Where(x => x.Type == Types.Enum.ContainerType.ClickAndContinue);
            //continue process based on valid container
            if (clickAndExtractContainers.Any())
            {
                if (!CheckStock(clickAndExtractContainers.First()))
                {
                    return [new ScrapResult() { Messages = "موجود نیست", HaveMessage = true, ProviderID = providerObject.ID, ArticleID = articleObject.ID, Source = providerObject.Name }];
                }
                //extract nostock tag from container
                var clickScrapResult = ClickAndExtract(clickAndExtractContainers, articleObject, providerObject, clickAndContinueContainers);
                //try to relate extracted result to article available colors
                if (clickScrapResult != null)
                {
                    var bindRelationList = BindRelations(clickScrapResult, articleObject);
                    return bindRelationList;
                }
                else
                {
                    return clickScrapResult;
                }
            }
            else if (listExtractContainer.Any())
            {
                if (!CheckStock(listExtractContainer.First()))
                {
                    return [new ScrapResult() { Messages = "موجود نیست", HaveMessage = true, ProviderID = providerObject.ID, ArticleID = articleObject.ID, Source = providerObject.Name }];
                }
                var listScrapResult = ListExtract(listExtractContainer, articleObject, providerObject);
                //try to relate extracted result to article available colors
                if (listScrapResult != null)
                {
                    var bindRelationList = BindRelations(listScrapResult, articleObject);
                    return bindRelationList;
                }
                else
                {
                    return listScrapResult;
                }

            }
            else
            {
                return null;
            }
        }
        private IEnumerable<ScrapResult> BindRelations(IEnumerable<ScrapResult> scrapList, Article article)
        {
            //get a list of articles
            var storedScrapList = scrapList.ToList();
            var readableArticleList = DataHolder.ReadableArticleList.Where(x => x.ID == article.ID);
            if (readableArticleList.Any())
            {
                foreach (var scrapitem in storedScrapList)
                {
                    foreach (var readableArticle in readableArticleList)
                    {
                        bool colorFounded = false;
                        //check both side color
                        foreach (var color in readableArticle.Colors)
                        {
                            if (ColorNameEquality(scrapitem.ColorName, color.Name))
                            {
                                scrapitem.ColorID = color.ID;
                                colorFounded = true;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (colorFounded)
                            break;
                    }
                }
                return storedScrapList;
            }
            else
            {

                return storedScrapList;
            }
        }
        private IEnumerable<ScrapResult> ClickAndExtract(IEnumerable<Container> container, Article article, Provider provider, IEnumerable<Container>? clickAndContinueContainer = null)
        {
            //loop through click elements
            //try to find container
            var containerObject = container.First();
            var clickableElements = FindChildren(containerObject.Path);
            var scrapList = new List<ScrapResult>();
            if (clickableElements == null || clickableElements.Count == 0)
            {
                scrapList.Add(new ScrapResult() { HaveMessage = true, Messages = "Can't find click elements", ProviderID = provider.ID, ArticleID = article.ID, Source = provider.Name });
                return scrapList;
            }
            else
            {
                foreach (var clickElement in clickableElements)
                {
                    if (clickElement == null)
                        continue;
                    RemoveAd();
                    //try to find element
                    clickElement.Click();
                    //wait 1 second after click on element for processing AJAX
                    Thread.Sleep(500);
                    //after a successfull click , check for clickAndContinue elements
                    if (clickAndContinueContainer != null && clickAndContinueContainer.Any())
                    {
                        var clickAndContinue = clickAndContinueContainer.First();
                        ClickAndContinue(clickAndContinue);

                    }
                    //start extrating data from webpage
                    var scrapResult = new ScrapResult()
                    {
                        ProviderID = provider.ID,
                        ArticleID = article.ID,
                        HaveMessage = false,
                        Source = provider.Name,
                    };
                    foreach (PathItem pathItem in containerObject.PathItems)
                    {
                        if (pathItem.PathTag == "NoStock")
                            continue;
                        //collection resources with straight xpath 
                        var element = FindSingleElement(pathItem.Path);
                        if (element == null) continue;
                        ScrapExtratedElement(element, pathItem.PathTag, ref scrapResult);
                    }
                    //out
                    scrapList.Add(scrapResult);
                }
            }
            return scrapList;
        }
        private IEnumerable<ScrapResult> ListExtract(IEnumerable<Container> container, Article article, Provider provider)
        {
            var table = container.FirstOrDefault();
            if (table == null)
            {
                yield return new ScrapResult() { HaveMessage = true, Messages = "Container not found", ProviderID = provider.ID, ArticleID = article.ID, Source = provider.Name };
            }
            else
            {
                var scrapRows = FindChildren(table.Path);
                if (scrapRows == null)
                {
                    yield return new ScrapResult() { HaveMessage = true, Messages = "Container Row Not Found ", ProviderID = provider.ID, ArticleID = article.ID, Source = provider.Name };
                }
                else
                {
                    foreach (IWebElement scrapRow in scrapRows)
                    {
                        var scrapCell = new ScrapResult()
                        {
                            ProviderID = provider.ID,
                            ArticleID = article.ID,
                            HaveMessage = false,
                            Source = provider.Name,
                        };
                        foreach (PathItem pathItem in table.PathItems)
                        {
                            if (pathItem.PathTag == "NoStock")
                                continue;
                            IWebElement? scrappedItem = null;
                            bool scrapValidation;
                            //extract value from scrapResultElements
                            try
                            {
                                scrappedItem = scrapRow.FindElement(By.XPath($".{pathItem.Path}"));
                                scrapValidation = true;
                            }
                            catch (Exception)
                            {
                                scrapValidation = false;
                            }

                            if (scrapValidation == false)
                            {
                                yield return new ScrapResult() { HaveMessage = true, Messages = "Container Row Not Found ", ProviderID = provider.ID, ArticleID = article.ID, Source = provider.Name };
                            }
                            else
                            {
                                if (scrappedItem != null)
                                {
                                    ScrapExtratedElement(scrappedItem, pathItem.PathTag, ref scrapCell);
                                }
                            }
                        }
                        yield return scrapCell;
                    }
                }
            }
        }
        private void ScrapExtratedElement(IWebElement element, string pathTag, ref ScrapResult scrapResult)
        {
            //check if path item have color tag
            if (element.Text.Contains("رنگ") || pathTag == "رنگ")
            {
                scrapResult.ColorName = ExtractColor(element.Text);
            }
            //Check if path item have price Tag
            else if (pathTag == "قیمت")
            {
                scrapResult.Price = ExtractPrice(element.Text);
            }
        }
        private void ClickAndContinue(Container container)
        {
            if (container != null)
            {
                var clickElements = FindChildren(container.Path);
                if (clickElements == null) return;
                //if click element is 1 , ignore the loop and just click on it
                foreach (var clickElement in clickElements)
                {
                    if (clickElement == null)
                        continue;
                    //extract path items
                    var diffPathSearch = container.PathItems.FirstOrDefault();
                    if (diffPathSearch == null) return;
                    if (clickElement.Text == diffPathSearch.Path || clickElement.Text.Contains(diffPathSearch.Path))
                    {
                        clickElement.Click();
                        Thread.Sleep(500);
                    }
                }
            }
        }
        private bool ColorNameEquality(string rightName, string leftName)
        {
            //first clean both sides
            rightName = CleanColorName(rightName);
            leftName = CleanColorName(leftName);
            //check dictionary for exising
            //side right to left contain comparisson
            if (rightName.Contains(leftName))
            {
                return true;
            }
            //side left to right contain comparisson
            else if (leftName.Contains(rightName))
            {
                return true;
            }
            //check equality
            else if (leftName.Equals(rightName))
            {
                return true;
            }
            //if it didn't found do nothing
            else
            {
                return false;
            }
        }
        private bool CheckStock(Container containerObject)
        {
            if (containerObject == null)
                return true;
            //extract container nostock tag value
            var searchResult = containerObject.PathItems.FirstOrDefault(x => x.PathTag == "NoStock");
            if (searchResult != null)
            {
                //try to find nostock element
                try
                {
                    var searchElementResult = _drive.FindElement(By.XPath(searchResult.Path));
                    if (searchElementResult == null)
                    {
                        return true;
                    }
                    else
                    {
                        var targetText = searchElementResult.Text;
                        //try for search 'نا موجود' key word
                        return !(targetText.Contains("موجود") || targetText == "نا موجود" || targetText == "ناموجود");
                    }
                }
                catch (Exception)
                {

                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        private string CleanColorName(string txt)
        {
            return txt.Replace(":", string.Empty).Replace("رنگ", string.Empty).Replace(" ", string.Empty).Trim();
        }
        private string ExtractColor(string colorName)
        {
            if (colorName.Contains("سورمه"))
            {
                colorName = colorName.Replace("سورمه", "سرمه");
            }
            //search in color collection and find related color id
            return colorName.Replace("رنگ", string.Empty).Replace(":", string.Empty).Trim();
        }
        private double ExtractPrice(string scrapString)
        {
            //try to split the string
            var result = scrapString.Split("\n\r");
            if (result.Length > 1)
            {

            }
            //clean string
            return double.TryParse(scrapString.RemoveWords().CorrectPersianNumber().Replace(",", string.Empty), out double price) ? price : 0;
        }
        private void RemoveAd()
        {
            try
            {
                var adDetector = _scripter.ExecuteScript($"return document.evaluate(\"/html/body/div[3]/div[2]\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null);
                if (adDetector is IWebElement adCloseButton)
                {
                    adCloseButton.Click();
                }
            }
            catch (Exception)
            {
                return;
            }


        }
        private WebDriver _drive;
        private WebDriverWait _driveWaiter;
        private IJavaScriptExecutor _scripter;
        #endregion

        #region WebScrap From ExcelFiles
        private List<ScrapResult>? ScrapFromExcelFile()
        {
            return null;
        }

        #endregion

        #region WebScrap From ImageFile
        private List<ScrapResult>? ScrapFromImageFile()
        {
            return null;
        }

        #endregion

        #region WebScrap From Social Media

        #endregion

        #region Main Entry
        public IEnumerable<ScrapResult>? Scrap(Provider providerObject, Article? articleObject = null)
        {
            //detect scrap type
            switch (providerObject.Extraction)
            {
                case Types.Enum.ExtractionTypes.Scrap:
                    return ScrapFromWeb(providerObject, articleObject);
                case Types.Enum.ExtractionTypes.Excel:
                    return ScrapFromExcelFile();
                case Types.Enum.ExtractionTypes.Image:
                    return ScrapFromImageFile();
            }
            return null;
        }

        #endregion



        public void Dispose()
        {
            _drive.Quit();
        }
    }
}
