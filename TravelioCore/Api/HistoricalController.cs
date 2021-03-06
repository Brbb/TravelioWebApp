﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NcdcLib.Model;
using NcdcLib.Api;
using Microsoft.Extensions.Configuration;

namespace TravelioCore.Api
{
    [Route("api/[controller]/[action]")]
    public class HistoricalController : Controller
    {
        private IMemoryCache _memCache;
        private IConfiguration _configuration;
        private NcdcApiManager _ncdcApiManager;
        private DateTime _historicalStartDate;

        public HistoricalController(IMemoryCache memCache, IConfiguration configuration)
        {
            _configuration = configuration;
            _memCache = memCache;
            var ncdcToken = _configuration.GetValue<string>("Authentication:NcdcToken");
            _historicalStartDate = _configuration.GetValue<DateTime>("Services:HistoricalStartDate");
            _ncdcApiManager = new NcdcApiManager(ncdcToken);
        }

        // GET: /<controller>/
        [HttpGet]
        [ActionName("cities")]
        public IEnumerable<Location> GetCitiesHistorical()
        {
            if (_memCache.TryGetValue("CitiesHistorical", out IEnumerable<Location> cities))
            {
                return cities;
            }

            var locationApi = new LocationApi(_ncdcApiManager);

            var citiesTask = locationApi.GetAllCitiesAsync();
            citiesTask.Wait();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));

            _memCache.Set("CitiesHistorical", citiesTask.Result, cacheEntryOptions);
            return citiesTask.Result;
        }

		[HttpGet]
		[ActionName("countries")]
		public IEnumerable<Location> GetHistoricalCountryList()
		{
			if (_memCache.TryGetValue("HistoricalCountryList", out IEnumerable<Location> countries))
			{
				return countries;
			}

			var locationApi = new LocationApi(_ncdcApiManager);

			var countriesTask = locationApi.GetAllCountriesAsync();
			countriesTask.Wait();

			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set("HistoricalCountryList", countriesTask.Result, cacheEntryOptions);
			return countriesTask.Result;
		}

        /// <summary>
        /// Searchs the historical definition of the country in the historical countries list provided by NCDC.
        /// </summary>
        /// <returns>The country as a Location object.</returns>
        /// <param name="code">The country code as provided by the Geo Api.</param>
        /// <param name="name">The country name to improve the matches of those countries with a NCDC code different by the Geo Code.</param>
		[HttpGet]
        [ActionName("countries/search/{code}")]
		public Location SearchCountry(string code,[FromQuery]  string name = "" )
		{
			if (_memCache.TryGetValue("LocationInfo" + code, out Location info))
			{
                return info;
			}

            // Let's try to speed up if the list of countries is already here
            if (_memCache.TryGetValue("HistoricalCountryList", out IEnumerable<Location> countries))
            {
                var country = countries.FirstOrDefault(c => c.LocationId.Split(':').Last().Equals(code) ||
                                                  string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));


                return country;
            }

			var locationApi = new LocationApi(_ncdcApiManager);
            info = locationApi.GetCountry(code,name).Result;

			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set("LocationInfo" + code, info, cacheEntryOptions);
            return info;
		}

        /// <summary>
        /// Gets the temperature for location starting from a server side configured historical starting date and ending approx. today.
        /// </summary>
        /// <returns>The temperature for location.</returns>
        /// <param name="locationId">Location identifier.</param>
		[HttpGet]
        [ActionName("data/temperature/{locationId}")]
		public IEnumerable<Data> GetTemperatureForLocation(string locationId)
		{
            if (_memCache.TryGetValue("TempData"+locationId, out IEnumerable<Data> data))
			{
				return data;
			}

            var dataApi = new DataApi(_ncdcApiManager);

            var dataTask = dataApi.GetDataAsync(new List<DataType>{DataType.TMAX, DataType.TMIN, DataType.TAVG }, DataSet.GSOM, locationId, _historicalStartDate, DateTime.Now);
			dataTask.Wait();

			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set("TempData"+locationId, dataTask.Result, cacheEntryOptions);
			return dataTask.Result;
		}

		[HttpGet]
		[ActionName("data/precipitation/{locationId}")]
		public IEnumerable<Data> GetPrecipitationForLocation(string locationId)
		{
			if (_memCache.TryGetValue("PrcpData" + locationId, out IEnumerable<Data> data))
			{
				return data;
			}

			var dataApi = new DataApi(_ncdcApiManager);

            var dataTask = dataApi.GetDataAsync(new List<DataType> { DataType.PRCP, DataType.DP01 }, DataSet.GSOM, locationId, _historicalStartDate, DateTime.Now);
			dataTask.Wait();

			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set("PrcpData" + locationId, dataTask.Result, cacheEntryOptions);
			return dataTask.Result;
		}

		[HttpGet]
		[ActionName("data/all/{locationId}")]
		public async Task<IEnumerable<Data>> GetAllDataForLocation(string locationId)
		{
			if (_memCache.TryGetValue("AllData" + locationId, out IEnumerable<Data> data))
			{
				return data;
			}

			var dataApi = new DataApi(_ncdcApiManager);

            var dataTask = await dataApi.GetDataAsync(new List<DataType> { DataType.PRCP, DataType.TMAX, DataType.TMIN, DataType.TAVG, DataType.DP01 },
                                                DataSet.GSOM, locationId, _historicalStartDate, DateTime.Now);
			
			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set("AllData" + locationId, dataTask, cacheEntryOptions);
			return dataTask;
		}

		/// <summary>
		/// Gets the quick data for location based on AVG temperature and PRCP.
		/// </summary>
		/// <returns>The quick data for location.</returns>
		/// <param name="locationId">Location identifier.</param>
		/// <param name="month">The desired month.</param>
		/// <param name="ignoreCache">Ignores the cache and reloads data for that month/country if set as true.</param>
		[HttpGet]
        [ActionName("data/monthly/{month}/{locationId}")]
        public async Task<IEnumerable<Data>> GetQuickDataForLocation(int month, string locationId, [FromQuery] bool ignoreCache = false)
		{
			if (!ignoreCache && _memCache.TryGetValue(month+"MonthlyData" + locationId, out IEnumerable<Data> data))
			{
				return data;
			}

			var dataApi = new DataApi(_ncdcApiManager);
            var taskList = new List<Task<IEnumerable<Data>>>();

            for (int i = 0; i <= 5;i++)
            {
                var endDateString = DateTime.Now.Year-i + "-" + month.ToString().PadLeft(2, '0') + "-" + DateTime.DaysInMonth(DateTime.Now.Year, month).ToString().PadLeft(2, '0');
				var endDate = DateTime.Parse(endDateString);
				var startDate = DateTime.Parse(DateTime.Now.Year-i + "-" + month.ToString().PadLeft(2, '0') + "-01");

                taskList.Add(dataApi.GetDataAsync(new List<DataType> { DataType.PRCP,DataType.DP01, DataType.TMAX, DataType.TMIN }, DataSet.GSOM, locationId, startDate, endDate));
            }

            var results = await Task.WhenAll(taskList);
            var result = results.SelectMany(r => r).ToList();

			var cacheEntryOptions = new MemoryCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromDays(1));

			_memCache.Set(month+"MonthlyData" + locationId, result, cacheEntryOptions);
			return result;
		}
    }
}
