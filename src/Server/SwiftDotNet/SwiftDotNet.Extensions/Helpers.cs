using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwiftDotNet.Extensions
{
    public static class Helpers
    {
        /// <summary>
        /// This method is used for DateTime properties to convert the date 
        /// to an Epoch date that can be used for DocumentDB range indexes.
        /// See this article for more information: 
        /// http://azure.microsoft.com/blog/2014/11/19/working-with-dates-in-azure-documentdb-4/
        /// </summary>
        /// <param name="date">The incoming date to convert to epoch integer.</param>
        /// <returns></returns>
        public static int ToEpoch(this DateTime date)
        {
            if (date == null) return int.MinValue;
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan epochTimeSpan = date - epoch;
            return (int)epochTimeSpan.TotalSeconds;
        }
    }
}
