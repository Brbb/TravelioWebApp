using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TravelioCore.Config;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TravelioCore.Api
{
    [Route("api/config/[action]")]
    public class ConfigurationController : Controller
    {
        private IConfiguration Configuration { get; set; }
		public ConfigurationController(IConfiguration configuration)
		{
			Configuration = configuration;
        }

		// GET: api/values
		[HttpGet]
		[ActionName("init")]
		public JsonResult Init()
		{
			return Json(Ok());
		}
    }
}
