using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace OLA.ScribanMethodsObject
{ 
    public class WeekReturn
    {
        public DateTime Start
        {
            get; set;
        }
        public DateTime End
        {
            get; set;
        }
    }

    public class DateMethods : ScriptObject
    {
        /// <summary>
        /// Takes single date and returns object with start and end of week containing date. 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static ScriptObject GetWeek(DateTime? date)
        {
            if (date == null)
            {
                throw new Exception("Called get_week on null value.");
            }
            var baseDate = date.Value.Date;

            WeekReturn week = new()
            {
                Start = baseDate.AddDays(-(int)baseDate.DayOfWeek + 1)
            };
            week.End = week.Start.AddDays(7).AddSeconds(-1);

            return ScriptObject.From(week);
        }
    }
}
