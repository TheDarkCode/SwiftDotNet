using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift.Net.Extensions
{
    public static class IntExtensions
    {
        public static int RoundUp(this int input, int modulus = 10)
        {
            return (modulus - input % modulus) + input;
        }

        public static int RoundDown(this int input, int modulus = 10)
        {
            return input - input % modulus;
        }
    }
}
