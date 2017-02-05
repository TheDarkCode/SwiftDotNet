using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Swift.Net.WebAPI.Entities.Analytics;
using Swift.Net.WebAPI.Hubs;
using Swift.Net.WebAPI.Models;
using Swift.Net.WebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Swift.Net.WebAPI.Controllers.api
{
    [RoutePrefix("api/Analytics")]
    public class AnalyticsController : ApiController
    {

        #region ctors

        private readonly IAnalyticRepository _repo;
        private readonly IHubContext _context;
        private static HttpClient _client = new HttpClient();

        public AnalyticsController(IAnalyticRepository repo)
        {
            this._repo = repo;
            this._context = GlobalHost.ConnectionManager.GetHubContext<AnalyticsHub>();
        }

        #endregion

        #region Standard CRUD

        /// <summary>
        /// Gets a list of all Analytics in the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Analytic> GetAllInCollection()
        {
            var result = _repo.Get();

            return result;
        }

        /// <summary>
        /// Gets an Analytic in the Database by Id.
        /// </summary>
        /// <param name="id">The Analytic's GUID.</param>
        /// <returns></returns>
        public Task<Analytic> GetById(string id)
        {
            return _repo.GetById(id);
        }

        /// <summary>
        /// Creates a new Analytic in the Database and passes it to the AnalyticsHub.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        public async Task<IHttpActionResult> Post([FromBody] Analytic entity)
        {
            var result = await _repo.CreateDocumentAsync(entity);
            var id = result.Resource.Id;
            var model = _repo.GetById(id);

            var hubEventParameters = new
            {
                Analytic = entity
            };

            _context.Clients.All.locationReceived(hubEventParameters);

            return Ok(model);
        }

        /// <summary>
        /// Updates an existing Analytic in the Database and passes it to the AnalyticsHub.
        /// </summary>
        /// <param name="id">The Analytic's GUID.</param>
        /// <param name="entity"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [System.Web.Http.Authorize]
        public async Task<IHttpActionResult> Put(string id, [FromBody]Analytic entity)
        {
            await _repo.UpdateDocumentAsync(entity);
            var model = _repo.GetById(id);

            // Send Analytic Put to All Clients
            _context.Clients.All.putNewAnalytic(entity);

            return Ok(model);
        }

        /// <summary>
        /// Deletes an existing Analytic in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An HTTP Status code - 200 (OK) or 400 (Bad Request)</returns>
        [System.Web.Http.Authorize]
        public async Task<IHttpActionResult> Delete(string id)
        {
            await _repo.DeleteDocumentAsync(id);

            // Remove Analytic from All Clients
            _context.Clients.All.deleteAnalytic(id);

            return Ok();
        }

        /// <summary>
        /// Returns the IP Address and Browser Info of the requester.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Ping")]
        public async Task<HttpResponseMessage> EchoIP()
        {
            var userIP = await GetIP();

            var bc = HttpContext.Current.Request.Browser;

            var browserType = bc.Type.ToString();
            var browser = bc.Browser.ToString();
            var version = bc.Version.ToString();
            var platform = bc.Platform.ToString();
            var isMobile = bc.IsMobileDevice;

            var output = "Your IP is: " + userIP + ". Your browser is of type: " + browserType + ". Which is the browser: " + browser + ". Running version: " + version + ". On the platform: " + platform + ".";

            if (isMobile)
            {
                output += " Which is a mobile device.";
            }
            else
            {
                output += " Which is not a mobile device.";
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(
                    output,
                    Encoding.UTF8,
                    "application/json"
                )
            };

        }

        /// <summary>
        /// Returns the Location for the requester.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Location")]
        public async Task<UserLocation> GetLocation()
        {
            var location = await GetLocationAsync();
            var userLocation = new UserLocation()
            {
                ipaddress = location.geobytesipaddress,
                certainty = location.geobytescertainty,
                internet = location.geobytesinternet,
                country = location.geobytescountry,
                regionlocationcode = location.geobytesregionlocationcode,
                region = location.geobytesregion,
                code = location.geobytescode,
                locationcode = location.geobyteslocationcode,
                dma = location.geobytesdma,
                city = location.geobytescity,
                cityid = location.geobytescityid,
                fqcn = location.geobytesfqcn,
                latitude = location.geobyteslatitude,
                longitude = location.geobyteslongitude,
                capital = location.geobytescapital,
                timezone = location.geobytestimezone,
                nationalitysingular = location.geobytesnationalitysingular,
                population = location.geobytespopulation,
                nationalityplural = location.geobytesnationalityplural,
                mapreference = location.geobytesmapreference,
                currency = location.geobytescurrency,
                currencycode = location.geobytescurrencycode,
                title = location.geobytestitle
            };

            return userLocation;
        }

        /// <summary>
        /// Returns the UserDetails of the requester.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("Echo")]
        public async Task<UserDetails> GetUserDetails()
        {
            var location = await GetLocationAsync();
            var userLocation = new UserLocation()
            {
                ipaddress = location.geobytesipaddress,
                certainty = location.geobytescertainty,
                internet = location.geobytesinternet,
                country = location.geobytescountry,
                regionlocationcode = location.geobyteslocationcode,
                code = location.geobytescode,
                locationcode = location.geobyteslocationcode,
                dma = location.geobytesdma,
                city = location.geobytescity,
                cityid = location.geobytescityid,
                fqcn = location.geobytesfqcn,
                latitude = location.geobyteslatitude,
                longitude = location.geobyteslongitude,
                capital = location.geobytescapital,
                timezone = location.geobytestimezone,
                nationalitysingular = location.geobytesnationalitysingular,
                population = location.geobytespopulation,
                nationalityplural = location.geobytesnationalityplural,
                mapreference = location.geobytesmapreference,
                currency = location.geobytescurrency,
                currencycode = location.geobytescurrencycode,
                title = location.geobytestitle
            };

            var bc = HttpContext.Current.Request.Browser;
            var browserType = bc.Type.ToString();
            var browser = bc.Browser.ToString();
            var version = bc.Version.ToString();
            var platform = bc.Platform.ToString();
            var isMobile = bc.IsMobileDevice;
            var supportsFrames = bc.Frames;
            var supportsTables = bc.Tables;
            var supportsCookies = bc.Cookies;
            var supportsJS = (bc.EcmaScriptVersion.Major >= 1);

            var userDetails = new UserDetails()
            {
                browsertype = browserType,
                browser = browser,
                version = version,
                platform = platform,
                ismobile = isMobile,
                supports_frames = supportsFrames,
                supports_tables = supportsTables,
                supports_cookies = supportsCookies,
                supports_javascript = supportsJS,
                location = userLocation
            };

            return userDetails;
        }

        #endregion

        #region Private Methods

        private async Task<GeobytesLocation> GetLocationAsync()
        {

            var userIP = await GetIP();

            var locStr = "http://geobytes.dryverlessads.com/GetCityDetails?ipaddress=" + userIP;

            Task<HttpResponseMessage> locationTask = _client.GetAsync(locStr);
            var locResult = await locationTask;

            var locString = await locResult.Content.ReadAsStringAsync();

            GeobytesLocation clientLocation = await JsonConvert.DeserializeObjectAsync<GeobytesLocation>(locString);

            return clientLocation;
        }

        private async Task<string> GetIP()
        {
            string userIP = Request.Headers.GetValues("CF-CONNECTING-IP").FirstOrDef‌​ault() != null ? Request.Headers.GetValues("CF-CONNECTING-IP").FirstOrDef‌​ault() : Request.GetOwinContext().Request.RemoteIpAddress?.ToString();

            return userIP;
        }

        #endregion
    }
}
