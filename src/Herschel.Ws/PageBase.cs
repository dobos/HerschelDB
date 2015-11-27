using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Herschel.Ws
{
    public class PageBase : Page
    {
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                var scriptManager = ScriptManager.GetCurrent(this);
                if (scriptManager != null)
                {
                    Util.JQuery.Register(scriptManager);
                }
                else
                {
                    throw new ApplicationException("You must have a ScriptManager on the Page.");
                }
            }

            base.OnPreRender(e);
        }
    }
}