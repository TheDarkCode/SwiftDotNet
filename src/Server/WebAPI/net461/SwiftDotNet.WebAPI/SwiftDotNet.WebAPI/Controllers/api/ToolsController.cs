using SwiftDotNet.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DryverlessAds.AdminApi.Controllers.api
{
    [RoutePrefix("api/Tools")]
    public class ToolsController : ApiController
    {

        #region ctors
        private static HttpClient _client = new HttpClient();

        public ToolsController()
        {
        }

        #endregion

        [HttpGet]
        [AllowAnonymous]
        [Route("ReverseNameserver")]
        public async Task<IHttpActionResult> GetDomainsForNameServer(string ns)
        {
            try
            {
                var domains = await Crawlers.CrawlReverseDomainsforNameServer(ns);

                if (domains == null)
                {
                    return BadRequest("Unable to lookup nameserver, please try again.");
                }

                return Ok(domains);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("IsSiteDown")]
        public async Task<IHttpActionResult> GetIsSiteDown(string domain)
        {
            try
            {
                var result = await Crawlers.CrawlIsSiteDown(domain);

                if (result == null)
                {
                    return BadRequest("Unable to check domain, please try again.");
                }

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("AbuseContact")]
        public async Task<IHttpActionResult> GetAbuseContact(string domain)
        {
            try
            {
                var result = await Crawlers.CrawlAbuseContact(domain);

                if (result == null)
                {
                    return BadRequest("Unable to lookup abuse contact, please check domain and try again.");
                }

                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
