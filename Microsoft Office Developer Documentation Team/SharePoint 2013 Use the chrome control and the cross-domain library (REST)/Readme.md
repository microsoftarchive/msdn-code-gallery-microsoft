# SharePoint 2013: Use the chrome control and the cross-domain library (REST)
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- REST
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- User Experience
## Updated
- 06/24/2014
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Use the chrome control and the cross-domain library in a remote app (REST)</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div class="summary">
<p><span class="label">Summary:</span>&nbsp;&nbsp;This code sample includes a remote webpage that hosts the chrome control and retrieves information from the host web using the cross domain library and Representational State Transfer (REST) in SharePoint
 2013.</p>
</div>
<div class="introduction">
<p><strong>Last modified: </strong>June 13, 2014</p>
<p><strong>In this article</strong><br>
<a href="#O15Readme_Description">Description of the sample</a><br>
<a href="#O15Readme_Prereq">Prerequisites</a><br>
<a href="#O15Readme_components">Key components of the sample</a><br>
<a href="#O15Readme_config">Configure the sample</a><br>
<a href="#O15Readme_test">Run and test the sample</a><br>
<a href="#O15Readme_Troubleshoot">Troubleshooting</a><br>
<a href="#O15Readme_Changelog">Change log</a><br>
<a href="#O15Readme_RelatedContent">Related content</a></p>
<p>This sample provider-hosted app demonstrates how to add the chrome control to your app for SharePoint and how to retrieve data from a SharePoint website (an app web). With the chrome control, you can develop a user experience that resembles the user experience
 in the SharePoint website that your app is deployed to. When using the chrome control in your page, you can also use the SharePoint website's style sheet.</p>
</div>
<a name="O15Readme_Description"></a>
<h2 class="heading">Description of the sample</h2>
<div class="section" id="sectionSection0">
<p>The markup that declares the chrome control and issues the cross-domain call is in the ChromeCrossDomain.aspx file in the ChromeCrossDomainWeb project. Figure 1 shows the chrome control.</p>
<div class="caption">Figure 1. Chrome control and cross-domain library</div>
<br>
<img id="117524" src="117524-chromecrossdomain_result.jpg" alt="Chrome control and cross-domain library" width="689" height="682"></div>
<a name="O15Readme_Prereq"></a>
<h2 class="heading">Prerequisites</h2>
<div class="section" id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Microsoft Visual Studio 2012</p>
</li><li>
<p>Office Developer Tools for Visual Studio 2012</p>
</li><li>
<p>A SharePoint 2013 development environment (app isolation required for on-premises scenarios)</p>
</li></ul>
<p>For more information, see <a href="http://msdn.microsoft.com/library/jj163980.aspx" target="_blank">
Get started developing apps for SharePoint</a>.</p>
</div>
<a name="O15Readme_components"></a>
<h2 class="heading">Key components of the sample</h2>
<div class="section" id="sectionSection2">
<p>The sample contains the following:</p>
<ul>
<li>
<p>ChromeCrossDomainApp project</p>
<ul>
<li>
<p>AppManifest.xml file</p>
</li></ul>
</li><li>
<p>ChromeCrossDomainWeb project</p>
<ul>
<li>
<p>ChromeCrossDomain.aspx file, which renders the chrome control and issues the cross-domain call</p>
</li><li>
<p>Web.config file.</p>
</li></ul>
</li></ul>
</div>
<a name="O15Readme_config"></a>
<h2 class="heading">Configure the sample</h2>
<div class="section" id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<ul>
<li>
<p>Update the <strong>SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint website.</p>
</li></ul>
</div>
<a name="O15Readme_test"></a>
<h2 class="heading">Run and test the sample</h2>
<div class="section" id="sectionSection4">
<p>&nbsp;</p>
<ol>
<li>
<p>Press F5 to build and deploy the app.</p>
</li><li>
<p>Choose <span class="ui">Trust It</span> on the consent page to grant permissions to the app.</p>
</li><li>
<p>Click the <span class="ui">ChromeControlCrossDomainREST</span> app icon.</p>
</li></ol>
</div>
<a name="O15Readme_Troubleshoot"></a>
<h2 class="heading">Troubleshooting</h2>
<div class="section" id="sectionSection5">
<p>For troubleshooting steps, visit the <a href="http://msdn.microsoft.com/library/bc37ff5c-1285-40af-98ae-01286696242d# SP15Accessdatafromremoteapp_Troubleshoot" target="_blank">
Troubleshooting the solution</a> table in the cross-domain library documentation article.</p>
</div>
<a name="O15Readme_Changelog"></a>
<h2 class="heading">Change log</h2>
<div class="section" id="sectionSection6">
<ul>
<li>
<p>First version: July 2012</p>
</li><li>
<p>Updated version: October 2013. Changed the start page of the project to an ASPX page that works better with provider-hosted apps.</p>
</li><li>
<p>2nd version: June 2014</p>
</li></ul>
</div>
<a name="O15Readme_RelatedContent"></a>
<h2 class="heading">Related content</h2>
<div class="section" id="sectionSection7">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">Setting up a SharePoint 2013 development environment for apps</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/bfdd0a58-2cc5-4805-ac89-4bd2fe6f3b09" target="_blank">Create UX components</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/d60f409a-b292-4c06-8128-88629091b753" target="_blank">UX design for apps</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/2148980b-c2b6-4294-b8f7-cfc07f925091" target="_blank">Data access options for apps in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/1534a5f4-1d83-45b4-9714-3a1995677d85" target="_blank">Working with data</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/7c2d0812-76e8-44c1-88bf-4a75eb6f82b1" target="_blank">How to: Use the client chrome control in apps for SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/bc37ff5c-1285-40af-98ae-01286696242d" target="_blank">How to: Read the host web's title from your remote app using the cross-domain library</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/26f2999e-db7f-4fe7-a00f-05b009b1927d" target="_blank">What you can do in an app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/0942fdce-3227-496a-8873-399fc1dbb72c" target="_blank">Design considerations for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88" target="_blank">Critical aspects of the app for SharePoint architecture and development landscape</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/bde5647a-fff1-4b51-b67b-2139de79ce4a" target="_blank">OAuth in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/d4b5c277-ed50-420c-8a9b-860342284b72" target="_blank">Programming using the SharePoint 2013 REST service</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/0942fdce-3227-496a-8873-399fc1dbb72c" target="_blank">Design considerations for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88" target="_blank">Critical aspects of the app for SharePoint architecture and development landscape</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/3034f03c-2d5a-46de-9cb8-2c101ff194fa" target="_blank">Data storage options in apps for SharePoint</a></p>
</li></ul>
</div>
</div>
</div>
