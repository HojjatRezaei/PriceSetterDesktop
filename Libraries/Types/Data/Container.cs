namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Types.Enum;
    using WPFCollection.Data.Interface.Generic;

    public class Container : IJsonConverter<Container>
    {
        public int ProviderID { get; set; } = -1;
        public string ContainerPath { get; set; } = "";
        public ContainerType ContainerType { get; set; } = 0;
        public List<PathItem> PathItems { get; }
        public int ID { get; set; }

        public Container ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            ProviderID = jObjectItem.Value<int>("ProviderID");
            ContainerPath = jObjectItem.Value<string>("ContainerPath") ?? "";
            ContainerType = (ContainerType)jObjectItem.Value<int>("ContainerType");
            return this;
        }

        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(ProviderID), ProviderID },
                { nameof(ContainerPath), ContainerPath },
                { nameof(ContainerType), (int)ContainerType }
            };
            return jobject;
        }
    }
}
