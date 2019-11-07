# SharePoint 2013: Use list views, callouts, and dialogs in SharePoint-hosted apps
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Visual Studio 2012
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- SharePoint
- Javascript
## Updated
- 05/08/2013
## Description

<div id="header"><span class="label">Summary:</span> This sample shows how to build user interfaces in SharePoint-hosted apps that use the callout and dialog controls. You'll also learn to use the people picker and promoted links list template, how to call
 external services with the SharePoint 2013 web proxy, and how to post items to a user's newsfeed.</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<p><span class="label">Provided by:</span> Yina Arenas, Microsoft Corporation</p>
</div>
<h1 class="heading">Description of the samples</h1>
<div class="section" id="sectionSection0">
<p>When you deploy and launch the app, you'll see a page that includes these key features:</p>
<ul>
<li>
<p>A promoted links list at the top of the page.</p>
</li><li>
<p>A series of news items retrieved from Bing.com through the SharePoint 2013 web proxy. These news items are based on a list of interests that the user can choose.</p>
</li></ul>
<div class="caption">Figure 1. Page with promoted links and a series of news items, shown when you launch the sample</div>
<br>
<img src="81262-image.png" alt="">
<p>The promoted links SharePoint list, <span class="ui">TileList</span>, deploys with the app. The first four items in the list open built-in, list-editing pages inside dialog boxes. For example, the
<span class="ui">Manage Interests</span> item opens the dialog box that lets you edit another SharePoint list (<span class="ui">PinList</span>), also deployed with the app.
<span class="ui">PinList</span> stores keywords that the app uses to find news items.</p>
<div class="caption">Figure 2. Manage Interests promoted links list item opens a dialog box for editing your list of interests</div>
<img src="81265-image.png" alt=""><br>
<p>Each news item contains a <span class="ui">Details</span> link to launch a callout control. This callout contains a summary of the news item, an option to open a link to the news item, and a
<span class="ui">Post to Feed</span> link.</p>
<div class="caption">Figure 3. Callout control that shows news item summary</div>
<br>
<img src="81263-image.png" alt="">
<ul>
<li>
<p>The <span class="ui">Post to Feed</span> link opens a dialog box you can use to post a news item link to a user's newsfeed. The
<span class="ui">Person</span> box uses the People Picker.</p>
<div class="caption">Figure 4. Post to Feed link inside the callout control that opens a dialog box that lets you to post to a user's newsfeed</div>
<br>
<img src="81266-image.png" alt=""> </li></ul>
<p>The sample inherits several SharePoint classes to make the app look like a regular SharePoint page. It also employs any changes the user makes to the host site's look and feel.</p>
<div class="caption">Figure 5. App inherits the green background added by the user</div>
<br>
<img src="81264-image.png" alt=""></div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A SharePoint 2013 Developer Site. This site can be in Office 365 or in an on-premises installation of SharePoint 2013 that is enabled for apps. See
<a href="http://msdn.microsoft.com/en-us/library/sharepoint/fp179923.aspx" target="_blank">
How to: Set up an on-premises development environment for apps for SharePoint</a> to create an on-premises SharePoint Developer Site.</p>
</li><li>
<p>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012 installed on your development computer.</p>
</li></ul>
</div>
<h1 class="heading">Key components</h1>
<div class="section" id="sectionSection2">
<p>The app's <span class="ui">DemoApp</span> project includes the following:</p>
<ul>
<li>
<p>A <span class="ui">Pages</span> folder that contains three .aspx pages:</p>
<ul>
<li>
<p><strong>Default.aspx</strong> is the default page of the SharePoint-hosted app.</p>
</li><li>
<p><strong>About.aspx</strong> contains app information that starts when you click the
<span class="ui">Some other cool stuff</span> and <span class="ui">About</span> promoted links list items.</p>
</li><li>
<p><strong>Post.aspx</strong> displays the <span class="ui">Post to Feed</span> dialog box from a callout control.</p>
</li></ul>
</li><li>
<p><span class="ui">Scripts</span> folder that contains the <span class="ui">
App.js</span> file with custom JavaScript for this app.</p>
</li><li>
<p>List definitions for <span class="ui">PinList</span> and <span class="ui">
TileList</span>. Both lists are deployed with the app to the app web of your SharePoint 2013 site.</p>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<ol>
<li>
<p>Open the <strong>DemoApp.sln</strong> file in Visual Studio 2012.</p>
</li><li>
<p>In the <span class="ui">Properties</span> pane, change the <span class="ui">
Site URL</span> property. It's the absolute URL of your SharePoint site.</p>
</li></ol>
</div>
<h1 class="heading">Build the sample</h1>
<div class="section" id="sectionSection4">
<p>Press F5 to build and deploy the app.</p>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection5">
<ol>
<li>
<p>Choose <span class="ui">Trust It</span> on the consent page to grant permissions to the app.</p>
</li><li>
<p>Click the <span class="ui">New Interest</span> or <span class="ui">Manage Interests</span> link to edit your list of interests.</p>
</li><li>
<p>Click the <span class="ui">Details</span> link for each news item that you want to view in the callout control. Click the
<span class="ui">Post to Feed</span> link to post to someone's newsfeed. You can select only users who have accounts on your site.</p>
</li></ol>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection6">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and ways to solve them.</p>
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
<p>Visual Studio doesn't open the browser after you press F5.</p>
</td>
<td>
<p>Set the app for SharePoint project as the startup project.</p>
</td>
</tr>
<tr>
<td>
<p>HTTP error 405 <strong>Method not allowed</strong>.</p>
</td>
<td>
<p>Locate the <span><span class="keyword">applicationhost.config</span></span> file in
<strong>%userprofile%\Documents\IISExpress\config</strong>.</p>
<p>Locate the handler entry for <strong>StaticFile</strong>, and add the verbs <span>
<span class="keyword">GET</span></span>, <span><span class="keyword">HEAD</span></span>,
<span><span class="keyword">POST</span></span>, <span><span class="keyword">DEBUG</span></span>, and
<span><span class="keyword">TRACE</span></span>.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection7">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<td>
<p>First release</p>
</td>
<td>
<p>April 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection8">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/1b992485-6efe-4ea4-a18c-221689b0b66f.aspx" target="_blank">How to: Create a basic SharePoint-hosted app</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/sharepoint/fp179923.aspx" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
