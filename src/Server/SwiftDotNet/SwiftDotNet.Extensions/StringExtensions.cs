using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SwiftDotNet.Extensions
{
    public static class StringExtensions
    {
        public static bool IsIPv6(this string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                switch (ip.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        return false;
                    //break;
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        return true;
                    //break;
                    default:
                        // Error
                        return false;
                        //break;
                }
            }

            // Fallback
            return ipAddress.Contains(':');
        }
        public static bool IsIPv4(this string ipAddress)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
            {
                switch (ip.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        return true;
                    //break;
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        return false;
                    //break;
                    default:
                        // Error
                        return false;
                        //break;
                }
            }

            // Fallback
            return ipAddress.Contains('.');
        }
        public static bool IsValidIP(this string ipAddress)
        {
            IPAddress ip;
            return IPAddress.TryParse(ipAddress, out ip);
        }
    }
}
