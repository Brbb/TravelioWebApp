﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace NcdcLib.Model
{
    public class Location
    {
		//"mindate": "1891-07-01",
		
        public DateTime MinDate { get; set; }

        //"maxdate": "2017-04-26",

        public DateTime MaxDate { get; set; }
      
        //"name": "Aberdeen, WA US",

        public string Name { get; set; }
      
        //"datacoverage": 1,

        public double DataCoverage { get; set; }

        //"id": "CITY:US530001"
        [JsonProperty("id")]
        public string LocationId { get; set; }
    }
}
