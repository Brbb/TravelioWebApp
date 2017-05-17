using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TravelioApi.Models;
using TravelioCore.Api;
using TravelioCore.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelioCore.Controllers
{
    public class DestinationController : Controller
    {
		private IConfiguration Configuration { get; set; }
		private IMemoryCache MemoryCache { get; set; }
		public string Endpoint { get; private set; }

		public DestinationController(IConfiguration configuration, IMemoryCache memoryCache)
		{
			MemoryCache = memoryCache;
			Configuration = configuration;
			Endpoint = Configuration.GetValue<string>("Services:Endpoint");
			var memoryCacheOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(5));

			MemoryCache.Set("ApiEndpoint", Endpoint, memoryCacheOptions);

		}

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchBestPlaceToGo(DestinationModel bestDestination)
        {
            var shortList = new List<CountryData>();
            try
            {
                switch (bestDestination.Weather)
                {
                    //case "Any": { tMax = 100; tMin = -100; }; break;
                    //case "Hot": { tMax = 100; tMin = 18; }; break;
                    //case "Warm": { tMax = 30; tMin = 12; }; break;
                    //case "Cold": { tMax = 15; tMin = -100; }; break;
                    default:; break;
                }

                if (!MemoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries))
                {
                    using (var client = new HttpClient())
                    {
                        var countriesString = await client.GetStringAsync(string.Format("{0}/visa/map", Endpoint));
                        countries = JsonConvert.DeserializeObject<List<CountryData>>(countriesString);

                        var memoryCacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromDays(5));
                        MemoryCache.Set("GeoCountryList", countries, memoryCacheOptions);
                    }
                }
                // first reduction
                shortList = countries.Where(c => c.Region.Equals("World") ||
                                            string.Equals(c.Region, bestDestination.Area, StringComparison.CurrentCultureIgnoreCase))
                                     .ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Destination exception:" + exception.Message);
            }

            return View(shortList);
        }

        public IActionResult InvokeComponentView(string code)
        {
            MemoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries);
            var country = countries.FirstOrDefault(c => string.Equals(c.Alpha2Code,code));
            return ViewComponent("TravelioCountry",country);
        }

		public string ToTitleCase(TextInfo textInfo, string str)
		{
			var tokens = str.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < tokens.Length; i++)
			{
				var token = tokens[i];
				tokens[i] = token.Substring(0, 1).ToUpper() + token.Substring(1);
			}

			return string.Join(" ", tokens);
		}
    }
}
