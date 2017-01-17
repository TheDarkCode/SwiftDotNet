using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDotNet.WebAPI.Extensions
{
    public static class DateExtensions
    {
        // http://stackoverflow.com/questions/1847580/how-do-i-loop-through-a-date-range (http://stackoverflow.com/a/26055127)
        // Original inspiration from @mquander and @Yogurt The Wise.
        public static IEnumerable<DateTime> EachDay(this DateTime from, DateTime thru, int days = 1)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(days))
                yield return day;
        }

        public static IEnumerable<DateTime> EachMonth(this DateTime from, DateTime thru, int months = 1)
        {
            for (var month = from.Date; month.Date <= thru.Date || month.Month == thru.Month; month = month.AddMonths(months))
                yield return month;
        }

        public static IEnumerable<DateTime> EachDayTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> EachMonthTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> EveryOtherDayTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo, 2);
        }

        public static IEnumerable<DateTime> EveryOtherMonthTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo, 2);
        }

        public static IEnumerable<DateTime> GetEachDay(DateTime from, DateTime thru, int days = 1)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(days))
                yield return day;
        }

        public static IEnumerable<DateTime> GetEachMonth(DateTime from, DateTime thru, int months = 1)
        {
            for (var month = from.Date; month.Date <= thru.Date || month.Month == thru.Month; month = month.AddMonths(months))
                yield return month;
        }

        public static IEnumerable<DateTime> GetEachDayTo(DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> GetEachMonthTo(DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo);
        }

        public static IEnumerable<DateTime> GetEveryOtherDayTo(DateTime dateFrom, DateTime dateTo)
        {
            return EachDay(dateFrom, dateTo, 2);
        }

        public static IEnumerable<DateTime> GetEveryOtherMonthTo(DateTime dateFrom, DateTime dateTo)
        {
            return EachMonth(dateFrom, dateTo, 2);
        }
    }
}
