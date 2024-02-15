namespace PriceSetterDesktop.Libraries.APIManager
{
    using OpenQA.Selenium;
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
        public T GetSingle(T searchItem)
        {
            if (!string.IsNullOrEmpty(_api))
            {
                //send http get request
                return HTTPUtility.SendGetSingleRequest(_api, searchItem);
            }
            else
            {
                return new();
            }

        }
        public void Get()
        {
            if (string.IsNullOrEmpty(_api))
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
        public bool Add(T newItem)
        {
            if (_getOnlyApi)
                return false;
            if (!newItem.IsValidData())
                return false;
            var respond = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Post);
            if (respond.Value<int>("status code") < 500)
            {
                if (respond.Value<string>("messagetype") != "success_noupdate")
                {
                    newItem.ID = respond.Value<int>("ID");
                    _list.Add(newItem);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public bool Update(T newItem, int id)
        {
            if (_getOnlyApi) return false;
            if (!newItem.IsValidData()) return false;
            newItem.ID = id;
            var respond = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Put);
            if (respond != null)
            {
                if (respond.Value<string>("messagetype") != "success_noupdate")
                {
                    var searchResult = _list.FirstOrDefault(x => x.ID == id);
                    if (searchResult != null)
                    {
                        int index = _list.IndexOf(searchResult);
                        _list[index] = newItem;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
                }
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public bool Remove(int id)
        {
            if (_getOnlyApi) return false;
            T newItem = new T
            {
                ID = id
            };
            var respond = HTTPUtility.SendRequest(_api, newItem, HttpMethod.Delete);
            if (respond != null)
            {
                if (respond.Value<string>("messagetype") != "success_noupdate")
                {
                    var searchResult = _list.FirstOrDefault(x => x.ID == id);
                    if (searchResult != null)
                    {
                        _list.Remove(searchResult);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
