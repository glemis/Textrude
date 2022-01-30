using Scriban.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace MyCompany.ScribanMethodsObject
{
    public class ArrayMethods:ScriptObject
    {

        public static ScriptArray MakeTree(ScriptArray array, string parentProperty, string primaryKey)
        {
            ScriptArray scriptArray = new ScriptArray();

            var roots = array.Where(i => ((ScriptObject)i)[parentProperty] == null).Select(i => ((ScriptObject)i)[primaryKey]).Distinct();
            foreach (var root in roots)
            {
                ScriptObject tree = new()
                {
                    ["key"] = root,
                    ["value"] = array.FirstOrDefault(g => ((ScriptObject)g)[primaryKey] == root),
                    ["descendents"] = new ScriptArray(),
                    ["depth"] = 0
                };
                AddDescendents(tree, array, parentProperty,primaryKey,0);
                scriptArray.Add(tree);
            }

            return scriptArray;
        }

        private static void AddDescendents(ScriptObject parent, ScriptArray array, string parentProperty, string primaryKey, int treeDepth)
        {
            //Get distinct child groupings (assuming a child can have multiple rows to accomidate a joined table
            //Based on if parents key is in childs parent property. 
            var descendents = array.Where(i=> ((ScriptObject)i)[parentProperty]?.Equals(parent["key"]) == true).Select(i => ((ScriptObject)i)[primaryKey]).Distinct();
            foreach (var descendent in descendents)
            {
                
                ScriptObject tree = new()
                {
                    ["key"] = descendent,
                    ["value"] = array.FirstOrDefault(i=> ((ScriptObject)i)[primaryKey] == descendent),
                    ["descendents"] = new ScriptArray(),
                    ["depth"] = treeDepth+1
                };
                AddDescendents(tree, array, parentProperty, primaryKey, treeDepth + 1);
                ((ScriptArray)parent["descendents"]).Add(tree);
            }
        }

        public static IEnumerable GroupBy(IEnumerable list, string propertyName)
        {
            if (list.GetType() == typeof(ScriptArray))
            {
                var groupings = ((ScriptArray)list).GroupBy(i => ((ScriptObject)i)[propertyName]);
                var newList = new ScriptArray();
                foreach (var group in groupings)
                {
                    yield return new ScriptObject()
                    {
                        ["group"] = group.Key,
                        ["items"] = group.ToList()
                    };
                }
            }
            else if(list.GetType() == typeof(ScriptRange))
            {
                var groupings = ((ScriptRange)list).GroupBy(i => ((ScriptObject)i)[propertyName]);
                foreach (var group in groupings)
                {
                    yield return new ScriptObject()
                    {
                        ["group"] = group.Key,
                        ["items"] = group.ToList()
                    };
                    
                }
            }
        }

        public static IEnumerable CollapseDates(IEnumerable list, string datePropertyName, string language = "en-us")
        {
           
            //Build smaller list of distinct sibling rows with similar dates. 

            //If dates are in same month join with ',' if only 2 records join instead with 'and' or 'et' in french
            //If month changes add a ';' and list new months name.

            //Loop through list 



            //Goal is to create new list by grouping items where all columns are equal other then the date column. 
            //The dates will be moved into an array and placed in the new list.

            //In loop push current items date to pendingDates
            //Push current item (clone) into past item
            //If current does not match past item push dates onto passed item and add to list

            //Get first item
            var Enumerator = list.GetEnumerator();
            if (!Enumerator.MoveNext())
            {
                yield break;
            }


            var Items = new List<ScriptObject>();
            var previousItem = (ScriptObject)((ScriptObject)Enumerator.Current).Clone(true);
            var datesPending = new List<DateTime>();
            //Add pending dates
            if((DateTime?)previousItem[datePropertyName] != null)
            {
                datesPending.Add((DateTime)previousItem[datePropertyName]);
                previousItem.Remove(datePropertyName);
            }
            while (Enumerator.MoveNext())
            {
                var currentItem = (ScriptObject)((ScriptObject)Enumerator.Current).Clone(true);
                var currentDate = (DateTime?)currentItem[datePropertyName];
                currentItem.Remove(datePropertyName);

                //If previous is same as current then just add to dates
                if (previousItem.ToString() == currentItem.ToString())
                {
                    if (currentDate.HasValue)
                    {
                        datesPending.Add(currentDate.Value);
                    }
                }
                //If they are not equal
                else
                {
                    //add the previous into new list along with any dates accumulated so far
                    previousItem["Dates"] = new List<DateTime>(datesPending);
                    previousItem["DateString"] = DatesToCollapsedString((List<DateTime>)previousItem["Dates"], language);
                    yield return previousItem;

                    //Will now start new list of dates using current
                    datesPending.Clear();
                    if (currentDate.HasValue)
                    {
                        datesPending.Add(currentDate.Value);
                    }
                }

                //Always then push current to previous
                previousItem = currentItem;
            }
            //When finished last row was not added so need to add
            previousItem["Dates"] = new List<DateTime>(datesPending);
            previousItem["DateString"] = DatesToCollapsedString((List<DateTime>)previousItem["Dates"], language);
            yield return previousItem;
        }

        private static string DatesToCollapsedString(List<DateTime> dates, string language)
        {
            var ci = CultureInfo.GetCultureInfo(language);
            var dateString = "";
            for (var k = 0; k < dates.Count; k++)
            {
                //If the first item just add MMMM dd
                if (k == 0)
                {
                    dateString += dates[k].ToString("MMMM d", ci);
                }
                else if (dates[k] != dates[k - 1])
                {
                    //If month changes start seperator ';' then month day
                    if (dates[k - 1].Month != dates[k].Month)
                    {
                        //If year changes add year before next date
                        if (dates[k - 1].Year != dates[k].Year)
                        {
                            dateString += " " + dates[k].Year + ". ; ";
                        }
                        dateString += "; " + dates[k].ToString("MMMM d", ci);
                    }
                    //If month didn't change just adding day part
                    else
                    {
                        if (dates.Where(d => d.Month == dates[k].Month).Count() == 2)
                        {
                            //only 2 dates in month use "and" instead of ","
                            if (language.Contains("fr"))
                            {
                                dateString += " et " + dates[k].Day;
                            }
                            else
                            {
                                dateString += " and " + dates[k].Day;
                            }

                        }
                        else
                        {
                            dateString += ", " + dates[k].Day;
                        }

                    }
                }

            }
            if (dates.Count > 0)
            {
                dateString += ", " + dates.Last().Year + ".";
            }

            return dateString;
        }

        public static IEnumerable FilterBy(IEnumerable list, string propertyName, dynamic value)
        {
            foreach (var item in list)
            {
                if (((ScriptObject)item)[propertyName].Equals(value))
                {
                    yield return item;
                }
            }
        }

    }
}
