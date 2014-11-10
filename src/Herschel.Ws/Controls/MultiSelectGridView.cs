using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Herschel.Ws.Controls.MultiSelectGridView.js", "text/javascript", PerformSubstitution = true)]

namespace Herschel.Ws.Controls
{
    public class MultiSelectGridView : GridView, IScriptControl
    {
        protected const string ViewStateSelectedDataKeys = "SelectedDataKeys";

        private int selectionFieldIndex = -1;
        private CheckBox selectAllCheckbox;
        private HashSet<string> selectedDataKeys = new HashSet<string>();

        public ListSelectionMode SelectionMode
        {
            get { return (ListSelectionMode)(ViewState["SelectionMode"] ?? ListSelectionMode.Multiple); }
            set { ViewState["SelectionMode"] = value; }
        }

        public HashSet<string> SelectedDataKeys
        {
            get { return selectedDataKeys; }
        }

        private string GetKey(DataKey key)
        {
            string res = "";

            for (int i = 0; i < key.Values.Count; i++)
            {
                if (i > 0)
                {
                    res += "|";
                }

                res += key.Values[i].ToString();
            }

            return res;
        }

        protected override System.Collections.ICollection CreateColumns(PagedDataSource dataSource, bool useDataSource)
        {
            var columns = base.CreateColumns(dataSource, useDataSource);

            // Look for duplicate selection fields
            var found = false;

            int i = 0;
            foreach (DataControlField col in columns)
            {
                if (col is SelectionField)
                {
                    if (found)
                    {
                        throw new InvalidOperationException("MultiSelectGridView can contain one selection column only.");
                    }
                    selectionFieldIndex = i;
                    found = true;
                }

                i++;
            }

            return columns;
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header && selectionFieldIndex != -1 && SelectionMode == ListSelectionMode.Multiple)
            {
                selectAllCheckbox = new CheckBox()
                {
                    ID = "SelectAllCheckBox",
                    AutoPostBack = true,
                    CausesValidation = false
                };

                selectAllCheckbox.CheckedChanged += SelectAllCheckBox_CheckedChanged;

                e.Row.Cells[selectionFieldIndex].Controls.Add(selectAllCheckbox);
            }

            base.OnRowCreated(e);
        }

        void SelectAllCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAllCheckbox.Checked)
            {
                // Select all data items
                foreach (GridViewRow row in Rows)
                {
                    var key = GetKey(DataKeys[row.RowIndex]);
                    if (!selectedDataKeys.Contains(key))
                    {
                        selectedDataKeys.Add(key);
                    }
                }
            }
            else
            {
                SelectedDataKeys.Clear();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                selectedDataKeys = new HashSet<string>();
            }
            else if (Visible)
            {
                switch (SelectionMode)
                {
                    case ListSelectionMode.Single:
                        selectedDataKeys = new HashSet<string>();
                        break;
                    case ListSelectionMode.Multiple:
                        selectedDataKeys = (HashSet<string>)(ViewState[ViewStateSelectedDataKeys] ?? new HashSet<string>());
                        break;
                }

                // Save selection
                foreach (GridViewRow row in Rows)
                {
                    var key = GetKey(DataKeys[row.RowIndex]);
                    var cb = (CheckBox)row.FindControl(SelectionField.DefaultSelectionCheckBoxID);

                    if (cb != null)
                    {
                        if (cb.Checked && !selectedDataKeys.Contains(key))
                        {
                            selectedDataKeys.Add(key);

                            if (SelectionMode == ListSelectionMode.Single)
                            {
                                break;
                            }
                        }

                        if (!cb.Checked && selectedDataKeys.Contains(key))
                        {
                            selectedDataKeys.Remove(key);
                        }
                    }
                }
            }

            ViewState[ViewStateSelectedDataKeys] = selectedDataKeys;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                if (selectAllCheckbox != null)
                {
                    selectAllCheckbox.Checked = SelectedDataKeys.Count > 0;
                }

                var scriptManager = ScriptManager.GetCurrent(this.Page);
                if (scriptManager != null)
                {
                    scriptManager.RegisterScriptControl(this);
                    Util.JQuery.Register(scriptManager);
                }
                else
                {
                    throw new InvalidOperationException("You must have a ScriptManager on the Page.");
                }
            }

            if (DataKeyNames == null)
            {
                throw new InvalidOperationException("DataKeyNames must be set");
            }

            base.OnPreRender(e);

            ApplySelection();
        }

        private void ApplySelection()
        {
            if (selectedDataKeys != null)
            {
                foreach (GridViewRow row in Rows)
                {
                    var key = GetKey(DataKeys[row.RowIndex]);
                    var selected = selectedDataKeys.Contains(key);

                    var cb = row.FindControl(SelectionField.DefaultSelectionCheckBoxID) as CheckBox;

                    if (cb != null)
                    {
                        cb.Checked = selected;
                    }
                }
            }
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            if (Rows.Count > 0)
            {
                var descriptor = new ScriptControlDescriptor("Graywulf.MultiSelectGridView", this.ClientID);
                descriptor.AddProperty("SelectionMode", this.SelectionMode);
                descriptor.AddProperty("SelectionCheckboxID", SelectionField.DefaultSelectionCheckBoxID);
                descriptor.AddProperty("CssClass", this.CssClass);
                descriptor.AddProperty("SelectedRowCssClass", this.SelectedRowStyle.CssClass);

                yield return descriptor;
            }
            else
            {
                yield break;
            }
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (Rows.Count > 0)
            {
                yield return new ScriptReference("Herschel.Ws.Controls.MultiSelectGridView.js", "Herschel.Ws");
            }
            else
            {
                yield break;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
            }

            base.Render(writer);
        }
    }
}