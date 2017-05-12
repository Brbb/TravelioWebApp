using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using TravelioCore.Models;
using Newtonsoft.Json;

namespace TravelioCore.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration Configuration { get; set; }
        private IMemoryCache MemoryCache { get; set; }
        public string Endpoint { get; private set; }

        public HomeController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
            Configuration = configuration;
            Endpoint = Configuration.GetValue<string>("Services:Endpoint");
            var memoryCacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(5));

            MemoryCache.Set("ApiEndpoint", Endpoint, memoryCacheOptions);
        }

        public async Task<IActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                var countriesString = await client.GetStringAsync(string.Format("{0}/geo/countries", Endpoint));
                var countries = JsonConvert.DeserializeObject<List<CountryData>>(countriesString);

				var memoryCacheOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(5));
                MemoryCache.Set("GeoCountryList", countries, memoryCacheOptions);

                return View(new HomeModel() { Countries = countries });
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpPut]
        public async Task<JsonResult> UpdateDepartureCountryByCode(string code)
        {
            //         using(var client = new HttpClient())
            //         {
            //             var response = await client.GetAsync(geoCountriesUrl);
            //         }

            //var country = VisaDatabaseManager.Instance.GetCountryByCode(code);
            return UpsertDepartureCountry(new CountryData() { Alpha2Code = "IT", Name = "Italy" });
        }

        [HttpPut]
        public JsonResult UpdateDepartureCountryByName(string name)
        {
            //var country = VisaDatabaseManager.Instance.GetCountryByName(name);
            return UpsertDepartureCountry(new CountryData() { Alpha2Code = "IT", Name = "Italy" });
        }

        private JsonResult UpsertDepartureCountry(CountryData country)
        {
            return Json(country);
        }


		[HttpPost]
        public JsonResult GetFilteredCountries(string prefix)
		{
            MemoryCache.TryGetValue("GeoCountryList",out List<CountryData> countries);

            var result =
                from c in countries
                where c.Name.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)
            	select new { c.Name, Code = c.Alpha2Code };
            
            return Json(result.Take(5));
		}
    }
}
