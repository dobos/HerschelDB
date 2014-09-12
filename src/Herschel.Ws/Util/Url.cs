using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Herschel.Ws.Util
{
    public static class Url
    {
        public static string ToAbsolute(this string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
            {
                return relativeUrl;
            }

            if (HttpContext.Current == null)
            {
                return relativeUrl;
            }

            if (relativeUrl.StartsWith("/"))
            {
                relativeUrl = relativeUrl.Insert(0, "~");
            }

            if (!relativeUrl.StartsWith("~/"))
            {
                relativeUrl = relativeUrl.Insert(0, "~/");
            }

            var url = HttpContext.Current.Request.Url;
            var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

            return string.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl));
        }
    }
}