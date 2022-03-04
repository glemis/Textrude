using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace OLA.ScribanMethodsObject
{
    public class LanguageMethods: ScriptObject
    {
        public static string PrintByLanguage(TemplateContext templateContext ,string english,string french)
        {
            if ((templateContext.GetValue(new ScriptVariableGlobal("lang"))) as string == "FR")
            {
                return french;
            }
            else
            {
                return english;
            }
        }

        public static string PrintByGender(int gender, string male, string female, string other)
        {
            if (gender == 1 )
            {
                return male;
            }
            else if(gender == 2)
            {
                return female;
            }
            else
            {
                return other;
            }
        }
    }
}
