using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Net;
using System.Xml;

namespace Herschel.Ws.Util
{
    public static class Astro
    {
        public static bool TryParseCoordinates(string value, out double ra, out double dec)
        {
            ra = dec = double.NaN;

            // First identify parts seperated by whitespaces, colons, semicolons,
            // anything that's not used in degree notation

            var parts = value.Split(new char[] { ' ', '\t', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                // Coordinates have two parts

                return false;
            }

            // Now try to parse coordinates as decimal values

            if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out ra) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out dec))
            {
                // Coordinates are decimal numbers

                return true;
            }

            // Now try to interpret them as HMS and DMS values

            if (TryParseHms(parts[0], out ra) &&
                TryParseDms(parts[1], out dec))
            {
                // Coordinates are indeed HMS and DMS values

                return true;
            }

            // If everything fails, it must be some invalid string (possibly object name)

            return false;
        }

        public static bool TryParseHms(string value, out double ra)
        {
            ra = double.NaN;

            // Break string into parts
            var parts = value.Split(new char[] { ':', 'h', 'H', 'm', 'M', 's', 'S', '\'', '"' });

            // There can be three or four parts. Three parts means fractional seconds
            // are attached to the seconds part, in case of four parts, they're separate, but
            // stich them now and handle them together

            if (parts.Length < 3 || parts.Length > 4)
            {
                return false;
            }
            else if (parts.Length == 4)
            {
                // Stich fractional part to seconds
                parts[2] += parts[3];
            }

            // Now we have to parse the three parts only
            int hours, minutes;
            double seconds;

            if (!int.TryParse(parts[0], out hours) ||
                !int.TryParse(parts[1], out minutes) ||
                !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out seconds))
            {
                // parsing any of the three parts failed
                return false;
            }

            // So far so good, but check ranges
            if (hours < -12 || hours > 23)
            {
                return false;
            }

            if (minutes < 0 || minutes > 59)
            {
                return false;
            }

            if (seconds < 0 || seconds >= 60)
            {
                return false;
            }

            // Everything seems fine, convert to degrees

            ra = 15.0 * hours + 0.25 * minutes + 0.25 / 60.0 * seconds;

            return true;
        }

        public static bool TryParseDms(string value, out double dec)
        {
            dec = double.NaN;

            // Break string into parts

            var parts = value.Split(new char[] { ':', '°', '\'', '"' });

            // There can be three or four parts. Three parts means fractional seconds
            // are attached to the seconds part, in case of four parts, they're separate, but
            // stich them now and handle them together

            if (parts.Length < 3 || parts.Length > 4)
            {
                return false;
            }
            else if (parts.Length == 4)
            {
                // Stich fractional part to seconds
                parts[2] += parts[3];
            }

            // Now we have to parse the three parts only
            int degrees, minutes;
            double seconds;

            if (!int.TryParse(parts[0], out degrees) ||
                !int.TryParse(parts[1], out minutes) ||
                !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out seconds))
            {
                // parsing any of the three parts failed
                return false;
            }

            // So far so good, but check ranges
            if (degrees < -90 || degrees > 90)
            {
                return false;
            }

            if (minutes < 0 || minutes > 59)
            {
                return false;
            }

            if (seconds < 0 || seconds >= 60)
            {
                return false;
            }

            // Everything seems fine, convert to degrees

            dec = degrees + minutes / 60.0 + seconds / 3600.0;

            return true;
        }

        public static bool TryResolveObject(string value, out double ra, out double dec)
        {
            ra = dec = double.NaN;

            try
            {
                // Send a web request to Simbad

                var query = HttpUtility.UrlEncode(value);
                var req = HttpWebRequest.Create("http://cdsweb.u-strasbg.fr/cgi-bin/nph-sesame/-oxp/S?" + query);
                var res = (HttpWebResponse)req.GetResponse();

                // Extract XML from response
                var xml = new XmlDocument();
                xml.Load(res.GetResponseStream());

                // Extract coordinates from response
                var jradeg = xml.DocumentElement.SelectSingleNode("//Resolver/jradeg");
                var jdedeg = xml.DocumentElement.SelectSingleNode("//Resolver/jdedeg");

                ra = double.Parse(jradeg.InnerText, CultureInfo.InvariantCulture);
                dec = double.Parse(jdedeg.InnerText, CultureInfo.InvariantCulture);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}