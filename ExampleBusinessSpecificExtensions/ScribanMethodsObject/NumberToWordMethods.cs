#nullable enable
using Scriban.Runtime;
using System;
namespace OLA.ScribanMethodsObject
{
    public class NumberToWordMethods : ScriptObject
    {
        private static String[] units = { "Zero", "First", "Second", "Third",
    "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth", "Tenth", "Eleventh",
    "Twelfth", "Thirteenth", "Fourteenth", "Fifteenth", "Sixteenth",
    "Seventeenth", "Eighteenth", "Nineteenth" };
        private static String[] tens = { "", "", "Twenty", "Thirty", "Forty",
    "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        private static String[] units_french = { "zéro", "un", "deux", "trois",
    "quatre", "cinq", "six", "sept", "huit", "neuf", "dix", "onze",
    "douze", "treize", "quatorze", "quinze", "seize",
    "dix-sept", "dix-huit", "dix-neuf" };
        private static String[] tens_french = { "", "", "vingt", "trente", "quarante",
    "cinquante", "soixante", "soixante", "quatre-vingt", "quatre-vingt-dix" };

        public static string numberToWord(int number, string lang = "EN")
        {

            if (number < 20)
            {
                if (lang != "FR")
                {
                    return units[number];
                }
                else
                {
                    return units_french[number];
                }
            }
            if (number < 100)
            {
                if (lang != "FR")
                {
                    return tens[number / 10] + ((number % 10 > 0) ? "-" + numberToWord(number % 10).ToLower() : "");
                }
                else
                {
                    if (number < 70)
                    {
                        //Tens and units are joined with a hyphen. So, 22 = vingt-deux, 45 = quarante-cinq etc.
                        //If the unit is a 1, then the word et is inserted between tens and units: 21 = vingt et un, 31 = trente et un etc.
                        var joiningCharacter = "-";
                        if (number % 10 == 1)
                        {
                            joiningCharacter = " et ";
                        }
                        return tens_french[number / 10] + ((number % 10 > 0) ? joiningCharacter + numberToWord(number % 10, lang).ToLower() : "");
                    }
                    else if (number < 80)
                    {
                        //These continue on from soixante-neuf: 70 = soixante-dix, 71 = soixante et onze, 72 = soixante-douze, 73 = soixante-treize etc.
                        //Notice the et in 71 which mimics the behaviour of 21, 31 etc.
                        return tens_french[number / 10] + "-" + numberToWord(number % 60, lang).ToLower();
                    }
                    else
                    {
                        //The French for eighty is quatre-vingts.
                        //Numbers 81-99 consist of quatre-vingt- (minus the -s) plus a number 1-19: 81 = quatre-vingt-un, 82 = quatre-vingt-deux, 90 = quatre-vingt-dix, 91 = quatre-vingt-onze etc.
                        //Notice that none of these numbers use the word et.
                        if (number == 80)
                        {
                            return "quatre-vingts";
                        }
                        else
                        {
                            return tens_french[number / 10] + "-" + numberToWord(number % 80, lang).ToLower();
                        }
                    }
                }
            }
            return "number to large";
        }
    }
}
