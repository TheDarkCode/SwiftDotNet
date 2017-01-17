using Newtonsoft.Json;
using SwiftDotNet.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SwiftDotNet.WebAPI.Extensions
{
    public static class RequestExtensions
    {
        public static string GetIPWithCloudflare(this HttpRequestMessage request)
        {
            string userIP = request.Headers.GetValues("CF-CONNECTING-IP").FirstOrDef‌​ault() != null ? request.Headers.GetValues("CF-CONNECTING-IP").FirstOrDef‌​ault() : "127.0.0.1";

            // Fallback from Cloudflare to X-Forwarded-For
            if (userIP == "127.0.0.1")
            {
                userIP = request.Headers.GetValues("X-Forwarded-For").FirstOrDefault‌​().Split(',').FirstOrDefault() != null ?
                            request.Headers.GetValues("X-Forwarded-For").FirstOrDefault‌​().Split(',').FirstOrDefault() :
                            // Fallback from X-Forwarded-For to HttpContext UserHostAddress If No X-Forwarded-For
                            (request.Properties.ContainsKey("MS_HttpContext") ? ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress : "127.0.0.1");
            }

            return userIP;
        }

        public static string GetIPWithoutCloudflare(this HttpRequestMessage request)
        {

            string userIP = request.Properties.ContainsKey("MS_HttpContext") ? ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress : "127.0.0.1";

            return userIP;
        }

        public static string GetCountryCodeWithCloudflare(this HttpRequestMessage request)
        {
            string userCountry = request.Headers.GetValues("CF-IPCountry").FirstOrDef‌​ault() != null ? request.Headers.GetValues("CF-IPCountry").FirstOrDef‌​ault() : null;

            return userCountry;
        }
        public static async Task<string> GetCountryCodeWithoutCloudflare(this HttpRequestMessage request)
        {
            var client = new HttpClient();

            var userIP = GetIPWithoutCloudflare(request);

            var locStr = "http://geobytes.dryverlessads.com/GetCityDetails?ipaddress=" + userIP;

            Task<HttpResponseMessage> locationTask = client.GetAsync(locStr);
            var locResult = await locationTask;

            var locString = await locResult.Content.ReadAsStringAsync();

            GeobytesLocation clientLocation = await JsonConvert.DeserializeObjectAsync<GeobytesLocation>(locString);

            client = null;

            return clientLocation.geobytescode;
        }
    }
}
