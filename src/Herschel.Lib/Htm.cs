using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spherical;
using Spherical.Htm;

namespace Herschel.Lib
{
    public class Htm
    {
        public struct HtmRange
        {
            public bool FullOnly;
            public long HtmIDStart;
            public long HtmIDEnd;

            public HtmRange(bool fullOnly, long htmIdStart, long htmIdEnd)
            {
                FullOnly = fullOnly;
                HtmIDStart = htmIdStart;
                HtmIDEnd = htmIdEnd;
            }
        }

        public static Cover GetCover(Region region)
        {
            return GetCoverAdvanced(region, 0.9, 2);
        }

        public static Cover GetCoverAdvanced(Region region, double fraction, double seconds)
        {
            Cover cover = new Cover(region);
            double areaFrac = 0;
            DateTime start = DateTime.Now;
            TimeSpan maxtime = TimeSpan.FromSeconds(seconds);

            for (TimeSpan time = TimeSpan.Zero;
                time < maxtime && areaFrac < fraction;
                time = DateTime.Now - start)
            {
                cover.Step();
                long areaInner = cover.GetPseudoArea(Markup.Inner);
                long areaOuter = cover.GetPseudoArea(Markup.Outer);
                areaFrac = areaInner / (double)(areaOuter);
            }

            return cover;
        }
    }
}
