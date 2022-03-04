using Newtonsoft.Json;
using OLA.Conversion;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OLA.DataAccess
{ 

    public class DataverseData : ScriptObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// <paramref name="fetchMockId"/> Is only used in the authoring app and unit testing. Retrieve for both API and Text/Authoring app need to match mathod signitures.
        /// Purpose is for having the ability to retireve a prefetched version of fetch on file system for testing without hitting databases.
        /// </remarks>
        /// <param name="queryString"></param>
        /// <param name="fetchIdentifier"></param>
        /// <returns></returns>
        public dynamic Retrieve(string queryString, string fetchMockId = "")
        {
            var text = File.ReadAllText(fetchMockId+".json");
            return JsonConvert.DeserializeObject<ScriptObject>(text, new[] { new ScriptObjectConverter() });
        }
    }
}
