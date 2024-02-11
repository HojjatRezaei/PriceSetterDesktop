namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;

    public class ScrapResult
    {
        public ScrapResult()
        {
            
        }
        public int ArticleID { get; set; } = -1;
        public string ArticleName 
        { 
            get
            {
                var searchResult = GetArticle();
                return searchResult == null? string.Empty : searchResult.ArticleName;
            } 
        }
        public int ProviderID { get; set; } = -1;
        public int ColorID { get; set; } = -1;
        public string ColorName { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public int PriceID { get; set; } = -1;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Source { get; set; } = string.Empty;
        public bool HaveMessage { get; set; }
        public string Messages { get; set; } = string.Empty;
        public bool ValidData => !(ColorID != -1 && PriceID != -1);
        public Article? GetArticle()
        {
            return APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == ArticleID);
        }
        public Provider? GetProvider()
        {
            return APIDataStorage.ProviderManager.List.FirstOrDefault(x => x.ID == ProviderID);
        }
    }
}
