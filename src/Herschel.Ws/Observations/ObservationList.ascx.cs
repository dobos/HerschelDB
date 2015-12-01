using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Herschel.Ws.Observations
{
    public partial class ObservationList : ObservationWebControl
    {
        #region Properties

        public HashSet<string> SelectedDataKeys
        {
            get { return observationList.SelectedDataKeys; }
        }

        #endregion
        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Visible)
            {
                observationList.DataBind();
            }
        }

        protected void observationDataSource_ObjectCreating(object sender, ObjectDataSourceEventArgs e)
        {
            var sf = GetSearchForm();
            e.ObjectInstance = sf.GetSearchObject();
        }

        protected void observationListValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = observationList.SelectedDataKeys.Count > 0;
        }

        #endregion
    }
}