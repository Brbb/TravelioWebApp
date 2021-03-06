﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TimaticApi
{
    public class TimaticManager
    {
        private string _token;

        public TimaticManager(string token)
        {
            _token = token;
        }


		private static string timaticBaseUri = "https://www.timaticweb2.com/integration/external.php";
		static HttpClient client;

        private async Task<string> SendVisaRequirementsRequest(TimaticRequestProfile visaRequestProfile)
		{
			using (client = new HttpClient())
			{
				//pvh_destinationcountrycode = LK & pvh_documenttype = passport & pvh_nationalitycode = IT

				var fuec = new FormUrlEncodedContent(visaRequestProfile.GetRequestParameters());

				HttpResponseMessage response = await client.PostAsync(timaticBaseUri, fuec);
				var responseString = string.Empty;
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsStringAsync();
				}

				return responseString;
			}
		}

        public async Task<string> GetTravelRequirements(string departureCountryCode, string destinationCountryCode)
        {
            var visaRequestProfile = new TimaticRequestProfile()
            {
                Ref = _token,
                DocumentType = "passport",
                DestinationCountryCode = destinationCountryCode,
                NationalityCode = departureCountryCode
            };
            return await Task.Run(() => SendVisaRequirementsRequest(visaRequestProfile));
        }
    }
}
