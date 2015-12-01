using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Herschel.Ws.Util
{
    public class WebControls
    {
        public static Control FindControl(Control parent, string name)
        {
            Control c;

            c = parent.FindControl(name);

            if (c != null)
            {
                return c;
            }
            else if (parent.HasControls())
            {
                foreach (Control ci in parent.Controls)
                {
                    c = FindControl(ci, name);

                    if (c != null)
                    {
                        return c;
                    }
                }
            }

            return null;
        }
    }
}