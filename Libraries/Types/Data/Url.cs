namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Support.UI;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WPFCollection.Data.Interface.Generic;

    public partial class Url : IJsonConverter<Url>
    {
        public int ID { get; set; } = -1;
        public string URL { get; set; } = "";
        public int ArticleID { get; set; } = -1;
        public int ProviderID { get; set; } = -1;
        public int LoginInfoID { get; set; } = -1;
        public Provider? GetProvider()
        {
            return APIDataStorage.ProviderManager.List.FirstOrDefault(x => x.ID == ProviderID);
        }
        public string GetArticleName()
        {
            var result = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == ArticleID);
            return result == null ? "" : result.ArticleName;
        }
        public Article? GetArticle()
        {
            return APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == ArticleID);
        }
        public Url ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            URL = jObjectItem.Value<string>("URL") ?? string.Empty;
            ArticleID = jObjectItem.Value<int>("ArticleID");
            ProviderID = jObjectItem.Value<int>("ProviderID");
            return this;
        }
        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(URL), URL },
                { nameof(ArticleID), ArticleID },
                { nameof(ProviderID), ProviderID },
            };
            return jobject;
        }
        public bool IsValidData()
        {
            return true;
        }
    }
}
