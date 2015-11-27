<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchForm.ascx.cs" Inherits="Herschel.Ws.Observations.SearchForm" %>


<table runat="server" class="form">
    <tr>
        <td class="label">
            <asp:Label ID="obsModeLabel" runat="server" Text="Observation modes:" />
        </td>
        <td class="field" style="width: 520px">
            <table cellpadding="0">
                <tr>
                    <td>HIFI</td>
                    <td>PACS</td>
                    <td>SPIRE</td>
                    <td>PACS/SPIRE parallel</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBox runat="server" Text="Single Point" ID="hifiSinglePoint" Checked="true" /><br />
                        <asp:CheckBox runat="server" Text="Mapping" ID="hifiMapping" Checked="true" /><br />
                        <asp:CheckBox runat="server" Text="Spectral Scan" ID="hifiSpectralScan" Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" Text="Photometry" ID="pacsPhotometry" Checked="true" />
                        <asp:CheckBox runat="server" Text="Range Spectroscopy" ID="pacsRangeSpec" Checked="true" />
                        <asp:CheckBox runat="server" Text="Line Spectroscopy" ID="pacsLineSpec" Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" Text="Photometry" ID="spirePhotometry" Checked="true" />
                        <asp:CheckBox runat="server" Text="Spectroscopy" ID="spireSpectroscopy" Checked="true" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" Text="Photometry" ID="parallelPhotometry" Checked="true" />
                    </td>
                </tr>
            </table>
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
            <asp:Label ID="filtersLabel" runat="server" Text="Filters:"></asp:Label>
        </td>
        <td class="field" style="width: 520px">
            <asp:CheckBox ID="CheckBox1" runat="server" Text="Calibration" />
            &nbsp;<asp:CheckBox ID="CheckBox2" runat="server" Text="Failed" />
            &nbsp;<asp:CheckBox ID="CheckBox3" runat="server" Text="Solar System Object" />
        </td>
    </tr>
    <tr>
        <td class="label">
            <asp:Label runat="server" ID="searchMethodLabel" Text="Method:" /></td>
        <td class="field" style="width: 520px">
            <asp:RadioButtonList ID="searchMethod" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" OnSelectedIndexChanged="searchMethod_SelectedIndexChanged">
                <%--<asp:ListItem Value="Cover" Enabled="False">Cover</asp:ListItem>--%>
                <asp:ListItem Value="Cone">Cone search</asp:ListItem>
                <asp:ListItem Selected="True" Value="Point">Coordinates</asp:ListItem>
                <asp:ListItem Value="Intersect">Intersect</asp:ListItem>
                <asp:ListItem>Contain</asp:ListItem>
                <asp:ListItem Value="ObsID">OBSID lookup</asp:ListItem>
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
    <tr runat="server" id="radiusTr" visible="false">
        <td class="label">
            <asp:Label ID="radiusLabel" runat="server" Text="Radius:" /></td>
        <td class="field" style="width: 520px">
            <asp:TextBox ID="radius" runat="server" Text="10" />
            arc sec
        </td>
    </tr>
    <tr runat="server" id="regionTr" visible="false">
        <td class="label">
            <asp:Label ID="regionLabel" runat="server" Text="Region Description:" /></td>
        <td class="field" style="width: 520px">
            <asp:TextBox ID="region" runat="server" TextMode="MultiLine">CIRCLE J2000 207.25 -28.4 20</asp:TextBox>
        </td>
    </tr>
    <tr runat="server" id="idlistTr" visible="false">
        <td class="label">
            <asp:Label ID="idlistLabel" runat="server" Text="ObsID list:" /></td>
        <td class="field" style="width: 520px">
            <asp:TextBox ID="idlist" runat="server" TextMode="MultiLine">1342185581
1342185582</asp:TextBox>
        </td>
    </tr>
</table>
<asp:CustomValidator runat="server" ID="pointFormatValidator" Display="Dynamic"
    ValidationGroup="search" OnServerValidate="pointFormatValidator_ServerValidate"
    Text="Invalid coordinates." />
<asp:CustomValidator runat="server" ID="regionFormatValidator" Display="Dynamic"
    ValidationGroup="search" OnServerValidate="regionFormatValidator_ServerValidate"
    Text="Invalid region definition, please see the documentation." />
<asp:CustomValidator runat="server" ID="idlistFormatValidator" Display="Dynamic"
    ValidationGroup="search" OnServerValidate="idlistFormatValidator_ServerValidate"
    Text="Invalid ID List, please see the documentation." />
