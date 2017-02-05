using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swift.Net.Crawlers;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Swift.Net.Core.Controllers.api
{
    [Route("api/[controller]")]
    public class ToolsController : Controller
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
        public async Task<IActionResult> GetDomainsForNameServer(string ns)
        {
            try
            {
                var domains = await ViewDNS.CrawlReverseDomainsforNameServer(ns);

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
        public async Task<IActionResult> PingIsSiteDown(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.ContinueTimeout = 3000;
                request.Method = "HEAD";

                using (var response = await request.GetResponseAsync())
                {
                    return Ok(true);
                }
            }
            catch
            {
                return Ok(false);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("CrawlIsSiteDown")]
        public async Task<IActionResult> CrawlIsSiteDown(string domain)
        {
            try
            {
                var result = await ViewDNS.CrawlIsSiteDown(domain);

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
        public async Task<IActionResult> GetAbuseContact(string domain)
        {
            try
            {
                var result = await ViewDNS.CrawlAbuseContact(domain);

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
