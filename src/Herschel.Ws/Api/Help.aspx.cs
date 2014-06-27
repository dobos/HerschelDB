using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Herschel.Ws.Api
{
    public partial class Help : System.Web.UI.Page
    {
        class OperationParameter
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        class OperationDescription
        {
            public string Name { get; set; }
            public string UriTemplate { get; set; }
            public string Description { get; set; }
            public List<OperationParameter> Parameters { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var ops = new List<OperationDescription>();

            serviceUrl.Text = Util.Url.ToAbsolute("~/Api/Observations");

            foreach (var method in typeof(ISearch).GetMethods())
            {
                var attr = method.GetCustomAttributes(typeof(OperationContractAttribute), true);

                if (attr != null && attr.Length > 0)
                {
                    var op = new OperationDescription()
                    {
                        Name = method.Name
                    };

                    attr = method.GetCustomAttributes(typeof(DescriptionAttribute), true);
                    if (attr != null && attr.Length > 0)
                    {
                        op.Description = ((DescriptionAttribute)attr[0]).Description;
                    }

                    attr = method.GetCustomAttributes(typeof(WebGetAttribute), true);
                    if (attr != null && attr.Length > 0)
                    {
                        op.UriTemplate = Util.Url.ToAbsolute("~/Api/") + ((WebGetAttribute)attr[0]).UriTemplate;
                    }

                    ops.Add(op);

                    op.Parameters = new List<OperationParameter>();

                    foreach (var par in method.GetParameters())
                    {
                        var oppar = new OperationParameter()
                        {
                            Name = par.Name
                        };

                        attr = par.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        if (attr != null && attr.Length > 0)
                        {
                            oppar.Description = ((DescriptionAttribute)attr[0]).Description;
                        }

                        op.Parameters.Add(oppar);
                    }
                }
            }

            operationList.DataSource = ops;
            operationList.DataBind();
        }

        /*
        protected void operationList_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var parameterList = (ListView)e.Item.FindControl("parameterList");
            parameterList.DataSource
        }*/
    }
}