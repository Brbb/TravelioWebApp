using System;
using GeoApi.Models;
using NcdcLib.Model;
using System.Collections.Generic;

namespace TravelioApi.Models
{
    /// <summary>
    /// This class joins all the data and info regarding a Travelio interest point/destination
    /// </summary>
    public class TravelioCountry
    {
        public TravelioCountry(CountryData geoData, Location locationInfo, IEnumerable<Data> historicalData)
        {
            GeoData = geoData;
            LocationInfo = locationInfo;
            HistoricalData = historicalData;
        }

        public CountryData GeoData { get; set; }
        public Location LocationInfo { get; set; }
        public IEnumerable<Data> HistoricalData { get; set; }
    }
}
