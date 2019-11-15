# SharePoint 2013: Use Knockout.js and the REST service to display list data
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Visual Studio 2012
## Topics
- apps for SharePoint
## Updated
- 02/12/2014
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Use Knockout.js and the REST service to display list data</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>Use the SharePoint 2013 REST interface and Knockout.js to display list data in a responsive grid view.</p>
</div>
<div>
<p><span>Provided by:</span> Todd Baginski, <a href="http://www.canviz.com" target="_blank">
Canviz Consulting </a></p>
<p>This sample app for SharePoint shows how to use the SharePoint 2013 REST interface to get list item data from a SharePoint list and then to use Knockout.js to create a rich and responsive display with a clean underlying data model.</p>
<strong>
<div class="caption">Figure 1. Knockout sample client web part</div>
</strong><br>
&nbsp;<img src="96375-image.png" alt=""></div>
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
<h1>Sample for demo only: deviations from best practices</h1>
<div id="sectionSection1">
<p>The sample is focused on demonstrating how to use the SharePoint 2013 REST service and Knockout.js to retrieve data and render data, so it doesn't conform to all the best practices that you should use in a production app. Specifically, be aware of the following:</p>
<ul>
<li>
<p>The app has no exception handling.</p>
</li></ul>
</div>
<h1>Key components</h1>
<div id="sectionSection2">
<p>The app's <strong><span class="ui">KnockoutSampleApp</span></strong> project includes the following:</p>
<ul>
<li>
<p><strong>list_Customers</strong>. A sample SharePoint list of customers and contact information that you deploy to the app web.</p>
</li><li>
<p><strong>Pages\Default.aspx</strong> project. The landing page for the app. This page displays a button that populates the
<strong>list_Customers</strong> list.</p>
</li><li>
<p><strong>Pages\KnockoutSampleClientWebPart.aspx</strong>. The page that displays the app part.</p>
</li><li>
<p><strong>Scripts\App.js</strong>. The JavaScript file that contains the logic that populates the
<strong>list_Customers</strong> list.</p>
</li></ul>
</div>
<h1>Build and deploy the sample</h1>
<div id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<ul>
<li>
<p>Open the <strong>KnockoutSampleApp.sln</strong> file in Visual Studio 2012.</p>
</li><li>
<p>In the <strong><span class="ui">Properties</span></strong> pane, change the <strong>
<span class="ui">Site URL</span></strong> property. It is the absolute URL of your SharePoint test site collection on Office 365: https://<span>&lt;my tenant&gt;</span>.sharepoint.com/sites/dev.</p>
</li><li>
<p>Right-click the <strong><span class="ui">KnockoutSampleApp.sln</span></strong> project in
<strong><span class="ui">Solution Explorer</span></strong>, and choose <strong>
<span class="ui">Publish</span></strong>.</p>
</li><li>
<p>Choose <strong><span class="ui">Finish</span></strong>.</p>
</li><li>
<p>The resulting app package file has an .app extension (KnockoutSampleApp.app) and is saved in the
<strong><span class="ui">app.publish</span></strong> subfolder of the <strong><span class="ui">bin\Debug</span></strong> folder of the Visual Studio solution.</p>
</li><li>
<p>In your browser, navigate to the home page of your Office 365 Developer Site. In the left panel, choose the
<strong><span class="ui">Apps in Testing</span></strong> link.</p>
</li><li>
<p>Choose <strong><span class="ui">new app to deploy</span></strong>, and follow the instructions to upload the KnockoutSampleApp.app package file and deploy it to your Developer Site.</p>
</li><li>
<p>Choose <strong><span class="ui">Trust It</span></strong>, and wait for the app to install.</p>
</li></ul>
</div>
<h1>Run and test the sample</h1>
<div id="sectionSection4">
<ol>
<li>
<p>In your browser, navigate to the home page of your Office 365 Developer Site. Click on the
<strong><span class="ui">KnockoutSampleApp</span></strong> link in the <strong>
<span class="ui">Apps in Testing</span></strong> list to run your app for SharePoint. Click the
<strong><span class="ui">Generate Sample Data</span></strong> button to add sample data to the list of customers.</p>
<strong>
<div class="caption">Figure 2. Generate Sample Data button</div>
</strong><br>
&nbsp;<img src="96374-image.png" alt=""> </li><li>
<p>Navigate to the home page of your Office 365 Developer Site. Edit the page and add the KnockoutSampleClientWebPart App Part.</p>
<strong>
<div class="caption">Figure 3. Add the sample App Part</div>
</strong><br>
<img id="108520" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-use-65c59e34/image/file/108520/1/sp15_knockoutapppart.png" alt="" width="516" height="229">
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
<p>September 2013</p>
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
<p><a href="http://msdn.microsoft.com/en-us/library/fp142379.aspx" target="_blank">How to: Create a basic provider-hosted app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp179933.aspx" target="_blank">Deploying and installing apps for SharePoint: methods and options</a></p>
</li></ul>
</div>
</div>
</div>
