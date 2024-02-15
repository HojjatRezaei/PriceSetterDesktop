namespace PriceSetterDesktop.Libraries.Statics
{
    using PriceSetterDesktop.Libraries.APIManager;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;

    public static class APIDataStorage
    {
        public static APIManager<Article> ArticleManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/test/ArticleManager", false);
        public static APIManager<Container> ContainerManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/test/ContainerManager", false);
        public static APIManager<Provider> ProviderManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/test/ProviderManager", false);
        public static APIManager<Url> UrlManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/test/URLManager", false);
        public static APIManager<PathItem> PathManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/test/PathItemManager", false);
        public static APIManager<LoginInfo> LoginManager { get; set; } = new("", false);
        public static APIManager<ScrapResult> ScrapManager { get; set; } = new("https://vetos-mobile.com/hojjatDebugTest/api/test/ScrapManager", false);
    }
}
