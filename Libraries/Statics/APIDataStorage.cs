namespace PriceSetterDesktop.Libraries.Statics
{
    using PriceSetterDesktop.Libraries.APIManager;
    using PriceSetterDesktop.Libraries.Types.Data;

    public static class APIDataStorage
    {
        public static APIManager<Article> ArticleManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/fetch/vetosdb/GetArticleList", true);
        public static APIManager<Container> ContainerManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/fetch/hojjatdb/ContainerManager", false);
        public static APIManager<Provider> ProviderManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/fetch/hojjatdb/ProviderManager", false);
        public static APIManager<Url> UrlManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/fetch/hojjatdb/URLManager", false);
        public static APIManager<PathItem> PathManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/fetch/hojjatdb/PathItemManager", false);
    }
}
