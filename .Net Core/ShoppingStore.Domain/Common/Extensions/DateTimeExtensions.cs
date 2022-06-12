using System;
using System.Collections.Generic;
using MD.PersianDateTime.Standard;

namespace ShoppingStore.Domain.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertShamsiToMiladi(this string date)
        {
            PersianDateTime persianDateTime = PersianDateTime.Parse(date);
            return persianDateTime.ToDateTime();
        }

        public static string ConvertMiladiToShamsi(this DateTime? date, string format)
        {
            var persianDateTime = new PersianDateTime(date);
            return persianDateTime.ToString(format);
        }

        public static string DateTimeEn2Fa(this DateTime? date, string format)
        {
            if (date != null && date != (new DateTime(01, 01, 01)) && date != (new DateTime(01, 01, 01, 00, 00, 00)))
                return date.ConvertMiladiToShamsi(format).En2Fa();
            return null;
        }

        public static bool IsLeapYear(this DateTime? date)
        {
            PersianDateTime persianDateTime = new PersianDateTime(date);
            return persianDateTime.IsLeapYear;
        }

        public static DateTimeResult CheckShamsiDateTime(this string date)
        {
            try
            {
                DateTime miladiDate = PersianDateTime.Parse(date).ToDateTime();
                return new DateTimeResult { MiladiDate = miladiDate, IsShamsi = true };
            }
            catch
            {
                return new DateTimeResult { IsShamsi = false };
            }
        }

        public static List<DateTime?> GetDateTimeForSearch(this string searchText)
        {
            DateTime? startDateTime = Convert.ToDateTime("01/01/01");
            DateTime? endDateTime = Convert.ToDateTime("01/01/01");
            var dateTimeResult = searchText.CheckShamsiDateTime();

            if (dateTimeResult.IsShamsi)
            {
                startDateTime = dateTimeResult.MiladiDate;
                if (searchText.Contains(":"))
                    endDateTime = startDateTime;
                else
                    endDateTime = startDateTime.Value.Date + new TimeSpan(23, 59, 59);
            }

            return new List<DateTime?>() { startDateTime, endDateTime };
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime GetNextWeekday(this DateTime dt, DayOfWeek day)
        {
            var currentDay = (int)dt.DayOfWeek;
            var diff = (int)day - currentDay;

            var nextWeekday = dt.AddDays(1);
            if (diff <= 0)
                nextWeekday.AddDays(7);

            while (nextWeekday.DayOfWeek != day)
                nextWeekday = nextWeekday.AddDays(1);
            return nextWeekday;
        }

        public static List<DateTime> GetFridays(this DateTime startDate, DateTime endDate)
        {
            var fridays = new List<DateTime>();
            if(startDate != null && endDate != null)
            {
                while (startDate.Date <= endDate.Date)
                {
                    if (startDate.DayOfWeek == DayOfWeek.Friday)
                        fridays.Add(startDate);
                    startDate = startDate.AddDays(1);
                }
            }
            return fridays;
        }

        public static DateTime JustDate(DateTime dateTime) => dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerHour)).Date;
        public static DateTime DateTimeWithOutMilliSecends(DateTime dateTime) => dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
    }

    public class DateTimeResult
    {
        public bool IsShamsi { get; set; }
        public string searchText { get; set; }
        public DateTime? MiladiDate { get; set; }
    }
}
