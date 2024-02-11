namespace PriceSetterDesktop.Libraries.Types.Data
{
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Statics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WPFCollection.Data.Interface.Generic;

    public class LoginInfo : IJsonConverter<LoginInfo>
    {
        public int ID { get; set; } = -1;
        public int ProviderID { get; set; } = -1;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Url? LoginPage 
        { 
            get
            {
                return APIDataStorage.UrlManager.List.FirstOrDefault(x => x.LoginInfoID == ID);
            }
        }

        public LoginInfo ConvertFromJson(JToken jObjectItem)
        {
            throw new NotImplementedException();
        }

        public JObject CreateJsonObject()
        {
            throw new NotImplementedException();
        }
    }
}
