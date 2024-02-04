namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using WPFCollection.Data.Interface.Generic;

    public partial class Article : IJsonConverter<Article>
    {
        public Article()
        {
            
        }
        public int ID { get; set; }
        public string ArticleName { get; set; }

        public Article ConvertFromJson(JToken jObjectItem)
        {
            if (jObjectItem == null)
                return this;
            //extract values from passed jobjectitem
            try
            {
                ID = jObjectItem.Value<int>("ID");
            }
            catch (Exception){}
            try
            {
                var artName = jObjectItem.Value<string>("post_title");
                if (artName != null)
                    ArticleName = artName;
            }
            catch (Exception){}
            return this;
        }
    }
}
