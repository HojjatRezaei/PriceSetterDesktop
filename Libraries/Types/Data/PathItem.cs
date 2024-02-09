namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using WPFCollection.Data.Interface.Generic;

    public class PathItem : IJsonConverter<PathItem>
    {
        public PathItem()
        {

        }
        public int ContainerID { get; set; }
        public string Path { get; set; } = "";
        public string PathTag { get; set; } = "";
        public int ID { get; set; } = -1;
        public PathItem ConvertFromJson(JToken jObjectItem)
        {
            ContainerID = jObjectItem.Value<int>("ContainerID");
            Path = jObjectItem.Value<string>("Path") ?? "";
            PathTag = jObjectItem.Value<string>("PathTag") ?? "";
            ID = jObjectItem.Value<int>("ID");
            return this;
        }
        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ContainerID), ContainerID },
                { nameof(Path), Path },
                { nameof(PathTag), PathTag },
                { nameof(ID), ID }
            };
            return jobject;
        }
    }
}
