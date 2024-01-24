namespace PriceSetterDesktop.Libraries.Types
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Base;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;

    [XmlMarker(nameof(URLScrap))]
    public class URLScrap : BaseFileType<Article>, IXmlItem
    {
        public URLScrap()
        {
            
        }
        [XmlItem(nameof(ArticleID) , "int")]
        public int ArticleID { get; set; } = 0;
        [XmlItem(nameof(URL), "string")]
        public string URL { get; set; } = "";
        [XmlItem(nameof(XPath), "string")]
        public string XPath { get; set; } = "";
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; }
        public Provider? Provider { 
            get 
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Provider>(nameof(Provider));
                return tb.List.FirstOrDefault(x => x.ElementSeed == ProviderID);
            } }
    }
}
