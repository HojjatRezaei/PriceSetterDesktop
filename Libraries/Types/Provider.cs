namespace PriceSetterDesktop.Libraries.Types
{
    using PriceSetterDesktop.Libraries.Statics;
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
        [XmlItem(nameof(Name), "string")]
        public string Name { get; set; } = "";
        [XmlItem(nameof(PricesID), "int")]
        public int PricesID { get; set; }
        public IEnumerable<Prices> Prices
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Prices>(nameof(Prices));
                return tb.List.Where(x => x.ProviderID == ElementSeed);
            }
        }
    }
}
