namespace PriceSetterDesktop.Libraries.Statics
{
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using WPFCollection.Data.Statics;
    public static class DataHolder
    {
        public static XmlManager XMLData { get; set; } = new();
        public static string XMLDataBaseName { get; set; } = "appDataXML";
        /// <summary>
        /// back code generated
        /// </summary>
        public static List<ReadableArticle> ReadableArticleList { get; set; } = [];
        /// <summary>
        /// fetch article list from vetos website
        /// </summary>
        public static void UpdateReadableArticleList()
        {
            var articles = APIDataStorage.ArticleManager.List.ToList();
            ReadableArticleList = articles.GroupBy(x => x.ID).Select((x) =>
            {
                var newArticleView = new ReadableArticle()
                {
                    ID = x.Key,
                };
                var extractedArticle = articles.FirstOrDefault(y => y.ID == x.Key);
                newArticleView.Name = extractedArticle == null ? string.Empty : extractedArticle.ArticleName;
                newArticleView.Colors = x.Where(y => y.ID == x.Key).Select((y) =>
                {
                    var extractedColor = new ColorView()
                    {
                        ID = y.ColorMetaID,
                        Name = y.ColorName,
                        PriceMetaID = y.PriceMetaID,
                    };
                    return extractedColor;
                }).ToList();
                return newArticleView;
            }).ToList();
        }
    }
}
