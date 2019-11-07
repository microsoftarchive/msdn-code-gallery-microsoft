# SharePoint 2013: Add the client-side People Picker control to apps
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
- sites and content
## Updated
- 02/06/2013
## Description

<div id="header"><span class="label">Summary:</span> Learn how to add the client-side People Picker control to an app so users can quickly search for and select user accounts for people, groups, and claims.</div>
<div>&nbsp;</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<div>This sample SharePoint-hosted app shows how to add the client-side People Picker control to your app and how to query the picker from other client-side controls. The app lets you enter user names and then calls the picker's
<span><span class="keyword">GetAllUserInfo</span></span> and <span><span class="keyword">GetAllUserKeys</span></span> methods and displays the returned user information on the page.</div>
<div>&nbsp;</div>
</div>
<h1 class="heading">Description of the client-side People Picker control sample</h1>
<div class="section" id="sectionSection0">
<div>The client-side People Picker control is an HTML and JavaScript control that lets users quickly find and select people, groups, and claims from their organization. The picker is represented by the
<span><span class="keyword">SPClientPeoplePicker</span></span> object, which provides methods that other client-side controls can use to get information or to perform other operations. You can use
<span><span class="keyword">SPClientPeoplePicker</span></span> properties to configure the picker with control-specific settings, such as allowing users to select multiple people or specifying caching options. The picker also uses the web application configuration
 settings that are specified in the <a href="http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.administration.spwebapplication.peoplepickersettings(v=office.15).aspx" target="_blank">
SPWebApplication.PeoplePickerSettings</a> property.</div>
<div>&nbsp;</div>
<div>The sample demonstrates how to add the picker to your page markup, and how to initialize it and query it from your script. Rendering, initializing, and other functionality for the picker are handled by the server, including searching and resolving user
 input against the SharePoint authentication providers.</div>
<div>&nbsp;</div>
</div>
<div class="section" id="sectionSection0">
<div>For instructions about how to create this sample, see <a href="http://msdn.microsoft.com/en-us/library/jj713593(v=office.15).aspx" target="_blank">
How to: Use the client-side People Picker control in apps for SharePoint</a>.</div>
<div>&nbsp;&nbsp;</div>
</div>
<h1 class="heading">Prerequisites</h1>
<div class="section" id="sectionSection1">
<div>This sample requires the following:</div>
<ul>
<li>
<div>An Office 365 Developer Site</div>
</li><li>
<div>&quot;Napa&quot; Office 365 Development Tools</div>
</li></ul>
<div>or</div>
<ul>
<li>
<div>A SharePoint 2013 development environment that is configured for app isolation. If you're developing remotely, either the server must support sideloading of apps or you must install the app on a Developer Site.</div>
</li><li>
<div>Visual Studio 2012 and Office Developer Tools for Visual Studio 2012</div>
</li><li>
<div>Local administrator permissions on the development computer.</div>
</li><li>
<div>Manage Web Site and Create Subsites user permissions to the SharePoint site where you're installing the app. (By default, these permissions are available only to users who have the Full Control permission level or who are in the site Owners group.) To
 install the app, you must be logged on as someone other than the system account.</div>
</li></ul>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<div>See Sign up for an Office 365 Developer Site to sign up for a Developer Site and start using &quot;Napa&quot; Office 365 Development Tools or see How to: Set up an on-premises SharePoint 2013 development environment for apps for guidance about how to set up an on-premises
 development environment (and how to disable the loopback check, if necessary). If you are developing remotely, see Developing apps for SharePoint on a remote system.&nbsp;&nbsp;</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Key components of the sample</h1>
