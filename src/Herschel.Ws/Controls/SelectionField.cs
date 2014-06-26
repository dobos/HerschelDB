using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Herschel.Ws.Controls
{
    public class SelectionField : CheckBoxField
    {
        public const string DefaultSelectionCheckBoxID = "SelectionCheckBox";

        public SelectionField()
        {
        }

        protected override DataControlField CreateField()
        {
            return new SelectionField();
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            base.InitializeDataCell(cell, rowState);

            if (cell.Controls.Count == 0)
            {
                CheckBox chk = new CheckBox();
                chk.ID = SelectionField.DefaultSelectionCheckBoxID;
                cell.Controls.Add(chk);
            }
        }
    }
}