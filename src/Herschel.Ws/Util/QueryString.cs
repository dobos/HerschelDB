using System;
using System.Collections;
using System.Linq;
using System.Web;

namespace Herschel.Ws.Util
{
    public static class QueryString
    {
        public delegate T ParserDelegate<T>(string value);

        public static string ToList(IEnumerable parts)
        {
            var list = "";
            foreach (var part in parts)
            {
                if (list != "")
                {
                    list += ",";
                }
                list += part.ToString();
            }

            return list;
        }

        public static T[] FromList<T>(string list, ParserDelegate<T> parser)
        {
            var parts = list.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var res = new T[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                res[i] = parser(parts[i]);
            }

            return res;
        }

    }
}