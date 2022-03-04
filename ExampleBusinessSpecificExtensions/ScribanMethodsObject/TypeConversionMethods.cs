using Newtonsoft.Json;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace OLA.ScribanMethodsObject
{
    public class TypeConversionMethods:ScriptObject
    {
        /// <summary>
        /// Converts a UTC date to a specific local time, default 'Eastern Standard Time'
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime ConvertToLocal(dynamic date, string timeZone = "Eastern Standard Time")
        {
            if (date == null)
            {
                throw new Exception("Called convert to local on null value.");
            }
            var dateUtc = DateTime.SpecifyKind((DateTime)date, DateTimeKind.Utc);

            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return (DateTime)TimeZoneInfo.ConvertTimeFromUtc(dateUtc, timeZoneInfo);
        }

        /// <summary>
        /// Produces a date from a string. If value is null throw an error.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime AsDate(dynamic date)
        {
            if (date == null)
            {
                throw new Exception("Called as_date on null value.");
            }

            if (date.GetType().Name == "String")
            {
                return DateTime.Parse(date);
            }
            else
            {
                return (DateTime)date;
            }
        }

        /// <summary>
        /// Takes any value and outputs string representation.
        /// Usually used on a json object to output as string in a Javascript file or for testing.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string AsString(dynamic item)
        {
            if (item.GetType().Name == "ScriptArray" || item.GetType().Name == "ScriptObject")
            {
                return JsonConvert.SerializeObject(item, new[] { new OLA.Conversion.ScriptObjectConverter() });
            }
            else
            {
                return Convert.ToString(item);
            }
        }
    }
}
