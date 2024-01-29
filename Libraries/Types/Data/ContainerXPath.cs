namespace PriceSetterDesktop.Libraries.Types.Data
{
    using System;
    using System.Xml;
    using PriceSetterDesktop.Libraries.Types.Enum;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    [XmlMarker(nameof(ContainerXPath))]
    public class ContainerXPath : IGeneratable, IXmlItem
    {
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; } = -1;
        [XmlItem(nameof(ContainerPath), "string")]
        public string ContainerPath { get; set; } = "";
        [XmlItem(nameof(ContainerType), "int")]
        public ContainerType ContainerType { get; set; } = 0;
        public List<XPathItem> PathItems { get; set; } = [];
        public int ElementSeed { get; set; }

        public IXmlItem CreateObject()
        {
            return this;
        }
        public IXmlItem CreateObjectFromNode(XmlNodeList nodeList, int seed)
        {
            var newObject = new Provider();
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
