namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WPFCollection.Data.Interface.Generic;

    public partial class Url : IJsonConverter<Url>
    {
        public int ID { get; set; } = -1;
        public string URL { get; set; } = "";
        public int ArticleID { get; set; } = -1;
        public int ProviderID { get; set; } = -1;
        private string CleanString(string txt)
        {
            return "";
        }
        private List<ArticleDetails> ScrapWeb(WebDriver driver, Provider provider)
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
                    if(adDetector is IWebElement adCloseButton)
                    {
                        adCloseButton.Click();
                    }
                    //click on element 
                    clickElement.Click();
                    //find next click element and loop through it 
                    var clickAndContinue = provider.Containers.Where(x => x.Type == Types.Enum.ContainerType.ClickAndContinue).FirstOrDefault();
                    if(clickAndContinue != null)
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
                    //check if click container should collect any list containers
                    if (listContainers != null && listContainers.Any())
                    {
                        //loop throught each row of the list and collect resources
                        foreach (Container listContainer in listContainers)
                        {
                            if (listContainer == null || listContainer.PathItems.Count == 0)
                                continue;
                            //extract container elements
                            //check if containerpath is valid
                            var checkResult = scripter.ExecuteScript($"return document.evaluate(\"{listContainer.Path}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null);
                            //check if CheckResult is Valid
                            if (checkResult is not IWebElement)
                                continue;
                            //get rows from ListContainer Object
                            var scrapResultElements = driver.FindElements(By.XPath(listContainer.Path));
                            //loop through each row
                            foreach (IWebElement scrapElement in scrapResultElements)
                            {
                                //open up xpathitems book
                                foreach (PathItem pathItem in listContainer.PathItems)
                                {
                                    //extract value from scrapResultElements
                                    try
                                    {
                                        //try to find element inside a row 
                                        var listScrapResult = scrapElement.FindElement(By.XPath($".{pathItem.Path}"));
                                        //if element founded
                                        if (listScrapResult != null)
                                        {
                                            //create new tag and add it to the collection
                                            TagView newTag = new() { TagName = pathItem.PathTag, TagValue = listScrapResult.Text };
                                            newPriceView.Tags.Add(newTag);
                                        }
                                    }
                                    catch (Exception)
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
            return providerObject == null
                ? ReturnError("مشکل در پیدا کردن تامین کننده مربوط به کالا", driver)
                : providerObject.Extraction switch
                {
                    Types.Enum.ExtractionTypes.Scrap => ScrapWeb(driver, providerObject),
                    Types.Enum.ExtractionTypes.Excel => ScrapExcel(providerObject),
                    Types.Enum.ExtractionTypes.Image => ScrapImage(providerObject),
                    _ => ReturnError("مشکل در تشخیص نوع استخراج", driver),
                };
        }
        public Provider? GetProvider()
        {
            var result = APIDataStorage.ProviderManager.List.FirstOrDefault(x => x.ID == ProviderID);
            return result;
        }
        public string GetArticleName()
        {
            var result = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == ArticleID);
            return result == null ? "" : result.ArticleName;
        }
        public Url ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            URL = jObjectItem.Value<string>("URL") ?? string.Empty;
            ArticleID = jObjectItem.Value<int>("ArticleID");
            ProviderID = jObjectItem.Value<int>("ProviderID");
            return this;
        }
        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(URL), URL },
                { nameof(ArticleID), ArticleID },
                { nameof(ProviderID), ProviderID },
            };
            return jobject;
        }
    }
}
