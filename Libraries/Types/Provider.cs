namespace PriceSetterDesktop.Libraries.Types
{
    using PriceSetterDesktop.Libraries.Types.Base;
    using System.Xml;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;

    [XmlMarker(nameof(Provider))]
    public class Provider : BaseFileType<Article>, IXmlItem
    {
        public Provider()
        {
            
        }
        [XmlItem(nameof(ID) , "int")]
        public int ID { get; set; } = -1;
        [XmlItem(nameof(Name), "string")]
        public string Name { get; set; } = "";
    }
}
