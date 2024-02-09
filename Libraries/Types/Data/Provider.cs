namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Enum;
    using System;
    using WPFCollection.Data.Interface.Generic;

    public partial class Provider : IJsonConverter<Provider>
    {
        public Provider()
        {
        }
        public int ID { get; set; } = -1;
        public string Name { get; set; } = "";
        public ExtractionTypes Extraction { get; set; } = 0;
        public List<Container> Containers
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Container>(nameof(Container));
                return tb.List.Where(x => x.ProviderID == ID).ToList();

            }

        }
        public bool HaveURL { get; set; } = false;
        public bool HaveData
        {
            get
            {
                return Containers.Count != 0;
            }
        }

        public override bool Equals(object? obj)
        {
            var CompareObject = obj as Provider;
            return CompareObject == null ? false : Name == CompareObject.Name;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
        public override string ToString()
        {
            return $"{Name}";
        }

        public Provider ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            Name = jObjectItem.Value<string>("Name") ?? "";
            Extraction = (ExtractionTypes)jObjectItem.Value<int>("Extraction");
            return this;
        }

        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(Name), Name },
                { nameof(Extraction),(int)Extraction }
            };
            return jobject;
        }
    }
}
