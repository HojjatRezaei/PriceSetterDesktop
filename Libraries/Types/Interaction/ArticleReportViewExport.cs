namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types.Generic;

    public class ArticleReportViewExport
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
    }
}
