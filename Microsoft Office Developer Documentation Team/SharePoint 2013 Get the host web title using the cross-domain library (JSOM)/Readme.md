# SharePoint 2013: Get the host web title using the cross-domain library (JSOM)
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
- data and storage
## Updated
- 05/30/2014
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Get the host web title using the cross-domain library and JSOM</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>Learn how to use the cross-domain library in apps for SharePoint to read the host web title by using the JavaScript object model (JSOM).</p>
</div>
<div>
<p><strong>Last modified: </strong>May 28, 2014</p>
<p><strong>In this article</strong> <br>
<a href="#O15Readme_Description">Description of the sample</a> <br>
<a href="#O15Readme_Prereq">Prerequisites</a> <br>
<a href="#O15Readme_components">Key components of the sample</a> <br>
<a href="#O15Readme_config">Configure the sample</a> <br>
<a href="#O15Readme_test">Run and test the sample</a> <br>
<a href="#O15Readme_Troubleshoot">Troubleshooting</a> <br>
<a href="#O15Readme_Changelog">Change log</a> <br>
<a href="#O15Readme_RelatedContent">Related content</a></p>
<p>This sample provider-hosted app demonstrates how to use the cross-domain library in SharePoint 2013 to read the title property of the host web. The app displays the title in a simple page using the JavaScript object model (JSOM).</p>
</div>
<h2>Description of the sample</h2>
<div id="sectionSection0">
<p>The code that uses the cross-domain library is in the ReadTitle.aspx file of the CrossDomainWeb project. Figure 1 shows the ReadTitle page after you install and run the app.</p>
<strong>
<div class="caption">Figure 1. Browser windows after running the solution</div>
</strong><br>
<strong></strong><img src="115847-image.png" alt=""></div>
<h2>Prerequisites</h2>
<div id="sectionSection1">
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
<h2>Key components of the sample</h2>
<div id="sectionSection2">
<p>The sample contains the following:</p>
<ul>
<li>
<p>CrossDomainApp project, which contains the AppManifest.xml file</p>
<ul>
<li>
<p>AppHost.aspx page, which implements a best practice for using the cross-domain library in cross-zone scenarios. For more information, see
<a href="http://msdn.microsoft.com/library/3d24f916-60b2-4ea9-b182-82e33cad06e8" target="_blank">
Work with the cross-domain library across different Internet Explorer security zones in apps for SharePoint</a>.</p>
</li></ul>
</li><li>
<p>CrossDomainWeb project</p>
<ul>
<li>
<p>ReadTitle.aspx file, which contains a reference to the cross-domain library</p>
</li><li>
<p>Web.config file</p>
</li></ul>
</li></ul>
</div>
<h2>Configure the sample</h2>
<div id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<ul>
<li>
<p>Update the <strong>Site URL</strong> property of the solution with the URL of the home page of your SharePoint website.</p>
</li></ul>
</div>
<h2>Run and test the sample</h2>
<div id="sectionSection4">
<p>&nbsp;</p>
<ol>
<li>
<p>Press F5 to build and deploy the app.</p>
</li><li>
<p>Choose <strong><span class="ui">Trust It</span></strong> on the consent page to grant permissions to the app.</p>
</li></ol>
<p>You should see an HTML page with the text <strong>The host web title is:</strong> followed by the SharePoint website title.</p>
</div>
<h2>Troubleshooting</h2>
<div id="sectionSection5">
<p>For troubleshooting steps, visit the <a href="http://msdn.microsoft.com/library/bc37ff5c-1285-40af-98ae-01286696242d# SP15Accessdatafromremoteapp_Troubleshoot" target="_blank">
Troubleshooting the solution</a> table in the cross-domain library documentation article.</p>
</div>
<h2>Change log</h2>
<div id="sectionSection6">
<ul>
<li>
<p>First version: September 2012</p>
</li><li>
<p>Updated version: October 2013. Changed the start page of the project to an ASPX page that works better with provider-hosted apps.</p>
</li><li>
<p>2nd version: May 2014</p>
</li></ul>
</div>
<h2>Related content</h2>
<div id="sectionSection7">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/1534a5f4-1d83-45b4-9714-3a1995677d85" target="_blank">Working with data in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/bc37ff5c-1285-40af-98ae-01286696242d" target="_blank">How to: Access SharePoint 2013 data from remote apps using the cross-domain library</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/3d24f916-60b2-4ea9-b182-82e33cad06e8" target="_blank">Work with the cross-domain library across different Internet Explorer security zones in apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/26f2999e-db7f-4fe7-a00f-05b009b1927d" target="_blank">What you can do in an app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/bde5647a-fff1-4b51-b67b-2139de79ce4a" target="_blank">Authorization and authentication for apps in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/f36645da-77c5-47f1-a2ca-13d4b62b320d" target="_blank">Choose the right API set in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/0942fdce-3227-496a-8873-399fc1dbb72c" target="_blank">Three ways to think about design options for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88" target="_blank">Important aspects of the app for SharePoint architecture and development landscape</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/3034f03c-2d5a-46de-9cb8-2c101ff194fa" target="_blank">Data storage options in apps for SharePoint</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
