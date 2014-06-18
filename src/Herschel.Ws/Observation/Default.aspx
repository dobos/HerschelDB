<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Herschel.Ws.Observation.Default" %>

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
                                <table runat="server" class="form">
                                    <tr>
                                        <td class="label">
                                            <asp:Label ID="instrumentLabel" runat="server" Text="Instruments:" />
                                        </td>
                                        <td class="field" style="width: 520px">
                                            <asp:CheckBoxList ID="instrumentList" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="PACS" Text="PACS" Selected="True" />
                                                <asp:ListItem Value="SPIRE" Text="SPIRE" Enabled="false" />
                                                <asp:ListItem Value="PARALLEL" Text="PACS/SPIRE parallel" Enabled="false" />
                                                <asp:ListItem Value="HIFI" Text="HIFI" Enabled="false" />
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            <asp:Label runat="server" ID="searchMethodLabel" Text="Method:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:RadioButtonList ID="searchMethod" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="searchMethod_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Value="Point">Coordinates</asp:ListItem>
                                                <asp:ListItem Value="Intersect">Intersect</asp:ListItem>
                                                <asp:ListItem Value="Cover" Enabled="False">Cover</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="pointTr">
                                        <td class="label">
                                            <asp:Label ID="pointLabel" runat="server" Text="Coordinates:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:TextBox ID="point" runat="server" Text="207.25, -28.4" /></td>
                                    </tr>
                                    <tr runat="server" id="regionTr" visible="false">
                                        <td class="label">
                                            <asp:Label ID="regionLabel" runat="server" Text="Region Description:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:TextBox ID="region" runat="server" TextMode="MultiLine">CIRCLE J2000 207.25 -28.4 20</asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                        <td class="block_buttons">
                            <p class="buttons">
                                <asp:LinkButton runat="server" ID="search" Text="search >" OnClick="search_Click" />
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
            <asp:Panel runat="server" ID="observationListPanel" Visible="false">
                <h2>search results</h2>
                <table class="block">
                    <tr>
                        <td class="block_left">
                            <asp:ObjectDataSource runat="server" ID="observationDataSource" DataObjectTypeName="Herschel.Lib.Observation"
                                OnObjectCreating="observationDataSource_ObjectCreating" TypeName="Herschel.Lib.ObservationSearch"
                                SelectMethod="Find" />
                            <hwc:MultiSelectGridView runat="server" ID="observationList" DataSourceID="observationDataSource"
                                AutoGenerateColumns="false" DataKeyNames="ObsID">
                                <Columns>
                                    <hwc:SelectionField />
                                    <asp:BoundField HeaderText="obs ID" DataField="ObsID" />
                                    <asp:BoundField HeaderText="fine time" DataField="FineTime.Start" />
                                    <asp:BoundField HeaderText="angular velocity" DataField="AV" />
                                    <asp:BoundField HeaderText="area" DataField="Region.Area" />
                                </Columns>
                            </hwc:MultiSelectGridView>
                        </td>
                        <td class="block_buttons">
                            <p class="buttons">
                                <asp:LinkButton runat="server" ID="save" Text="save >" />
                                <asp:LinkButton runat="server" ID="plot" Text="plot >" OnClick="plot_Click" />
                            </p>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <%-- Plot --%>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <asp:Panel runat="server" ID="observationPlotPanel" Visible="false">
                <h2>observation footprint</h2>
                <table class="block">
                    <tr>
                        <td class="block_left">
                            <spherical:PlotCanvas runat="server" ID="canvas" Width="800" Height="450" CssClass="plot" />
                        </td>
                        <td class="block_buttons">
                            <p class="buttons">
                                <asp:HyperLink runat="server" ID="savePlotPdfLink" Text="save pdf >"/>
                            </p>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:LinkButton runat="server" ID="savePlotPdf" onclick="savePlotPdf_Click" Text="save pdf" style="display:none;" />
</asp:Content>
