using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GunGameV.Shared
{
    public static class Json
    {
        public static string To(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static object From(string json)
        {
            return JObject.Parse(json);
        }
    }
}
