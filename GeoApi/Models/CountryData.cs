using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeoApi.Api;
using Newtonsoft.Json;

namespace GeoApi.Models
{
	public class CountryData
	{
		public string Name { get; set; }
		public string NativeName { get; set; }
		public string Alpha2Code { get; set; }
		public string Alpha3Code { get; set; }
		public List<string> CallingCodes { get; set; } = new List<string>();
		public string Capital { get; set; }
		public string Region { get; set; }
		public int Population { get; set; }
		public decimal[] LatLng { get; set; }
		public Dictionary<string, string> Translations { get; set; }
		public string[] Timezones { get; set; }
		public List<string> Borders { get; set; }
		public List<LanguageDto> Languages { get; set; }
		public List<CurrencyDto> Currencies { get; set; }
		public List<string> AltSpellings { get; set; }
		public string Flag { get; set; }


		[JsonIgnore]
		public List<string> VOACountries { get; set; } = new List<string>();
		[JsonIgnore]
		public List<string> VFCountries { get; set; } = new List<string>();
		[JsonIgnore]
		public List<string> ETACountries { get; set; } = new List<string>();
		[JsonIgnore]
		public List<string> VReqCountries { get; set; } = new List<string>();

	}

	public class LanguageDto
	{
		public string ISO639_1 { get; set; }
		public string ISO639_2 { get; set; }
		public string Name { get; set; }
		public string NativeName { get; set; }
	}

	public class CurrencyDto
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }

		public double ConvertTo(CurrencyDto destinationCurrency, double amount = 1.0)
		{
			if (Code == destinationCurrency.Code)
				return amount;

			var api = new GeoApiManager();
			var currencyConversion = Task.Run(async () => await api.ConvertCurrency(Code, destinationCurrency.Code, amount));
			return currencyConversion.Result;
		}
	}
}
