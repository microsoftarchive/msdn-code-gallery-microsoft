# SharePoint 2013: Import, validate, and manage app licenses
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- C#
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- sites and content
## Updated
- 06/20/2013
## Description

<div id="header">
<table id="bottomTable" cellpadding="0" cellspacing="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText"></span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Import, validate, and manage app licenses</span>
</td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<p></p>
<div>
<p>Learn how to import and manage app licenses for apps for SharePoint, and how to retrieve, validate, and communicate licensing states.
</p>
</div>
<div>
<p>This sample provides tools to help you implement the app license framework in your apps for SharePoint. Using the tools in the sample, you can import test licenses, validate app licenses, and manage license assignment within a SharePoint site.</p>
<p>The sample demonstrates how to do the following:</p>
<ul>
<li>
<p>Import test licenses.</p>
</li><li>
<p>Validate licenses, including test licenses.</p>
</li></ul>
<p>This sample also includes helper classes that you can use to implement licensing in your own apps for SharePoint projects. These helper classes assist in retrieving and validating app licenses.</p>
</div>
<h1>Prerequisites</h1>
<div id="sectionSection0" name="collapseableSection">
<p>This sample requires the following:</p>
<ul>
<li>
<p>An Office 365 Developer Site</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your developer computer</p>
</li></ul>
</div>
<h1>Key components</h1>
<div id="sectionSection1" name="collapseableSection">
<p>The sample presents a user interface (UI) that lets you do the following:</p>
<ul>
<li>
<p><span>Import Test Licenses</span> </p>
<p>Enables site collection administrators to import test licenses. Use this tool to simulate purchases; you should then manually start your app to verify whether license checks work appropriately.</p>
</li><li>
<p><span>Validate License Sample</span> </p>
<p>An ASP.NET web form example of how to retrieve and validate a license in your own apps, in both production and test modes. The sample retrieves the top-most license, and displays a warning message at the top of the page.</p>
</li><li>
<p><span>Manage Existing Licenses</span> </p>
<p>Provides a shortcut to the SharePoint license management UI. Site collection administrators can view license details, assign seats, and delete licenses.</p>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection2" name="collapseableSection">
<p>To configure the basic data operations sample app, update the <b>SiteUrl</b> property of the solution with the URL of the home page of your Office 365 Developer Site.</p>
</div>
<h1>Run and test the sample</h1>
<div id="sectionSection3" name="collapseableSection">
<ol>
<li>
<p>Press the F5 key to build and deploy the app.</p>
</li><li>
<p>Choose <b><span class="ui">Trust It</span></b> on the consent page to grant permissions to the app.</p>
</li><li>
<p>Use the app's interface to import, validate, and manage app licenses.</p>
</li></ol>
</div>
<h1>Troubleshooting</h1>
<div id="sectionSection4" name="collapseableSection">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how you can solve them.</p>
<b>
<div class="caption"></div>
</b>
<div>
<table width="50%" cellspacing="2" cellpadding="5" frame="lhs">
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
<p>HTTP error 405 <b>Method not allowed</b>.</p>
</td>
<td>
<p>Locate the <span value="applicationhost.config"><b><span class="keyword">applicationhost.config</span></b></span> file in %userprofile%\Documents\IISExpress\config.</p>
<p>Locate the handler entry for <b>StaticFile</b>, and add the verbs <span value="GET">
<b><span class="keyword">GET</span></b></span>, <span value="HEAD"><b><span class="keyword">HEAD</span></b></span>,
<span value="POST"><b><span class="keyword">POST</span></b></span>, <span value="DEBUG">
<b><span class="keyword">DEBUG</span></b></span>, and <span value="TRACE"><b><span class="keyword">TRACE</span></b></span>.</p>
</td>
</tr>
<tr>
<td>
<p>An attempt to delete a folder does not work.</p>
</td>
<td>
<p>This likely happens because the folder is used internally and cannot be deleted.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Change log</h1>
<div id="sectionSection5" name="collapseableSection"><b>
<div class="caption"></div>
</b>
<div>
<table width="50%" cellspacing="2" cellpadding="5" frame="lhs">
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
<p>June 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Related content</h1>
<div id="sectionSection6" name="collapseableSection">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/office/apps/verificationsvc.aspx" target="_blank">VerificationSvc namespace</a>
</p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/office/apps/microsoft.sharepoint.client.utilities.utility.importapplicense.aspx" target="_blank">Utility.ImportAppLicense method</a>
</p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/office/apps/microsoft.sharepoint.client.utilities.utility.getapplicenseinformation.aspx" target="_blank">Utility.GetAppLicenseInformation method</a>
</p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/office/apps/jj163257(v=office.15)" target="_blank">Licensing apps for Office and SharePoint</a>
</p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/office/apps/jj164035.aspx" target="_blank">How to: Add license checks to your app for SharePoint</a>
</p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/office/apps/jj163264.aspx" target="_blank">Best practices and design patterns for app license checking</a>
</p>
</li></ul>
</div>
</div>
</div>
