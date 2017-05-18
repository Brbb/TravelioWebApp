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
using TimaticApi;
using System.Globalization;
using NcdcLib.Model;

namespace TravelioCore.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration { get; set; }
        private IMemoryCache _memoryCache { get; set; }
        private string _endpoint { get; set; }
        private string _timaticToken;

        public HomeController(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _endpoint = _configuration.GetValue<string>("Services:Endpoint");
            _timaticToken = _configuration.GetValue<string>("Authentication:TimaticToken");

            var memoryCacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(5));

            _memoryCache.Set("ApiEndpoint", _endpoint, memoryCacheOptions);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
				if (!_memoryCache.TryGetValue("HistoricalCountryList", out List<Location> historicalCountryList))
				{
					using (var client = new HttpClient())
					{
						var historicalCountryListString = await client.GetStringAsync(string.Format("{0}/historical/countries", _endpoint));
						historicalCountryList = JsonConvert.DeserializeObject<List<Location>>(historicalCountryListString);

						var memoryCacheOptions = new MemoryCacheEntryOptions()
						.SetAbsoluteExpiration(TimeSpan.FromDays(5));
						_memoryCache.Set("HistoricalCountryList", historicalCountryList, memoryCacheOptions);
					}
				}

                if (!_memoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries))
                {
                    using (var client = new HttpClient())
                    {
                        var countriesString = await client.GetStringAsync(string.Format("{0}/visa/map", _endpoint));
                        countries = JsonConvert.DeserializeObject<List<CountryData>>(countriesString);

						var memoryCacheOptions = new MemoryCacheEntryOptions()
						.SetAbsoluteExpiration(TimeSpan.FromDays(5));

                        if (historicalCountryList != null)
                        {
                            var joinedCountries = countries.Where(c => historicalCountryList.Any(h => h.LocationId.Split(':').Last().Equals(c.Alpha2Code) ||
                                                                           string.Equals(c.Name, h.Name, StringComparison.CurrentCultureIgnoreCase)));
                            _memoryCache.Set("GeoCountryList", joinedCountries.ToList(), memoryCacheOptions);
                        }
                        else
                        {
                            _memoryCache.Set("GeoCountryList", countries, memoryCacheOptions);
                        }
                    }
                }

                return View(new HomeModel() { Countries = countries });
            }
            catch(Exception exception)
            {
                Console.WriteLine("Home error:"+ exception.Message);
                return View();
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
        public JsonResult UpdateDepartureCountryByCode(string code)
        {
            _memoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries);
            var newDepartureCountry = countries.FirstOrDefault(c => string.Equals(c.Alpha2Code, code, StringComparison.CurrentCultureIgnoreCase));

            return UpsertDepartureCountry(newDepartureCountry);
        }

        [HttpPut]
        public JsonResult UpdateDepartureCountryByName(string name)
		{
			_memoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries);
            var newDepartureCountry = countries.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
            return UpsertDepartureCountry(newDepartureCountry);
		}

        private JsonResult UpsertDepartureCountry(CountryData country)
        {
            return Json(country);
        }


		[HttpPost]
        public JsonResult GetFilteredCountries(string prefix)
		{
            _memoryCache.TryGetValue("GeoCountryList",out List<CountryData> countries);

            var result =
                from c in countries
                where c.Name.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase)
            	select new { c.Name, Code = c.Alpha2Code };
            
            return Json(result.Take(5));
		}

        public async Task<ActionResult> SearchVisaByCountryName(string departureCountryName, string destinationCountryName)
        {
            _memoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries);

            var departureCountry = countries.FirstOrDefault(c => string.Equals(c.Name, departureCountryName, StringComparison.CurrentCultureIgnoreCase));
            var destinationCountry = countries.FirstOrDefault(c => string.Equals(c.Name, destinationCountryName, StringComparison.CurrentCultureIgnoreCase));

            return await GetTravelRequirements(departureCountry, destinationCountry);
        }

		private async Task<ActionResult> GetTravelRequirements(CountryData departureCountry, CountryData destinationCountry)
		{
			try
			{
                var timaticManager = new TimaticManager(_timaticToken);
				var travelRequirements = await timaticManager.GetTravelRequirements(departureCountry.Alpha2Code, destinationCountry.Alpha2Code);

                var vrp = new TimaticResultParser();
                var parsedResult = vrp.ParseTimaticResult(travelRequirements);
				ViewBag.Summary = parsedResult.FirstOrDefault(p => p.SectionName == "Summary");
				ViewBag.Passport = parsedResult.FirstOrDefault(p => p.SectionName == "Passport");
				ViewBag.Visa = parsedResult.FirstOrDefault(p => p.SectionName == "Visa");
				ViewBag.Health = parsedResult.FirstOrDefault(p => p.SectionName == "Health");
				ViewBag.CountryDetails = destinationCountry;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
                return NotFound();
			}

            return View("Visa", new VisaModel(departureCountry,destinationCountry));
		}

        [HttpPost]
        public JsonResult CheckCountryName(string countryName)
        {
			_memoryCache.TryGetValue("GeoCountryList", out List<CountryData> countries);
            return Json(countries.Any(c => string.Equals(c.Name,countryName)));
        }
    }
}
