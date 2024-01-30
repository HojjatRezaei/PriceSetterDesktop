namespace PriceSetterDesktop.Libraries.Types.Data
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
        [XmlItem(nameof(ContainerID), "int")]
        public int ContainerID { get; set; }
        [XmlItem(nameof(XPath), "string")]
        public string XPath { get; set; } = "";
        [XmlItem(nameof(XPathTag), "string")]
        public string XPathTag { get; set; } = "";
        public int ElementSeed { get; set; } = -1;

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
