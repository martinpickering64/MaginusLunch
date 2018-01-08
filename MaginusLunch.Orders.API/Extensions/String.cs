using System;
using System.Text.RegularExpressions;

namespace MaginusLunch.Orders.API.Extensions
{
    /// <summary>
    /// Extensions of string
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Is the string (obtained from the Request Rouet Data) a representation of a valid delivery date
        /// </summary>
        /// <param name="deliveryDate">the date from the URL Path/Route Data</param>
        /// <returns>true - Looks like a delivery date
        /// false - doesn't look like a delivery date</returns>
        public static bool IsValidDeliveryDate(this string deliveryDate)
        {
            if (string.IsNullOrWhiteSpace(deliveryDate)) return false;
            if (deliveryDate.Length != 8) return false;
            if (!Regex.IsMatch(deliveryDate, @"^20[1|2]{1}[0-9]{1}[0|1]{1}[0-9]{1}[0|1|2|3]{1}[0-9]{1}$")) return false;
            try
            {
                var asDate = deliveryDate.ConvertToDeliveryDate();
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts a string in form of yyyyMMdd to a DateTime
        /// </summary>
        /// <param name="deliveryDate">The string in yyyyMMdd form.</param>
        /// <returns>The equivalent in DteTime form</returns>
        public static DateTime ConvertToDeliveryDate(this string deliveryDate)
        {
            var dateAsNumber = int.Parse(deliveryDate);
            var year = dateAsNumber / 10000;
            var month = (dateAsNumber - (year*10000)) / 100;
            var day = dateAsNumber - (month*100) -( year*10000);
            if (year < 2017 || year > 2029) throw new ArgumentOutOfRangeException("deliveryDate", "Year is not between 2017 and 2029.");
            if (month < 1 || month > 12) throw new ArgumentOutOfRangeException("deliveryDate", "Month is not between 1 and 12");
            if (day < 1 || day > 31) throw new ArgumentOutOfRangeException("deliveryDate", "Day is not between 1 and 31");
            return new DateTime(year, month, day, 0, 0, 0, 0, DateTimeKind.Utc);
        }

    }
}
