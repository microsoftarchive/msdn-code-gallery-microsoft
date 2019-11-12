# SharePoint 2013: Localize the app web, host web, and remote components of an app
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- Data Access
- User Experience
## Updated
- 08/24/2015
## Description

<p><span style="color:#ff0000; font-size:medium">This sample has been moved to <a href="https://github.com/OfficeDev/SharePoint-Add-in-Localization">
SharePoint-Add-in-Localization</a>.</span></p>
<p><span style="color:#ff0000; font-size:medium"><br>
</span></p>
<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Localize the app web, host web, and remote components of an app</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div class="summary">
<p><span class="label">Summary:</span>&nbsp;&nbsp;This sample simulates a bookstore by using a SharePoint 2013 document library in which every document is a book. End users can request and buy new books by using the provided custom actions. The components
 of the app are localized for English and Spanish.</p>
</div>
<div class="introduction">
<p><strong>Last modified: </strong>May 16, 2014</p>
<p><strong>In this article</strong><br>
<a href="#sectionSection0"></a><br>
<a href="#sectionSection1">Prerequisites</a><br>
<a href="#sectionSection2">Key components of the sample</a><br>
<a href="#sectionSection3">Configure the sample</a><br>
<a href="#sectionSection4">Run and test the sample</a><br>
<a href="#sectionSection5">Troubleshooting</a><br>
<a href="#sectionSection6">Change log</a><br>
<a href="#sectionSection7">Related content</a></p>
</div>
<a name="O15Readme_Description"></a><a name="sectionSection0"></a>
<div class="section" id="sectionSection0">
<p>This sample includes a remote app that has webpages to handle the book request and book buying experiences in a simulated bookstore. Custom actions provide the link between the document library and the app pages. The cross-domain library provides data access
 from the remote app to the document library. The chrome control and SharePoint style sheet provide the classes to consistently style the app pages. A custom list provides storage for the orders placed by the end user. End users can use the provided app part
 to display the orders history.</p>
