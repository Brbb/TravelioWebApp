﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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
        [Description("Maximum Temperature (°C)")]
        TMAX,
        [Description("Minimum Temperature (°C)")]
        TMIN,
        [Description("Average Temperature (°C)")]
        TAVG,
        [Description("Precipitation (mm)")]
        PRCP,
        [Description("Rainy days")]
        DP01
    }

    public static class DataHelper
    {
        
        public static string Description(this Enum dataType)
        {
            // variables  
            var enumType = dataType.GetType().GetTypeInfo();
            var field = enumType.GetField(dataType.ToString());
            var descriptionAttribute = (DescriptionAttribute) field.GetCustomAttribute(typeof(DescriptionAttribute), false);

            return descriptionAttribute == null ? dataType.ToString() : descriptionAttribute.Description;
        }
    }
}


