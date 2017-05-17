using System;
using System.Collections.Generic;
using GeoApi.Models;

namespace TravelioCore.Models
{
    public class HomeModel
    {
        public List<CountryData> Countries { get; set; } = new List<CountryData>();
        public DestinationModel BestDestination { get; set; } = new DestinationModel();
    }
}
