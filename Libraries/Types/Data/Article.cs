namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
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
        public int PriceMetaID { get; set; } = -1;
        public double PriceValue { get; set; } = -1;
        public Article ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ArticleID");
            ArticleName = jObjectItem.Value<string>("ArticleName") ?? "";
            ColorMetaID = jObjectItem.Value<int>("ColorID");
            ColorName = jObjectItem.Value<string>("ColorName") ?? "";
            PriceMetaID = jObjectItem.Value<int>("PriceMetaID");
            PriceValue = double.TryParse(jObjectItem.Value<string>("PriceValue") ?? "", out double castedPrice) ? castedPrice : 0;
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
                { nameof(PriceMetaID), PriceMetaID },
                { nameof(PriceValue), PriceValue }
            };
            return jobject;
        }

        public bool IsValidData()
        {
            return true;
        }
    }
}
