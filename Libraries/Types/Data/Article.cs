namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium.DevTools.V119.DOM;
    using PriceSetterDesktop.Libraries.Statics;
    using WPFCollection.Data.Interface.Generic;

    public partial class Article : IJsonConverter<Article>
    {
        public Article()
        {

        }
        public int ID { get; set; } = -1;
        public string ArticleName { get; set; } = string.Empty;
        public int ColorMetaID { get; set; } = -1;
        public string ColorName { get; set; } = string.Empty;
        public int PriceID { get; set; } = -1;
        public int RegularPriceID { get; set; } = -1;
        public int OptionID { get; set; } = -1;
        public string OptionValue { get; set; } = string.Empty;

        public Article ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ArticleID");
            ArticleName = jObjectItem.Value<string>("ArticleName") ?? "";
            ColorMetaID = jObjectItem.Value<int>("ColorID");
            ColorName = jObjectItem.Value<string>("ColorName") ?? "";
            PriceID = jObjectItem.Value<int>("PriceID");
            RegularPriceID = jObjectItem.Value<int>("RegularPriceID");
            OptionID = jObjectItem.Value<int>("OptionID");
            OptionValue = jObjectItem.Value<string>("OptionValue") ?? "";
            return this;
        }

        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(ArticleName), ArticleName },
                { nameof(ColorMetaID), ColorMetaID },
                { nameof(ColorName), ColorName },
                { nameof(PriceID), PriceID },
                { nameof(RegularPriceID), RegularPriceID },
                { nameof(OptionID), OptionID },
                { nameof(OptionValue), OptionValue },
            };
            return jobject;
        }

        public bool IsValidData()
        {
            return true;
        }
    }
}
