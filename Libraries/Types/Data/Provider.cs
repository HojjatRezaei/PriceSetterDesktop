namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using OpenQA.Selenium.DevTools.V119.Network;
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
        public int LoginInfoID { get; set; } = -1;
        public ExtractionTypes Extraction { get; set; } = 0;
        public bool HaveURL { get; set; } = false;
        public LoginInfo? LoginInfo 
        {
            get
            {
                if (LoginInfoID == -1)
                    return null;
                return APIDataStorage.LoginManager.List.FirstOrDefault(x => x.ID == LoginInfoID);
            }
        }
        public IEnumerable<Container> Containers
        {
            get
            {
                return APIDataStorage.ContainerManager.List.Where(x => x.ProviderID == ID);
            }

        }
        public bool HaveData
        {
            get
            {
                return Containers.Any();
            }
        }
        public IEnumerable<Url> UrlList
        {
            get
            {
                return APIDataStorage.UrlManager.List.Where(x => x.ProviderID == ID);
            }
        }
        public List<Article> ArticleList
        {
            get
            {
                var newList = new List<Article>();
                foreach (var item in UrlList)
                {
                    var newItem = item.GetArticle();
                    if (newItem == null)
                        continue;
                    newList.Add(newItem);
                }
                return newList;
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
        public bool IsValidData()
        {
            return true;
        }
    }
}
