using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TravelioApi.Models;
using Newtonsoft.Json;
using GeoApi.Api;
using NcdcLib.Api;
using NcdcLib.Model;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using GeoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelioCore.Api
{
    [Route("api/[controller]/[action]")]
    public class TravelioController : Controller
    {

        private IMemoryCache _memCache;
        private IConfiguration _configuration { get; set; }

        public TravelioController(IMemoryCache memCache, IConfiguration configuration)
        {
            _configuration = configuration;
            _memCache = memCache;
        }

        [HttpGet]
        [ActionName("countries/code/{code}")]
        public async Task<TravelioCountry> GetTravelioCountryByCode(string code,[FromQuery] int month)
        {
            var visaApi = new VisaController(_memCache, _configuration);
            var countryData = await visaApi.GetCountryMap(code);

            if (_memCache.TryGetValue(countryData.Alpha2Code + "TravelioInfo", out TravelioCountry travelioCountry))
            {
                return travelioCountry;
            }

            return await PopulateTravelioCountry(month,countryData);
        }

        [HttpGet]
        [ActionName("countries/name/{name}")]
        public async Task<TravelioCountry> GetTravelioCountryByName(string name, [FromQuery] int month)
        {
            var visaApi = new VisaController(_memCache, _configuration);
            var countryData = await visaApi.GetCountryMapByName(name);

            if (_memCache.TryGetValue(countryData.Alpha2Code + "TravelioInfo", out TravelioCountry travelioCountry))
            {
                return travelioCountry;
            }

            return await PopulateTravelioCountry(month,countryData);
		}

        private async Task<TravelioCountry> PopulateTravelioCountry(int month,CountryData countryData)
        {
            var historicalController = new HistoricalController(_memCache, _configuration);
            var locationInfo = historicalController.SearchCountry(countryData.Alpha2Code, countryData.Name);

            if (locationInfo != null)
            {
                var data = await historicalController.GetQuickDataForLocation(month, locationInfo.LocationId);
                var travelioCountry = new TravelioCountry(countryData, locationInfo, data);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromDays(1));

                _memCache.Set(countryData.Alpha2Code + "TravelioInfo", travelioCountry, cacheEntryOptions);

                return travelioCountry;
            }

            return null;
        }
    }
}
