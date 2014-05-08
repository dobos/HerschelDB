<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Herschel.Ws.Observation.Search" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1>observation search</h1>
    <div class="form">
        <table runat="server" class="form">
            <tr>
                <td rowspan="2" class="label">
                    <asp:Label ID="instrumentLabel" runat="server" Text="Instrument:" />
                </td>
                <td rowspan="2" class="field" style="width: 200px">
                    <asp:CheckBoxList ID="instrument" runat="server">
                        <asp:ListItem Value="PACS" Text="PACS" Selected="True" />
                        <asp:ListItem Value="SPIRE" Text="SPIRE" Enabled="false" />
                        <asp:ListItem Value="PARALLEL" Text="PACS/SPIRE parallel" Enabled="false" />
                        <asp:ListItem Value="HIFI" Text="HIFI" Enabled="false" />
                    </asp:CheckBoxList>
                </td>
                <td class="label">
                    <asp:Label runat="server" ID="searchMethodLabel" Text="Method:" /></td>
                <td class="field">
                    <asp:RadioButtonList ID="searchMethod" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="searchMethod_SelectedIndexChanged">
                        <asp:ListItem Selected="True" Value="Point">Coordinate</asp:ListItem>
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
    <p class="buttons">
        <asp:LinkButton runat="server" ID="search" Text="search >" OnClick="search_Click" />
    </p>
    <asp:Panel runat="server" ID="observationListPanel" Visible="false">
        <h2>search results</h2>
        <asp:ObjectDataSource runat="server" ID="observationDataSource" DataObjectTypeName="Herschel.Lib.Observation"
            OnObjectCreating="observationDataSource_ObjectCreating" TypeName="Herschel.Lib.ObservationSearch"
            SelectMethod="FindEq" />
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
        <p class="buttons">
            <asp:LinkButton runat="server" ID="save" Text="save >" Visible="false" />
            <asp:LinkButton runat="server" ID="plot" Text="plot >" OnClick="plot_Click" />
        </p>
    </asp:Panel>
</asp:Content>
