# SharePoint 2013: Add list item properties with a remote event receiver
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
<td align="left"><span id="nsrTitle">SharePoint 2013: Add list item properties with a remote event receiver</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>Learn how to create a remote event receiver in a provider-hosted app for SharePoint.</p>
</div>
<div>
<p><strong>Last modified: </strong>April 07, 2014</p>
<p><strong>In this article</strong> <br>
<a href="#sectionSection0">Prerequisites</a> <br>
<a href="#sectionSection1">Key components</a> <br>
<a href="#sectionSection2">Configure the sample</a> <br>
<a href="#sectionSection3">Build and deploy the sample</a> <br>
<a href="#sectionSection4">Run and test the sample</a> <br>
<a href="#sectionSection5">Change log</a> <br>
<a href="#sectionSection6">Related content</a></p>
<p><span>Provided by:</span> Thomas Mechelke, Microsoft Corporation</p>
<p>This sample shows how to add a remote event receiver to a provider-hosted app for SharePoint. The app creates a list of plants on the app web of your SharePoint site. Whenever you add an item to the list, the remote event receiver populates the new item's
 image search property.</p>
<strong>
<div class="caption">Figure 1. New list item with image search property</div>
</strong><br>
<strong></strong><img src="116801-image.png" alt=""></div>
<h1>Prerequisites</h1>
<div id="sectionSection0">
<p>This sample requires the following:</p>
<ul>
<li>
<p>An Office 365 Developer Site</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your development computer</p>
</li><li>
<p>A <a href="https://manage.windowsazure.com" target="_blank">Microsoft Azure account</a> with permissions to create a service bus</p>
</li></ul>
</div>
<h1>Key components</h1>
<div id="sectionSection1">
<p>&nbsp;</p>
<ul>
<li>
<p><strong>RemoteEventReceiverApp</strong> project, which contains:</p>
<ul>
<li>
<p><strong>AppManifest.xml</strong>: The configuration file that defines the app as a provider-hosted app for SharePoint.</p>
</li><li>
<p><strong>Plants</strong>: The custom list that deploys with the app.</p>
</li></ul>
</li><li>
<p><strong>RemoteEventReceiverAppWeb</strong> project, which contains:</p>
<ul>
<li>
<p><strong>Services\OnPlantUpdated.svc</strong>. Defines the remote event receiver for the app.</p>
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
<p>Open the <strong>RemoteEventReceiverApp.sln</strong> file in Visual Studio 2012.</p>
</li><li>
<p>In the <strong><span class="ui">Properties</span></strong> pane, change the <strong>
<span class="ui">Site URL</span></strong> property. It is the absolute URL of your SharePoint test site collection on Office 365:
<span>https://</span><span>&lt;my tenant&gt;</span><span>.sharepoint.com/sites/dev</span>.</p>
</li></ol>
</div>
<h1>Build and deploy the sample</h1>
<div id="sectionSection3">
<p>Follow these steps to build and deploy the sample.</p>
<h3>To configure and deploy the app for SharePoint</h3>
<div>
<ol>
<li>
<p>Create a service bus namespace on <a href="https://manage.windowsazure.com" target="_blank">
Microsoft Azure</a>, and copy its connection string. <a href="http://msdn.microsoft.com/en-us/library/windowsazure/hh690931.aspx" target="_blank">
How To: Create or Modify a Service Bus Service Namespace</a> explains how to do this.</p>
</li><li>
<p>Right-click the <strong><span class="ui">RemoteEventReceiverApp</span></strong> project in
<strong><span class="ui">Solution Explorer</span></strong>, and choose <strong>
<span class="ui">Properties</span></strong>.</p>
</li><li>
<p>Navigate to the SharePoint tab, and select the <strong><span class="ui">Enable remote event debugging</span></strong> check box.</p>
</li><li>
<p>For <strong><span class="ui">Microsoft Azure Service Bus Connection String</span></strong>, enter the connection string for the new service bus namespace that you created.</p>
<strong>
<div class="caption">Figure 2. Select the enable remote event debugging check box and enter the Microsoft Azure service bus connection string</div>
</strong><br>
<strong></strong><img src="116802-image.png" alt=""> </li><li>
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
<p>In the web browser, add a plant name to the list that appears when you start the app.</p>
<strong>
<div class="caption">Figure 3. Add a plant name to the list that deploys with the app</div>
</strong><br>
<strong></strong><img src="116803-image.png" alt=""> </li><li>
<p>The app has a break point in the code for the remote event receiver, so you will return to Visual Studio 2012 and see the code. Press F5 to continue.</p>
</li><li>
<p>The new item will appear in the list, along with the image search field that the remote event receiver has populated.</p>
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
<p><a href="http://channel9.msdn.com/Series/Reimagine-SharePoint-Development/Migrating-a-SharePoint-Event-Receiver-to-a-Remote-Event-Receiver" target="_blank">Migrating a SharePoint Event Receiver to a Remote Event Receiver</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
