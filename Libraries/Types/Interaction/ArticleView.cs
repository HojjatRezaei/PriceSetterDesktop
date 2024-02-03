﻿namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using WPFCollection.Data.Interface;

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
    }
}
