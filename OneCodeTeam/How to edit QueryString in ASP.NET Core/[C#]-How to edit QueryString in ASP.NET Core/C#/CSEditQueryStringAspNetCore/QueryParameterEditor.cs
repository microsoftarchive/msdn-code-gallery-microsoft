using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSEditQueryStringAspNetCore
{
    public class QueryParameterEditor
    {
        private List<QueryParameter> _querys = new List<QueryParameter>();

        public string Authority { get; set; }
        public string QueryString
        {
            get
            {
                return string.Join("&", _querys);
            }
        }

        public string Fragment { get; set; }

        public string[] AllKeys
        {
            get
            {
                return _querys.Select(m => m.Key).ToArray();
            }
        }

        public string this[string key]
        {
            get
            {
                var param = _querys.FirstOrDefault(m => m.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                return param == null ? null : param.Value;
            }
            set { SetQueryParam(key, value); }
        }

        public QueryParameterEditor(string uriString)
        {
            string authority, queryString, fragment;
            uriString.CutStringByFirstKey("?", out authority, out queryString);

            queryString.CutStringByFirstKey("#", out queryString, out fragment);

            Authority = authority;
            InitParameters(queryString);
            Fragment = fragment;
        }

        public QueryParameterEditor SetQueryParam(string key, string value)
        {
            var param = new QueryParameter { Key = key, Value = value };

            SetQueryParam(param);

            return this;
        }

        public QueryParameterEditor SetQueryParam(params QueryParameter[] parameters)
        {
            foreach (var item in parameters)
            {
                var queryParam = _querys.FirstOrDefault(m => m.Key.Equals(item.Key, StringComparison.CurrentCultureIgnoreCase));
                if (queryParam == null)
                {
                    _querys.Add(item);
                }
                else
                {
                    queryParam.Value = item.Value;
                }
            }

            return this;
        }

        public QueryParameterEditor RemoveQueryParam(params string[] keys)
        {
            foreach (var key in keys)
            {
                var queryParam = _querys.FirstOrDefault(m => m.Key == key);
                _querys.Remove(queryParam);
            }
            return this;
        }

        public override string ToString()
        {
            return Authority +
                (string.IsNullOrEmpty(QueryString) ? "" : $"?{QueryString}") +
                (string.IsNullOrEmpty(Fragment) ? "" : $"#{Fragment}");
        }

        private void InitParameters(string queryString)
        {
            string[] queryArr = queryString.Split('&');
            foreach (var item in queryArr)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string key, value;
                    item.CutStringByFirstKey("=", out key, out value);
                    _querys.Add(new QueryParameter { Key = key, Value = value });
                }
            }
        }
    }

    public class QueryParameter
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Key}={Value}";
        }
    }
}
