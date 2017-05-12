using System.Collections.Generic;
using System.Linq;

namespace NcdcLib.Api
{
	public abstract class NcdcApi
	{
        protected NcdcApiManager ApiManager { get; set; }

        /// <summary>
        /// Gets or sets the endpoint value of https://api/version/{endpoint}
        /// </summary>
        /// <value>The endpoint string.</value>
        public string EndpointString { get; set; }
        public string FullUrl
        {
            get
            {
                return string.Format("{0}/{1}", ApiManager.BaseUrl, EndpointString);
            }
        }

		protected NcdcApi(NcdcApiManager api ,string endpoint)
        {
            EndpointString = endpoint;
            ApiManager = api;
        }

        protected string GetRequestString(List<KeyValuePair<string, string>> parameters)
        {
            var appendParameters = "";
            if (parameters != null)
            {
                appendParameters = "?" + string.Join("&", parameters.Select(p => p.Key + "=" + p.Value));
            }

            return string.Format(@"{0}{1}", FullUrl, appendParameters);
        }

        /// <summary>
        /// Gets the request string.
        /// </summary>
        /// <returns>The request string.</returns>
        /// <param name="pathToAppend">Append to the existing endpoint path to obtain https://api/version/endpoint/{pathToAppend}?{parameters} .</param>
        /// <param name="parameters">The query string parameters</param>
        protected string GetRequestString(string pathToAppend, List<KeyValuePair<string, string>> parameters)
        {
            var appendParameters = "";
            if (parameters != null)
            {
                appendParameters = "?" + string.Join("&", parameters.Select(p => p.Key + "=" + p.Value));
            }
            if (!string.IsNullOrEmpty(pathToAppend))
                pathToAppend = "/" + pathToAppend;
            else
                pathToAppend = string.Empty;

            return string.Format(@"{0}{1}{2}", FullUrl, pathToAppend, appendParameters);
        }
	}
}