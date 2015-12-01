<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Herschel.Ws.Observations.Default" %>

<%@ Register Src="~/Observations/SearchForm.ascx" TagPrefix="hws" TagName="SearchForm" %>
<%@ Register Src="~/Observations/ObservationList.ascx" TagPrefix="hws" TagName="ObservationList" %>
<%@ Register Src="~/Observations/FootprintPlot.ascx" TagPrefix="hws" TagName="FootprintPlot" %>


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
            <asp:Panel runat="server" ID="footprintPlotPanel" Visible="false">
                <h2>observation footprint</h2>
                <hws:FootprintPlot runat="server" id="footprintPlot" SearchFormID="searchForm" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