<div class="section" id="sectionSection2">
<div>The client-side People Picker control sample app contains the following key components:</div>
<ul>
<li>
<div>The clientPeoplePickerApp.sln solution file</div>
</li><li>
<div>The clientPeoplePickerApp.csproj project file</div>
</li><li>
<div>The Default.aspx file (in the <span class="ui">Pages</span> folder), which contains the HTML markup for the
<span><span class="keyword">div</span></span> element that hosts the picker and the references to the picker's script dependencies</div>
</li><li>
<div>The App.js file, which contains the script that initializes the picker, gets the picker object from the page, and queries the picker from a client-side control</div>
</li><li>
<div>The appManifest.xml file, which specifies properties for the app for SharePoint</div>
</li></ul>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<div>The ClientWebPart.aspx file and the <span><span class="keyword">ClientWebPart1</span></span> folder are automatically generated when you create a SharePoint-hosted app in &quot;Napa&quot; Office 365 Development Tools. They are not used in this sample.&nbsp;</div>
</td>
</tr>
</tbody>
</table>
</div>
</div>
<h1 class="heading">Configure the sample</h1>
<div class="section" id="sectionSection3">
<div>Follow these steps to configure the client-side People Picker control sample app.</div>
<div class="subSection">
<ol>
<li>
<div>Run Visual Studio 2012 as an administrator, and then open the extracted sample solution file.</div>
</li><li>
<div>In the <span class="ui">Properties</span> window for the <span class="ui">
clientPeoplePickerApp</span> project, update the <span class="ui">SiteUrl</span> property with the URL for your SharePoint 2013 site.</div>
</li></ol>
</div>
</div>
<h1 class="heading">Run and test the sample</h1>
<div class="section" id="sectionSection4">
<div class="subSection">
<ol>
<li>
<div>Press <span class="ui">F5</span> to build and deploy the app.</div>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<div>You'll be prompted to log in to the SharePoint site if you're running the app remotely. You'll also be prompted to log in to the isolated app domain if it isn't listed as a trusted site in your browser.</div>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<div>In the picker's text box, enter the names or email addresses of valid SharePoint accounts for users, groups, or claims, separated by semicolons.</div>
</li><li>
<div>Choose the <span class="ui">Get User Info</span> button to display information about the resolved accounts.</div>
</li></ol>
</div>
</div>
<h1 class="heading">Change log</h1>
<div class="section" id="sectionSection5">
<div>First release.</div>
<div>&nbsp;</div>
</div>
<h1 class="heading">Troubleshooting</h1>
<div class="section" id="sectionSection6">
<div>If the app fails to install, verify that the logged-on account is not the system account and that it has the required user permissions to the SharePoint site that you're deploying the app to (as described in</div>
</div>
<div class="section" id="sectionSection6">
<div><a href="#O15Readme_Prereq.htm">Prerequisites</a>). Also verify that the <span class="ui">
SiteUrl</span> property you specified matches the URL of your SharePoint 2013 site. If you made any changes to the appManifest.xml file, verify that the changes are correct and the XML parses successfully.</div>
<div>&nbsp;</div>
</div>
<h1 class="heading">Related content</h1>
<div class="section" id="sectionSection7">
<ul>
<li>
<div><a href="http://msdn.microsoft.com/en-us/library/jj713593(v=office.15).aspx" target="_blank">How to: Use the client-side People Picker control in apps for SharePoint</a></div>
</li><li>
<div><a href="http://technet.microsoft.com/en-us/library/gg602078.aspx" target="_blank">People Picker and claims providers overview (SharePoint 2013)</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179928(v=office.15).aspx" target="_blank">Create UX components in SharePoint 2013</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp161179(v=office.15).aspx" target="_blank">How to: Set up an environment for developing apps for SharePoint on Office 365</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179923(v=office.15).aspx" target="_blank">How to: Set up an on-premises development environment for apps for SharePoint</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/jj220047.aspx" target="_blank">Developing apps for SharePoint on a remote system</a></div>
</li><li>
<div><a href="http://msdn.microsoft.com/en-us/library/fp179930(v=office.15).aspx" target="_blank">Apps for SharePoint overview</a></div>
</li></ul>
</div>
</div>
</div>
