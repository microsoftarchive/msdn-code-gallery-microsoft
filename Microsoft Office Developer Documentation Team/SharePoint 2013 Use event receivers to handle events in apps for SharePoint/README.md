# SharePoint 2013: Use event receivers to handle events in apps for SharePoint
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- C#
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- data and storage
## Updated
- 03/19/2013
## Description

<p><span style="font-size:small">This sample demonstrates the basics of how to use a remote event receiver to handle events that occur to list items in an app for SharePoint. The sample also shows how to use an app event receiver to handle events that occur
 to the app itself. This sample extends the sample <a href="http://code.msdn.microsoft.com/SharePoint-2013-Perform-eba8df54">
SharePoint 2013: Perform basic data access operations by using CSOM in apps</a> by adding a remote event receiver and an app event receiver to it. When you install the app, the app event handler handles the app installation and creates a log file in a folder
 in your \My Documents folder. When you add an item to a list, an entry is added to a log list in the app.</span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following components:</span></p>
<ul>
<li><span style="font-size:small">A development environment that includes SharePoint 2013 and that's configured for app isolation and OAuth</span>
</li><li><span style="font-size:small">Visual Studio 2012 and SharePoint development tools in Visual Studio 2012</span>
</li><li><span style="font-size:small">Basic familiarity with C# and the client object model for SharePoint</span>
</li><li><span style="font-size:small">The sample <a href="http://code.msdn.microsoft.com/SharePoint-2013-Perform-eba8df54">
SharePoint 2013: Perform basic data access operations by using CSOM in apps</a></span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The basic sample app for event handling contains the following components:</span></p>
<ul>
<li><span style="font-size:small">The BasicDataOperations project, which contains the AppManifest.xml file</span>
</li><li><span style="font-size:small">The BasicDataOperationsWeb project, which contains the following files:</span>
<ul>
<li><span style="font-size:small">Home.aspx file, which contains the HTML and ASP.NET controls for the app&rsquo;s user interface</span>
</li><li><span style="font-size:small">Home.aspx.cs file, which contains the C# code that uses the client object model for SharePoint to read and write data</span>
</li><li><span style="font-size:small">RemoteEventReceiver1.svc.cs, which contains the C# code that handles the events for list items in the app</span>
</li><li><span style="font-size:small">AppEventReceiver.svc.cs, which contains the C# code that handles the events for the app for SharePoint</span>
</li><li><span style="font-size:small">Web.config file</span> </li></ul>
</li></ul>
<h1>Configure the sample</h1>
<p><span style="font-size:small">To configure the sample app for basic data operations, update the
<strong>SiteUrl</strong> property of the solution with the local URL of the SharePoint 2013 site. You must develop remote event receivers and app event receivers on the local system. After you deploy them, they can be used on remote systems.</span></p>
<h1>Build the sample</h1>
<p><span style="font-size:small">Choose the F5 key to build and deploy the app.</span></p>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Choose the F5 key to build and deploy the app.</span>
</li><li><span style="font-size:small">On the consent page, choose the Trust It link&nbsp; to grant permissions to the app.</span>
</li><li><span style="font-size:small">Use the app&rsquo;s interface to create a list, and then add an item to the list on the parent SharePoint 2013 site.</span>
</li><li><span style="font-size:small">View the EventLog list to verify that the remote event receiver handled the list item addition by adding an entry to the list.</span>
</li><li><span style="font-size:small">In your \My Documents folder, view the contents of the new folder that's named SPAppEventLog. This folder contains files that the app event receiver adds, and each file represents an app event that occurred.</span>
</li></ol>
<h1>Troubleshooting</h1>
<p><span style="font-size:small">The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how to solve them.</span></p>
<table border="0" cellspacing="5" cellpadding="5" frame="void" align="left" style="width:601px; height:212px">
<tbody>
<tr style="background-color:#a9a9a9">
<th align="left" scope="col"><strong><span style="font-size:small">Problem </span>
</strong></th>
<th align="left" scope="col"><strong><span style="font-size:small">Solution</span></strong></th>
</tr>
<tr valign="top">
<td><span style="font-size:small">Visual Studio does not open the browser after you press the F5 key.</span></td>
<td><span style="font-size:small">Set the app for SharePoint project as the startup project.</span></td>
</tr>
<tr valign="top">
<td><span style="font-size:small">HTTP error 405 <strong>Method not allowed</strong>.</span></td>
<td><span style="font-size:small">Locate the <strong>applicationhost.config </strong>
file in <em>%userprofile%</em>\Documents\IISExpress\config.</span>
<p><span style="font-size:small">Locate the handler entry for <strong>StaticFile</strong>, and add the verbs
<strong>GET</strong>,<strong>HEAD</strong>, <strong>POST</strong>, <strong>DEBUG</strong>, and
<strong>TRACE</strong>.</span></p>
</td>
</tr>
</tbody>
</table>
<h1><br>
<br>
<span style="font-size:small">&nbsp;</span><br>
<br>
<br>
</h1>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<h1>Change log</h1>
<ul>
<li><span style="font-size:small">First version: July 16, 2012</span> </li><li><span style="font-size:small">Second version: Updated code to fix breaking change.&nbsp;November 30, 2012&nbsp;</span>
</li></ul>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/en-us/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx" href="http://msdn.microsoft.com/en-us/library/f86e2695-4d7a-4fc5-bc23-689de96c4b06.aspx">SharePoint 2013 development overview</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/71ddde4b-fac4-4d8c-aa2e-524f9c2c4c99" href="http://msdn.microsoft.com/library/71ddde4b-fac4-4d8c-aa2e-524f9c2c4c99">Develop apps for SharePoint</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/library/f36645da-77c5-47f1-a2ca-13d4b62b320d.aspx" href="http://msdn.microsoft.com/library/f36645da-77c5-47f1-a2ca-13d4b62b320d.aspx">Choose the right API set in SharePoint 2013</a></span>
</li></ul>
