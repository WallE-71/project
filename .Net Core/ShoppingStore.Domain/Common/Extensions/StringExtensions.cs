using System;
using System.Collections.Generic;

namespace ShoppingStore.Domain.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value, bool ignoreWhiteSpace = true)
        {
            return ignoreWhiteSpace ? !string.IsNullOrWhiteSpace(value) : !string.IsNullOrEmpty(value);
        }

        public static int ToInt(this string value)
        {
            return Convert.ToInt32(value);
        }

        public static decimal ToDecimal(this string value)
        {
            return Convert.ToDecimal(value);
        }

        public static string ToNumeric(this int value)
        {
            return value.ToString("N0"); //"123,456"
        }

        public static string ToNumeric(this decimal value)
        {
            return value.ToString("N0");
        }

        public static string ToCurrency(this int value)
        {
            return value.ToString("C0");
        }

        public static string ToCurrency(this decimal value)
        {
            return value.ToString("C0");
        }

        public static string En2Fa(this string str)
        {
            return str.Replace("0", "۰")
                .Replace("1", "۱")
                .Replace("2", "۲")
                .Replace("3", "۳")
                .Replace("4", "۴")
                .Replace("5", "۵")
                .Replace("6", "۶")
                .Replace("7", "۷")
                .Replace("8", "۸")
                .Replace("9", "۹");
        }

        public static string Fa2En(this string str)
        {
            return str.Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9")

                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");
        }

        public static string FixPersianChars(this string str)
        {
            return str.Replace("ﮎ", "ک")
                .Replace("ﮏ", "ک")
                .Replace("ﮐ", "ک")
                .Replace("ﮑ", "ک")
                .Replace("ك", "ک")
                .Replace("ي", "ی")
                .Replace(" ", " ")
                .Replace("‌", " ")
                .Replace("ھ", "ه");
        }

        public static string CleanString(this string str)
        {
            return str.Trim().FixPersianChars().Fa2En().NullIfEmpty();
        }

        public static string NullIfEmpty(this string str)
        {
            return str?.Length == 0 ? null : str;
        }

        public static string GenerateId(int numOfCharacter)
        {
            return Guid.NewGuid().ToString("N").Substring(0, numOfCharacter);
        }
        
        public static string CombineWith(this string[] array, char character)
        {
            var newString = "";
            foreach (var item in array)
            {
                if (newString == "")
                    newString = item;
                else
                    newString = newString + character + item;
            }

            return newString;
        }

        public static int GetNumOfWeek(this string week)
        {
            string[] weekArray = { "شنبه", "یکشنبه", "دوشنبه", "سه شنبه", "چهار شنبه", "پنج شنبه", "جمعه" };
            return Array.IndexOf(weekArray, week);
        }

        public static string[] GetMonth()
        {
            string[] month = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return month;
        }

        public static List<long?> GetValueForSearch(this string searchValue)
        {
            try
            {
                long? OfValue = Convert.ToInt64(0);
                long? ToValue = Convert.ToInt64(9999999999);
                var valueResult = searchValue.ToInt().ToNumeric();
                if (valueResult.ToInt() != 0)
                {
                    OfValue = valueResult.ToInt();
                    if (searchValue.Equals(0))
                        ToValue = OfValue;
                    else
                        ToValue = OfValue.Value + new Int64();
                }
                return new List<long?>() { OfValue, ToValue };
            }
            catch(Exception e) { return new List<long?>(); }       
        }
    }

    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) => Convert.FromBase64String(value.Base64Encode());

        public static string Base64Encode(this string value)
        {
            var encode = System.Text.Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(encode);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var decode = Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(decode);
        }
    }
}
