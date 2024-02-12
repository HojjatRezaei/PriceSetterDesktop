namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using System.ComponentModel;
    using WPFCollection.Data.Interface.Generic;
    using WPFCollection.Data.Types;

    public class ScrapResult : IJsonConverter<ScrapResult>
    {
        public ScrapResult()
        {
            
        }
        public int ID { get; set; }
        public int ArticleID { get; set; } = -1;
        public string ArticleName 
        { 
            get
            {
                var searchResult = GetArticle();
                return searchResult == null? "" : searchResult.ArticleName;
            } 
        }
        public int ProviderID { get; set; } = -1;
        public int ColorID { get; set; } = -1;
        public string ColorName { get; set; } = string.Empty;
        public double Price { get; set; } = 0;
        public PersianDate Date { get; set; } = PersianDate.Now;
        public string Time { get; set; } = "";
        public int PriceID { get; set; } = -1;
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
        public ScrapResult ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            Date = jObjectItem.Value<string>("Date") ?? "";
            Time = jObjectItem.Value<string>("Time") ?? "";
            Price = jObjectItem.Value<double>("Price");
            ArticleID = jObjectItem.Value<int>("ArticleID");
            ProviderID = jObjectItem.Value<int>("ProviderID");
            Source = jObjectItem.Value<string>("Source") ?? string.Empty;
            ColorID = jObjectItem.Value<int>("ColorID");
            PriceID = jObjectItem.Value<int>("PriceID");
            return this;
        }
        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(ProviderID), ProviderID },
                { nameof(Date), Date.ToString() },
                { nameof(Time), Time },
                { nameof(Price), Price },
                { nameof(ArticleID), ArticleID },
                { nameof(ProviderID), ProviderID },
                { nameof(Source), Source },
                { nameof(ColorID), ColorID },
                { nameof(PriceID), PriceID },
            };
            return jobject;
        }
        public bool IsValidData()
        {
            return true;
        }
    }
}
