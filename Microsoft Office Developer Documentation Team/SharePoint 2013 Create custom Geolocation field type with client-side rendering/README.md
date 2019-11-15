# SharePoint 2013: Create custom Geolocation field type with client-side rendering
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- SharePoint Server 2013
- SharePoint Foundation 2013
## Topics
- geographic information systems
## Updated
- 01/11/2013
## Description

<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<div>
<div>
<div id="mainBody">
<div class="introduction">
<h1 class="heading">Description of the sample</h1>
<div class="section" id="sectionSection0">
<div>This sample demonstrates how to customize the rendering process for a Geolocation field type in SharePoint 2013. You can provide custom logic to control the rendering process of the field when it is displayed in the View, Edit, New, and Display forms.</div>
<div>The JavaScript code that controls the rendering process is in the CSRFieldType.js file.</div>
<div>Figure 1 shows the new custom Geolocation field type as it appears when you create a column.</div>
<div></div>
<div><strong>Figure 1. New custom field type column</strong></div>
<br>
<img id="68576" src="http://i1.code.msdn.s-msft.com/sharepoint-2013-create-eb30a620/image/file/68576/1/fig1.png" alt="" width="565" height="328"></div>
</div>
</div>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<div>This sample requires the following:</div>
<ul>
<li>
<div>Microsoft Visual Studio 2012</div>
</li><li>
<div>SharePoint development tools in Visual Studio 2012</div>
</li><li>
<div>A SharePoint 2013 development environment</div>
</li></ul>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<div>The sample contains the following:</div>
<ul>
<li>
<div>CustomGeolocationControl project, which contains these files:</div>
<ul>
<li>
<div>fldtypes_CustomGeolocationControl.xml, which contains the custom field type definition</div>
</li><li>
<div>CustomGeolocationField.cs, which contains the class declaration for the custom field type</div>
</li><li>
<div>CustomGeolocationControl.js, which contains the rendering logic</div>
</li></ul>
</li></ul>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<ul>
<li>
<div>Update the <strong>SiteUrl</strong> property of the solution with the URL of your SharePoint website.</div>
</li></ul>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection4">
<div>&nbsp;</div>
<ol>
<li>
<div>Press the F5 key to build and deploy the solution.</div>
</li><li>
<div>Create a custom list and add a new Custom Geolocation field column.</div>
</li></ol>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection5">
<div>The following table lists common configuration and environment errors that prevent the sample from running or deploying properly and how to solve them.</div>
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<div>Problem</div>
</th>
<th>
<div>Solution</div>
</th>
</tr>
<tr>
<td>
<div>You see the error message &quot;Field type FavoriteColorField is not installed properly. Go to the list settings page to delete this field.&quot;</div>
</td>
<td>
<div>Execute the following command from an elevated command prompt: <strong>iisreset /noforce</strong>.</div>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Caution</strong></th>
</tr>
<tr>
<td>
<div>If you are deploying the solution to a production environment, wait for an appropriate time to reset the web server by using
<strong>iisreset /noforce</strong>.</div>
</td>
</tr>
</tbody>
</table>
</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection6">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<div>Version</div>
</th>
<th>
<div>Date</div>
</th>
</tr>
<tr>
<td>
<div>First version</div>
</td>
<td>
<div>October 2, 2012</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Related content</h1>
</div>
<div class="section" id="sectionSection7">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/library/7360633a-a7cf-4194-8bbd-8dd7c323e80b.aspx" target="_blank">How to: Extend the Geolocation field type using client-side rendering</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/8d5cabb2-70d0-46a0-bfe0-9e21f8d67d86" target="_blank">How to: Customize a list view in an apps for SharePoint using client-side rendering</a></div>
</li><li>
<div><a href="http://code.msdn.microsoft.com/SharePoint-2013-Create-a-d9a91551" target="_blank">SharePoint 2013: Create a GeoLocation field that renders maps using Nokia Maps</a></div>
</li></ul>
</div>
</div>
</div>
</div>
