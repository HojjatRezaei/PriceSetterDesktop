namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using WPFCollection.Data.Interface.Generic;
    using WPFCollection.Data.Types;

    public class ScrapResult : IJsonConverter<ScrapResult>, IComparable<ScrapResult>
    {
        public ScrapResult()
        {

        }
        public int ID { get; set; }
        public int ArticleID { get; set; } = -1;
        public string ArticleName
        {
            get
            {
                var searchResult = GetArticle();
                return searchResult == null ? "" : searchResult.ArticleName;
            }
        }
        public int ProviderID { get; set; } = -1;
        public int ColorID { get; set; } = -1;
        public string ColorName
        {
            get
            {
                if (string.IsNullOrEmpty(_colorName))
                {
                    //try to find color name
                    //extract article list 
                    var res = APIDataStorage.ArticleManager.List.FirstOrDefault(x => x.ID == ArticleID && x.ColorID == ColorID);
                    if (res != null)
                        _colorName = res.ColorName.Replace("رنگ", string.Empty).Replace(":", string.Empty);
                }
                return _colorName;
            }
            set { _colorName = value; }
        }
        public double Price { get; set; } = 0;
        public PersianDate Date { get; set; } = PersianDate.Now;
        public string Time { get; set; } = "";
        public int PriceID
        {
            get
            {
                var artObject = GetArticle();
                if (artObject == null)
                {
                    return -1;
                }
                else
                {
                    return artObject.PriceID;
                }
}
        }
        public int RegularPriceID
        {
            get
            {
                var artObject = GetArticle();
                if (artObject == null)
                {
                    return -1;
                }
                else
                {
                    return artObject.RegularPriceID;
                }
            }
        }
        public int OptionID
        {
            get
            {
                var artObject = GetArticle();
                if (artObject == null)
                {
                    return -1;
                }
                else
                {
                    return artObject.OptionID;
                }
            }
        }
        public string OptionValue
        {
            get
            {
                var artObject = GetArticle();
                if(artObject == null)
                {
                    return string.Empty;
                }
                else
                {
                    return artObject.OptionValue;
                }
            }
        }
        public string Source { get; set; } = string.Empty;
        public bool HaveMessage { get; set; }
        public string Messages { get; set; } = string.Empty;
        public bool ValidData => ColorID != -1 && PriceID != -1 && Price > 10;
        public string DateTimeStr => Date.ToString() + "-" + Time;
        public JObject? OptionValueJson => OptionValue != string.Empty ? JObject.Parse(OptionValue) : null;
        public Article? GetArticle()
        {
            var searchList = APIDataStorage.ArticleManager.List.ToList();
            var artObject = searchList.FirstOrDefault(x => x.ID == ArticleID);
            if(artObject == null)
            {
                return null;
            }
            else
            {
                if (artObject.HaveVariable && ColorID != -1)
                {
                    var artObjectWithColor = searchList.FirstOrDefault(x => x.ID == ArticleID && x.ColorID == ColorID);
                    if(artObjectWithColor != null)
                    {
                        return artObjectWithColor;
                    }else
                    {
                        return artObject;
                    }
                }
                else
                {
                    return artObject;
                }
            }
        }
        public Provider? GetProvider()
        {
            return APIDataStorage.ProviderManager.List.FirstOrDefault(x => x.ID == ProviderID);
        }
        public ScrapResult ConvertFromJson(JToken jObjectItem)
        {
            ID = jObjectItem.Value<int>("ID");
            Date = jObjectItem.Value<string>("Date") ?? "";
            Time = jObjectItem.Value<string>("Time") ?? "";
            Price = jObjectItem.Value<double>("Price");
            ArticleID = jObjectItem.Value<int>("ArticleID");
            ProviderID = jObjectItem.Value<int>("ProviderID");
            Source = jObjectItem.Value<string>("Source") ?? string.Empty;
            ColorID = jObjectItem.Value<int>("ColorID");
            return this;
        }
        public JObject CreateJsonObject()
        {
            var jobject = new JObject
            {
                { nameof(ID), ID },
                { nameof(ArticleID), ArticleID },
                { nameof(ProviderID), ProviderID },
                { nameof(ColorID), ColorID },
                { nameof(Price), Price.ToString() },
                { nameof(Date), Date.ToString() },
                { nameof(Time), Time },
                { nameof(Source), Source },
            };
            return jobject;
        }
        public bool IsValidData()
        {
            return ValidData && Price != 0;
        }
        private string _colorName = string.Empty;

        public int CompareTo(ScrapResult? other)
        {
            if (other == null)
                return -1;
            return this.ArticleName.CompareTo(other.ArticleName);
        }
    }
}