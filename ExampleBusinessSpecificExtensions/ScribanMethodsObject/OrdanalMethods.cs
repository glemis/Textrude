#nullable enable
using Scriban.Runtime;
using System;
namespace OLA.ScribanMethodsObject
{
    public class OrdanalMethods:ScriptObject
    {
        public static string Ordinal(int number, string lang = "EN")
        {
            int ones = number % 10;
            int tens = (int)Math.Floor(number / 10M) % 10;
            string suffix;
            if (tens == 1)
            {
                suffix = lang != "FR" ? "th" : "e";

            }
            else
            {
                suffix = ones switch
                {
                    1 => lang != "FR" ? "st" : "er",
                    2 => lang != "FR" ? "nd" : "e",
                    3 => lang != "FR" ? "rd" : "e",
                    _ => lang != "FR" ? "th" : "e",
                };
            }
            return String.Format("{0}{1}", number, suffix);
        }
    }
}
