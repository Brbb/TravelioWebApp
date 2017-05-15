using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoApi.Models;
using GeoApi.Visa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using StorageManager;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelioCore.Api
{
    [Route("api/[controller]/[action]")]
    public class VisaController : Controller
    {
        private IConfiguration _configuration;
        private IMemoryCache _memoryCache;

        public VisaController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }


        // GET: api/visa/map
        [HttpGet]
        [ActionName("map")]
        public async Task<IEnumerable<CountryData>> GetCountriesMap()
        {
            if (_memoryCache.TryGetValue("WorldVisaMap", out IEnumerable<CountryData> worldVisaMap))
                return worldVisaMap;


            var visaManager = new VisaManager();
			if (!_memoryCache.TryGetValue("WorldVisaMapSource", out string worldVisaMapSource))
			{
                var cloudController = new CloudController(_memoryCache, _configuration);
                worldVisaMapSource = await cloudController.DownloadVisaMapContent();
			}

            worldVisaMap = await visaManager.LoadWorldVisaMap(worldVisaMapSource);
            _memoryCache.Set("WorldVisaMap", worldVisaMap, TimeSpan.FromDays(1));

            return worldVisaMap;

        }

        // GET: api/visa/country/{code}
		[HttpGet]
        [ActionName("country/code/{code}")]
		public async Task<CountryData> GetCountryMap(string code)
		{
            var map = await GetCountriesMap();
            return map.FirstOrDefault(c =>string.Equals(c.Alpha2Code, code, StringComparison.CurrentCultureIgnoreCase));
		}

		[HttpGet]
		[ActionName("country/name/{name}")]
		public async Task<CountryData> GetCountryMapByName(string name)
		{
			var map = await GetCountriesMap();
            return map.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
		}
    }
}
