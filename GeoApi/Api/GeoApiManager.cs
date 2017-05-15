using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using GeoApi.Models;
using System.Net.Http;
using System.Globalization;
using System.Linq;

namespace GeoApi.Api
{
    public class GeoApiManager
    {
        public string GeoUri { get; set; } = @"https://restcountries.eu/rest/v2";

		public async Task<List<CountryData>> GetCountries()
		{
			using (var client = new HttpClient())
			{
                var fullUrl = string.Format("{0}/all", GeoUri);
                var response = await client.GetAsync(fullUrl);
				var countryJson = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<CountryData>>(countryJson);
			}
		}

		public async Task<List<CountryData>> GetCountriesByRegion(string region)
		{
			using (var client = new HttpClient())
			{
                var fullUrl = string.Format("{0}/region/{1}", GeoUri,region);
				var response = await client.GetAsync(fullUrl);
				var countryJson = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<CountryData>>(countryJson);
			}
		}

		//https://restcountries.eu/rest/v2/name/{name}
		public async Task<CountryData> GetCountryByName(string countryName)
        {
			using (var client = new HttpClient())
			{
                var fullUrl = string.Format("{0}/name/{1}", GeoUri,countryName);
				var response = await client.GetAsync(fullUrl);
				var countryJson = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<IEnumerable<CountryData>>(countryJson);
                return results.FirstOrDefault();
			}
        }

		public async Task<CountryData> GetCountryByCode(string countryCode)
		{
			using (var client = new HttpClient())
			{
				var fullUrl = string.Format("{0}/alpha/{1}", GeoUri, countryCode);
				var response = await client.GetAsync(fullUrl);
				var countryJson = await response.Content.ReadAsStringAsync();
				var results = JsonConvert.DeserializeObject<IEnumerable<CountryData>>(countryJson);
				return results.FirstOrDefault();
			}
		}

        public async Task<double> ConvertCurrency(string code1, string code2, double amount)
        {
            return amount;
        }
    }
}
