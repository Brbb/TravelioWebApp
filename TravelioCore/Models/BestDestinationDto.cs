using System;
using System.Collections.Generic;
using GeoApi.Models;

namespace TravelioCore.Models
{
    public class BestDestinationDto
    {
        public BestDestinationDto(List<CountryData> countries,CountryData departureCountry)
        {
            DepartureCountry = departureCountry;
            Countries = countries;
        }

        public List<CountryData> Countries { get; set; } = new List<CountryData>();
        public CountryData DepartureCountry { get; set; }
    }
}
