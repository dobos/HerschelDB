<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Herschel.Ws.Docs.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1>documentation</h1>
    <h2>samples</h2>
    <p>We suggest using curl to access the Herschel Footprint Database REST API.</p>
    <ul>
        <li>Find observations by coordinates (J2000 decimal)</li>
    </ul>
    <pre>curl -H &quot;Accept:text/plain&quot; &quot;http://herschel.vo.elte.hu/HerschelWs/Api/Observation/Find?ra=207.25&amp;dec=-28.4&quot;</pre>
    <ul>
        <li>See footprint description of observation</li>
    </ul>
    <pre>curl -H &quot;Accept:text/plain&quot; &quot;http://herschel.vo.elte.hu/HerschelWs/Api/Observation/Footprint?obsid=1342182235&quot;</pre>
    <h2>region examples</h2>
    <p>Circle with a radius of 20 arcminutes centered on J2000 coordinates</p>
    <pre>CIRCLE J2000 207.25 -28.4 20</pre>
    <p>
        Rectangle with two corners specified in J2000 coordinates</p>
    <pre>RECT J2000 207.0 -28.3 207.1 -28.4</pre>
    <p>
        Polygon with J2000 coordinates (please avoid bowtie and don&#39;t repeat first point as last)</p>
    <pre>POLY J2000 0 10 10 10 10 0</pre>
    <p>
        Convex hull of a list of J2000 coordinates (please avoid points spreading more than half of the sphere)</p>
    <pre>CHULL J2000 0 10 10 10 10 0</pre>
    
</asp:Content>
