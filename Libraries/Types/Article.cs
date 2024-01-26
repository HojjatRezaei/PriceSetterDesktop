namespace PriceSetterDesktop.Libraries.Types
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Base;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    using WPFCollection.Data.List;

    [XmlMarker(nameof(Article))]
    public class Article : BaseFileType<Article>, IXmlItem
    {
        public Article()
        {
            
        }
        [XmlItem(nameof(Name), "string")]
        public string Name { get; set; }
        [XmlItem(nameof(URLID), "int")]
        public int URLID { get; set; }
        [XmlItem(nameof(PricesID), "int")]
        public int PricesID { get; set; }
        public IEnumerable<Prices> Prices
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Prices>(nameof(Prices));
                return tb.List.Where(x => x.ArticleID == ElementSeed);
            }
        }
    }
}
