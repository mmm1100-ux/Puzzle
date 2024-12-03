using MD.PersianDateTime.Standard;
using System;
using System.Globalization;

namespace Extensions
{
    public static class DateTimeExtensions
    {
        public static PersianDateTime ToShamsi(this DateTime dateTime)
        {
            return new PersianDateTime(dateTime) { PersianNumber = false };
        }

        public static string ToShortDate(this DateTime D)
        {
            PersianCalendar P = new PersianCalendar();

            string Date = P.GetYear(D) + "/" + P.GetMonth(D).ToString("D2") + "/" + P.GetDayOfMonth(D).ToString("D2");

            return Date;
        }

        public static string GetDayOfWeek(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Saturday: return "شنبه";

                case DayOfWeek.Sunday: return "يکشنبه";

                case DayOfWeek.Monday: return "دوشنبه";

                case DayOfWeek.Tuesday: return "سه‏ شنبه";

                case DayOfWeek.Wednesday: return "چهارشنبه";

                case DayOfWeek.Thursday: return "پنجشنبه";

                case DayOfWeek.Friday: return "جمعه";

                default: return "";
            }
        }

        public static string GetElapsedTime(this DateTime Date)
        {
            TimeSpan T = (DateTime.Now - Date);

            if (T.Days >= 365)
            {
                return (T.Days / 365) + " سال پیش ";
            }
            else if (T.Days >= 30)
            {
                return (T.Days / 30) + " ماه پیش ";
            }
            else if (T.Days < 30 & T.Days > 0)
            {
                return (T.Days) + " روز پیش ";
            }

            return "امروز";
        }

        public static string LastActivityToString(this DateTime Date)
        {
            TimeSpan T = (DateTime.Now - Date);

            if (T.Days >= 365)
            {
                return (T.Days / 365) + " سال پیش ";
            }
            else if (T.Days >= 30)
            {
                return (T.Days / 30) + " ماه پیش ";
            }
            else if (T.Days > 0)
            {
                return T.Days + " روز پیش ";
            }
            else if (T.Hours > 0)
            {
                return T.Hours + " ساعت پیش ";
            }
            else if (T.Minutes > 5)
            {
                return T.Minutes + " دقیقه پیش ";
            }

            return "لحظاتی پیش";
        }

        public static string GetMonth(this DateTime Date)
        {
            string[] MonthName = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };

            return MonthName[Date.Month - 1];
        }
    }
}
