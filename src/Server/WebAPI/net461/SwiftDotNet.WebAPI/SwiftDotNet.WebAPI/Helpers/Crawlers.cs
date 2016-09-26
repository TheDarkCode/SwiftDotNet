using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDotNet.WebAPI
{
    public class Crawlers
    {
        public static async Task<List<string>> CrawlReverseDomainsforNameServer(string nameserver)
        {
            var _webClient = new WebClient();

            var sanitizedNameserver = nameserver.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");

            Uri nameserverUri = new Uri("http://viewdns.info/reversens/?ns=" + sanitizedNameserver);

            var resultString = await _webClient.DownloadStringTaskAsync(nameserverUri);

            if (resultString == null)
            {
                return null;
            }

            var tableString = ExtractString(resultString, "<table border=\"1\"><tr><td>Domain</td></tr>", "</table>").Replace("<tr><td>", "").Replace("</td></tr>", ",");

            string[] domainsArray = tableString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray(); // .Where(x => !string.IsNullOrWhiteSpace(x)) (not needed since always ends with empty)

            _webClient = null;

            return domainsArray.ToList();
        }

        // <img src="/images/error.GIF" height="20" alt="FAILED" data-pin-nopin="true">
        public static async Task<bool> CrawlIsSiteDown(string domain)
        {
            var _webClient = new WebClient();

            var sanitizedDomain = domain.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");

            Uri domainUri = new Uri("http://viewdns.info/ismysitedown/?domain=" + sanitizedDomain);

            var resultString = await _webClient.DownloadStringTaskAsync(domainUri);

            _webClient = null;

            if (resultString == null)
            {
                return true;
            }

            if (resultString.Contains("<img src=\"/images/error.GIF\" height=\"20\" alt=\"FAILED\" data-pin-nopin=\"true\">"))
            {
                return true;
            }
            else if (resultString.Contains("error.GIF"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Crawl Abuse Contact
        public static async Task<string> CrawlAbuseContact(string domain)
        {
            var _webClient = new WebClient();

            var sanitizedDomain = domain.Replace("http://www.", "").Replace("https://www.", "").Replace("http://", "").Replace("https://", "");

            Uri domainUri = new Uri("http://viewdns.info/abuselookup/?domain=" + sanitizedDomain);

            var resultString = await _webClient.DownloadStringTaskAsync(domainUri);

            _webClient = null;

            if (resultString == null)
            {
                return null;
            }

            var contact = ExtractString(resultString, "<br>==============<br><br>", "<br><br>");

            if (contact == null || contact.Length < 1)
            {
                return null;
            }

            return contact;
        }

        #region Private Methods
        private static string ExtractString(string s, string begin, string end)
        {
            int startIndex = s.IndexOf(begin) + begin.Length;
            int endIndex = s.IndexOf(end, startIndex);
            return s.Substring(startIndex, endIndex - startIndex);
        }
        #endregion

    }
}
