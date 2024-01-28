namespace PriceSetterDesktop.Libraries.Types
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using PriceSetterDesktop.Libraries.Statics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Serialization;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    using WPFCollection.Data.Statics;
    using WPFCollection.Network.Error;

    [XmlMarker(nameof(URLType))]
    public partial class URLType : IGeneratable, IXmlItem
    {
        public URLType()
        {
            
        }
        [XmlItem(nameof(URL), "string")]
        public string URL { get; set; }
        [XmlItem(nameof(XPath), "string")]
        public string XPath { get; set; }
        [XmlItem(nameof(ClickPath) , "string")]
        public string ClickPath { get; set; }
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; }
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; }
        public int ElementSeed { get; set; }

        public double GetPriceFromWeb(WebDriver driver)
        {
            if (URL == string.Empty || XPath == string.Empty)
                return 0;
            IWebElement? clickElement=null;
            IWebElement? price=null;
            driver.Navigate().GoToUrl(URL);
            //section of waiting for DOM TO load currectly 
            WebDriverWait wait = new(driver, new(0, 0, 20));
            
            IJavaScriptExecutor scripter = driver;
            try
            {
                _ = wait.Until(x => scripter.ExecuteScript($"return document.evaluate(\"{XPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null);
            }
            catch (Exception e)
            {
                ErrorManager.SendError(e);
                return -1;
                throw;
            }
            if (!string.IsNullOrEmpty(ClickPath))
            {
                _ = wait.Until(x => scripter.ExecuteScript($"return document.evaluate(\"{ClickPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null);
            }
            try
            {
                if(!string.IsNullOrEmpty(ClickPath))
                    clickElement = driver.FindElement(By.XPath(ClickPath));
                if (!string.IsNullOrEmpty(XPath))
                    price = driver.FindElement(By.XPath(XPath));
            }
            catch (Exception e)
            {
                ErrorManager.SendError(e);
                return -1;
            }
            clickElement?.Click();
            Thread.Sleep(1000);
            try
            {
                if (price == null)
                    return -1;
                //replace persian number with english numbers
                var ConvertableTxt = price.Text.CorrectPersianNumber().RemoveWords();
                _ = double.TryParse(ConvertableTxt, out double foundedPrice);
                return foundedPrice;
            }
            catch (Exception e)
            {
                ErrorManager.SendError(e);
                return -1;
            }
        }
        public string GetProviderName()
        {
            var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            var tb = db.GetTable<Provider>(nameof(Provider));
            var result = tb.List.FirstOrDefault(x => x.ElementSeed == ProviderID);
            if(result == null)
                return "";
            return result.Name;
        }
        public IXmlItem CreateObject()
        {
            return (IXmlItem)this;
        }

        public IXmlItem CreateObjectFromNode(XmlNodeList nodeList, int seed)
        {
            var newObject = new URLType();
            foreach (XmlNode node in nodeList)
            {
                var searchProperty = newObject.GetType().GetProperty(node.Name);
                if (searchProperty != null)
                {
                    var newVal = Convert.ChangeType(node.InnerText, searchProperty.PropertyType);
                    searchProperty.SetValue(newObject, newVal);
                }
            }
            ElementSeed = seed;
            return newObject;
        }
        public string GenerateIdentifier()
        {
            //propertyHash;
            return "";
        }
        public override bool Equals(object? obj)
        {
            return obj is URLType newObject &&
                   URL == newObject.URL &&
                   XPath == newObject.XPath &&
                   ProviderID == newObject.ProviderID &&
                   ArticleID == newObject.ArticleID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(URL, XPath, ProviderID, ArticleID);
        }
        public override string ToString()
        {
            return $"{URL} ==> {XPath}";
        }
    }
}
