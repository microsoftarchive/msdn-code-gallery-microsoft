# SharePoint 2013: Use the People Picker experimental control in an app
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Sharepoint Online
- SharePoint Server 2013
- SharePoint Foundation 2013
- apps for SharePoint
## Topics
- Data Access
- User Experience
- apps for SharePoint
## Updated
- 02/27/2014
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">SharePoint 2013: Use the People Picker experimental control in an app for SharePoint</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p><span>Summary:</span> Learn how to use the People Picker experimental control in an app that is not hosted on SharePoint.</p>
</div>
<div>
<p><strong>Last modified: </strong>February 20, 2014</p>
<p><strong>In this article</strong> <br>
<a href="#sectionSection0">Description of the sample</a> <br>
<a href="#sectionSection1">Prerequisites</a> <br>
<a href="#sectionSection2">Key components of the sample</a> <br>
<a href="#sectionSection3">Configure the sample</a> <br>
<a href="#sectionSection4">Run and test the sample</a> <br>
<a href="#sectionSection5">Change log</a> <br>
<a href="#sectionSection6">Related content</a></p>
<p>This provider-hosted app shows how to use the People Picker experimental control. Learn how to declare the control in HTML markup or by using JavaScript. The sample also shows how to specify options and event handlers for the People Picker control.</p>
<div>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Caution</strong> </th>
</tr>
<tr>
<td>
<p>The Office Web Widgets - Experimental are only provided for research and feedback purposes. Do not use in production scenarios. The Office Web Widgets behavior may change significantly in future releases.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1>Description of the sample</h1>
<div id="sectionSection0">
<p>The sample consists of an app that has four webpages that show you how to declare the control with the following variations:</p>
<ul>
<li>
<p>Basic control declaration in HTML markup</p>
</li><li>
<p>Basic control declaration in JavaScript</p>
</li><li>
<p>Control declaration in HTML markup with options</p>
</li><li>
<p>Control declaration in JavaScript with options</p>
</li></ul>
<p>The following screenshot shows how the People Picker control looks displayed on a simple HTML page.</p>
<strong>
<div class="caption">Figure 1. Browser window after running the solution</div>
</strong><br>
<strong></strong><img src="109574-image.png" alt=""></div>
<h1>Prerequisites</h1>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>Microsoft Visual Studio 2013</p>
</li><li>
<p>A SharePoint 2013 development environment (app isolation required for on-premises scenarios). If you need help setting up a development environment, see
<a href="http://msdn.microsoft.com/library/jj163980.aspx" target="_blank">Get started developing apps for SharePoint</a>.</p>
</li></ul>
</div>
<h1>Key components of the sample</h1>
<div id="sectionSection2">
<p>The sample contains the following:</p>
<ul>
<li>
<p>PeoplePickerBasic project</p>
<ul>
<li>
<p>PeoplePickerSamples.aspx page, this is the app start page.</p>
</li></ul>
</li><li>
<p>PeoplePickerBasicWeb project</p>
<ul>
<li>
<p>MarkupSimple.html page, which shows how to declare the control in HTML markup.</p>
</li><li>
<p>JSSimple.html page, which shows how to declare the control in JavaScript.</p>
</li><li>
<p>MarkupOptions.html page, which shows how to declare the control in HTML markup with options.</p>
</li><li>
<p>JSOptions.html page, which shows how to declare the control in JavaScript with options.</p>
</li><li>
<p>Scripts and resources required by the controls.</p>
</li></ul>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection3">
<p>Follow these steps to configure the sample.</p>
<ul>
<li>
<p>Update the <strong>SiteUrl</strong> property of the solution with the URL of your SharePoint website.</p>
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
</li></ol>
<p>You should see the PeoplePickerSamples.aspx page.</p>
</div>
<h1>Change log</h1>
<div id="sectionSection5">
<ul>
<li>
<p>First version: March 2014</p>
</li></ul>
</div>
<h1>Related content</h1>
<div id="sectionSection6">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/library/6ce01956-6bda-45bf-9b4a-cffc0687a913" target="_blank">Office Web Widgets - Experimental overview</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/library/edc60550-67d2-4230-8e27-06a328c0d1f1" target="_blank">How to: Use the People Picker control in apps for SharePoint</a></p>
</li></ul>
</div>
</div>
</div>
<p>&nbsp;</p>
