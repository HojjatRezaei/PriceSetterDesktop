namespace PriceSetterDesktop.Libraries.Engines
{
    using OpenQA.Selenium.Support.UI;
    using OpenQA.Selenium;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using OpenQA.Selenium.Chrome;
    using Microsoft.VisualBasic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using System.Security.Policy;
    using System.Xml.XPath;
    using WPFCollection.Data.Statics;
    using PriceSetterDesktop.Libraries.Statics;
    using System.Windows.Automation.Provider;

    public class WebScrapEngine : IDisposable
    {
        private WebDriver _drive;
        private WebDriverWait _driveWaiter;
        private IJavaScriptExecutor _scripter;
        private List<ScrapResult> _scrapResult;
        public WebScrapEngine()
        {

        }
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
            _drive.Quit();
            _drive.Dispose();
        }
        public IEnumerable<ScrapResult>? Scrap(Provider providerObject , Article? articleObject = null)
        {
            //detect scrap type
            switch (providerObject.Extraction)
            {
                case Types.Enum.ExtractionTypes.Scrap:
                    return ScrapFromWeb(providerObject , articleObject);
                case Types.Enum.ExtractionTypes.Excel:
                    return ScrapFromExcelFile();
                case Types.Enum.ExtractionTypes.Image:
                    return ScrapFromImageFile();
            }
            return null;
        }

        /// <summary>
        /// method for handling errors in process
        /// </summary>
        /// <param name="exception">throwed exception</param>
        private void HandleError(Exception exception)
        {

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
                    if(url != null)
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
                var url = providerObject.UrlList.FirstOrDefault(x=> x.ArticleID == articleObject.ID);
                if(url != null)
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
        private List<ScrapResult>? ScrapFromExcelFile()
        {
            return null;
        }
        private List<ScrapResult>? ScrapFromImageFile()
        {
            return null;
        }

        private IEnumerable<ScrapResult>? ScrapSingleURL(Types.Data.Url urlObject)
        {
            //try to get providerObject
            var providerObject = urlObject.GetProvider();
            if (providerObject == null)
                return [new ScrapResult() { HaveMessage = true , Messages = "cannot find provider object"}];
            //try to extract containers
            var articleObject = urlObject.GetArticle();
            if (articleObject == null)
                return [new ScrapResult() { HaveMessage = true, Messages = "cannot find article object" }];
            //nagivate to url and for page to load completely 
            //catch navigation time out error
            try
            {
                //create new instance for dirve waiter
                _driveWaiter = new(_drive, new(0, 0, 10));
                //time out for waiting a page to load
                _drive.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 20);
                //time out for searching for elements
                _drive.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);
                //time out for executing javascript commands
                _drive.Manage().Timeouts().AsynchronousJavaScript = new TimeSpan(0, 0, 10);
                _drive.Navigate().GoToUrl(urlObject.URL);
            }
            catch (Exception e)
            {
                return [new ScrapResult() { HaveMessage = true, Messages = "TimeOut" }];
            }
            /*Create Task for checking ad popup and try to close it*/

            /*Log in Part*/

            /* Scrap Part*/

            var clickAndExtractContainers = providerObject.Containers.Where(x=> x.Type == Types.Enum.ContainerType.ClickAndExtract);
            var listExtractContainer = providerObject.Containers.Where(x => x.Type == Types.Enum.ContainerType.List);
            var clickAndContinueContainers = providerObject.Containers.Where(x => x.Type == Types.Enum.ContainerType.ClickAndContinue);
            //continue process based on valid container
            if (clickAndExtractContainers.Any())
            {
                var clickScrapResult = ClickAndExtract(clickAndExtractContainers , articleObject , providerObject , clickAndContinueContainers);
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
                var listScrapResult = ListExtract(listExtractContainer, articleObject, providerObject);
                //try to relate extracted result to article available colors
                if(listScrapResult != null)
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
        private IEnumerable<ScrapResult> BindRelations(IEnumerable<ScrapResult> scrapList , Article article)
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
                            if(ColorNameEquality(scrapitem.ColorName , color.Name))
                            {
                                scrapitem.ColorID = color.ID;
                                scrapitem.PriceID = color.PriceMetaID;
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
        private string CleanColorName(string txt)
        {
            return txt.Replace(":", string.Empty).Replace("رنگ", string.Empty).Replace(" ", string.Empty).Trim();
        }
        private bool ColorNameEquality(string rightName , string leftName)
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
        private IEnumerable<ScrapResult> ClickAndExtract(
            IEnumerable<Container> container , 
            Article article, Provider provider, 
            IEnumerable<Container>? clickAndContinueContainer = null)
        {
            //loop through click elements
            //try to find container
            var containerObject = container.First();
            var clickableElements = FindChildren(containerObject.Path);
            var scrapList = new List<ScrapResult>();
            if (clickableElements == null || clickableElements.Count == 0)
            {
                scrapList.Add(new ScrapResult() { HaveMessage = true, Messages = "Can't find click elements" });
                return scrapList;
            }
            else
            {
                foreach (var clickElement in clickableElements)
                {
                    if (clickElement == null)
                        continue;
                    RemoveAd();
                    //containerObject.Path+"/"+clickElement.TagName + "[class='" + clickElement.GetAttribute("class") + "']"
                    //try to get xpath string from webelement
                    //clickElement.header + clickElement.TagName + clickElement.GetAttribute("class") 
                    //try to find element
                        clickElement.Click();
                    //try
                    //{
                    //}
                    //catch (Exception)
                    //{
                    //    RemoveAd();
                    //    clickElement.Click();
                    //}
                    //wait 1 second after click on element for processing AJAX
                    Thread.Sleep(1000);
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
        private IEnumerable<ScrapResult> ListExtract(IEnumerable<Container> container , Article article ,Provider provider)
        {
            var table = container.FirstOrDefault();
            if(table == null)
            {
                yield return new ScrapResult() { HaveMessage = true , Messages="Container not found" };
            }
            else
            {
                var scrapRows = FindChildren(table.Path);
                if (scrapRows == null)
                {
                    yield return new ScrapResult() { HaveMessage = true, Messages = "Container Row Not Found " };
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
                            IWebElement? scrappedItem=null;
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

                            if(scrapValidation == false)
                            {
                                yield return new ScrapResult() { HaveMessage = true, Messages = "Container Row Not Found " };
                            }
                            else
                            {
                                if (scrappedItem == null)
                                {

                                }
                                else
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
        private void ScrapExtratedElement(IWebElement element , string pathTag , ref ScrapResult scrapResult)
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
                        Thread.Sleep(1000);
                    }
                }
            }
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
                    return _drive.FindElements(By.XPath(path+"/*"));
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
        private string ExtractColor(string colorName)
        {
            //search in color collection and find related color id
            return colorName.Replace("رنگ" , string.Empty).Replace(":",string.Empty).Trim();
        }
        private double ExtractPrice(string scrapString)
        {
            //clean string
            return double.TryParse(scrapString.RemoveWords().CorrectPersianNumber().Replace(",",string.Empty), out double price) ? price : 0;
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
        public void Dispose()
        {
            _drive.Quit();
        }

        /*private ArticleDetails Example(WebDriver driver, Provider provider)
        {
            //navigate to the url
            try
            {
                driver.Navigate().GoToUrl(URL);
            }
            catch (Exception)
            {
                return ReturnError("InternetProblem", driver);
            }
            WebDriverWait waiter = new(driver, new(0, 0, 10));
            //create new instance for javascript
            IJavaScriptExecutor scripter = driver;
            //try to find click and extract containers
            var clickAndExtractContainers = provider.Containers.Where(x => x.Type == Types.Enum.ContainerType.ClickAndExtract);
            //try to find list containers
            var listContainers = provider.Containers.Where(x => x.Type == Types.Enum.ContainerType.List);
            if (clickAndExtractContainers != null && clickAndExtractContainers.Any())
            {
                var priceViewList = new List<ArticleDetails>();
                //loop trough click containers and click on each one of them
                var clickContainer = clickAndExtractContainers.First();
                // if clickContainer is null , go to next click container
                if (clickContainer == null)
                    return ReturnError("ظرف خالی میباشد", driver);
                //wait for DOM to load completely
                try
                {
                    waiter.Until((x) => { return scripter.ExecuteScript($"return document.evaluate(\"{clickContainer.Path}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null; });
                }
                catch (Exception ex)
                {
                    //if couldn't find element , goto the next container
                    ReturnError(ex.Message, driver);
                }
                var clickElementContainer = driver.FindElement(By.XPath(clickContainer.Path));
                var clickElements = clickElementContainer.FindElements(By.XPath("*"));
                foreach (IWebElement clickElement in clickElements)
                {
                    ArticleDetails newPriceView = new()
                    {
                        Provider = provider,
                        Color = ""
                    };
                    if (clickElement == null)
                        continue;
                    //try to find close ad button
                    ///html/body/div[3]/div[2]
                    var adDetector = scripter.ExecuteScript($"return document.evaluate(\"/html/body/div[3]/div[2]\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null);
                    if (adDetector is IWebElement adCloseButton)
                    {
                        adCloseButton.Click();
                    }
                    //click on element 
                    clickElement.Click();
                    //find next click element and loop through it 
                    var clickAndContinue = provider.Containers.Where(x => x.Type == Types.Enum.ContainerType.ClickAndContinue).FirstOrDefault();
                    if (clickAndContinue != null)
                    {
                        var diffClickElement = driver.FindElement(By.XPath(clickAndContinue.Path));
                        //extract all children
                        var diffClickElementChilds = diffClickElement.FindElements(By.XPath("*"));
                        //if click element is 1 , ignore the loop and just click on it
                        foreach (var diffClick in diffClickElementChilds)
                        {
                            if (diffClick == null)
                                continue;
                            //extract path items
                            var diffClickPathes = clickAndContinue.PathItems;
                            var diffPathSearch = diffClickPathes.Where(x => x.PathTag == "Search").FirstOrDefault();
                            if (diffClickPathes.Count != 0 && diffPathSearch != null)
                            {
                                if (diffClick.Text == diffPathSearch.Path || diffClick.Text.Contains(diffPathSearch.Path))
                                {
                                    diffClick.Click();
                                }
                            }
                        }
                    }
                    //wait 1 second to give html page some times to load AJAX
                    Thread.Sleep(1000);
                    //extract resources using xpath
                    foreach (PathItem pathItem in clickContainer.PathItems)
                    {
                        //collection resources with straight xpath 
                        var extractionResult = driver.FindElement(By.XPath(pathItem.Path)).Text;
                        //check if extractionResult have any definition of color ,
                        //if it does , set the extraction result as PriceView Color
                        if (extractionResult.Contains("رنگ") || pathItem.PathTag == "رنگ")
                        {
                            newPriceView.Color = CleanString(extractionResult);
                        }
                        //create tagView object and add it to the tagList
                        else
                        {
                            TagView newTag = new() { TagName = pathItem.PathTag, TagValue = CleanString(extractionResult) };
                            newPriceView.Tags.Add(newTag);
                        }
                    }
                    //return collected result
                    priceViewList.Add(newPriceView);
                }

                //return scrap result collection
                return priceViewList;
            }
            //collection resources based on List Container Type
            else if (listContainers != null && listContainers.Any())
            {
                var priceViewList = new List<ArticleDetails>();
                //if url doesn't have click container , continue with list containers
                if (listContainers != null && listContainers.Any())
                {
                    foreach (Container listContainer in listContainers)
                    {
                        if (listContainer == null || listContainer.PathItems.Count == 0)
                            continue;
                        //extract container elements
                        //check if containerpath is valid
                        var checkResult = scripter.ExecuteScript($"return document.evaluate(\"{listContainer.Path}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null);
                        if (checkResult is not IWebElement)
                            continue;
                        var scrapResultElements = driver.FindElements(By.XPath(listContainer.Path + "/*"));
                        foreach (IWebElement scrapElement in scrapResultElements)
                        {
                            ArticleDetails articleDetails = new()
                            {
                                Provider = provider,
                                Color = "",
                            };
                            foreach (PathItem pathItem in listContainer.PathItems)
                            {
                                //extract value from scrapResultElements
                                try
                                {
                                    var listScrapResult = scrapElement.FindElement(By.XPath($".{pathItem.Path}"));
                                    if (listScrapResult.Text.Contains("رنگ") || pathItem.PathTag == "رنگ")
                                    {
                                        articleDetails.Color = listScrapResult.Text;
                                    }
                                    //create tagView object and add it to the tagList
                                    else
                                    {
                                        TagView newTag = new() { TagName = pathItem.PathTag, TagValue = listScrapResult.Text };
                                        articleDetails.Tags.Add(newTag);
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                            priceViewList.Add(articleDetails);
                        }
                    }
                }
                else
                {
                    // if page doesn't have containers , check for straight xpath
                    return ReturnError("مشکل در پیدا کردن ظروف", driver);
                }
                return priceViewList;
            }
            // if code reach here , it didn't find any source to collect
            //return with error instance
            return ReturnError("اطلاعاتی از منابع پیدا نشد", driver);
        }*/
    }
}