<p>The app can be installed in a SharePoint website provisioned in English (en-US) or Spanish (es-ES). The following app components are localized:</p>
<ul>
<li>
<p>App title</p>
</li><li>
<p>Custom lists</p>
</li><li>
<p>SharePoint page</p>
</li><li>
<p>Custom actions</p>
</li><li>
<p>App part</p>
</li><li>
<p>Web application pages</p>
</li><li>
<p>Chrome control</p>
</li></ul>
<p>Figure 1 shows the bookstore custom actions in English and Spanish.</p>
<div class="caption"><strong>Figure 1. Bookstore custom actions in English and Spanish</strong></div>
<br>
<img id="114765" src="114765-image.png" alt="" width="663" height="515"></div>
<a name="O15Readme_Prereq"></a><a name="sectionSection1"></a>
<h2 class="heading">Prerequisites</h2>
<div class="section" id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Microsoft Visual Studio 2013</p>
</li><li>
<p>Office Developer Tools for Visual Studio 2013 March, 2014, version or later.</p>
</li><li>
<p>A SharePoint 2013 development environment configured for apps</p>
</li></ul>
</div>
<a name="O15Readme_components"></a><a name="sectionSection2"></a>
<h2 class="heading">Key components of the sample</h2>
<div class="section" id="sectionSection2">
<p>The sample contains the following:</p>
<ul>
<li>
<p><strong>BookstoreApp project</strong>, which contains the following components:</p>
<ul>
<li>
<p>Request a book custom action</p>
</li><li>
<p>But this book custom action</p>
</li><li>
<p>My orders app part</p>
</li><li>
<p>Orders custom list</p>
</li><li>
<p>Order status custom list</p>
</li><li>
<p>Home SharePoint page</p>
</li><li>
<p>App web resource files</p>
</li><li>
<p>Host web resource files</p>
</li><li>
<p>JavaScript resource files</p>
</li></ul>
<p>&nbsp;</p>
</li><li>
<p><strong>BookstoreWeb project</strong>, which contains the following components:</p>
<ul>
<li>
<p>BookOrders app page and JavaScript file</p>
</li><li>
<p>BookPurchase app page and JavaScript file</p>
</li><li>
<p>BookRequest app page and JavaScript file</p>
</li><li>
<p>ChromeLoader JavaScript file</p>
</li><li>
<p>Common JavaScript file</p>
</li><li>
<p>StyleLoader JavaScript file</p>
</li><li>
<p>Resource files</p>
</li><li>
<p>JavaScript resource files</p>
</li></ul>
</li></ul>
</div>
<a name="O15Readme_config"></a><a name="sectionSection3"></a>
<h2 class="heading">Configure the sample</h2>
<div class="section" id="sectionSection3">
<p>Update the <strong>SiteUrl</strong> property of the solution with the URL of your SharePoint website. This should be an American English (en-US) site collection.</p>
</div>
<a name="O15Readme_test"></a><a name="sectionSection4"></a>
<h2 class="heading">Run and test the sample</h2>
<div class="section" id="sectionSection4">
<p>You can test the sample by deploying it to an English SharePoint website, to a Spanish SharePoint website and to a French SharePoint website.</p>
<div class="subSection">
<ol>
<li>
<p>Press F5 to build and deploy the app.</p>
</li><li>
<p>In the <span class="ui">Grant permissions to the App</span> page, click <span class="ui">
Trust It</span>.</p>
</li><li>
<p>Follow the instructions on the SharePoint page.</p>
<p>Request a book by using the Ribbon custom action. Buy a book by using the ECB custom action.</p>
<p>Figure 2 shows the bookstore app start page.</p>
<div class="caption"><strong>Figure 2. Bookstore app start page</strong></div>
<br>
<img id="114766" src="114766-image2.png" alt="" width="584" height="369">
</li><li>
<p>Create a Spanish (es-ES) site collection using the Developer Site template.</p>
</li><li>
<p>Change the <span class="ui">SiteUrl</span> property of the solution to the Spanish site collection.</p>
</li><li>
<p>Repeat the test for the Spanish site collection.</p>
</li><li>
<p>Create a French (fr-FR) site collection using the Developer Site template.</p>
</li><li>
<p>Change the <span class="ui">SiteUrl</span> property of the solution to the French site collection.</p>
</li><li>
<p>Repeat the test for the French site collection. Since the sample does not support French, you should get UI elements in the invariant language, which is English in this sample.</p>
</li></ol>
</div>
</div>
<a name="O15Readme_Troubleshoot"></a><a name="sectionSection5"></a>
<h2 class="heading">Troubleshooting</h2>
<div class="section" id="sectionSection5">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly, and how to solve them.</p>
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
<p>The app part does not display any content. The app part displays the following error:
<strong>Navigation to the webpage was canceled</strong>.</p>
</td>
<td>
<p>The browser blocked the content page. The solution might be different depending on the browser you are using:</p>
<ul>
<li>
<p>Internet Explorer 9 and 10 display the following message at the bottom of the page:
<strong>Only secure content is displayed</strong>. Click <span class="ui">Show all content</span> to display the app part content.</p>
</li><li>
<p>Internet Explorer 8 shows a dialog box with the following message: <strong>Do you want to view only the webpage content that was delivered securely?</strong>. Click
<span class="ui">No</span> to display the app part content.</p>
</li></ul>
</td>
</tr>
<tr>
<td>
<p>Error &quot;This content cannot be displayed in a frame.&quot; when the user selects the ECB custom action.</p>
</td>
<td>
<p>See this forum discussion: http://social.msdn.microsoft.com/Forums/sharepoint/en-US/fa6abb31-7251-4744-ab14-634cde38a42d/error-when-viewing-apps-that-utilize-webparts-this-content-cannot-be-displayed-in-a-frame?forum=appsforsharepoint</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<a name="O15Readme_Changelog"></a><a name="sectionSection6"></a>
<h2 class="heading">Change log</h2>
<div class="section" id="sectionSection6">
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
<tr>
<td>
<p>Revised</p>
</td>
<td>
<p>April 2013</p>
</td>
</tr>
<tr>
<td>
<p>2nd Revision</p>
</td>
<td>
<p>May 2014</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<a name="O15Readme_RelatedContent"></a><a name="sectionSection7"></a>
<h2 class="heading">Related content</h2>
<div class="section" id="sectionSection7">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">Setting up a SharePoint 2013 development environment for apps</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/fp179919.aspx" target="_blank">How to: Localize apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/bfdd0a58-2cc5-4805-ac89-4bd2fe6f3b09" target="_blank">Create UX components</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/d60f409a-b292-4c06-8128-88629091b753" target="_blank">UX design for apps</a></p>
</li></ul>
</div>
</div>
</div>
