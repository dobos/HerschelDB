<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FootprintPlot.ascx.cs" Inherits="Herschel.Ws.Observations.FootprintPlot" %>
<table class="block">
    <tr>
        <td class="block_left">
            <spherical:PlotCanvas runat="server" ID="canvas" Width="840" Height="450" CssClass="plot" />
        </td>
        <td class="block_buttons">
            <p class="controls">
                <asp:CheckBox runat="server" ID="plotOutline" Text="Outline" Checked="true" AutoPostBack="true" /><br />
                <asp:CheckBox runat="server" ID="plotFill" Text="Fill" Checked="true" AutoPostBack="true" /><br />
                <asp:CheckBox runat="server" ID="plotSsos" Text="SSO trajectories" Checked="true" AutoPostBack="true" />
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
                <asp:LinkButton runat="server" ID="savePlotPdf" OnClick="savePlotPdf_Click" Text="save pdf" Style="display: none;" />
                <asp:HyperLink runat="server" ID="savePlotPdfLink" Text="save pdf >" />
            </p>
        </td>
    </tr>
</table>
