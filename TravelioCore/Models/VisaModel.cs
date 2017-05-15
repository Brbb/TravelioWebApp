using System;
using GeoApi.Models;

namespace TravelioCore.Models
{
    public class VisaModel
    {
        public CountryData DepartureCountry { get; set; }
        public CountryData DestinationCountry { get; set; }

        public VisaModel(CountryData departureCountry, CountryData destinationCountry)
        {
            DepartureCountry = departureCountry;
            DestinationCountry = destinationCountry;

        }
    }
}
