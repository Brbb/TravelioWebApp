using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using StorageManager;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelioCore.Api
{
    [Route("api/[controller]/[action]")]
    public class CloudController : Controller
    {
        // GET: api/values

        private string _cloudConnectionString;
        private IConfiguration _configuration;
        private IMemoryCache _memoryCache;

        public CloudController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _cloudConnectionString = configuration.GetValue<string>("Sources:AzureStorage");
            _configuration = configuration;
        }

        [HttpGet]
        [ActionName("download")]
        public async Task<string> DownloadFileContent([FromQuery] string share,[FromQuery]  string directory,[FromQuery]  string fileName)
        {
            var cloudManager = new CloudStorageManager(_cloudConnectionString);
            var fileContent = await cloudManager.DownloadContentAsync(share, directory, fileName);

            return fileContent;
        }

		[HttpGet]
		[ActionName("visamap")]
		public async Task<string> DownloadVisaMapContent()
		{
            if(_memoryCache.TryGetValue("WorldVisaMapSource",out string worldVisaMap))
            {
                return worldVisaMap;
            }


			var cloudManager = new CloudStorageManager(_cloudConnectionString);
            var share = _configuration.GetValue<string>("Sources:VisaMapShare");
            var directory = _configuration.GetValue<string>("Sources:VisaMapDirectory");
            var fileName = _configuration.GetValue<string>("Sources:VisaMapSource");

			worldVisaMap = await cloudManager.DownloadContentAsync(share, directory, fileName);
            _memoryCache.Set("WorldVisaMapSource", worldVisaMap, TimeSpan.FromDays(1));

			return worldVisaMap;
		}
    }
}
