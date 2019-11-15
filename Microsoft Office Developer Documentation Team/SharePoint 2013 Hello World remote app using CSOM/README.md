# SharePoint 2013: Hello World remote app using CSOM
## License
- Apache License, Version 2.0
## Technologies
- C#
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
- SharePoint Add-ins
## Topics
- Authentication
- data and storage
## Updated
- 08/08/2015
## Description

<p><span style="font-size:medium; color:#ff0000">This sample has been removed. Please use the sample at:
<a href="http://code.msdn.microsoft.com/sharepoint-2013-perform-eba8df54">Perform basic data access operations by using CSOM in apps</a>.&nbsp;</span></p>
<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Hello World remote app using CSOM</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div class="summary">
<p><span class="label">Summary:</span>&nbsp;&nbsp;Learn how to create a basic &quot;hello world&quot; provider-hosted app for SharePoint that uses the SharePoint CSOM to read information about a SharePoint 2013 site from a remote web application.</p>
</div>
<div class="introduction">
<p><strong>Last modified: </strong>July 01, 2012</p>
<p><strong>In this article</strong><br>
<a href="#sectionSection0">Description of the sample</a><br>
<a href="#sectionSection1">Prerequisites</a><br>
<a href="#sectionSection2">Key components of the sample</a><br>
<a href="#sectionSection3">Configure the sample</a><br>
<a href="#sectionSection4">Build the sample</a><br>
<a href="#sectionSection5">Run and test the sample</a><br>
<a href="#sectionSection6">Troubleshooting</a><br>
<a href="#sectionSection7">Change log</a><br>
<a href="#sectionSection8">Related content</a></p>
<p>&nbsp;</p>
</div>
<a name="O15Readme_Description"></a><a name="sectionSection0"></a>
<h2 class="heading">Description of the sample</h2>
<div class="section" id="sectionSection0">
<p>The sample demonstrates how to read data about the parent SharePoint site on which the app is installed by using OAuth and the SharePoint CSOM.</p>
<p>The code that uses the SharePoint CSOM is located in the Home.aspx.cs file of the BasicSelfHostedAppWeb project. The following screen shot shows how the Home.aspx page of the app appears after you install and launch the app.</p>
<div class="caption"><strong>Figure 1. Home.aspx page in the Hello World remote app for SharePoint using CSOM</strong></div>
<br>
<img id="116784" src="http://i1.code.msdn.s-msft.com/sharepoint/sharepoint-2013-hello-0fd15fbf/image/file/116784/1/sp15_basicself-hostedapp.gif" alt="Basic self-hosted app launch page" width="197" height="320"></div>
<a name="O15Readme_Prereq"></a><a name="sectionSection1"></a>
<h2 class="heading">Prerequisites</h2>
<div class="section" id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A SharePoint 2013 development environment that is configured for app isolation and OAuth</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your developer computer</p>
</li></ul>
</div>
<a name="O15Readme_components"></a><a name="sectionSection2"></a>
<h2 class="heading">Key components of the sample</h2>
<div class="section" id="sectionSection2">
<p>The sample app contains the following:</p>
<ul>
<li>
<p>BasicSelfHostedApp project, which contains the AppManifest.xml file</p>
</li><li>
<p>BasicSelfHostedAppWeb project, which contains the following:</p>
<ul>
<li>
<p>Home.aspx file, which contains the HTML and ASP.NET controls for the app's user interface</p>
</li><li>
<p>Home.aspx.cs file, which contains the C# code that uses the CSOM to read and write data to and from the parent web</p>
</li><li>
<p>web.config file</p>
</li></ul>
</li></ul>
</div>
<a name="O15Readme_config"></a><a name="sectionSection3"></a>
<h2 class="heading">Configure the sample</h2>
<div class="section" id="sectionSection3">
<p>To configure the hello world remote app using CSOM sample, update the <strong>
SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint 2013 site.</p>
</div>
<a name="O15Readme_build"></a><a name="sectionSection4"></a>
<h2 class="heading">Build the sample</h2>
<div class="section" id="sectionSection4">
<p>Press the F5 key to build and deploy the app.</p>
</div>
<a name="O15Readme_test"></a><a name="sectionSection5"></a>
<h2 class="heading">Run and test the sample</h2>
<div class="section" id="sectionSection5">
<p>&nbsp;</p>
<ol>
<li>
<p>Choose <span class="ui">Trust It</span> on the consent page to grant permissions to the app.</p>
</li><li>
<p>Choose <span class="ui">Populate Data</span> to see some basic information about the parent web.</p>
</li></ol>
<h3 class="subHeading">Example</h3>
<div class="subsection">
<p>The following figure shows an example of the kinds of information that this sample app can read and display.</p>
<div class="caption"><strong>Figure 2. View populated data about the parent web</strong></div>
<br>
<img id="116785" src="http://i1.code.msdn.s-msft.com/sharepoint/sharepoint-2013-hello-0fd15fbf/image/file/116785/1/sp15_basicselfhostedpopulated.gif" alt="Basic self-hosted app with data populated" width="735" height="746"></div>
</div>
<a name="O15Readme_Troubleshoot"></a><a name="sectionSection6"></a>
<h2 class="heading">Troubleshooting</h2>
<div class="section" id="sectionSection6">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how you can solve them.</p>
<div class="caption"></div>
<div class="tableSection">
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
<p>Locate the <span class="keyword">applicationhost.config</span> file in %userprofile%\Documents\IISExpress\config.</p>
<p>Locate the handler entry for <strong>StaticFile</strong>, and add the verbs <span class="keyword">
GET</span>, <span class="keyword">HEAD</span>, <span class="keyword">POST</span>,
<span class="keyword">DEBUG</span>, and <span class="keyword">TRACE</span>.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<a name="O15Readme_Changelog"></a><a name="sectionSection7"></a>
<h2 class="heading">Change log</h2>
<div class="section" id="sectionSection7">
<div class="caption"></div>
<div class="tableSection">
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
<a name="O15Readme_RelatedContent"></a><a name="sectionSection8"></a>
<h2 class="heading">Related content</h2>
<div class="section" id="sectionSection8">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/3038dd73-41ee-436f-8c78-ef8e6869bf7b.aspx" target="_blank">How to: Create a basic provider-hosted app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" target="_blank">SharePoint 2013 development overview</a></p>
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
</li></ul>
</div>
</div>
</div>
