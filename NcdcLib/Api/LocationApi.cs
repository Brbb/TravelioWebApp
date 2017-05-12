using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using NcdcLib.Model;

namespace NcdcLib.Api
{
    public class LocationApi : NcdcApi
    {
        public LocationApi(NcdcApiManager api) : base(api, "locations"){}

        //locationcategoryid=CITY
        public async Task<string> GetLocationsAsync(List<KeyValuePair<string, string>> parameters = null)
        {
            return await ApiManager.GetStringAsync(GetRequestString(parameters));
        }

        public async Task<string> GetLocationsAsync(string pathToAppend, List<KeyValuePair<string, string>> parameters = null)
        {
            return await ApiManager.GetStringAsync(GetRequestString(pathToAppend, parameters));
        }

		/// <summary>
		/// Gets the first 25 cities async.
		/// </summary>
		/// <returns>The cities async.</returns>
		/// <param name="parameters">Set the parameter limit=1000 to get the max number of results available and change the offset parameter to move to the next chunck of results.</param>
		public async Task<string> GetCitiesAsync(List<KeyValuePair<string, string>> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new List<KeyValuePair<string, string>>();
            }

            parameters.Add(new KeyValuePair<string, string>("locationcategoryid", "CITY"));

            return await GetLocationsAsync(parameters);
        }


        public async Task<List<Location>> GetAllCitiesAsync(List<KeyValuePair<string, string>> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new List<KeyValuePair<string, string>>();
            }

            parameters.Add(new KeyValuePair<string, string>("locationcategoryid", "CITY"));
            parameters.Add(new KeyValuePair<string, string>("limit", "1000"));


            var locationsString = await GetLocationsAsync(parameters);
            var locationsDto = JsonConvert.DeserializeObject<NcdcResult<Location>>(locationsString);

            var cities = locationsDto.Results;

            while (locationsDto.Metadata.ResultSet.Limit + locationsDto.Metadata.ResultSet.Offset < locationsDto.Metadata.ResultSet.Count)
            {
                var victim = parameters.FirstOrDefault(p => string.Equals(p.Key, "Offset", StringComparison.CurrentCultureIgnoreCase));
                parameters.Remove(victim);

                var threshold = locationsDto.Metadata.ResultSet.Limit + locationsDto.Metadata.ResultSet.Offset;
                parameters.Add(new KeyValuePair<string, string>("offset", threshold.ToString()));
                locationsString = await GetLocationsAsync(parameters);
                locationsDto = JsonConvert.DeserializeObject<NcdcResult<Location>>(locationsString);
                cities.AddRange(locationsDto.Results);
            }

            return cities;
        }

		public async Task<List<Location>> GetAllCountriesAsync(List<KeyValuePair<string, string>> parameters = null)
		{
			if (parameters == null)
			{
				parameters = new List<KeyValuePair<string, string>>();
			}

			parameters.Add(new KeyValuePair<string, string>("locationcategoryid", "CNTRY"));
            parameters.Add(new KeyValuePair<string, string>("limit", "1000"));

			var locationsString = await GetLocationsAsync(parameters);
            var locations = JsonConvert.DeserializeObject<NcdcResult<Location>>(locationsString);

            return locations.Results;
		}


        //https://www.ncdc.noaa.gov/cdo-web/api/v2/locations/FIPS:37
        public async Task<Location> GetLocationByIdAsync(string id)
        {
            var result = await GetLocationsAsync(id);
            return JsonConvert.DeserializeObject<Location>(result);
        }
    }
}
