﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using GeoApi.Api;
using GeoApi.Models;
using StorageManager;

namespace GeoApi.Visa
{
    public class VisaManager
    {
        /// <summary>
        /// Loads the world visa map from a storage source or a third-party web service.
        /// </summary>
        /// <returns>The world visa map.</returns>
		public async Task<IEnumerable<CountryData>> LoadWorldVisaMap(string xmlContent)
		{
            var xmlDoc = XDocument.Parse(xmlContent);

			var countryCodes = xmlDoc.Descendants("country").Select(n => n.Attribute("value").Value.ToUpper()).ToList();
			var countryNodes = xmlDoc.Descendants("country").ToList();

            var geoApiManager = new GeoApiManager();
            var countriesList = await geoApiManager.GetCountries();

			countryNodes.ForEach(node =>
			{
				try
				{
					var visaChildNodes = node.Descendants("visa").ToList();
					var country = countriesList.FirstOrDefault(c => c.Alpha2Code == node.Attribute("value").Value.ToUpper());

					country.VFCountries = visaChildNodes.Where(n => n.Attribute("type").Value == "vf").Select(n => n.Attribute("code").Value).ToList();
					country.VOACountries = visaChildNodes.Where(n => n.Attribute("type").Value == "voa").Select(n => n.Attribute("code").Value).ToList();
					country.ETACountries = visaChildNodes.Where(n => n.Attribute("type").Value == "eta").Select(n => n.Attribute("code").Value).ToList();

					country.VReqCountries.AddRange(countryCodes.Except(country.VFCountries.Concat(country.VOACountries).Concat(country.ETACountries)));
					country.VReqCountries.Remove(country.Alpha2Code);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message + ":::" + node.Value);
				}
			});


            return countriesList;
		}
    }
}
