# SharePoint 2013: Perform basic data access operations by using CSOM in apps
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- C#
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
- SharePoint Add-ins
## Topics
- data and storage
## Updated
- 08/17/2015
## Description

<p><span style="font-size:medium; color:#ff0000">This sample has been moved to <a href="https://github.com/OfficeDev/SharePoint-Add-in-CSOM-BasicDataOperations">
SharePoint-Add-in-CSOM-BasicDataOperations</a>.</span></p>
<p>&nbsp;</p>
<p><span style="font-size:small">The sample demonstrates how to read and write list and list item data by using objects and methods in the SharePoint 2013 client object model (CSOM). Additionally, it demonstrates how to obtain the context and access tokens
 that are necessary for reading and writing data in a provider-hosted app for SharePoint.</span></p>
<p><span style="font-size:small">The code that uses the CSOM is located in the Home.aspx.cs file of the BasicDataOperationsWeb project. The following screen shot shows how the Home.aspx page of the app appears after you install and launch the app.</span></p>
<p><strong><span style="font-size:small">Figure 1. Home.aspx page in the app, which displays the controls for viewing and adding lists to and from the parent web</span></strong></p>
<p><span style="font-size:small"><img id="60522" src="60522-fig1sm.gif" alt="" width="735" height="472"></span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">A SharePoint 2013 development environment that is configured for app isolation and OAuth</span>
</li><li><span style="font-size:small">Visual Studio 2012 and SharePoint development tools in Visual Studio 2012 installed on your developer computer</span>
</li><li><span style="font-size:small">Basic familiarity with the SharePoint CSOM and C#</span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The basic data operations sample app contains the following:</span></p>
<ul>
<li><span style="font-size:small">BasicDataOperations project, which contains the AppManifest.xml file</span>
</li><li><span style="font-size:small">BasicDataOperationsWeb project</span>
<ul>
<li><span style="font-size:small">Home.aspx file, which contains the HTML and ASP.NET controls for the app&rsquo;s user interface.</span>
</li><li><span style="font-size:small">Home.aspx.cs file, which contains the C# code that uses the SharePoint CSOM to read and write data</span>
</li><li><span style="font-size:small">web.config file</span> </li></ul>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">To configure the perform basic data operations by using CSOM sample app, update the
<strong>SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint 2013 site.</span></p>
<h1>Build the sample</h1>
<p><span style="font-size:small">Press the F5 key to build and deploy the app.</span></p>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Choose <strong>Trust It</strong> on the consent page to grant permissions to the app.</span>
</li><li><span style="font-size:small">Use the app&rsquo;s interface to read, create, and update lists and add list items on the parent SharePoint 2013 site.</span>
</li></ol>
<h1>Examples</h1>
<p><span style="font-size:small">The following figure shows an example of how to use this app for SharePoint to view list items.</span></p>
<p><strong><span style="font-size:small">Figure 2. View list items from a list on the parent web</span></strong></p>
<p><span style="font-size:small"><img id="60523" src="60523-fig1sm.gif" alt="" width="735" height="780"></span></p>
<p><span style="font-size:small">The following figure shows an example of how to use this app for SharePoint to add list items.</span></p>
<p><strong><span style="font-size:small">Figure 3. Add list items to a list on the parent web</span></strong></p>
<p><span style="font-size:small"><img id="60524" src="60524-fig1.gif" alt="" width="644" height="321"></span></p>
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
<td><span style="font-size:small">HTTP error 405 <strong>Method not allowed</strong>.</span></td>
<td><span style="font-size:small">Locate the applicationhost.config file in <em>%userprofile%</em>\Documents\IISExpress\config.</span>
<p><span style="font-size:small">Locate the handler entry for <strong>StaticFile</strong>, and add the verbs
<strong>GET</strong>, <strong>HEAD</strong>, <strong>POST</strong>, <strong>DEBUG</strong>, and
<strong>TRACE</strong>.</span></p>
</td>
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
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx">SharePoint 2013 development overview</a></span>
</li><li><span style="line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-size:11pt"><a title="http://msdn.microsoft.com/library/f36645da-77c5-47f1-a2ca-13d4b62b320d.aspx" href="http://msdn.microsoft.com/library/f36645da-77c5-47f1-a2ca-13d4b62b320d.aspx"><span style="line-height:115%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; font-size:10pt">Choose
 the right API set in SharePoint 2013</span></a></span> </li></ul>
