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
        /// web generated
        /// </summary>
        public static List<Article> Articles { get; set; } = [];
        /// <summary>
        /// back code generated
        /// </summary>
        public static List<ArticleGroupView> ArticleGroups { get; set; } = [];
        public static void PullDataFromWeb()
        {
            //fetch article list using api
            PullArticleList();
        }
        public static void PushDataToWeb()
        {
            //extract stored data in xml tables and push it to the web using api 

            //fetch and push Container Items

            //fetch and push xpath items

            //fetch and push provider items

            //fetch and push url list
        }
        /// <summary>
        /// fetch article list from vetos website
        /// </summary>
        public static void PullArticleList()
        {
            Articles.Clear();
            Articles = HTTPUtility.SendGETRequest<Article>("https://vetos-mobile.com/hojjatDebugTest/api/fetch/vetosdb/GetArticleList");
            ArticleGroups = Articles.GroupBy(x => x.ArticleID).Select((x) =>
            {
                var newArticleView = new ArticleGroupView()
                {
                    ID = x.Key,
                };
                var extractedArticle = Articles.FirstOrDefault(y => y.ArticleID == x.Key);
                newArticleView.Name = extractedArticle == null ? string.Empty : extractedArticle.ArticleName;
                newArticleView.Colors = x.Where(y => y.ArticleID == x.Key).Select((y) =>
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
