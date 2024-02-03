namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types.Generic;

    public class ArticleView
    {
        public string ArticleName { get; set; } = string.Empty;
        public string ArticleColor { get; set; } = string.Empty;
        public string ArticleDispaly 
        {
            get
            {
                return ArticleName + " " + ArticleColor;    
            }
        }
        public List<ProviderView> Providers { get; set; } = [];
        public string GetString()
        {
            string finalString = string.Empty;
            string articleName = ArticleName;
            string articleColor = ArticleColor;
            foreach (ProviderView provider in Providers)
            {
                foreach (TagView tag in provider.ScrappedData)
                {
                    if(tag.TagName == "قیمت")
                    {
                        finalString += $"{articleName};{articleColor};{provider.ProviderName};{tag.TagValue}\n";
                    }
                }
            }
            return finalString;
        }
        public List<string> ToJson()
        {
            var list = new List<string>();
            string articleName = ArticleName;
            string articleColor = ArticleColor;
            foreach (ProviderView provider in Providers)
            {
                foreach (TagView tag in provider.ScrappedData)
                {
                    double price = SGenerator.GenerateRandomDoubleValue(9);
                    list.Add($"ArticleName=\"{articleName}\",\nArticleColor=\"{articleColor}\",\nProviderName=\"{provider.ProviderName}\",\nAnnouncedPrice=\"{price:#,##0}\"\n");
                }
            }
            return list;
        }
    }
}
