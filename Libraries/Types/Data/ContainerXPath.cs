namespace PriceSetterDesktop.Libraries.Types.Data
{
    using System;
    using System.Xml;
    using PriceSetterDesktop.Libraries.Statics;
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
        public List<XPathItem> PathItems
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<XPathItem>(nameof(XPathItem));
                return tb.List.Where(x => x.ContainerID == ElementSeed).ToList();
            }
        }
        public int ElementSeed { get; set; }

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
