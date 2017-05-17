using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NcdcLib.Model;
using Newtonsoft.Json;

namespace NcdcLib.Api
{
    public class DataApi : NcdcApi
    {
        public DataApi(NcdcApiManager api) : base(api,"data")
        {
        }

        //Retrieves all the data iterating until the limit of 1000 results is bypassed
        public async Task<IEnumerable<Data>> GetDataAsync(List<DataType> dataTypes, DataSet datasetId, string locationId, DateTime startDate, DateTime endDate)
        {
            var parameters = new List<KeyValuePair<string, string>>();
            var dataTypeValues = dataTypes.Select(dt => new KeyValuePair<string,string>("datatypeid",Enum.GetName(typeof(DataType),dt)));
            parameters.AddRange(dataTypeValues);

            parameters.Add(new KeyValuePair<string, string>("datasetid", Enum.GetName(typeof(DataSet), datasetId)));
            parameters.Add(new KeyValuePair<string, string>("locationid", locationId));
            parameters.Add(new KeyValuePair<string, string>("startdate", startDate.ToString("yyyy-MM-dd")));
            parameters.Add(new KeyValuePair<string, string>("enddate", endDate.ToString("yyyy-MM-dd")));
            parameters.Add(new KeyValuePair<string, string>("limit", "1000"));

            var totalData = new List<Data>();
            var dataString = await ApiManager.GetStringAsync(GetRequestString(parameters));
            if (!String.IsNullOrEmpty(dataString))
            {
                var data = JsonConvert.DeserializeObject<NcdcResult<Data>>(dataString);

                totalData.AddRange(data.Results);

                while (data.Metadata.ResultSet.Count > data.Metadata.ResultSet.Limit + data.Metadata.ResultSet.Offset)
                {
                    var victim = parameters.FirstOrDefault(p => string.Equals(p.Key, "Offset", StringComparison.CurrentCultureIgnoreCase));
                    parameters.Remove(victim);

                    var threshold = data.Metadata.ResultSet.Limit + data.Metadata.ResultSet.Offset;
                    parameters.Add(new KeyValuePair<string, string>("offset", threshold.ToString()));
                    dataString = await ApiManager.GetStringAsync(GetRequestString(parameters));
                    if (!String.IsNullOrEmpty(dataString))
                    {
                        data = JsonConvert.DeserializeObject<NcdcResult<Data>>(dataString);
                        totalData.AddRange(data.Results);
                    }
                }
            }

            return totalData;
        }
    }
}


//data?datasetid=GSOM&datatypeid=TMAX&datatypeid=TMIN&datatypeid=TAVG&locationid=CITY:US530001&startdate=2016-01-01&enddate=2017-04-01&limit=100
