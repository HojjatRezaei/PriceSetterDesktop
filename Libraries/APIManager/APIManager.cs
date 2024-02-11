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
        private bool _updated = false;
        public List<T> List 
        { 
            get 
            {
                if (!_updated)
                    Get();
                return _list;
            }
        }
        public APIManager(string apiUrl, bool isReadOnly = false)
        {
            _api = apiUrl;
            _getOnlyApi = isReadOnly;
            _list = [];
            if (!string.IsNullOrEmpty(apiUrl))
            {
                Get();
            }
        }
        public void Get()
        {
            if(string.IsNullOrEmpty(_api))
            {
                _updated = false; return;
            }
            else
            {
                //send http get request
                _list = HTTPUtility.SendGETRequest<T>(_api);
                _updated = true;
            }
        }
        public void Add(T newItem)
        {
            if (_getOnlyApi) return;
            var respond = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Post);
            if (respond != null )
            {
                newItem.ID = respond.Value<int>("ID");
                _list.Add(newItem);
            }
        }
        public void Update(T newItem , int id)
        {
            if (_getOnlyApi) return;
            newItem.ID = id;
            var respond = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Put);
            if (respond != null )
            {
                var searchResult = _list.FirstOrDefault(x => x.ID == id);
                if (searchResult != null)
                {
                    int index = _list.IndexOf(searchResult);
                    _list[index] = newItem;
                }
            }
        }
        public void Remove(int id)
        {
            if (_getOnlyApi) return;
            T newItem = new T
            {
                ID = id
            };
            var respond = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Delete);
            if (respond != null)
            {
                var searchResult = _list.FirstOrDefault(x => x.ID == id);
                if(searchResult != null)
                    _list.Remove(searchResult);
            }
        }
    }
}
