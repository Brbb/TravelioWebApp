using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GeoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TravelioApi.Models;

namespace TravelioCore.Components
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TravelioCountryViewComponent: ViewComponent
    {
        private IConfiguration _configuration;
        private string _endpoint;

        public TravelioCountryViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
            _endpoint = configuration.GetValue<string>("Services:Endpoint");
        }

        public async Task<IViewComponentResult> InvokeAsync(TravelioViewComponentDto dto)
        {
            using (var client = new HttpClient() { Timeout = TimeSpan.FromMinutes(2) })
            {
                try
                {
                    var travelioCountryString = await client.GetStringAsync(string.Format("{0}/travelio/countries/code/{1}?month={2}", _endpoint, dto.Country.Alpha2Code,dto.Month));
                    if (!String.IsNullOrEmpty(travelioCountryString))
                    {
                        var travelioCountry = JsonConvert.DeserializeObject<TravelioCountry>(travelioCountryString);

                        if(travelioCountry!= null)
                            return View(travelioCountry);
                    }
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }

                return View();
            }
        }
    }


    public class TravelioViewComponentDto
    {
        public int Month { get; set; }
        public CountryData Country { get; set; }
        public CountryData DepartureCountry { get; set; }
    }
}
