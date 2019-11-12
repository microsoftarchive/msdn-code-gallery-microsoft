# SharePoint 2013: Use the cross-domain library and REST for CRUD operations
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
- 02/27/2013
## Description

<p id="header">A provider-hosted app that uses the cross-domain library and the SharePoint REST API to perform standard CRUD operations on a list in the app web, from JavaScript that runs on pages served up by the remote web.</p>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<h1 class="heading">Description</h1>
<div class="section" id="sectionSection0">
<p><span class="label">Provided by:</span></p>
</div>
<div class="section" id="sectionSection0">
<p><a href="http://mvp.microsoft.com/en-US/findanmvp/Pages/profile.aspx?MVPID=52a3f2aa-710f-4496-9b78-f240eccc74ad" target="_blank">Ted Pattison</a>,
<a href="http://www.criticalpathtraining.com" target="_blank">Critical Path Training</a></p>
<p>The <strong>CrossDomainRestDemo</strong> sample project is a provider-hosted app that uses the cross-domain library and internal authentication to issue client-side calls against the app web from pages served up by the remote web. The example shows how to
 make calls that perform call CRUD (create, read, update, delete) operations against a Customers list that the app created in the app web during app installation.</p>
<p>Key features illustrated in the sample:</p>
<ul>
<li>
<div>Issuing Representational State Transfer (REST) calls using the cross-domain library</div>
</li><li>
<div>Using the REST API to perform all four CRUD operations against a SharePoint list</div>
</li><li>
<div>Using the <span><span class="keyword">jsRender</span></span> library to create an HTML table from a JavaScript Object Notation (JSON) result set.</div>
</li></ul>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<div>This sample requires the following:</div>
<ul>
<li>
<div>A SharePoint 2013 development environment with an Office 365 developer site or local SharePoint farm. The local SharePoint farm must be configured to support apps for SharePoint.</div>
</li><li>
<div>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012</div>
</li><li>
<div>Basic familiarity with the concepts of developing a provider-hosted app. (See</div>
</li></ul>
</div>
<div class="section" id="sectionSection1">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/en-us/library/office/apps/fp142381(v=office.15)" target="_blank">How to: Create a basic provider-hosted app for SharePoint</a>.)</div>
</li></ul>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<p>The sample consists of a Visual Studio 2012 solution with two projects. The first project, named
<strong>CrossDomainRestDemo</strong>, represents the portion of the provider-hosted app that is installed in the SharePoint host environment. The second project, named
<strong>CrossDomainRestDemoWeb</strong>, is an ASP.NET application that is used to implement the app's remote web.</p>
<ul>
<li>
<div>The <strong>CrossDomainRestDemo</strong> project contains an item <strong>Customers</strong>, which is used to create the Customers list in the app web when the app is installed.</div>
</li><li>
<div>The <strong>CrossDomainRestDemoWeb</strong> project has a start page named <strong>
Default.aspx</strong> that is linked to several JavaScript library files, including
<span><span class="keyword">SP.RequestExecutor.js</span></span>, which contains the cross-domain library.</div>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<ol>
<li>
<div>Open the <span class="ui">CrossDomainRestDemo</span> solution.</div>
</li><li>
<div>Assign the <span><span class="keyword">Site URL</span></span> property of the CrossDomainRestDemo project with a URL that points to a test site in the local farm or a developer site in Office 365.</div>
</li></ol>
</div>
<h1 class="heading">Build the sample</h1>
<div class="section" id="sectionSection4">
<p>Build and deploy the <span class="ui">CrossDomainRestDemo</span> project.</p>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection5">
<p>Deploy the <span class="ui">CrossDomainRestDemo</span> project. After the app has been installed, use the user interface on the stat page to view, add, update, and delete customer items.</p>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection6">
<p>If the app fails to install, make sure your environment supports apps. In Visual Studio 2012, create a new SharePoint-hosted app and ensure you can deploy it in a test site on your farm. If you cannot, your environment is not configured to support apps for
 SharePoint.</p>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection7">
<p>First release: January 2013</p>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection8">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179930(v=office.15)" target="_blank">Apps for SharePoint overview</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179923.aspx" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp161179.aspx" target="_blank">How to: Set up an environment for developing apps for SharePoint on Office 365</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/office/apps/fp142381(v=office.15)" target="_blank">How to: Create a basic provider-hosted app for SharePoint</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp142385.aspx" target="_blank">Programming using the SharePoint 2013 REST service</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/jj163782.aspx" target="_blank">Business Connectivity Services</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/sharepoint/ff521587.aspx" target="_blank">SharePoint Foundation REST Interface</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179927(v=office.15)" target="_blank">How to: Access SharePoint 2013 data from remote apps using the cross-domain library</a></div>
</li></ul>
</div>
</div>
</div>
</div>
