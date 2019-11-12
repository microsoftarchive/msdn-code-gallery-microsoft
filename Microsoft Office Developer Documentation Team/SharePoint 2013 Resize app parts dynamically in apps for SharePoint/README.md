# SharePoint 2013: Resize app parts dynamically in apps for SharePoint
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- apps for SharePoint
## Topics
- apps for SharePoint
## Updated
- 06/17/2013
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Resize app parts dynamically in apps for SharePoint</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<p>&nbsp;</p>
<div>
<p><span>Summary:</span> Learn how to change the size (width) of an app part dynamically from the app part content area in SharePoint 2013.</p>
</div>
<div>
<p>This sample autohosted app demonstrates how to change the size of an app part in an app for SharePoint. Although you can manually set a different size on your app part, you can set a particular size for the app part in the app part definition. You can also
 request that your app part be resized dynamically through <strong><span class="ui">postmessages</span></strong>, as demonstrated in this sample.</p>
</div>
<div id="sectionSection0">
<p>The markup that declares the app part is in the ResizeMyAppPart\Elements.xml file in the ResizeMyAppPart project. The rendering logic is in the Resize.js file in the ResizeMyAppPartWeb project. Figure 1 shows the Web Parts page with a
<strong><span class="ui">Resize AppPart</span></strong> app part with default size (width).</p>
<strong>
<div class="caption">Figure 1. Page with Resize app part</div>
</strong><br>
<img src="85191-image.png" alt="">
<p>Similarly, Figure 2 shows the Web Parts page with a <strong><span class="ui">Resize AppPart</span></strong> app part with the resized size (width) selected from the drop-down menu.</p>
<strong>
<div class="caption">Figure 2. Page with Resize app part with changed size</div>
</strong><br>
<img src="85193-image.png" alt=""></div>
<h1>Prerequisites</h1>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Visual Studio 2012</p>
</li><li>
<p>Office Developer Tools for Visual Studio 2012</p>
</li><li>
<p>A SharePoint 2013 development environment (app isolation required for on-premises scenarios)</p>
</li></ul>
</div>
<h1>Key components of the sample</h1>
<div id="sectionSection2">
<p>The sample contains the following:</p>
<ul>
<li>
<p>The <strong><span class="ui">AppPartsApp</span></strong> project, which contains the AppManifest.xml file</p>
</li><li>
<p>The <strong><span class="ui">AppPartsWeb</span></strong> project, which contains the following files:</p>
<ul>
<li>
<p>The <strong><span class="ui">AppPartContent.aspx</span></strong> file, which contains the controls to be displayed in the content area of the app part</p>
</li><li>
<p>The <strong><span class="ui">Resize.js</span></strong> file in the <strong><span class="ui">ResizeMyAppPartWeb\Scripts</span></strong> folder, which contains the logic for resizing the app part</p>
</li><li>
<p>The <strong><span class="ui">Web.config</span></strong> file</p>
</li></ul>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection3">
<ul>
<li>
<p>Update the <span><strong><span class="keyword">SiteUrl</span></strong></span> property of the solution with the URL of the home page of your SharePoint website.</p>
</li></ul>
</div>
<h1>Run and test the sample</h1>
<div id="sectionSection4">
<p>&nbsp;</p>
<ol>
<li>
<p>Press F5 to build and deploy the app.</p>
</li><li>
<p>Choose <strong><span class="ui">Trust It</span></strong> on the consent page to grant permissions to the app.</p>
<p>You should see a SharePoint page with additional instructions.</p>
</li><li>
<p>Go to any wiki page or Web Parts page in the host web.</p>
</li><li>
<p>Edit the page, and add the Resize app part from the Web Part gallery. Figure 3 shows the Resize app part in the Web Part gallery.</p>
<strong>
<div class="caption">Figure 3. Resize AppPart in the Web Part gallery</div>
</strong><br>
<img src="85192-image.png" alt=""> </li></ol>
</div>
<h1>Troubleshooting</h1>
<div id="sectionSection5">
<p>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how to solve them.</p>
<strong>
<div class="caption"></div>
</strong>
<div>
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
<p>Visual Studio does not open the browser after you press the F5 key.</p>
</td>
<td>
<p>Set the app for SharePoint project as the startup project.</p>
</td>
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
<strong>Only secure content is displayed</strong>. Choose <strong><span class="ui">Show all content</span></strong> to display the app part content.</p>
</li><li>
<p>Internet Explorer 8 shows a dialog box with the following message: <strong>Do you want to view only the webpage content that was delivered securely?</strong>. Choose
<strong><span class="ui">No</span></strong> to display the app part content.</p>
</li></ul>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Change log</h1>
<div id="sectionSection6"><strong>
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
<p>May 10, 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Related content</h1>
<div id="sectionSection7">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/b0878c12-27c9-4eea-ae3b-7e79e5a8838d" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/bfdd0a58-2cc5-4805-ac89-4bd2fe6f3b09" target="_blank">Create UX components</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/d60f409a-b292-4c06-8128-88629091b753" target="_blank">UX design for apps</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/a2664289-6c56-4cb1-987a-22367fad55eb" target="_blank">How to: Create app parts to deploy with apps for SharePoint</a>
<a href="http://msdn.microsoft.com/library/a2664289-6c56-4cb1-987a-22367fad55eb.aspx" target="_blank">
</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/26f2999e-db7f-4fe7-a00f-05b009b1927d" target="_blank">What you can do in an app for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/0942fdce-3227-496a-8873-399fc1dbb72c" target="_blank">Three ways to think about design options for apps for SharePoint</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/ae96572b-8f06-4fd3-854f-fc312f7f2d88" target="_blank">Important aspects of the app for SharePoint architecture and development landscape</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
