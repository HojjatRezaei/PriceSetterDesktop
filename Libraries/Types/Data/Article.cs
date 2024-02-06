namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using System.Windows.Forms;
    using WPFCollection.Data.Interface.Generic;

    public partial class Article : IJsonConverter<Article>
    {
        public Article()
        {
            
        }
        public int ArticleID { get; set; }
        public string ArticleName { get; set; }
        public int ColorMetaID { get; set; }
        public string ColorName { get; set; }
        public int PriceMetaID { get; set; }
        public double PriceValue { get; set; }

        public Article ConvertFromJson(JToken jObjectItem)
        {
            ArticleID = jObjectItem.Value<int>("ArticleID");
            ArticleName = jObjectItem.Value<string>("ArticleName") ?? "";
            ColorMetaID = jObjectItem.Value<int>("ColorID");
            ColorName = jObjectItem.Value<string>("ColorName") ?? "";
            PriceMetaID = jObjectItem.Value<int>("PriceMetaID");
            PriceValue = double.TryParse(jObjectItem.Value<string>("PriceValue") ?? "", out double castedPrice) ? castedPrice : 0;
            return this;
        }
    }
}
