# SharePoint 2013: Perform basic data operations on files and folders using REST
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
- data and storage
## Updated
- 08/14/2015
## Description

<div id="header"><span style="color:#ff0000; font-size:medium">This sample has been removed. For information about working with files and folders using the SharePoint REST APIs, see
<a href="https://msdn.microsoft.com/EN-US/library/office/dn450841.aspx">Files and folders REST API reference</a> and
<a href="https://msdn.microsoft.com/EN-US/library/office/dn292553.aspx">Working with folders and files with REST</a>.</span></div>
<div id="mainSection">
<div id="mainBody">
<h1 class="heading">Description of the sample</h1>
<div class="section" id="sectionSection0">
<div>The code that uses the REST APIs is located in the Files.js file of the FilesSampleWeb project. The following screen shot shows how the Files.html page of the app appears after you install and launch the app.</div>
<div>&nbsp;</div>
<div><strong>Figure 1. Files.html page in the app, which displays the controls for viewing and creating document library folders and files</strong></div>
<div><strong>&nbsp;</strong></div>
<img id="70112" src="70112-sp15_filesoperationslaunch.gif" alt="" width="543" height="385"><br>
<div>The sample demonstrates the following:</div>
<ul>
<li>
<div>How to read and write data to and from a host web by making REST calls with the cross-domain library.</div>
</li><li>
<div>How to retrieve folders and files from the REST endpoints where they are exposed and how to perform
<strong>Create</strong> and <strong>Update</strong> operations on them.</div>
</li></ul>
<div>For more information:</div>
</div>
<div class="section" id="sectionSection0">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx" target="_blank">Programming using the SharePoint 2013 REST service</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179927(v=office.15).aspx" target="_blank">How to: Access SharePoint 2013 data from remote apps using the cross-domain library</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/jj164022(v=office.15).aspx" target="_blank">How to: Complete basic operations using SharePoint 2013 REST endpoints</a></div>
</li></ul>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<div>This sample requires the following:</div>
<ul>
<li>
<div>An Office 365 Developer Site</div>
</li><li>
<div>Visual Studio 2012 and SharePoint development tools in Visual Studio 2012 installed on your developer computer</div>
</li><li>
<div>Basic familiarity with RESTful web services</div>
</li></ul>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<div>This sample app that shows how to perform basic data operations by using REST contains the following:</div>
<ul>
<li>
<div>FilesSample project, which contains an empty SharePoint-hosted app. This is required for the cross-domain library to work.</div>
</li><li>
<div>FilesSampleWeb project, which contains the following:</div>
<ul>
<li>
<div>Files.html file, which contains the HTML for the app's user interface.</div>
</li><li>
<div>Files.js file, which contains the JavaScript code that uses the REST APIs to read and write data.</div>
</li><li>
<div>web.config file.</div>
</li></ul>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<div>To configure the basic data operations sample app, update the <strong>SiteUrl</strong> property of the solution with the URL of the home page of your Office 365 Developer Site.</div>
<div>&nbsp;</div>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection4">
<ol>
<li>
<div>Press the F5 key to build and deploy the app.</div>
</li><li>
<div>Choose <span class="ui">Trust It</span> on the consent page to grant permissions to the app.</div>
</li><li>
<div>Use the app's interface to read, create, and update lists and add list items on the parent SharePoint 2013 site.</div>
</li></ol>
<h3 class="subHeading">Examples</h3>
<div class="subsection">
<div>The following figure shows an example of how to use this app for SharePoint to add a file to a folder.</div>
<div class="caption"></div>
<div class="caption"><strong>Figure 2. Add a file to a folder</strong></div>
<br>
<img id="70113" src="70113-sp15_filesoperationsaddfile.gif" alt="" width="506" height="380"></div>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection5">
<div>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how you can solve them.</div>
<div>&nbsp;</div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<div>Problem</div>
</th>
<th>
<div>Solution</div>
</th>
</tr>
<tr>
<td>
<div>Visual Studio does not open the browser after you press the F5 key.</div>
</td>
<td>
<div>Set the app for SharePoint project as the startup project.</div>
</td>
</tr>
<tr>
<td>
<div>HTTP error 405 <strong>Method not allowed</strong>.</div>
</td>
<td>
<div>Locate the <span><span class="keyword">applicationhost.config</span></span> file in %userprofile%\Documents\IISExpress\config.</div>
<div>Locate the handler entry for <strong>StaticFile</strong>, and add the verbs <span>
<span class="keyword">GET</span></span>, <span><span class="keyword">HEAD</span></span>,
<span><span class="keyword">POST</span></span>, <span><span class="keyword">DEBUG</span></span>, and
<span><span class="keyword">TRACE</span></span>.</div>
</td>
</tr>
<tr>
<td>
<div>An attempt to delete a folder does not work.</div>
</td>
<td>
<div>This likely happens because the folder is used internally and cannot be deleted.</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading"><br>
Change log</h1>
<div class="section" id="sectionSection6">
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<div>Version</div>
</th>
<th>
<div>Date</div>
</th>
</tr>
<tr>
<td>
<div>First version</div>
</td>
<td>
<div>October 30, 2012</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection7">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" target="_blank">SharePoint 2013 development overview</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72.aspx" target="_blank">Programming using the SharePoint 2013 REST service</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179927(v=office.15).aspx" target="_blank">How to: Access SharePoint 2013 data from remote apps using the cross-domain library</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/jj164022(v=office.15).aspx" target="_blank">How to: Complete basic operations using SharePoint 2013 REST endpoints</a></div>
</li><li>
<div><a href="http://www.odata.org/" target="_blank">Open Data Protocol</a></div>
</li><li>
<div><a href="http://www.odata.org/developers/protocols/json-format" target="_blank">OData: JavaScript Object Notation (JSON) Format</a></div>
</li><li>
<div><a href="http://www.odata.org/developers/protocols/atom-format" target="_blank">OData: AtomPub Format</a></div>
</li></ul>
</div>
</div>
</div>
