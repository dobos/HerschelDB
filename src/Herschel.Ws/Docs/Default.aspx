<%@ Page Title="" Language="C#" MasterPageFile="~/App_Masters/Herschel.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Herschel.Ws.Docs.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://balupton.github.com/jquery-syntaxhighlighter/scripts/jquery.syntaxhighlighter.min.js"></script>
    <script type="text/javascript">
        $.SyntaxHighlighter.init({
            'lineNumbers': false
        });
    </script>
    <style type="text/css">
        pre.prettyprint { border: 0px !important }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content" runat="server">
    <h1>documentation</h1>
    <h2>command-line examples</h2>
    <p>We suggest using curl to access the Herschel Footprint Database REST API.</p>
    <ul>
        <li>Find observations by coordinates (J2000 decimal)</li>
    </ul>
    <pre>curl -H &quot;Accept:text/plain&quot; &quot;http://herschel.vo.elte.hu/Api/Observations?findby=eq&amp;ra=207.25&amp;dec=-28.4&quot;</pre>
    <ul>
        <li>See footprint description of observation</li>
    </ul>
    <pre>curl -H &quot;Accept:text/plain&quot; &quot;http://herschel.vo.elte.hu/Api/Observations/1342182235/Footprint&quot;</pre>
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
    
    <h2>python examples</h2>

    <p>Find and plot footprints</p>

    <pre class="language-python">import matplotlib.pyplot as plt
import numpy as np
import urllib.parse
import urllib.request
import json

baseURL = "http://herschel.vo.elte.hu/Api/"

def openURL(url, values, headers):
    if values == None:
        url = baseURL + url
    else:
        params = urllib.parse.urlencode(values)
        url = baseURL + url + '?' + params
    req = urllib.request.Request(url, headers=headers)
    with urllib.request.urlopen(req) as response:
        data = response.read()
        data = data.decode("ascii")
        return data
    
def openTable(url, params):
    data = openURL(url, params, {'Accept': 'text/plain'})
    return np.loadtxt(data.splitlines())

def openJSON(url, params):
    data = openURL(url, params, {'Accept': 'application/json'})
    return json.loads(data)

def findObsIntersect(inst, region):
    url = "Observations"
    params = { "findby": "intersect", "inst": inst, 
               "region": region }
    return openJSON(url, params)

def getOutlinePoints(inst, obsID, res):
    url = "Observations/" + inst + "/" + str(obsID) + 
    	  "/Footprint/Outline/Points"
    params = { "res": res }
    return openTable(url, params)
    
obs = findObsIntersect("PACS", "CIRCLE J2000 207.25 -28.4 20")
for o in obs:
    points = getOutlinePoints(o["inst"], o["obsID"], 0.2)
    plt.plot(points[:,1], points[:,2], '-')</pre>

    <h2>database schema</h2>

    <p><a href="images/schema.png">
        <img alt="Database schema" class="auto-style1" src="images/schema.png" style="border-width: 0px; width: 640px; left: auto; right: auto; height: auto;" /></a></p>
</asp:Content>
