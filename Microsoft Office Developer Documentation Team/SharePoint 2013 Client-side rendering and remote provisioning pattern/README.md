# SharePoint 2013: Client-side rendering and remote provisioning pattern
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- SharePoint Server 2013
- apps for SharePoint
## Topics
- Client Side Rendering
- JSLink
## Updated
- 09/10/2014
## Description

<div id="header">
<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><strong><span id="nsrTitle" style="font-size:small">SharePoint 2013: Client-side rendering and remote provisioning of SharePoint artifacts and JSLink files</span></strong></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div class="summary">
<p><strong>Summary: </strong>This sample demonstrates how to use a provider-hosted app to remotely provision SharePoint artifacts and JSLink files that use client-side rendering to customize the look and behavior of SharePoint lists, list fields, etc.</p>
</div>
<div class="introduction">
<p><strong>Last modified: </strong>September 08, 2014</p>
<p><em><strong>Applies to: </strong>apps for SharePoint&nbsp;| SharePoint Server 2013</em></p>
<p><strong>In this article</strong><br>
<a href="file://ipoawsfs201/DropZone/Rawhide/FileDropOff/Readmes_SP15_Beta2/Jimcrowley/Branding.ClientSideRendering/Branding.ClientSideRendering.htm#O15Readme_Prereq">Prerequisites</a><br>
<a href="file://ipoawsfs201/DropZone/Rawhide/FileDropOff/Readmes_SP15_Beta2/Jimcrowley/Branding.ClientSideRendering/Branding.ClientSideRendering.htm#O15Readme_components">Key components</a><br>
<a href="file://ipoawsfs201/DropZone/Rawhide/FileDropOff/Readmes_SP15_Beta2/Jimcrowley/Branding.ClientSideRendering/Branding.ClientSideRendering.htm#O15Readme_config">Configure the sample</a><br>
<a href="file://ipoawsfs201/DropZone/Rawhide/FileDropOff/Readmes_SP15_Beta2/Jimcrowley/Branding.ClientSideRendering/Branding.ClientSideRendering.htm#O15Readme_test">Run and test the sample</a><br>
<a href="file://ipoawsfs201/DropZone/Rawhide/FileDropOff/Readmes_SP15_Beta2/Jimcrowley/Branding.ClientSideRendering/Branding.ClientSideRendering.htm#O15Readme_Changelog">Change log</a><br>
<a href="file://ipoawsfs201/DropZone/Rawhide/FileDropOff/Readmes_SP15_Beta2/Jimcrowley/Branding.ClientSideRendering/Branding.ClientSideRendering.htm#O15Readme_RelatedContent">Related content</a></p>
<p>This sample combines the JSLink samples from <a href="http://code.msdn.microsoft.com/office/Client-side-rendering-JS-2ed3538a" target="_blank">
Muawiyah Shannak's Client-side rendering (JSLink) code samples</a> into a single provider-hosted app for SharePoint that provisions the JSLink files. This sample demonstrates how the remote provisioning pattern may be used to deploy client-side rendering components
 and associate them with views and forms in a SharePoint list.</p>
<p>Client-side rendering provides you with a mechanism that allows you to render customized output for a set of controls that are hosted in a SharePoint page (list views, add and edit forms, etc.). This mechanism enables you to use standard web technologies,
 such as HTML and JavaScript, to define the rendering logic of custom and predefined field types.</p>
