using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSEditQueryStringAspNetCore
{
    public static class Extension
    {
        public static void CutStringByFirstKey(this string sourceString, string key, out string str1, out string str2)
        {
            int index = sourceString.IndexOf(key);

            if (index == -1)
            {
                str1 = sourceString;
                str2 = string.Empty;
            }
            else
            {
                str1 = sourceString.Substring(0, index);
                str2 = sourceString.Substring(index + 1);
            }
        }
    }
}
