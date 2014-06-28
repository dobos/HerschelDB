<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="Herschel.Ws.Api.Help" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1>rest web service</h1>
    <p>
        Use the REST service to access the footprint database programatically from your data processing and plotting scripts. Certain functions can return data in various formats (json, xml, ascii), while others support ascii only. We suggest using the command-line tool <em>curl</em> for testing. The service base URL is:
        <asp:Label runat="server" ID="serviceUrl" />
    </p>
    <h2>operations</h2>
    <asp:ListView runat="server" ID="operationList">
        <LayoutTemplate>
            <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
        </LayoutTemplate>
        <ItemTemplate>
            <p><b>GET</b> <%# HttpUtility.HtmlEncode((string)Eval("UriTemplate")) %></p>
            <p style="margin-left:30px"><%# Eval("Description") %></p>
            <asp:ListView runat="server" ID="parameterList" DataSource='<%# Eval("Parameters") %>'>
                <LayoutTemplate>
                    <p style="margin-left:30px">
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder" />
                    </p>
                </LayoutTemplate>
                <ItemTemplate>
                    <b><%# Eval("Name") %>:</b> <%# Eval("Description") %><br />
                </ItemTemplate>
            </asp:ListView>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
