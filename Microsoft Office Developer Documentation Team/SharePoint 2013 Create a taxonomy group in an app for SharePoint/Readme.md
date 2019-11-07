# SharePoint 2013: Create a taxonomy group in an app for SharePoint
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- SharePoint Server 2013
- apps for SharePoint
## Topics
- apps for SharePoint
## Updated
- 06/13/2014
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Create a taxonomy group in an app for SharePoint</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>Learn how to create a taxonomy group in a provider-hosted app for SharePoint.</p>
</div>
<div>
<p><span>Provided by:</span> Thomas Mechelke, Microsoft Corporation</p>
<p>This sample shows you how to add a taxonomy group to the term store of a SharePoint 2013 site with a provider-hosted app for SharePoint. This app works only for users who are administrators of the site's enterprise metadata service.</p>
<strong>
<div class="caption">Figure 1. Create a new taxonomy group</div>
</strong><br>
<strong></strong><img src="116794-image.png" alt=""></div>
<h1>Prerequisites</h1>
<div id="sectionSection0">
<p>This sample requires the following:</p>
<ul>
<li>
<p>An Office 365 Developer Site</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your development computer</p>
</li></ul>
</div>
<h1>Key components</h1>
<div id="sectionSection1">
<p>&nbsp;</p>
<ul>
<li>
<p><strong>TaxonomyApp</strong> project, which includes the AppManifest.xml file that contains the registration information for the provider-hosted app for SharePoint.</p>
</li><li>
<p><strong>TaxonomyAppWeb</strong> project, which contains:</p>
<ul>
<li>
<p><strong>Pages\Default.aspx</strong>. The page that displays the app's user interface.</p>
</li><li>
<p><strong>Web.config</strong>. Stores the client id and client secret.</p>
</li></ul>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection2">
<p>Follow these steps to configure the sample.</p>
<ol>
<li>
<p>Open the <strong>TaxonomyApp.sln</strong> file in Visual Studio 2012.</p>
</li><li>
<p>In the <strong><span class="ui">Properties</span></strong> pane, change the <strong>
<span class="ui">Site URL</span></strong> property. It is the absolute URL of your SharePoint test site collection on Office 365:
<span>https://</span><span>&lt;my tenant&gt;</span><span>.sharepoint.com/sites/dev</span>.</p>
</li></ol>
</div>
<h1>Build and deploy the sample</h1>
<div id="sectionSection3">
<p>Before you run the sample, you'll need to make sure that you are an administrator for the enterprise metadata service of your site.</p>
<h3>To make yourself an enterprise metadata service administrator</h3>
<div>
<ol>
<li>
<p>Navigate to the home page of your Office 365 Developer Site.</p>
</li><li>
<p>Click on the <strong><span class="ui">Admin</span></strong> dropdown menu in the upper right corner of the page. Select
<strong><span class="ui">SharePoint</span></strong>.</p>
</li><li>
<p>Click on the term store link in the left pane.</p>
</li><li>
<p>Enter the user id and domain that you created for your developer site in the <strong>
<span class="ui">Term Store Administrators</span></strong> box, as in Figure 2.</p>
<strong>
<div class="caption">Figure 2. Make yourself a term store administrator</div>
</strong><br>
<strong></strong><img src="116795-image.png" alt=""> </li><li>
<p>Press F5 to build and deploy the app.</p>
</li><li>
<p>Choose <strong><span class="ui">Trust It</span></strong>, and wait for the start page of the app to load.</p>
</li></ol>
</div>
</div>
<h1>Run and test the sample</h1>
<div id="sectionSection4">
<ol>
<li>
<p>In the web browser, click on the <strong><span class="ui">Create Plant Taxonomy</span></strong> box to create the new taxonomy group.</p>
</li><li>
<p>Click on the Display Plant Taxonomy box to see the new taxonomy group.</p>
</li></ol>
</div>
<h1>Change log</h1>
<div id="sectionSection5"><strong>
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
<p>July 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Related content</h1>
<div id="sectionSection6">
<ul>
<li>
<p><a href="http://channel9.msdn.com/Series/Reimagine-SharePoint-Development/Understanding-the-differences-between-Server-Side-Object-Model-and-the-Client-Side-Object-Model" target="_blank">Understanding the differences between Server-Side Object Model and
 the Client-Side Object Model</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
