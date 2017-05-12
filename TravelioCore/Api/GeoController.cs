using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeoApi.Models;
using Microsoft.Extensions.Caching.Memory;
using GeoApi.Api;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelioCore.Api
{
	[Route("api/[controller]/[action]")]
	public class GeoController : Controller
	{
		private IMemoryCache _memCache;

		public GeoController(IMemoryCache memCache)
		{
			_memCache = memCache;
		}

		// GET: /<controller>/
		[HttpGet]
		[ActionName("countries")]
		public IEnumerable<CountryData> GetAll()
		{
			if (_memCache.TryGetValue("GeoCountries", out IEnumerable<CountryData> countries))
			{
				return countries;
			}

			var geoApiManager = new GeoApiManager();
			var countryTask = geoApiManager.GetCountries();
			countryTask.Wait();

			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set("GeoCountries", countryTask.Result, cacheEntryOptions);
			return countryTask.Result;
		}

		[HttpGet]
		[ActionName("countries/name/{name}")]
		public CountryData GetCountryByName(string name)
		{
			var countries = GetAll();
			return countries.FirstOrDefault(c => string.Equals(name, c.Name, StringComparison.CurrentCultureIgnoreCase));
		}

		[HttpGet]
		[ActionName("countries/code/{code}")]
		public CountryData GetCountryByCode(string code)
		{
			var countries = GetAll();
			return countries.FirstOrDefault(c => string.Equals(code, c.Alpha2Code, StringComparison.CurrentCultureIgnoreCase));
		}

		[HttpGet]
		[ActionName("regions")]
		public IEnumerable<string> GetRegions(string name)
		{
			var regions = new List<string> { "Africa", "Americas", "Asia", "Europe", "Oceania" };
			return regions;
		}
	}
}
