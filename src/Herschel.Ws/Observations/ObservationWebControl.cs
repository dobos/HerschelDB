using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Herschel.Ws.Observations
{
    public class ObservationWebControl : System.Web.UI.UserControl
    {
        public string SearchFormID { get; set; }

        protected SearchForm GetSearchForm()
        {
            return (SearchForm)Util.WebControls.FindControl(Page, SearchFormID);
        }
    }
}