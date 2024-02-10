namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using WPFCollection.Data.Interface.Generic;

    public class PathItem : IJsonConverter<PathItem>
    {
        public PathItem()
        {

        }
        public int ID { get; set; } = -1;
        public int ContainerID { get; set; }
        public string Path { get; set; } = "";
        public string PathTag { get; set; } = "";
        public PathItem ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            ContainerID = jObjectItem.Value<int>("ContainerID");
            Path = jObjectItem.Value<string>("Path") ?? "";
            PathTag = jObjectItem.Value<string>("PathTag") ?? "";
            return this;
        }
        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(ContainerID), ContainerID },
                { nameof(Path), Path },
                { nameof(PathTag), PathTag },
            };
            return jobject;
        }
    }
}
