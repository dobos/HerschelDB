<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ObservationList.ascx.cs" Inherits="Herschel.Ws.Observations.ObservationList" %>
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
<asp:CustomValidator runat="server" ID="observationListValidator" ValidationGroup="observationList" OnServerValidate="observationListValidator_ServerValidate"
    Display="Dynamic" Text="No observations selected." />
