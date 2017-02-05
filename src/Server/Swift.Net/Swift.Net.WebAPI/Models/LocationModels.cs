using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.Net.WebAPI.Models
{
    public class UserLocation
    {
        //public string forwarderfor { get; set; }
        //public string remoteip { get; set; }
        public string ipaddress { get; set; }
        public string certainty { get; set; }
        public string internet { get; set; }
        public string country { get; set; }
        public string regionlocationcode { get; set; }
        public string region { get; set; }
        public string code { get; set; }
        public string locationcode { get; set; }
        public string dma { get; set; }
        public string city { get; set; }
        public string cityid { get; set; }
        public string fqcn { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string capital { get; set; }
        public string timezone { get; set; }
        public string nationalitysingular { get; set; }
        public string population { get; set; }
        public string nationalityplural { get; set; }
        public string mapreference { get; set; }
        public string currency { get; set; }
        public string currencycode { get; set; }
        public string title { get; set; }
    }

    public class UserDetails
    {
        public string browsertype { get; set; }
        public string browser { get; set; }
        public string version { get; set; }
        public string platform { get; set; }
        public bool ismobile { get; set; }
        public bool supports_frames { get; set; }
        public bool supports_tables { get; set; }
        public bool supports_cookies { get; set; }
        public bool supports_javascript { get; set; }
        public UserLocation location { get; set; }

    }
}
