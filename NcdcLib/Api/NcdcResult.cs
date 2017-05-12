using System;
using System.Collections.Generic;

namespace NcdcLib.Api
{
    public class NcdcResult<T>
    {
        //{"metadata":{"resultset":{"offset":1,"count":1980,"limit":25}},"results":[

        public Metadata Metadata { get; set; }
        public List<T> Results { get; set; }
    }

    public class Metadata
    {
        public ResultSet ResultSet { get; set; }
    }

    public class ResultSet
    {
        public int Offset { get; set; }
        public int Count { get; set; }
        public int Limit { get; set; }
    }
}
