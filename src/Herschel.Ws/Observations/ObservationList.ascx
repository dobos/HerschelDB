<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObservationList.ascx.cs" Inherits="Herschel.Ws.Observations.ObservationList" %>
<asp:ObjectDataSource runat="server" ID="observationDataSource" DataObjectTypeName="Herschel.Lib.Observation"
    OnObjectCreating="observationDataSource_ObjectCreating" TypeName="Herschel.Lib.ObservationSearch"
    SelectMethod="Find" />
<hwc:MultiSelectGridView runat="server" ID="observationList" DataSourceID="observationDataSource"
    AutoGenerateColumns="false" DataKeyNames="Instrument,ObsID"
    Width="100%">
    <Columns>
        <hwc:SelectionField ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="obs ID" DataField="ObsID" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="instrument" DataField="Instrument" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="type" DataField="Type" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
        <asp:BoundField HeaderText="band" DataField="Band" ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
        <%--<asp:BoundField HeaderText="fine time start" DataField="FineTimeStart" />
        <asp:BoundField HeaderText="fine time end" DataField="FineTimeEnd" />
        <asp:BoundField HeaderText="angular velocity" DataFormatString="{0:0}" DataField="ScanMap.AV" />--%>
        <asp:BoundField HeaderText="area" DataField="Region.Area" DataFormatString="{0:0.0000000} deg<sup>2</sup>" HtmlEncode="false" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="Right" />
        <asp:CheckBoxField HeaderText="calib" DataField="Calibration" ItemStyle-HorizontalAlign="Center"/>
        <asp:CheckBoxField HeaderText="failed" DataField="Failed" ItemStyle-HorizontalAlign="Center"/>
        <asp:CheckBoxField HeaderText="sso" DataField="Sso" ItemStyle-HorizontalAlign="Center"/>
    </Columns>
    <EmptyDataTemplate>
        No observations match the query.
    </EmptyDataTemplate>
</hwc:MultiSelectGridView>
<asp:CustomValidator runat="server" ID="observationListValidator" ValidationGroup="observationList" OnServerValidate="observationListValidator_ServerValidate"
    Display="Dynamic" Text="No observations selected." />
