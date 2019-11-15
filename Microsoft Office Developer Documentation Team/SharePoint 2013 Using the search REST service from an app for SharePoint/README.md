# SharePoint 2013: Using the search REST service from an app for SharePoint
## License
- Apache License, Version 2.0
## Technologies
- REST
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
- SharePoint Add-ins
## Topics
- Search
## Updated
- 08/11/2015
## Description

<p><span style="font-size:medium; color:#ff0000">This sample has moved to <a href="https://github.com/OfficeDev/SharePoint-Add-in-REST-SearchAPIs">
SharePoint-Add-in-REST-SearchAPIs</a>. The description below may not precisely match the new version.</span></p>
<p>&nbsp;</p>
<p><span style="font-size:small"><strong>Provided by</strong>:&nbsp;&nbsp; <a href="http://www.shillier.com/default.aspx">
Scot Hillier</a>, <a href="http://www.criticalpathtraining.com/Pages/default.aspx">
Critical Path Training</a></span></p>
<p><span style="font-size:small">This sample demonstrates how to return search results in an app for SharePoint.&nbsp; The SearchApp sample accepts a query using the keyword query language (KQL) syntax as an input. It subsequently makes a call to the search
 engine using the search Representational State Transfer (REST) service. The results are then displayed in a table, which is built dynamically using JQuery and JavaScript. The following screen shot shows search results displayed on the home page of the sample.</span></p>
<p><strong><span style="font-size:small">Figure 1. Home.aspx page in the app, displaying the search results</span></strong></p>
<p><span style="font-size:small"><img id="60652" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-perform-a-1bf3e87d/image/file/60652/1/fig1.jpg" alt="" width="645" height="261"></span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Visual Studio 2012</span> </li><li><span style="font-size:small">SharePoint development tools in Visual Studio 2012</span>
</li><li><span style="font-size:small">SharePoint 2013 development environment</span> </li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The sample app contains the following:</span></p>
<ul>
<li><span style="font-size:small">SearchApp project</span> </li><li><span style="font-size:small">AppManifest.xml file, located in the SearchApp directory</span>
</li><li><span style="font-size:small">Home.aspx file, located in the SearchApp\Pages directory, which contains the HTML and ASP.NET controls for the sample&rsquo;s user interface</span>
</li><li><span style="font-size:small">App.js file, located in the SearchApp\Scripts directory, which contains the JavaScript code that populates the controls in the Home.aspx file</span>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">To configure SearchApp sample, update the <strong>
SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint 2013 site.</span></p>
<h1>Build the sample</h1>
<p><span style="font-size:small">Press the <strong>F5</strong> key to build and deploy the app.</span></p>
<h1>Run and test the sample</h1>
<p><span style="font-size:small">Enter a KQL query in the Search box and click Search to view the results.</span></p>
<h1>Troubleshooting</h1>
<p><span style="font-size:small">The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how to solve them.</span></p>
<table border="0" cellspacing="5" cellpadding="5" frame="void" align="left" style="width:601px; height:212px">
<tbody>
<tr style="background-color:#a9a9a9">
<th align="left" scope="col"><strong><span style="font-size:small">Problem </span>
</strong></th>
<th align="left" scope="col"><strong><span style="font-size:small">Solution</span></strong></th>
</tr>
<tr valign="top">
<td><span style="font-size:small">Visual Studio does not open the browser after you press the F5 key.</span></td>
<td><span style="font-size:small">Set the app for SharePoint project as the startup project.</span></td>
</tr>
<tr valign="top">
<td><span style="font-size:small">The SearchApp app for SharePoint sample fails to install.</span></td>
<td><span style="font-size:small">Ensure that the value you specified for the <strong>
SiteUrl</strong> property is correct.</span></td>
</tr>
</tbody>
</table>
<h1><br>
<br>
<span style="font-size:small">&nbsp;</span><br>
<br>
<br>
</h1>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h1>Change log</h1>
<p><span style="font-size:small">First version: July 16, 2012</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" target="_blank">SharePoint 2013 development overview</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/d07e0a13-1e74-4128-857a-513dedbfef33.aspx" href="http://msdn.microsoft.com/library/d07e0a13-1e74-4128-857a-513dedbfef33.aspx" target="_blank">Get started developing apps for SharePoint</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/0e9efadb-aaf2-4c0d-afd5-d6cf25c4e7a8.aspx" href="http://msdn.microsoft.com/library/0e9efadb-aaf2-4c0d-afd5-d6cf25c4e7a8.aspx" target="_blank">Apps for SharePoint&nbsp;compared with&nbsp;SharePoint
 solutions</a></span> </li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx" href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx" target="_blank">Important aspects of the app for&nbsp;SharePoint
 architecture and development landscape</a></span> </li></ul>
