namespace PriceSetterDesktop.Libraries.Types.Data
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Interaction;
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
        public string URL { get; set; } = "";
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; } = -1;
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; } = -1;
        public int ElementSeed { get; set; } = -1;

        public string GetPriceFromWeb(WebDriver driver)
        {
            throw new NotImplementedException();
            //if (URL == string.Empty || XPathCollection == null)
            //    return "عدم اطلاعات کافی";
            //IWebElement? clickElement=null;
            //IWebElement? resourceElement=null;
            //driver.Navigate().GoToUrl(URL);
            ////section of waiting for DOM TO load currectly 
            //WebDriverWait wait = new(driver, new(0, 0, 20));

            //IJavaScriptExecutor scripter = driver;
            //try
            //{
            //    _ = wait.Until(x => scripter.ExecuteScript($"return document.evaluate(\"{XPathCollection[0].XPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null);
            //}
            //catch (Exception e)
            //{
            //    ErrorManager.SendError(e);
            //    return $"مشکل در سایت {GetProviderName()}";
            //}
            //if (!string.IsNullOrEmpty(ColorContainerXPath))
            //{

            //    try
            //    {
            //        _ = wait.Until(x => scripter.ExecuteScript($"return document.evaluate(\"{ColorContainerXPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null);
            //        ////try to find color from article name
            //        //var articleName = GetArticleName();
            //        //var articleColor = GetArticleColorFromName();
            //        //if (string.IsNullOrEmpty(articleName))
            //        //{
            //        //    _ = wait.Until(x => scripter.ExecuteScript($"return document.evaluate(\"{ClickPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null);
            //        //}
            //        //else
            //        //{
            //        //    _ = wait.Until(x => scripter.ExecuteScript($"return document.evaluate(\"{ClickPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) != null);
            //        //    //مشکی ، سفید ، صورتی ، سبز ، آبی
            //        //    //extract button color
            //        //    var colorCode = scripter.ExecuteScript($"document.evaluate(\"{ClickPath}\",document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue.style['background-color']");
            //        //    if(colorCode == null)
            //        //    {
            //        //        return "مشکل در پیدا کردن رنگ مربوطه";
            //        //    }
            //        //    else
            //        //    {
            //        //        //detect color

            //        //    }
            //        //}

            //    }
            //    catch (Exception e)
            //    {
            //        ErrorManager.SendError(e);
            //        return "مشکل در پیدا کردن رنگ مربوطه";
            //    }

            //}
            //clickElement?.Click();
            //Thread.Sleep(1000);
            //try
            //{

            //    if (resourceElement == null)
            //        return "مشکل در پیدا کردن منبع قیمت";
            //    //replace persian number with english numbers
            //    var ConvertableTxt = resourceElement.Text.CorrectPersianNumber().RemoveWords();
            //    _ = double.TryParse(ConvertableTxt, out double foundedPrice);
            //    return $"{foundedPrice}";
            //}
            //catch (Exception e)
            //{
            //    ErrorManager.SendError(e);
            //    return "مشکل در پیدا کردن قیمت در سایت";
            //}
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
        public IXmlItem CreateObjectFromNode(XmlNodeList nodeList, int seed)
        {
            var newObject = this;
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
    }
}
