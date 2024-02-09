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
        public static List<ArticleGroupView> ArticleGroups { get; set; } = [];
        /// <summary>
        /// fetch article list from vetos website
        /// </summary>
        public static void PullArticleList()
        {
            var articles = APIDataStorage.ArticleManager.List.ToList();
            ArticleGroups = articles.GroupBy(x => x.ID).Select((x) =>
            {
                var newArticleView = new ArticleGroupView()
                {
                    ID = x.Key,
                };
                var extractedArticle = articles.FirstOrDefault(y => y.ID == x.Key);
                newArticleView.Name = extractedArticle == null ? string.Empty : extractedArticle.ArticleName;
                newArticleView.Colors = x.Where(y => y.ID == x.Key).Select((y) =>
                {
                    var extractedColor = new ArticleColorView()
                    {
                        ColorMetaID = y.ColorMetaID,
                        ColorName = y.ColorName,
                        PriceMetaID = y.PriceMetaID,
                        PriceValue = y.PriceValue,
                    };
                    return extractedColor;
                }).ToList();
                return newArticleView;
            }).ToList();
        }
    }
}
