# SharePoint 2013: Get data from a remote service using the web proxy
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- data and storage
## Updated
- 02/06/2013
## Description

<p><span style="font-size:small">This sample SharePoint-hosted app demonstrates how to use the web proxy in SharePoint 2013 to read data from a remote service. The app displays RSS data from the Microsoft News Center in a SharePoint-hosted page.</span></p>
<p><span style="font-size:small">The code that uses the web proxy is in the ReadRSS.aspx file of the WebProxyApp project. Figure 1 shows the ReadRSS.aspx page of the app after you install and run the app.</span></p>
<p><strong><span style="font-size:small">Figure 1. SharePoint-hosted page with data from a remote service</span></strong></p>
<p><span style="font-size:small"><img id="67633" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-get-data-705bdcd5/image/file/67633/1/fig1.png" alt="" width="557" height="398"></span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Microsoft Visual Studio 2012</span> </li><li><span style="font-size:small">SharePoint development tools in Visual Studio 2012</span>
</li><li><span style="font-size:small">A SharePoint 2013 development environment (app isolation required for on-premise scenarios).</span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The sample contains the following:</span></p>
<ul>
<li><span style="font-size:small">WebProxyApp project</span>
<ul>
<li><span style="font-size:small">ReadRSS.aspx file, which uses the <strong>WebProxy</strong> object to issue the call to the remote service</span>
</li><li><span style="font-size:small">AppManifest.xml file</span> </li></ul>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">Follow these steps to configure the sample.</span></p>
<ul>
<li><span style="font-size:small">Update the <strong>SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint website.</span>
</li></ul>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Press F5 to build and deploy the app.</span> </li><li><span style="font-size:small">Choose <strong>Trust It</strong> on the consent page to grant permissions to the app.</span>
</li></ol>
<p><span style="font-size:small">You should see a SharePoint-hosted page that displays the most recent news titles in the Microsoft News Center.</span></p>
<h1>Troubleshooting</h1>
<p><span style="font-size:small">The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how to solve them.</span></p>
<table border="0" cellspacing="5" cellpadding="5" frame="void" align="left" style="width:602px; height:140px">
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
</tbody>
</table>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h1>Change log</h1>
<p><span style="font-size:small">First version: July 16, 2012</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" href="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">How to: Set&nbsp;up an on-premises&nbsp;development
 environment for apps for SharePoint</a></span> </li><li><span style="font-size:small"><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/1534a5f4-1d83-45b4-9714-3a1995677d85">Work with data in SharePoint 2013</a></span></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/16913e6d-4fc6-4c5e-84a4-6c2688703798">How to: Query a remote service using the web proxy</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/en-us/library/26f2999e-db7f-4fe7-a00f-05b009b1927d" href="http://msdn.microsoft.com/en-us/library/26f2999e-db7f-4fe7-a00f-05b009b1927d">What you can do in an app for SharePoint</a></span>
</li><li><span style="font-size:small"><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/bde5647a-fff1-4b51-b67b-2139de79ce4a">Authorization and authentication for apps&nbsp;in SharePoint 2013</a></span></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/0942fdce-3227-496a-8873-399fc1dbb72c">Three ways to think about design options for apps for SharePoint</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88">Important aspects of the app for SharePoint architecture and development landscape</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/3034f03c-2d5a-46de-9cb8-2c101ff194fa"><span style="font-size:small">Data storage options in apps for SharePoint</span></a></span>
</li></ul>
