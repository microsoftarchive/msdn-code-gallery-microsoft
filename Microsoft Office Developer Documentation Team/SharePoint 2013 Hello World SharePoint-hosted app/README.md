# SharePoint 2013: Hello World SharePoint-hosted app
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
- SharePoint Add-ins
## Topics
- Authentication
- data and storage
## Updated
- 08/14/2015
## Description

<p><span style="color:#ff0000; font-size:medium">This sample has been replaced with the series of samples found at:&nbsp;<a href="https://github.com/OfficeDev/SharePoint_SP-hosted_Add-Ins_Tutorials/">OfficeDev/SharePoint_SP-hosted_Add-Ins_Tutorials</a>.</span></p>
<p>&nbsp;</p>
<p><span>The sample demonstrates how to create, package, and deploy a custom list to the appweb and a custom action to the parent web.</span></p>
<p><span>The JavaScript code that reads data from the parent web and the app web is located in the Home.aspx file in the BasicSharePoint-hosted directory of the BasicSharePoint-hosted project. The following screen shot shows how the Home.aspx page of the app
 appears after you install and launch the app.</span></p>
<p><strong><span>Figure 1. Home.aspx page in the app, displaying the user, the Bing home page and a button for getting information about the appweb</span></strong></p>
<p><img id="133148" src="https://i1.code.msdn.s-msft.com/sharepoint/sharepoint-2013-hello-b3ca20f3/image/file/133148/1/fig1.gif" alt="" width="645" height="208"></p>
<h1>Prerequisites</h1>
<p><span>This sample requires the following:</span></p>
<ul>
<li><span>A SharePoint development environment that is configured for app isolation.</span>
</li><li><span>Visual Studio and Office Tools for Visual Studio installed on your developer computer.</span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span>The sample app contains the following:</span></p>
<ul>
<li><span>BasicSharePoint-hosted project</span> </li><li><span>AppManifest.xml file, located in the BasicSharePoint-hosted directory</span>
</li><li><span>Home.aspx file, located in the BasicSharePoint-hosted\Pages directory, which contains the HTML and ASP.NET controls for the app&rsquo;s user interface</span>
</li><li><span>App.js file, located in the BasicSharePoint-hosted\Scripts directory, which contains the JavaScript code that populates the controls in the Home.aspx file</span>
</li><li><span>Elements.xml file, located in the BasicSharePoint-hosted\HelloWorldCustomAction directory, which configures the custom action</span>
</li></ul>
<h1>Configure the sample</h1>
<p><span>To configure the hello world SharePoint-hosted sample app, update the <strong>
SiteUrl</strong> property of the solution with the URL of the home page of your SharePoint test site.</span></p>
<h1>Build the sample</h1>
<p><span>Press F5 to build and deploy the app.</span></p>
<h1>Run and test the sample</h1>
<ol>
<li><span>Choose <strong>Trust It</strong> on the consent page to grant permissions to the app.</span>
</li><li><span>Choose the <strong>Get Count of Lists</strong> button to see the number of lists in the appweb.</span>
</li></ol>
<h1>Troubleshooting</h1>
<p><span>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how to solve them.</span></p>
<table border="0" cellspacing="5" cellpadding="5" frame="void" align="left">
<tbody>
<tr>
<th align="left" scope="col"><strong><span>Problem </span></strong></th>
<th align="left" scope="col"><strong><span>Solution</span></strong></th>
</tr>
<tr valign="top">
<td><span>Visual Studio does not open the browser after you press the F5 key.</span></td>
<td><span>Set the app for SharePoint project as the startup project.</span></td>
</tr>
<tr valign="top">
<td><span>HTTP error 405 <strong>Method not allowed</strong>.</span></td>
<td><span>Locate the applicationhost.config file in <em>%userprofile%</em>\Documents\IISExpress\config.</span>
<p><span>Locate the handler entry for <strong>StaticFile</strong>, and add the verbs
<strong>GET</strong>, <strong>HEAD</strong>, <strong>POST</strong>, <strong>DEBUG</strong>, and
<strong>TRACE</strong>.</span></p>
</td>
</tr>
</tbody>
</table>
<h1><br>
<br>
<br>
</h1>
<h1>Change log</h1>
<p><span>First version: July 16, 2012</span></p>
<p><span>Second version: March 6, 2013</span></p>
<p><span>Third version: January 16, 2014</span></p>
<h1>Related content</h1>
<ul>
<li><span><a title="http://msdn.microsoft.com/library/1b992485-6efe-4ea4-a18c-221689b0b66f.aspx" href="http://msdn.microsoft.com/library/1b992485-6efe-4ea4-a18c-221689b0b66f.aspx">How to: Create a basic SharePoint-hosted app</a></span>
</li><li><span><a title="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" href="http://msdn.microsoft.com/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx">SharePoint 2013 development overview</a></span>
</li><li><span><a title="http://msdn.microsoft.com/library/d07e0a13-1e74-4128-857a-513dedbfef33.aspx" href="http://msdn.microsoft.com/library/d07e0a13-1e74-4128-857a-513dedbfef33.aspx">Get started developing SharePoint apps</a></span>
</li><li><span><a title="http://msdn.microsoft.com/library/3038dd73-41ee-436f-8c78-ef8e6869bf7b.aspx" href="http://msdn.microsoft.com/library/3038dd73-41ee-436f-8c78-ef8e6869bf7b.aspx">How to: Create a basic provider-hosted app for SharePoint</a></span>
</li><li><span><a title="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx" href="http://msdn.microsoft.com/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88.aspx">Important aspects of the app for SharePoint architecture and development landscape</a></span>
</li></ul>
