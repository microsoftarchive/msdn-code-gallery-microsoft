# SharePoint 2013: Perform operations on SharePoint Document Library from PHP site
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- REST
- Javascript
- Sharepoint Online
- PHP
- Visual Studio 2012
## Topics
- Cloud
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
<td align="left"><span id="nsrTitle">SharePoint 2013: Perform operations on a SharePoint document library from a PHP site</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p><span>Summary:</span> Learn how to perform operations on files stored in a SharePoint 2013 document library from a PHP site.</p>
</div>
<div>
<p><strong>Last modified: </strong>April 07, 2014</p>
<p><strong>In this article</strong> <br>
<a href="#sectionSection0">Description of the sample</a> <br>
<a href="#sectionSection1">Prerequisites</a> <br>
<a href="#sectionSection2">Sample for demo only: deviations from best practices</a>
<br>
<a href="#sectionSection3">Key components of the sample</a> <br>
<a href="#sectionSection4">Configure the sample</a> <br>
<a href="#sectionSection5">Build, deploy, and run the sample</a> <br>
<a href="#sectionSection6">Troubleshooting</a> <br>
<a href="#sectionSection7">Change log</a> <br>
<a href="#sectionSection8">Related content</a></p>
<p>&nbsp;</p>
</div>
<h1>Description of the sample</h1>
<div id="sectionSection0">
<p><span>Provided by:</span> <a href="http://www.canviz.com" target="_blank">Todd Baginski</a>, Canviz Consulting</p>
<p>This sample provider-hosted app for SharePoint shows how to perform the following operations on files stores in a SharePoint 2013 document library from a PHP site:</p>
<ul>
<li>
<p>List documents</p>
</li><li>
<p>Check out a document</p>
</li><li>
<p>Check in a document</p>
</li><li>
<p>Upload a document</p>
</li><li>
<p>Edit document metadata</p>
</li></ul>
<p>The index.php page of the app appears after you install and launch the app. It displays all items in a SharePoint 2013 document library and uses the SharePoint theme from the host web. The app includes an
<strong><span class="ui">Add New Document</span></strong> button at the bottom right corner, as shown in Figure 1.</p>
<strong>
<div class="caption">Figure 1. The download button on the index.php page of the app for SharePoint</div>
</strong><br>
<strong>&nbsp;</strong><img src="116576-image.png" alt="">
<p>You can upload a file to the SharePoint library by clicking the <strong><span class="ui">Add New Document</span></strong> button. The index.php page refreshes to display the new item, as shown in Figure 2.</p>
<strong>
<div class="caption">Figure 2. Newly added file in list of files</div>
</strong><br>
<strong>&nbsp;</strong><img src="116577-image.png" alt="">
<p>Right-click the title of the file to perform the following operations: download, check in, check out and edit properties, as shown in Figure 3.</p>
<strong>
<div class="caption">Figure 3. Metadata popup menu appears when you right-click title</div>
</strong><br>
<strong>&nbsp;</strong><img src="116578-image.png" alt=""></div>
<h1>Prerequisites</h1>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>An Office 365 Developer Site</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your development computer</p>
</li><li>
<p>A <a href="https://manage.windowsazure.com" target="_blank">Microsoft Azure account</a> with permissions to deploy a cloud service and a worker role</p>
</li><li>
<p>Microsoft Azure SDK for .NET (VS 2012) 1.8</p>
</li></ul>
</div>
<h1>Sample for demo only: deviations from best practices</h1>
<div id="sectionSection2">
<p>The sample's focus is to demonstrate a provider-hosted app for SharePoint that runs on a remote PHP site. It does not conform to all the best practices that you should use in a production app. Especially, be aware of the following:</p>
<ul>
<li>
<p>The app does not provide any protection from SQL injection attacks.</p>
</li><li>
<p>The app has no exception handling.</p>
</li><li>
<p>The app does not support spaces in the file name.</p>
</li></ul>
</div>
<h1>Key components of the sample</h1>
<div id="sectionSection3">
<p>The sample app contains the following:</p>
<ul>
<li>
<p><strong>PHPWithOAuth project</strong>, which contains:</p>
<ul>
<li>
<p><strong>AppManifest.xml file</strong>, which registers the provider-hosted application with SharePoint.</p>
</li><li>
<p><strong>SharedDoc module</strong>, which defines a document library that will be deployed to the app web of your SharePoint site.</p>
</li></ul>
</li><li>
<p><strong>PHPWithOAuthSite project</strong>, which contains:</p>
<ul>
<li>
<p><strong>index.php file:</strong> The default page of the app for SharePoint. It gets all items from a SharePoint document library and uses OAuth to authorize the request. It also displays the documents and allows you to interact with them. It uses the SharePoint
 theme from the host web.</p>
