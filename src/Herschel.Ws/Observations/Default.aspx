<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Herschel.Ws.Observations.Default" %>

<%@ Register Src="~/Observations/SearchForm.ascx" TagPrefix="hws" TagName="SearchForm" %>
<%@ Register Src="~/Observations/ObservationList.ascx" TagPrefix="hws" TagName="ObservationList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1>observation search</h1>
    <%-- Form --%>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="observationSearchPanel" Visible="true">
                <table class="block">
                    <tr>
                        <td class="block_left">
                            <div class="form">
                                <hws:SearchForm runat="server" id="searchForm" />
                            </div>
                        </td>
                        <td class="block_buttons">
                            <p class="buttons">
                                <asp:LinkButton runat="server" ID="search" ValidationGroup="search" Text="search >" OnClick="search_Click" />
                            </p>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress runat="server" DisplayAfter="3000">
        <ProgressTemplate>
            <div class="progress">
                <asp:Image runat="server" ImageUrl="~/images/progress.gif" />
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel runat="server" ID="observationListUpdatePanel">
        <ContentTemplate>
            <asp:Panel runat="server" ID="observationListPanel" Visible="false">
                <h2>search results</h2>
                <table class="block">
                    <tr>
                        <td class="block_left">
                            <hws:ObservationList runat="server" id="observationList" SearchFormID="searchForm" />
                        </td>
                        <td class="block_buttons">
                            <p class="buttons">
                                <asp:LinkButton runat="server" ID="save" ValidationGroup="observationList" Text="save >" Visible="false" />
                                <asp:LinkButton runat="server" ID="plot" ValidationGroup="observationList" Text="plot >" OnClick="plot_Click" />
                            </p>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- List --%>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="observationPlotPanel" Visible="false">
                <h2>observation footprint</h2>
                <table class="block">
                    <tr>
                        <td class="block_left">
                            <spherical:PlotCanvas runat="server" ID="canvas" Width="840" Height="450" CssClass="plot" />
                        </td>
                        <td class="block_buttons">
                            <p class="controls">
                                <asp:CheckBox runat="server" ID="plotOutline" Text="Outline" Checked="true" AutoPostBack="true" /><br />
                                <asp:CheckBox runat="server" ID="plotFill" Text="Fill" Checked="true" AutoPostBack="true" /><br />
                            </p>
                            <table>
                                <tr>
                                    <td>
                                        <asp:RadioButton runat="server" GroupName="reduction" Text="Original" AutoPostBack="true" Checked="true" /></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton ID="plotConvexHull" runat="server" GroupName="reduction" Text="Convex Hull" AutoPostBack="true" /></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:RadioButton runat="server" ID="plotReduce" GroupName="reduction" Text="Reduce:" Checked="false" AutoPostBack="true" />
                                        <asp:TextBox runat="server" ID="plotReduceEpsilon" Text="25" Width="36px" AutoPostBack="true" />&nbsp;"
                                        <asp:RegularExpressionValidator runat="server" ID="plotReduceEpsilonFormatValidator" ValidationExpression="[0-9\+\-]+" ControlToValidate="plotReduceEpsilon"
                                            Display="Dynamic" Text="Invalid format." />
                                        <asp:RangeValidator runat="server" ID="plotReduceEpsilonRangeValidator" MinimumValue="10" MaximumValue="250" ControlToValidate="plotReduceEpsilon"
                                            Display="Dynamic" Text="Invalid limit." />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:RadioButtonList runat="server" ID="plotDegreeStyle" AutoPostBack="true">
                                <asp:ListItem Text="Decimal" Value="Decimal" Selected="True" />
                                <asp:ListItem Text="HMS-DMS" Value="Sexagesimal" />
                            </asp:RadioButtonList><br />
                            <p class="controls">
                                <asp:CheckBox runat="server" ID="plotSsos" Text="SSO trajectories" Checked="true" AutoPostBack="true" />
                                <br />
                                <asp:CheckBox runat="server" ID="plotGrid" Text="Grid" Checked="true" AutoPostBack="true" /><br />
                                <asp:CheckBox runat="server" ID="plotQuery" Text="Plot Query" Checked="true" AutoPostBack="true" />
                            </p>
                            <p class="buttons">
                                <asp:HyperLink runat="server" ID="savePlotPdfLink" Text="save pdf >" />
                            </p>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LinkButton runat="server" ID="savePlotPdf" OnClick="savePlotPdf_Click" Text="save pdf" Style="display: none;" />
</asp:Content>
