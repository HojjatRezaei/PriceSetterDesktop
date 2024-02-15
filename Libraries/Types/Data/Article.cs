namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using WPFCollection.Data.Interface.Generic;

    public partial class Article : IJsonConverter<Article>
    {
        public Article()
        {

        }
        public int ID { get; set; } = -1;
        public string ArticleName { get; set; } = string.Empty;
        public int ColorID { get; set; } = -1;
        public string ColorName { get; set; } = string.Empty;
        public int PriceID { get; set; } = -1;
        public double Price { get; set; } = 0;
        public int RegularPriceID { get; set; } = -1;
        public double RegularPrice { get; set; } = 0;
        public int OptionID { get; set; } = -1;
        public string OptionValue { get; set; } = string.Empty;
        public JObject? OptionValueJson => OptionValue != string.Empty ? JObject.Parse(OptionValue) : null;
        public bool HaveVariable => ColorID != -1;
        public int RequestType = -1;
        public Article ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ArticleID");
            ArticleName = jObjectItem.Value<string>("ArticleName") ?? "";
            ColorID = jObjectItem.Value<int>("ColorID");
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
                { nameof(ColorID), ColorID },
                { nameof(ColorName), ColorName },
                { nameof(PriceID), PriceID },
                { nameof(Price), Price },
                { nameof(RegularPriceID), RegularPriceID },
                { nameof(RegularPrice), RegularPrice },
                { nameof(OptionID), OptionID },
                { nameof(OptionValue), OptionValue },
                { nameof(RequestType), RequestType },
            };
            return jobject;
        }
        public bool IsValidData()
        {
            return true;
        }
    }
}
