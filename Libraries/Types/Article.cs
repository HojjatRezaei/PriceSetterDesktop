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
        public string Name { get; set; } = "";
        public int ProviderSeed { get; set; }
        public ViewCollection<URLScrap> URLScrapList { get 
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<URLScrap>(nameof(URLScrap));
                List<URLScrap> data = tb.List.Where(x => x.ArticleID == ElementSeed).ToList();
                return data;
            } } 
        public override string ToString()
        {
            return $"{Name}";
        }
        public void 
    }
}
