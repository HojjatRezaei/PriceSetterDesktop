namespace PriceSetterDesktop.Libraries.Types
{
    using System;
    using System.Security.Policy;
    using System.Xml;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    [XmlMarker(nameof(XPathItem))]
    public class XPathItem : IGeneratable, IXmlItem
    {
        public XPathItem()
        {
            
        }
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; } = -1;
        [XmlItem(nameof(XPath), "string")]
        public string XPath { get; set; } = "";
        [XmlItem(nameof(XpathType), "bool")]
        public string XpathType { get; set; } = "";
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; } = -1;
        public int ElementSeed { get; set; } = -1;

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
    }
}
