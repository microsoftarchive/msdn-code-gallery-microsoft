# SharePoint 2013: Use the cross-domain library in a SharePoint-hosted app (REST)
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
- 11/20/2013
## Description

<p>&nbsp;</p>
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Use the cross-domain library in a tenant-scoped app (REST)</span></td>
</tr>
</tbody>
</table>
<p></p>
<div id="mainSection">
<div id="mainBody">
<div class="summary">
<p><span class="label">Summary:</span>&nbsp;Learn how to use the cross-domain library in a tenant-scoped app to read the web title of websites in multiple site collections by using the Representational State Transfer (REST) service.</p>
</div>
<div class="introduction">
<p>This sample SharePoint-hosted app demonstrates how to use the cross-domain library in SharePoint to read the title property of websites in multiple site collections. The cross-domain library lets you query data across site collections in the same tenant
 by using the <span class="keyword">AppContextSite</span> endpoint. The app accepts the URL of two site collections and displays the title property of the root website in a simple page by using the Representational State Transfer (REST) service.</p>
</div>
<h1 class="heading">Description of the sample</h1>
<div class="section" id="sectionSection0">
<p>The code that uses the cross-domain library is in the CrossDomainExec.js file of the CrossDomainApp project. The ReadTitle.aspx page provides the user interface. Figure 1 shows the ReadTitle page after you install and run the app.</p>
<div class="caption">Figure 1. Browser window after running the solution</div>
<br>
<img id="101489" src="101489-crossdomaintenantscope_result.png" alt="" width="555" height="485"></div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Microsoft Visual Studio 2012</p>
</li><li>
<p>Office Developer Tools for Visual Studio 2012</p>
</li><li>
<p>A SharePoint 2013 development environment (app isolation required for on-premises scenarios) with the following:</p>
<ul>
<li>
<p>A developer site</p>
</li><li>
<p>An app catalog site</p>
</li><li>
<p>Two or more site collections (for testing purposes)</p>
</li></ul>
</li></ul>
<p>For help with the prerequisites, review the following documentation:</p>
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/jj163980.aspx#SP15GettingStartedappdev_WhatDoYouNeed" target="_blank">Set up the development environment</a>.</p>
</li><li>
<p><a href="http://office.microsoft.com/en-us/sharepoint-help/use-the-app-catalog-to-make-custom-business-apps-available-for-your-sharepoint-online-environment-HA102772362.aspx#_Toc347303048" target="_blank">Create an App Catalog site</a> (SharePoint Online
 environments).</p>
</li><li>
<p><a href="http://technet.microsoft.com/library/fp161234.aspx#ConfigureAppGallery" target="_blank">Configure the App Catalog site for a web application</a> (on-premises environments).</p>
</li></ul>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<p>The sample contains the following:</p>
<ul>
<li>
<p>CrossDomainApp project</p>
<ul>
<li>
<p>AppManifest.xml file, which contains the tenant permission request required to read across site collections.</p>
</li><li>
<p>ReadTitle.aspx file, which declares the user interface of the sample.</p>
</li><li>
<p>CrossDomainExec.js file, which uses the cross-domain library.</p>
</li></ul>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<div class="subSection">
<ol>
<li>
<p>Update the <strong>SiteUrl</strong> property of the solution with the URL of your SharePoint developer site.</p>
</li><li>
<p>In <span class="ui">Solution Explorer</span>, right click the <span class="ui">
CrossDomainApp</span> project and choose <span class="ui">Deploy</span>. This generates the app package at the following route:</p>
<p><span class="placeholder">&lt;path_to_CrossDomainApp_project&gt;</span>\bin\<span class="placeholder">&lt;solution_configuration&gt;</span>\app.publish\<span class="placeholder">&lt;version&gt;</span></p>
</li><li>
<p>Close the <span class="ui">Do you trust CrossDomainApp?</span> page. You trust the app when you deploy at the tenant scope, in a further step.</p>
</li><li>
<p>Add the app to the app catalog, as explained in <a href="http://office.microsoft.com/en-us/sharepoint-help/use-the-app-catalog-to-make-custom-business-apps-available-for-your-sharepoint-online-environment-HA102772362.aspx#_Toc347303049" target="_blank">
Add custom apps to the App Catalog site</a>. Use the app package generated in step 2.</p>
</li><li>
<p>Deploy the app to one of the websites in the test site collections, as explained in
<a href="http://office.microsoft.com/en-us/sharepoint-help/use-the-app-catalog-to-make-custom-business-apps-available-for-your-sharepoint-online-environment-HA102772362.aspx#_Toc347303052" target="_blank">
Deploy a custom app</a>. You trust the app in this step.</p>
</li></ol>
</div>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection4">
<div class="subSection">
<ol>
<li>
<p>Go to the website where you deployed the app.</p>
</li><li>
<p>Provide the URL of the test site collections in the text boxes.</p>
</li><li>
<p>Click <span class="ui">Go</span>.</p>
</li></ol>
</div>
<p>The page displays the root websites title of the site collections.</p>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection5">
<p>For troubleshooting steps, visit the <a href="http://msdn.microsoft.com/library/bc37ff5c-1285-40af-98ae-01286696242d# SP15Accessdatafromremoteapp_Troubleshoot" target="_blank">
Troubleshooting the solution</a> table in the cross-domain library documentation.</p>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection6">
<ul>
<li>
<p>First version: October 2013</p>
</li></ul>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection7">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">Setting up a SharePoint 2013 development environment for apps</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/1534a5f4-1d83-45b4-9714-3a1995677d85" target="_blank">Working with data</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/bc37ff5c-1285-40af-98ae-01286696242d" target="_blank">How to: Read the host web's title from a remote app using the cross-domain library</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/3d24f916-60b2-4ea9-b182-82e33cad06e8" target="_blank">Work with the cross-domain library across different Internet Explorer security zones in apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/26f2999e-db7f-4fe7-a00f-05b009b1927d" target="_blank">What you can do in an app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/bde5647a-fff1-4b51-b67b-2139de79ce4a" target="_blank">OAuth in SharePoint 2013</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/d4b5c277-ed50-420c-8a9b-860342284b72" target="_blank">Programming using the SharePoint 2013 REST service</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/0942fdce-3227-496a-8873-399fc1dbb72c" target="_blank">Design considerations for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88" target="_blank">Critical aspects of the app for SharePoint architecture and development landscape</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/3034f03c-2d5a-46de-9cb8-2c101ff194fa" target="_blank">Data storage options in apps for SharePoint</a></p>
</li></ul>
</div>
</div>
</div>
<p></p>
