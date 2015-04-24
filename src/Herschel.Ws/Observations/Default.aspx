<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Herschel.Ws.Observations.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1>observation search</h1>
    <%-- Form --%>
    <%--<asp:UpdatePanel runat="server">
        <ContentTemplate>--%>
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
                                                <asp:ListItem Value="Pacs" Text="PACS" Selected="True" />
                                                <asp:ListItem Value="Spire" Text="SPIRE"/>
                                                <asp:ListItem Value="PacsSpireParallel" Text="PACS/SPIRE parallel"/>
                                                <asp:ListItem Value="Hifi" Text="HIFI" Enabled="false" />
                                            </asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            <asp:Label runat="server" ID="fineTimeLabel" Text="Fine Time:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:TextBox runat="server" ID="fineTimeStart" />
                                            -
                                            <asp:TextBox runat="server" ID="fineTimeEnd" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label">
                                            <asp:Label runat="server" ID="searchMethodLabel" Text="Method:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:RadioButtonList ID="searchMethod" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="searchMethod_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Value="Point">Coordinates</asp:ListItem>
                                                <asp:ListItem Value="Intersect">Intersect</asp:ListItem>
                                                <%--<asp:ListItem Value="Cover" Enabled="False">Cover</asp:ListItem>--%>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="pointTr">
                                        <td class="label">
                                            <asp:Label ID="pointLabel" runat="server" Text="Coordinates:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:TextBox ID="point" runat="server" Text="207.25, -28.4" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="resolvedTr" visible="false">
                                        <td></td>
                                        <td>Using coordinates from <a href="http://simbad.u-strasbg.fr/simbad/">Simbad</a>.</td>
                                    </tr>
                                    <tr runat="server" id="regionTr" visible="false">
                                        <td class="label">
                                            <asp:Label ID="regionLabel" runat="server" Text="Region Description:" /></td>
                                        <td class="field" style="width: 520px">
                                            <asp:TextBox ID="region" runat="server" TextMode="MultiLine">CIRCLE J2000 207.25 -28.4 20</asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                                <asp:CustomValidator runat="server" ID="pointFormatValidator" Display="Dynamic"
                                    ValidationGroup="search" OnServerValidate="pointFormatValidator_ServerValidate"
                                    Text="Invalid coordinates." />
                                <asp:CustomValidator runat="server" ID="regionFormatValidator" Display="Dynamic"
                                    ValidationGroup="search" OnServerValidate="regionFormatValidator_ServerValidate"
                                    Text="Invalid region definition, please see the documentation." />
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
        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
    <%-- List --%>
    <%--<asp:UpdatePanel runat="server">
        <ContentTemplate>--%>
            <asp:Panel runat="server" ID="observationListPanel" Visible="false">
                <h2>search results</h2>
                <table class="block">
                    <tr>
                        <td class="block_left">
                            <asp:ObjectDataSource runat="server" ID="observationDataSource" DataObjectTypeName="Herschel.Lib.Observation"
                                OnObjectCreating="observationDataSource_ObjectCreating" TypeName="Herschel.Lib.ObservationSearch"
                                SelectMethod="Find" />
                            <hwc:MultiSelectGridView runat="server" ID="observationList" DataSourceID="observationDataSource"
                                AutoGenerateColumns="false" DataKeyNames="Instrument,ObsID"
                                Width="100%">
                                <Columns>
                                    <hwc:SelectionField ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField HeaderText="instrument" DataField="Instrument" />
                                    <asp:BoundField HeaderText="obs ID" DataField="ObsID" />
                                    <asp:BoundField HeaderText="fine time start" DataField="FineTimeStart" />
                                    <asp:BoundField HeaderText="fine time end" DataField="FineTimeEnd" />
                                    <asp:BoundField HeaderText="angular velocity" DataFormatString="{0:0}" DataField="ScanMap.AV" />
                                    <asp:BoundField HeaderText="area" DataField="Region.Area" DataFormatString="{0:0.00000}" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No observations match the query.
                                </EmptyDataTemplate>
                            </hwc:MultiSelectGridView>
                        </td>
                        <td class="block_buttons">
                            <p class="buttons">
                                <asp:LinkButton runat="server" ID="save" ValidationGroup="observationList" Text="save >" Visible="false" />
                                <asp:LinkButton runat="server" ID="plot" ValidationGroup="observationList" Text="plot >" OnClick="plot_Click" />
                            </p>
                        </td>
                    </tr>
                </table>
                <asp:CustomValidator runat="server" ID="observationListValidator" ValidationGroup="observationList" OnServerValidate="observationListValidator_ServerValidate"
                    Display="Dynamic" Text="No observations selected." />
            </asp:Panel>
        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
    <%-- Plot --%>
    <%--<asp:UpdatePanel runat="server">
        <ContentTemplate>--%>
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
        <%--</ContentTemplate>
    </asp:UpdatePanel>--%>
    <asp:LinkButton runat="server" ID="savePlotPdf" OnClick="savePlotPdf_Click" Text="save pdf" Style="display: none;" />
</asp:Content>
