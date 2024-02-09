namespace PriceSetterDesktop.Libraries.APIManager
{
    using System.Collections.Generic;
    using System.Net.Http;
    using WPFCollection.Data.Interface.Generic;
    using WPFCollection.Data.Statics;

    public class APIManager<T> where T : IJsonConverter<T>, new()
    {
        private readonly string _api;
        private readonly bool _getOnlyApi;
        private List<T> _list = [];
        public APIManager(string apiUrl, bool isReadOnly = false)
        {
            _api = apiUrl;
            _getOnlyApi = isReadOnly;
            GetData();
        }
        public void GetData()
        {
            _list.Clear();
            //send http get request
            _list = HTTPUtility.SendGETRequest<T>(_api);
        }
        public void SendNewData(T newItem)
        {
            if (_getOnlyApi) return;
            _ = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Post);
        }
        public void UpdateData(T newItem)
        {
            if (_getOnlyApi) return;
            _ = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Put);
        }
        public void DeleteData(T newItem)
        {
            if (_getOnlyApi) return;
            _ = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Delete);
        }
    }
}