</li><li>
<p><strong>uploader.html:</strong> Contains the code that contains the form that uploads files</p>
</li><li>
<p><strong>upload_file.php:</strong> Contains the code that submits the file from the remotely hosted app to the SharePoint document library.</p>
</li><li>
<p><strong>php-jwt folder:</strong> Contains the code that handles OAuth.</p>
</li><li>
<p><strong>web.config file.</strong></p>
</li></ul>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection4">
<p>Follow these steps to configure the sample.</p>
<ol>
<li>
<p>Open the <strong>PHPWithOAuth.sln</strong> file in Visual Studio 2012.</p>
</li><li>
<p>In the <strong><span class="ui">Properties</span></strong> pane, change the <strong>
<span class="ui">Site URL</span></strong> property. It is the absolute URL of your SharePoint test site collection on Office 365: https://<span>&lt;my tenant&gt;</span>.sharepoint.com/sites/dev.</p>
</li></ol>
</div>
<h1>Build, deploy, and run the sample</h1>
<div id="sectionSection5">
<h3>To build and deploy the PHP site</h3>
<div>
<ol>
<li>
<p>Create an empty website on Microsoft Azure and download the publishing profile for that site.</p>
</li><li>
<p>Register an app at the /_layouts/15/appregnew.aspx page of your SharePoint test site collection on Office 365: https://<span>&lt;my tenant&gt;</span>.sharepoint.com/sites/dev/_layouts/15/appregnew.aspx. Be sure to fill in the following details:</p>
<ul>
<li>
<p>Generate a client ID and client secret. You'll need to add the client secret to the
<strong>index.php</strong> file in the solution. You'll also need to provide the client secret to the publishing wizard.</p>
</li><li>
<p>Enter the URL of the website that you created on Microsoft Azure for <strong><span class="ui">App Domain</span></strong>.</p>
</li><li>
<p>Leave the <strong><span class="ui">Redirect URI</span></strong> field empty.</p>
</li></ul>
</li><li>
<p>Open the <strong>index.php</strong> file and update the <strong><span class="keyword">$key</span></strong> variable on line 8 with the client secret value that you created when you registered the app.</p>
</li><li>
<p>Right-click the <strong><span class="ui">PHPWithOAuthSite</span></strong> project in
<strong><span class="ui">Solution Explorer</span></strong>, and then select <strong>
<span class="ui">Publishing</span></strong>.</p>
</li><li>
<p>Follow the instructions to import the publishing profile of your Microsoft Azure site, and publish the PHP project to Microsoft Azure.</p>
</li></ol>
</div>
<h3>To build and deploy the app for SharePoint</h3>
<div>
<ol>
<li>
<p>Right-click the <strong><span class="ui">PHPWithOAuth</span></strong> project in
<strong><span class="ui">Solution Explorer</span></strong>, and then select <strong>
<span class="ui">Publish</span></strong>.</p>
</li><li>
<p>For <strong><span class="ui">Which profile do you want to publish</span></strong>, type
<span>PHPWithOAuth</span> to create a publishing profile. Click <strong><span class="ui">Next</span></strong>.</p>
</li><li>
<p>For <strong><span class="ui">Where is your website hosted</span></strong>, type the location of the Microsoft Azure site where you published the
<strong><span class="ui">PHPWithOAuthSite</span></strong> PHP project.</p>
</li><li>
<p>For client ID, type the client ID value that you created when you registered the app.</p>
</li><li>
<p>For client secret, type the client secret value that you created when you registered the app.</p>
</li><li>
<p>Click <strong><span class="ui">Next</span></strong>, and then click <strong>
<span class="ui">Finish</span></strong>.</p>
<p>The resulting app package file has an .app extension (PHPWithOAuth.app) and is saved in the
<strong><span class="ui">app.publish</span></strong> subfolder of the <strong><span class="ui">bin\Debug</span></strong> folder of the Visual Studio solution.</p>
</li><li>
<p>In your browser, navigate to your Office 365 Developer Site. Click the <strong>
<span class="ui">Admin</span></strong> drop-down list in the upper right corner of the page and select
<strong><span class="ui">SharePoint</span></strong> to go to the SharePoint admin center.</p>
</li><li>
<p>Click <strong><span class="ui">apps</span></strong> in the left panel, and then click
<strong><span class="ui">App Catalog</span></strong> in the center column.</p>
<p>If you don't have an App Catalog site, you'll have to follow the instructions to create a new one.</p>
</li><li>
<p>Upload the PHPWithOAuth.app file that you created when you published the PHPWithOAuth project to the App Catalog by following these steps:</p>
<ol>
<li>
<p>Click <strong><span class="ui">Apps for SharePoint</span></strong> in the left panel.</p>
</li><li>
<p>Click <strong><span class="ui">Files</span></strong>, <strong><span class="ui">Upload Document</span></strong> in the ribbon and browse to the PHPWithOAuth.app file. You don't have to add any metadata or change any of the default values.</p>
</li><li>
<p>Click <strong><span class="ui">OK</span></strong>.</p>
</li></ol>
</li><li>
<p>In your browser, navigate to the site collection in your Office 365 Developer Site where you want to deploy the app.</p>
</li><li>
<p>Click the gear icon in the top right of the page, and select <strong><span class="ui">Add an app</span></strong> from the drop-down menu.</p>
</li><li>
<p>You'll see a new app named <strong><span class="ui">PHPWithOAuth</span></strong>. Click the name, and then click
<strong><span class="ui">Trust It</span></strong>.</p>
<p>Wait for the app to install. This might take several minutes.</p>
</li></ol>
</div>
<h3>To run the sample</h3>
<div>
<ol>
<li>
<p>After the app installs completely, click the app icon to launch the app.</p>
</li><li>
<p>Click the <strong><span class="ui">Add New Document</span></strong> button and upload a document from your local machine.</p>
<p>This document can't have a space in its name. The document will be displayed after the page refreshes.</p>
</li><li>
<p>Perform operations on a file, such as download or check out and edit properties by right-clicking the file name.</p>
</li></ol>
</div>
</div>
<h1>Troubleshooting</h1>
<div id="sectionSection6">
<p>If you see JavaScript errors when you run the app, be sure that your Microsoft Azure site is on your browser's trusted sites list.</p>
<p>&nbsp;</p>
</div>
<h1>Change log</h1>
<div id="sectionSection7">
<p>First release.</p>
</div>
<h1>Related content</h1>
<div id="sectionSection8">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/3038dd73-41ee-436f-8c78-ef8e6869bf7b.aspx" target="_blank">How to: Create a basic provider-hosted app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/fp179933.aspx" target="_blank">Deploying and installing apps for SharePoint: methods and options</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj164022(v=office.15)" target="_blank">How to: Complete basic operations using SharePoint 2013 REST endpoints</a></p>
</li></ul>
</div>
</div>
</div>
