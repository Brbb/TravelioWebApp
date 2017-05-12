using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace NcdcLib.Api
{
    public class NcdcApiManager
    {
        //https://www.ncdc.noaa.gov/cdo-web/api/v2/locations?locationcategoryid=CITY&sortfield=name&limit=1000

        public string BaseUrl { get; set; } = @"https://www.ncdc.noaa.gov/cdo-web/api/v2";
        public string Token { get; set; }

        public NcdcApiManager(string token)
        {
            Token = token;
        }

        public async Task<string> GetStringAsync(string requestString)
        {
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
            {
                client.DefaultRequestHeaders.Add("token", Token);
                return await client.GetStringAsync(requestString);
            }
        }
    }
}
