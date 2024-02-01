namespace PriceSetterDesktop.Libraries.Types.Data
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;

    [XmlMarker(nameof(URLType))]
    public partial class URLType : IGeneratable, IXmlItem
    {
        public URLType()
        {

        }
        [XmlItem(nameof(URL), "string")]
        public string URL { get; set; } = "";
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; } = -1;
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; } = -1;
        public int ElementSeed { get; set; } = -1;
        public bool HaveLogin { get; set; } = false;
        public string CookieKey { get; set; } = string.Empty;
        public string CookieValue { get; set; } = string.Empty;

        private List<ArticleDetails> ScrapWeb(WebDriver driver, Provider provider)
        {
            //navigate to the url
            try
            {
                driver.Navigate().GoToUrl(URL);
            }
            catch (Exception e)
            {
                return ReturnError("InternetProblem", driver);
            }
            WebDriverWait waiter = new(driver, new(0, 0, 10));
            //create new instance for javascript
            IJavaScriptExecutor scripter = driver;
            //try to find click and extract containers
            var clickAndExtractContainers = provider.Containers.Where(x => x.ContainerType == Types.Enum.ContainerType.ClickAndExtract);
            //try to find list containers
            var listContainers = provider.Containers.Where(x => x.ContainerType == Types.Enum.ContainerType.List);
            if (clickAndExtractContainers != null && clickAndExtractContainers.Any())
            {
                var priceViewList = new List<ArticleDetails>();
                //loop trough click containers and click on each one of them
                var clickContainer = clickAndExtractContainers.FirstOrDefault();
                // if clickContainer is null , go to next click container
                if (clickContainer == null)
                    return ReturnError("ظرف خالی میباشد", driver);
                //wait for DOM to load completely
                try
                {
                    waiter.Until((x) => { return scripter.ExecuteScript($"return document.evaluate(\"{clickContainer.ContainerPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null; });
                }
                catch (Exception e)
                {
                    //if couldn't find element , goto the next container
                    ReturnError("خطا در پیدا کردن ظرف", driver);
                }
                IWebElement clickElementContainer;
                ReadOnlyCollection<IWebElement> clickElements;
                try
                {
                    clickElementContainer = driver.FindElement(By.XPath(clickContainer.ContainerPath));
                    clickElements = clickElementContainer.FindElements(By.XPath("*"));
                }
                catch (Exception e)
                {
                    var newPriceViewError = new ArticleDetails()
                    {
                        Provider = provider,
                        Color = string.Empty,
                        IsError = true,
                        ErrorDescription = "مشکل در پیدا کردن ظرف",
                    };
                    priceViewList.Add(newPriceViewError);
                    return priceViewList;
                }
                foreach (IWebElement clickElement in clickElements)
                {
                    ArticleDetails newPriceView = new()
                    {
                        Provider = provider,
                        Color = ""
                    };
                    if (clickElement == null)
                        continue;
                    //click on element 
                    clickElement.Click();
                        
                    //wait 1 second to give html page some times to load AJAX
                    Thread.Sleep(1000);
                    //extract resources using xpath
                    foreach (XPathItem pathItem in clickContainer.PathItems)
                    {
                        string extractionResult;
                        //collection resources with straight xpath 
                        try
                        {
                            extractionResult = driver.FindElement(By.XPath(pathItem.XPath)).Text;

                        }
                        catch (Exception e)
                        {
                            extractionResult = "مشکل در پیدا کردن منبع";
                            throw;
                        }
                        //check if extractionResult have any definition of color ,
                        //if it does , set the extraction result as PriceView Color
                        if (extractionResult.Contains("رنگ") || pathItem.XPathTag == "رنگ")
                        {
                            newPriceView.Color = extractionResult.Replace("رنگ",string.Empty).Replace(":",string.Empty).Trim();
                        }
                        //create tagView object and add it to the tagList
                        else
                        {
                            TagView newTag = new() { TagName = pathItem.XPathTag, TagValue = extractionResult };
                            newPriceView.Tags.Add(newTag);
                        }
                    }
                    //check if click container should collect any list containers
                    if (listContainers != null && listContainers.Any())
                    {
                        //loop throught each row of the list and collect resources
                        foreach (ContainerXPath listContainer in listContainers)
                        {
                            if (listContainer == null || listContainer.PathItems.Count == 0)
                                continue;
                            //extract container elements
                            //check if containerpath is valid
                            var checkResult = scripter.ExecuteScript($"return document.evaluate(\"{listContainer.ContainerPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null);
                            //check if CheckResult is Valid
                            if (checkResult is not IWebElement)
                                continue;
                            //get rows from ListContainer Object
                            var scrapResultElements = driver.FindElements(By.XPath(listContainer.ContainerPath));
                            //loop through each row
                            foreach (IWebElement scrapElement in scrapResultElements)
                            {
                                //open up xpathitems book
                                foreach (XPathItem pathItem in listContainer.PathItems)
                                {
                                    //extract value from scrapResultElements
                                    try
                                    {
                                        //try to find element inside a row 
                                        var listScrapResult = scrapElement.FindElement(By.XPath($".{pathItem.XPath}"));
                                        //if element founded
                                        if (listScrapResult != null)
                                        {
                                            //create new tag and add it to the collection
                                            TagView newTag = new() { TagName = pathItem.XPathTag, TagValue = listScrapResult.Text };
                                            newPriceView.Tags.Add(newTag);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        //if any error occured , skip error and go for next item
                                        continue;
                                    }
                                }
                            }
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
                    foreach (ContainerXPath listContainer in listContainers)
                    {
                        if (listContainer == null || listContainer.PathItems.Count == 0)
                            continue;
                        //extract container elements
                        //check if containerpath is valid
                        var checkResult = scripter.ExecuteScript($"return document.evaluate(\"{listContainer.ContainerPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null);
                        if (checkResult is not IWebElement)
                            continue;
                        var scrapResultElements = driver.FindElements(By.XPath(listContainer.ContainerPath));
                        foreach (IWebElement scrapElement in scrapResultElements)
                        {
                            ArticleDetails articleDetails = new()
                            {
                                Provider = provider,
                                Color = "",
                            };
                            foreach (XPathItem pathItem in listContainer.PathItems)
                            {
                                //extract value from scrapResultElements
                                try
                                {
                                    var listScrapResult = scrapElement.FindElement(By.XPath($".{pathItem.XPath}"));
                                    if (listScrapResult.Text.Contains("رنگ") || pathItem.XPathTag == "رنگ")
                                    {
                                        articleDetails.Color = listScrapResult.Text.Replace("رنگ", string.Empty).Replace(":", string.Empty).Trim();
                                    }
                                    //create tagView object and add it to the tagList
                                    else
                                    {
                                        TagView newTag = new() { TagName = pathItem.XPathTag, TagValue = listScrapResult.Text };
                                        articleDetails.Tags.Add(newTag);
                                    }
                                }
                                catch (Exception e)
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
        }
        private List<ArticleDetails> ScrapExcel(Provider provider)
        {
            //create new priceView Object for valid return value
            ArticleDetails priceViewObject = new();
            throw new NotImplementedException();
        }
        private List<ArticleDetails> ScrapImage(Provider provider)
        {
            //create new priceView Object for valid return value
            ArticleDetails priceViewObject = new();
            throw new NotImplementedException();
        }
        private List<ArticleDetails> ReturnError(string errMessage, WebDriver driver)
        {
            return [new ArticleDetails() { IsError = true, ErrorDescription = errMessage }];
        }
        public List<ArticleDetails> Scrap(WebDriver driver)
        {
            //try to get provider object
            Provider? providerObject = GetProvider();
            // if couldn't find provider object , return with error
            if (providerObject == null) return ReturnError("مشکل در پیدا کردن تامین کننده مربوط به کالا", driver);

            return providerObject.Extraction switch
            {
                Types.Enum.ExtractionTypes.Scrap => ScrapWeb(driver, providerObject),
                Types.Enum.ExtractionTypes.Excel => ScrapExcel(providerObject),
                Types.Enum.ExtractionTypes.Image => ScrapImage(providerObject),
                _ => ReturnError("مشکل در تشخیص نوع استخراج", driver),
            };
        }
        public string GetProviderName()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Provider>(nameof(Provider));
            var result = tb.List.FirstOrDefault(x => x.ElementSeed == ProviderID);
            if (result == null)
                return "";
            return result.Name;
        }
        public Provider? GetProvider()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Provider>(nameof(Provider));
            return tb.List.FirstOrDefault(x => x.ElementSeed == ProviderID);
        }
        public Article? GetArticle()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Article>(nameof(Article));
            return tb.List.FirstOrDefault(x => x.ElementSeed == ArticleID);
        }
        public string GetArticleName()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Article>(nameof(Article));
            var result = tb.List.FirstOrDefault(x => x.ElementSeed == ArticleID);
            if (result == null)
                return "";
            return result.Name;
        }
        public ColorCollection GetArticleColorFromName()
        {
            var name = GetArticleName();
            ColorCollection colorIns = new ColorCollection(name);
            return colorIns;
        }
        public IXmlItem CreateObject()
        {
            return this;
        }
        public string GenerateIdentifier()
        {
            //propertyHash;
            return "";
        }
    }
}
