<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="Herschel.Ws.Controls.Menu" %>
<a href="http://herschel.esac.esa.int/">herschel home</a>
<asp:Hyperlink runat="server" NavigateUrl="~/Observations">observation search</asp:Hyperlink>
<asp:Hyperlink runat="server" NavigateUrl="~/Api/help">rest api</asp:Hyperlink>
<asp:Hyperlink runat="server" NavigateUrl="~/Docs">docs</asp:Hyperlink>
<asp:Hyperlink runat="server" NavigateUrl="~/Credits">credits</asp:Hyperlink>