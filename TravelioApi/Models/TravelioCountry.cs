using System;
using GeoApi.Models;
using NcdcLib.Model;
using System.Collections.Generic;
using System.Linq;

namespace TravelioApi.Models
{
    /// <summary>
    /// This class joins all the data and info regarding a Travelio interest point/destination
    /// </summary>
    public class TravelioCountry
    {
        public TravelioCountry(CountryData geoData, Location locationInfo, IEnumerable<AggregateData> aggregateData)
        {
            GeoData = geoData;
            LocationInfo = locationInfo;

            AggregateData.AddRange(aggregateData);

        }

        public CountryData GeoData { get; set; }
        public Location LocationInfo { get; set; }
        public List<AggregateData> AggregateData { get; private set; } = new List<AggregateData>();
    }

    public class AggregateData
    {
        public string Label { get; set; }
        //public DateTime Date { get; set; }
        public double Value { get; set; }
    }
}
