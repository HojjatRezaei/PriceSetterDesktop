namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Enum;
    using WPFCollection.Data.Interface.Generic;

    public class Container : IJsonConverter<Container>
    {
        public int ID { get; set; } = -1;
        public int ProviderID { get; set; } = -1;
        public string Path { get; set; } = "";
        public ContainerType Type { get; set; } = 0;
        public List<PathItem> PathItems
        {
            get
            {
                return APIDataStorage.PathManager.List.Where(x => x.ContainerID == ID).ToList();
            }
        }

        public Container ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            ProviderID = jObjectItem.Value<int>("ProviderID");
            Path = jObjectItem.Value<string>("ContainerPath") ?? "";
            Type = (ContainerType)jObjectItem.Value<int>("ContainerType");
            return this;
        }

        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(ProviderID), ProviderID },
                { nameof(Path), Path },
                { nameof(Type), (int)Type }
            };
            return jobject;
        }
    }
}
