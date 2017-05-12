using System.Collections.Generic;

namespace TimaticApi
{
	public class VisaRequestProfile
	{

		public string Ref { get; set; } // ref=7f2e16c71fd62c1a31e9ec6563504434
		public string DocumentType { get; set; } // pvh_documenttype: passport
		public string DestinationCountryCode { get; set; } // pvh_destinationcountrycode: LK
		public string NationalityCode { get; set; } // pvh_nationalitycode: IT
													//public String ResidentCountryCode { get; set } // pvh_residentcountrycode: IT


		public IEnumerable<KeyValuePair<string, string>> GetRequestParameters()
		{
			return new[]{
					new KeyValuePair<string, string>("ref",Ref),
					new KeyValuePair<string, string>("pvh_destinationcountrycode",DestinationCountryCode),
					new KeyValuePair<string, string>("pvh_documenttype",DocumentType),
					new KeyValuePair<string, string>("pvh_nationalitycode",NationalityCode)
			};
		}
	}
}