<p>You can see the GitHub version of this sample in the <a href="https://github.com/OfficeDev/PnP/tree/master/Samples/Branding.ClientSideRendering" target="_blank">
Office365 Development Patterns and Practices</a> repository.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>The <a href="https://github.com/SPCSR/DisplayTemplates" target="_blank">DisplayTemplates repository on GitHub</a> contains a number of JSLink display template samples that have been created by the SharePoint community.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<a name="O15Readme_Prereq">
<h2 class="heading">Prerequisites</h2>
<div class="section" id="sectionSection0">
<p>This sample requires the following:</p>
<ul>
<li>
<p>An Office 365 Developer Site</p>
</li><li>
<p>Visual Studio 2013 and Office Developer Tools for Visual Studio 2013 installed on your development computer</p>
</li></ul>
</div>
</a><a name="O15Readme_components"></a>
<h2 class="heading"><a name="O15Readme_components">Key components</a><a name="O15Readme_components" style="font-size:10px">
<div class="section" id="sectionSection1" style="display:inline!important">
<p style="display:inline!important">&nbsp;</p>
</div>
</a></h2>
<div class="section" id="sectionSection1">
<ul>
<li>
<p><strong>Branding.ClientSideRendering</strong> project, which contains:</p>
<p><strong>AppManifest.xml</strong>: The configuration file that defines the app as a provider-hosted app for SharePoint.</p>
</li><li>
<p><strong>Branding.ClientSideRenderingWeb</strong> project, which contains:</p>
<ul>
<li>
<p><strong>Pages\Default.aspx</strong>. The starter page that presents links to all of the JSLink samples.</p>
</li><li>
<p><strong>Pages\Default.aspx.cs</strong>. The file that contains the code that uploads the JSLink samples and remotely provisions all of the SharePoint artifacts (lists, list items, list views, etc.) that are affected by the JSLink files.</p>
</li><li>
<p><strong>Web.config</strong>. Stores the client id and client secret.</p>
</li></ul>
</li></ul>
</div>
<a name="O15Readme_config">
<h2 class="heading">Configure the sample</h2>
<div class="section" id="sectionSection2">
<p>Follow these steps to configure the sample.</p>
<ol>
<li>
<p>Open the <strong>Branding.ClientSideRendering.sln</strong> file in Visual Studio 2013.</p>
</li><li>
<p>In the <span class="ui">Properties</span> pane, change the <span class="ui">
Site URL</span> property. It is the absolute URL of your SharePoint test site collection on Office 365:
<span class="code">https://</span><span class="placeholder">&lt;my tenant&gt;</span><span class="code">.sharepoint.com/sites/dev</span>.</p>
<p>You may be prompted to enter your user name and password to log in to your developer site.</p>
</li></ol>
</div>
</a><a name="O15Readme_test">
<h2 class="heading">Run and test the sample</h2>
<div class="section" id="sectionSection3">
<ol>
<li>
<p>In Visual Studio 2013, press F5. The trust dialog will appear in your web browser. Figure 1 shows how this dialog will appear.</p>
<div class="caption">Figure 1: Client-side rendering trust dialog</div>
<br>
<img id="125126" src="125126-sp15_clientrenderingperms.png" alt="" width="561" height="242">
</li><li>
<p>In the trust dialog, click the <span class="ui">Trust It</span> button. The start page of the provider-hosted app will appear in your web browser.</p>
</li><li>
<p>On the start page of the app, click the Provision Samples button. This remotely provisions all of the JSLink files and SharePoint artifacts that the sample requires.</p>
<div class="caption">Figure 2: Client-side rendering start page and provision samples button</div>
<br>
<img id="125127" src="125127-sp15_clientrenderingstart.png" alt="" width="574" height="434">
</li><li>
<p>After the app has finished provisioning the samples, use the links to view each JSLink sample.</p>
</li></ol>
</div>
</a><a name="O15Readme_Changelog">
<h2 class="heading">Change log</h2>
<div class="section" id="sectionSection4">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p style="text-align:left"><span style="font-size:x-small">Version</span></p>
</th>
<th>
<p style="text-align:left"><span style="font-size:x-small">Date</span></p>
</th>
</tr>
<tr>
<td>
<p>First version</p>
</td>
<td>
<p>September 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</a><a name="O15Readme_RelatedContent">
<h2 class="heading">Related content</h2>
</a>
<div class="section" id="sectionSection5"><a name="O15Readme_RelatedContent"></a>
<ul>
<a name="O15Readme_RelatedContent"></a>
<li><a name="O15Readme_RelatedContent"></a>
<p><a name="O15Readme_RelatedContent"></a><a href="https://github.com/OfficeDev/PnP/tree/master/Samples/Branding.ClientSideRendering" target="_blank">Office365 Development Patterns and Practices</a></p>
</li><li>
<p><a href="https://github.com/SPCSR/DisplayTemplates" target="_blank">DisplayTemplates repository on GitHub</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/magazine/dn745867.aspx" target="_blank">Using JSLink with SharePoint 2013</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/office/Client-side-rendering-JS-2ed3538a" target="_blank">Client-side rendering (JSLink) code samples</a></p>
</li></ul>
</div>
</div>
</div>
</div>
