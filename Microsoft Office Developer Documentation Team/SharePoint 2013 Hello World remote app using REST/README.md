# SharePoint 2013: Hello World remote app using REST
## License
- Apache License, Version 2.0
## Technologies
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

<p><span style="font-size:medium; color:#ff0000">This has been removed. Please use the sample at:&nbsp;<a href="https://github.com/OfficeDev/SharePoint-Add-in-REST-OData-BasicDataOperations">SharePoint Add-in REST\OData Basic Data perations</a>. The description
 below does not apply to the new sample.</span></p>
<p><span style="font-size:medium; color:#ff0000"><br>
</span></p>
<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Hello World remote app using REST</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p><span>Summary:</span> Learn how to create a basic &quot;hello world&quot; provider-hosted app for SharePoint that uses the SharePoint REST interface to read information about a SharePoint 2013 site from a remote web application.</p>
</div>
<div>
<p><strong>Last modified: </strong>July 01, 2012</p>
<p><strong>In this article</strong> <br>
<a href="#sectionSection0">Description of the sample</a> <br>
<a href="#sectionSection1">Prerequisites</a> <br>
<a href="#sectionSection2">Key components of the sample</a> <br>
<a href="#sectionSection3">Configure the sample</a> <br>
<a href="#sectionSection4">Build the sample</a> <br>
<a href="#sectionSection5">Run and test the sample</a> <br>
<a href="#sectionSection6">Troubleshooting</a> <br>
<a href="#sectionSection7">Change log</a> <br>
<a href="#sectionSection8">Related content</a></p>
<p>&nbsp;</p>
</div>
<h2>Description of the sample</h2>
<div id="sectionSection0">
<p>The sample demonstrates how to read data that conforms with the OData protocol from the REST endpoints where the basic SharePoint entities, such as lists and users, are exposed. Additionally, it demonstrates how to parse Atom-formatted XML returned from
 these endpoints.</p>
<p>The code that uses the REST APIs is located in the Home.aspx.cs file of the BasicSelfHostedAppRESTWeb project. The following screen shot shows how the Home.aspx page in the app appears after you install and launch the app.</p>
<strong>
<div class="caption">Figure 1. Home.aspx page in the Hello World remote app for SharePoint using REST</div>
</strong><br>
<img id="116799" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-hello-25f8c6f1/image/file/116799/1/sp15_basicself-hostedapp.gif" alt="Basic self-hosted app launch page" width="197" height="320">
<p>For more information about the SharePoint REST APIs, see <a href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx" target="_blank">
Programming using the SharePoint 2013 REST service</a>. For more information about working with JSON, Atom, and OData, see
<a href="http://www.odata.org/developers/protocols/json-format" target="_blank">OData: JavaScript Object Notation (JSON) Format</a> and
<a href="http://www.odata.org/developers/protocols/atom-format" target="_blank">OData: AtomPub Format</a>.</p>
</div>
<h2>Prerequisites</h2>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A SharePoint 2013 development environment that is configured for app isolation and OAuth</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your developer computer</p>
</li><li>
<p>Basic familiarity with RESTful web services</p>
</li></ul>
</div>
<h2>Key components of the sample</h2>
<div id="sectionSection2">
<p>The sample app contains the following:</p>
<ul>
<li>
<p>BasicSelfHostedAppREST project, which contains the AppManifest.xml file</p>
</li><li>
<p>BasicSelfHostedAppRESTWeb project, which contains the following:</p>
<ul>
<li>
<p>Home.aspx file, which contains the HTML and ASP.NET controls for the app's user interface</p>
</li><li>
<p>Home.aspx.cs file, which contains the C# code that uses the REST APIs to read and write data to and from the parent web</p>
</li><li>
<p>web.config file</p>
</li></ul>
</li></ul>
</div>
<h2>Configure the sample</h2>
<div id="sectionSection3">
<p>To configure the Hello world remote app using REST sample, update the <strong>
SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint 2013 site.</p>
</div>
<h2>Build the sample</h2>
<div id="sectionSection4">
<p>Press F5 to build and deploy the app.</p>
</div>
<h2>Run and test the sample</h2>
<div id="sectionSection5">
<p>&nbsp;</p>
<ol>
<li>
<p>Choose <strong><span class="ui">Trust It</span></strong> on the consent page to grant permissions to the app.</p>
</li><li>
<p>Choose <strong><span class="ui">Populate Data</span></strong> to see some basic information about the parent web.</p>
</li></ol>
<h3>Example</h3>
<div>
<p>The following figure shows an example of the kinds of information that this sample app can read and display.</p>
<strong>
<div class="caption">Figure 2. View populated data about the parent web</div>
</strong><br>
<img id="116800" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-hello-25f8c6f1/image/file/116800/1/sp15_basicselfhostedpopulated.gif" alt="Basic self-hosted app with data populated" width="735" height="746"></div>
</div>
<h2>Troubleshooting</h2>
<div id="sectionSection6">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how you can solve them.</p>
<strong>
<div class="caption"></div>
</strong>
<div>
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p>Problem</p>
</th>
<th>
<p>Solution</p>
</th>
</tr>
<tr>
<td>
<p>Visual Studio does not open the browser after you press the F5 key.</p>
</td>
<td>
<p>Set the app for SharePoint project as the startup project.</p>
</td>
</tr>
<tr>
<td>
<p>HTTP error 405 <strong>Method not allowed</strong>.</p>
</td>
<td>
<p>Locate the <strong><span class="keyword">applicationhost.config</span></strong> file in %userprofile%\Documents\IISExpress\config.</p>
<p>Locate the handler entry for <strong>StaticFile</strong>, and add the verbs <strong>
<span class="keyword">GET</span></strong>, <strong><span class="keyword">HEAD</span></strong>,
<strong><span class="keyword">POST</span></strong>, <strong><span class="keyword">DEBUG</span></strong>, and
<strong><span class="keyword">TRACE</span></strong>.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h2>Change log</h2>
<div id="sectionSection7"><strong>
<div class="caption"></div>
</strong>
<div>
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p>Version</p>
</th>
<th>
<p>Date</p>
</th>
</tr>
<tr>
<td>
<p>First version</p>
</td>
<td>
<p>July 16, 2012</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h2>Related content</h2>
<div id="sectionSection8">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/3038dd73-41ee-436f-8c78-ef8e6869bf7b.aspx" target="_blank">How to: Create a basic provider-hosted app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" target="_blank">SharePoint 2013 development overview</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx" target="_blank">Programming using the SharePoint 2013 REST service</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/d07e0a13-1e74-4128-857a-513dedbfef33.aspx" target="_blank">Getting started developing SharePoint apps</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/1b992485-6efe-4ea4-a18c-221689b0b66f.aspx" target="_blank">Building a basic SharePoint-hosted app</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/bde5647a-fff1-4b51-b67b-2139de79ce4a.aspx" target="_blank">OAuth in SharePoint 2013 Preview</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx" target="_blank">Architecture of the app for SharePoint model</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx" target="_blank">Apps for SharePoint vs. classic SharePoint solutions</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx" target="_blank">Detailed introduction to the SharePoint app model</a></p>
</li><li>
<p><a href="http://www.odata.org/" target="_blank">Open Data Protocol</a></p>
</li><li>
<p><a href="http://www.odata.org/developers/protocols/json-format" target="_blank">OData: JavaScript Object Notation (JSON) Format</a></p>
</li><li>
<p><a href="http://www.odata.org/developers/protocols/atom-format" target="_blank">OData: AtomPub Format</a></p>
</li></ul>
</div>
</div>
</div>
