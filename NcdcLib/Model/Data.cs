﻿using System;
using System.ComponentModel;

namespace NcdcLib.Model
{
    public class Data
    {
        public DateTime Date { get; set; }
        public DataType DataType { get; set; }
        public string Station { get; set; }
        public string Attributes { get; set; }
        public double Value { get; set; }
    }

    public enum DataSet
    {
        GSOM
    }

    public enum DataType
    {
        TMAX,
        TMIN,
        TAVG,
        PRCP
    }
}
