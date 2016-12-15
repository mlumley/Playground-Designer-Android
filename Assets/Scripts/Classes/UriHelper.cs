using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Classes
{
    class UriHelper
    {
        public static NameValueCollection GetQueryString(string url) {
            string[] urlSegments = url.Split('?');
            NameValueCollection queryParameters = new NameValueCollection();

            if (urlSegments.Length > 1)
            {
                string queryString = urlSegments[1];
                string[] querySegments = queryString.Split('&');
                foreach (string segment in querySegments)
                {
                    string[] parts = segment.Split('=');
                    if (parts.Length > 0)
                    {
                        string key = parts[0].Trim(new char[] { '?', ' ' });
                        string val = parts[1].Trim();

                        queryParameters.Add(key, val);
                    }
                }
            }
            return queryParameters;
        }
    }
}
