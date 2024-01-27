namespace PriceSetterDesktop.Libraries.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
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
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; }
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; }
        public int ElementSeed { get; set; }

        public double GetPriceFromWeb()
        {
            return 0;
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
