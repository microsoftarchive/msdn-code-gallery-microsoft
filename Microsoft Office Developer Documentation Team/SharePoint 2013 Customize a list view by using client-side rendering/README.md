# SharePoint 2013: Customize a list view by using client-side rendering
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Javascript
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- User Experience
## Updated
- 02/06/2013
## Description

<p><span style="font-size:small">This sample demonstrates how to customize the rendering process for a list view in SharePoint 2013 . You can provide custom logic to control the rendering process of different template sets, such as the header, footer, or individual
 items in the list. You can use client-side rendering in SharePoint-hosted apps to manipulate the rendering process of list views.</span></p>
<p><span style="font-size:small">The JavaScript code that controls the rendering process is in the CSRListView.js file.</span></p>
<p><span style="font-size:small">Figure 1 shows the announcements list rendered in the custom view.</span></p>
<p><strong><span style="font-size:small">Figure 1. Custom client-side rendered view of an announcements list</span></strong></p>
<p><span style="font-size:small"><img id="60488" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-customize-61761017/image/file/60488/1/fig2.png" alt="" width="535" height="342"></span></p>
<h1>Prerequisites</h1>
<p><span style="font-size:small">This sample requires the following:</span></p>
<ul>
<li><span style="font-size:small">Visual Studio 2012</span> </li><li><span style="font-size:small">SharePoint development tools in Visual Studio 2012</span>
</li><li><span style="font-size:small">SharePoint 2013 development environment (app isolation required for on-premises scenarios)</span>
</li></ul>
<h1>Key components of the sample</h1>
<p><span style="font-size:small">The sample contains the following:</span></p>
<ul>
<li><span style="font-size:small">CustomListApp project, which contains the following files:</span>
<ul>
<li><span style="font-size:small">Schema.xml, which declares the list and the custom view</span>
</li><li><span style="font-size:small">CSRListView.js, which contains the rendering logic</span>
</li></ul>
</li></ul>
<h1>Configure the sample</h1>
<ul>
<li><span style="font-size:small">Update the <strong>SiteUrl</strong> property of the solution with the URL of your SharePoint website.</span>
</li></ul>
<h1>Run and test the sample</h1>
<ol>
<li><span style="font-size:small">Press F5 to build and deploy the solution.</span>
</li><li><span style="font-size:small">Provide a title for the list in the dialog box.</span>
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
<td><span style="font-size:small">Unhandled exception <strong>SP is undefined</strong>.</span></td>
<td><span style="font-size:small">Make sure you can access the SP.RequestExecutor.js file from a browser window.</span></td>
</tr>
<tr valign="top">
<td><span style="font-size:small">HTTP error 405 <strong>Method not allowed</strong>.</span></td>
<td><span style="font-size:small">Locate the applicationhost.config file in <em>%userprofile%</em>\Documents\IISExpress\config.</span>
<p><span style="font-size:small">Locate the handler entry for <strong>StaticFile</strong>, and add the verbs
<strong>GET</strong>, <strong>HEAD</strong>, <strong>POST</strong>, <strong>DEBUG</strong>, and
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
<p>&nbsp;</p>
<h1>Change log</h1>
<p><span style="font-size:small">First version: July 16, 2012</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small"><a title="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" href="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d">Setting up a SharePoint 2013 development environment
 for apps</a></span> </li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/bfdd0a58-2cc5-4805-ac89-4bd2fe6f3b09">Create UX components</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/d60f409a-b292-4c06-8128-88629091b753">UX design for apps</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/8d5cabb2-70d0-46a0-bfe0-9e21f8d67d86">How to: Customize a list view in an app for SharePoint using client-side rendering</a></span>
</li><li><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/18e32537-d7ed-4fe7-90cf-b6cfab3f85a3">How to: Customize a field type using client-side rendering</a></span>
</li><li><span style="font-size:small"><a title="http://msdn.microsoft.com/en-us/library/26f2999e-db7f-4fe7-a00f-05b009b1927d" href="http://msdn.microsoft.com/en-us/library/26f2999e-db7f-4fe7-a00f-05b009b1927d">What you can do in an app for SharePoint</a></span>
</li><li><span style="font-size:small"><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/0942fdce-3227-496a-8873-399fc1dbb72c">Design considerations for apps for SharePoint</a></span></span>
</li><li><span style="font-size:small"><span style="font-size:small"><span style="font-size:small"><a href="http://msdn.microsoft.com/en-us/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88">Critical aspects of the app for SharePoint architecture and development landscape</a></span></span></span>
</li></ul>
