using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftDotNet.Extensions
{
    public static class DateExtensions
    {
        public static int ToUTCEpochForTimeZoneId(this DateTime dt, string id)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(id);
            return dt.ToEpoch() - (int)(tz.GetUtcOffset(dt).TotalSeconds);
        }
        public static int FromUTCToEpochForTimeZoneId(this DateTime dt, string id)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(id);
            return dt.ToEpoch() + (int)(tz.GetUtcOffset(dt).TotalSeconds);
        }
        public static int ToUTCEpochForTimeZone(this DateTime dt, TimeZoneInfo tz)
        {
            return dt.ToEpoch() - (int)(tz.GetUtcOffset(dt).TotalSeconds);
        }

        public static int FromUTCToEpochForTimeZone(this DateTime dt, TimeZoneInfo tz)
        {
            return dt.ToEpoch() + (int)(tz.GetUtcOffset(dt).TotalSeconds);
        }

        public static int ToUTCEpochForOffset(this DateTime dt, double offset) // could be half hour offset
        {
            return dt.ToEpoch() - (int)(offset * 60 * 60); // hour * 60min/hr * 60s/min = # seconds
        }

        public static int FromUTCToEpochForOffset(this DateTime dt, double offset) // could be half hour offset
        {
            return dt.ToEpoch() + (int)(offset * 60 * 60); // hour * 60min/hr * 60s/min = # seconds
        }

        public static DateTime StartOfMinute(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Minute, 0, 0);
        }

        public static DateTime EndOfMinute(this DateTime dt)
        {
            return dt.StartOfMinute().AddMinutes(1).AddMilliseconds(-3);
        }

        public static DateTime StartOfQuarterHour(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute.RoundDown(15), 0);
        }

        public static DateTime EndOfQuarterHour(this DateTime dt)
        {
            return dt.StartOfQuarterHour().AddHours(0.25).AddMilliseconds(-3);
        }

        public static DateTime StartOfHalfHour(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute.RoundDown(30), 0);
        }

        public static DateTime EndOfHalfHour(this DateTime dt)
        {
            return dt.StartOfHalfHour().AddHours(0.5).AddMilliseconds(-3);
        }

        public static DateTime StartOfHour(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);
        }

        public static DateTime EndOfHour(this DateTime dt)
        {
            return dt.StartOfHour().AddHours(1).AddMilliseconds(-3);
        }

        public static DateTime StartOfDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        public static DateTime EndOfDay(this DateTime dt)
        {
            return dt.StartOfDay().AddDays(1).AddMilliseconds(-3);
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Sunday)
        {
            return dt.StartOfWeek(startOfWeek).AddDays(7).AddMilliseconds(-3);
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            return dt.StartOfMonth().AddMonths(1).AddMilliseconds(-3);
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        public static DateTime EndOfYear(this DateTime dt)
        {
            return dt.StartOfYear().AddYears(1).AddMilliseconds(-3);
        }

        public static DateTime StartOfDecade(this DateTime dt)
        {
            return new DateTime(dt.Year.RoundDown(10), 1, 1);
        }

        public static DateTime EndOfDecade(this DateTime dt)
        {
            return dt.StartOfDecade().AddYears(10).AddMilliseconds(-3);
        }

        public static DateTime StartOfCentury(this DateTime dt)
        {
            return new DateTime(dt.Year.RoundDown(100), 1, 1);
        }

        public static DateTime EndOfCentury(this DateTime dt)
        {
            return dt.StartOfCentury().AddYears(100).AddMilliseconds(-3);
        }

        public static IEnumerable<DateTime> EachHour(this DateTime from, DateTime thru, int hours = 1)
        {
            for (var hour = from.Date; hour.Date <= thru.Date; hour = hour.AddHours(hours))
                yield return hour;
        }

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

        public static IEnumerable<DateTime> EachHourTo(this DateTime dateFrom, DateTime dateTo)
        {
            return EachHour(dateFrom, dateTo);
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
