# SharePoint 2013: Perform basic data access operations by using REST in apps
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- C#
- REST
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
- SharePoint Add-ins
## Topics
- data and storage
## Updated
- 08/08/2015
## Description

<p><span style="font-size:medium; color:#ff0000">This sample has been moved to: <a title="SharePoint Add-in REST\OData Basic Data Operations" href="https://github.com/OfficeDev/SharePoint-Add-in-REST-OData-BasicDataOperations" target="_blank">
SharePoint Add-in REST\OData Basic Data Operations</a>. The description below may not match the new version.</span></p>
<p>&nbsp;</p>
<p><strong><span style="font-size:small">Old Description:</span></strong></p>
<div>
<p><span style="font-size:small">The code that uses the REST APIs is located in the Home.aspx.cs file of the BasicDataOperationsRESTWeb project. The following screen shot shows how the Home.aspx page of the app appears after you install and launch the app.</span></p>
<p><strong><span style="font-size:small">Figure 1. Home.aspx page in the app, which displays the controls for viewing and creating lists and list items</span></strong></p>
<p>&nbsp;<img id="60514" src="60514-fig1.gif" alt="" width="735" height="472"></p>
<p><span style="font-size:small">The sample demonstrates the following:</span></p>
<ul>
<li><span style="font-size:small">How to read and write data to and from a parent web. This data conforms with the OData protocol to the REST endpoints where the list and list item entities are exposed.
</span></li><li><span style="font-size:small">How to parse Atom-formatted XML returned from these endpoints and how to construct JSON-formatted representations of the list and list item entities so that you can perform
<strong>Create</strong> and <strong>Update</strong> operations on them.</span> </li><li><span style="font-size:small">Best practices for retrieving form digest and eTag values that are required for
<strong>Create</strong> and <strong>Update</strong> operations on lists and list items.</span>
</li></ul>
<p><span style="font-size:small">For more information about the SharePoint REST APIs, see
<a href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx">
Programming using the SharePoint 2013 REST service</a>. For more information about working with JSON, Atom, and OData, see
<a href="http://www.odata.org/developers/protocols/json-format">OData: JavaScript Object Notation (JSON) Format</a> and
<a href="http://www.odata.org/developers/protocols/atom-format">OData: AtomPub Format</a>.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">A SharePoint 2013 development environment that is configured for app isolation and OAuth</span>
</li><li><span style="font-size:small">Visual Studio 2012 and SharePoint development tools in Visual Studio 2012 installed on your developer computer</span>
</li><li><span style="font-size:small">Basic familiarity with RESTful web services</span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The perform basic data operations by using REST sample app contains the following:</span></p>
<ul>
<li><span style="font-size:small">BasicDataOperationsREST project, which contains the AppManifest.xml file</span>
</li><li><span style="font-size:small">BasicDataOperationsRESTWeb project, which contains the following:</span>
<ul>
<li><span style="font-size:small">Home.aspx file, which contains the HTML and ASP.NET controls for the app&rsquo;s user interface.</span>
</li><li><span style="font-size:small">Home.aspx.cs file, which contains the C# code that uses the REST APIs to read and write data.</span>
</li><li><span style="font-size:small">Web.config file.</span> </li></ul>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">To configure the basic data operations sample app, update the
<strong>SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint 2013 site.</span></p>
<h1>Build the sample</h1>
<p><span style="font-size:small">Press the F5 key to build and deploy the app.</span></p>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Press the F5 key to build and deploy the app.</span>
</li><li><span style="font-size:small">Choose <strong>Trust It</strong> on the consent page to grant permissions to the app.</span>
</li><li><span style="font-size:small">Use the app&rsquo;s interface to read, create, and update lists and add list items on the parent SharePoint 2013 Preview site.</span>
</li></ol>
<h1>Examples</h1>
<p><span style="font-size:small">The following figure shows an example of how to use this app for SharePoint to view list items.</span></p>
<p><strong><span style="font-size:small">Figure 2. View list items from a list on the parent web</span></strong></p>
<p><span style="font-size:small"><img id="60515" src="60515-fig1.gif" alt="" width="735" height="780"></span></p>
<p><span style="font-size:small">The following figure shows an example of how to use this app for SharePoint to add list items.</span></p>
<p><strong><span style="font-size:small">Figure 3. Add list items to a list on the parent web</span></strong></p>
<p><span style="font-size:small"><img id="60516" src="60516-fig1.gif" alt="" width="644" height="321"></span></p>
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
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" target="_blank">SharePoint 2013 development overview</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx" href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx" target="_blank">Programming using the SharePoint 2013
 REST service</a></span> </li><li><span style="font-size:small"><a title="http://www.odata.org/" href="http://www.odata.org/" target="_blank">Open Data Protocol</a></span>
</li><li><span style="font-size:small"><a title="http://www.odata.org/developers/protocols/json-format" href="http://www.odata.org/developers/protocols/json-format" target="_blank">OData: JavaScript Object Notation (JSON) Format</a></span>
</li><li><span style="font-size:small"><a title="http://www.odata.org/developers/protocols/atom-format" href="http://www.odata.org/developers/protocols/atom-format" target="_blank">OData: AtomPub Format</a></span>
</li></ul>
<div></div>
</div>
