# Office 365 APIs: How to use Discovery Service
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Sharepoint Online
- SharePoint Foundation 2013
## Topics
- App
## Updated
- 07/06/2015
## Description

<div>
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">Office 365 APIs: How to use Discovery Service</span></td>
</tr>
</tbody>
</table>
</div>
<div>
<div id="mainBody">
<div class="summary">
<p>This sample app demonstrates how to use discovery service to find user-specific end points for the various Microsoft services.</p>
</div>
<div class="introduction">
<p><strong>Last modified: </strong>January 12, 2015</p>
<p><strong>In this article</strong><br>
<a href="#O15Readme_Description">Description</a><br>
<a href="#O15Readme_Prereq">Prerequisites</a><br>
<a href="#O15Readme_KeyComponents">Key components of the sample</a><br>
<a href="#O15Readme_ManagingDirectory">Managing the directory for your Office 365 subscription in Azure</a><br>
<a href="#O15Readme_RegisteringApps">Registering an app and getting client ID in Azure Management Portal</a><br>
<a href="#O15Readme_ToRegisterApp">To register and add an app using Organizational accounts</a><br>
<a href="#sectionSection6">To get a client ID for Organizational accounts</a><br>
<a href="#O15Readme_ConfiguringSample">Configuring the sample</a><br>
<a href="#sectionSection8">Configuring permissions for the app to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal</a><br>
<a href="#O15Readme_build">To run the sample app</a><br>
<a href="#O15Readme_DiscoveryService">Discovery Service flow with Organization account</a><br>
<a href="#O15Readme_ChangeLog">Change log</a><br>
<a href="#O15Readme_RelatedContent">Related content</a></p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Important</strong></th>
</tr>
<tr>
<td>
<p>For this sample app to work, you need to also configure permissions for the app to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal. For more information, see the
<strong>Configuring permissions to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal</strong> section in this article.</p>
</td>
</tr>
</tbody>
</table>
</div>
<p>Your feedback about these features and APIs is important. <a href="http://officespdev.uservoice.com/" target="_blank">
Let us know</a> what you think. Have questions? Connect with us on <a href="https://stackoverflow.com/users/login?returnurl=%2fquestions%2fask%3ftags%3dms-office%2cpreview" target="_blank">
Stack</a>. Tag your questions with [Office365APIs].</p>
</div>
<div id="sectionSection0" class="section"><a name="O15Readme_Description"></a>
<h2>Description</h2>
<p>This solution is based on Windows app that finds user-specific end points for the various Microsoft services, using a Windows template from Visual Studio 2012 or Visual Studio 2013. The app describes the simple flow established by &quot;Discovery Service&quot; for
 using authenticated Microsoft productivity services locations for a given user. For more information on this sample, see the live demonstration at
<a href="http://channel9.msdn.com/events/SharePoint-Conference/2014/SPC3999">SharePoint Power Hour - New developer APIs and features for Apps for SharePoint | SharePoint Conference 2014 | Channel 9</a> demo start at 55.11 minutes into the video.</p>
</div>
<div id="mainBody"><a name="O15Readme_Prereq">
<h2 class="heading">Prerequisites</h2>
</a></div>
<div>This sample requires the following:</div>
<ul>
<li>Visual Studio 2012 or Visual Studio 2013. </li><li>An Office 365 Organizational account. </li><li>Windows app development registration. </li><li>A directory for your Office 365 subscription in Azure. For more information, see the
<strong>Managing the directory for your Office 365 subscription in Azure</strong> section in this article.
</li><li>App registration in the Azure Management Portal. For more information, see the
<strong>Registering an app and getting client ID in Azure Management Portal</strong> section in this article.
</li><li>Configuring permissions for the app to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal. For more information, see the
<strong>Configuring permissions to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal</strong> section in this article.
</li></ul>
<a name="O15Readme_KeyComponents"></a>
<div id="sectionSection2" class="section"><a name="O15Readme_KeyComponents"></a>
<h2 class="heading">Key components of the sample</h2>
The sample app contains the following:<br>
<ul>
<li><strong>MainPage.xaml.cs</strong> - a code-behind file of the main page containing the logic behind all the steps such as First Sign-In, Get Authorized, Discover, and Get Files.
</li><li><strong>FirstSignIn.xaml</strong>- contains the code for the dialog (sign-in) box that appears when a user clicks on First Sign-In.
</li><li><strong>DiscoveryXElements.cs</strong> - a file for XLINQ element and namespace names.
</li></ul>
</div>
<div id="sectionSection3" class="section"><a name="O15Readme_ManagingDirectory"></a>
<h2>Managing the directory for your Office 365 subscription in Azure</h2>
<p>You'll only see your Office 365 tenant in the Azure AD after you've added your Office 365 subscription to Azure AD.&nbsp;If you haven't done so, you need to associate and manage your Office 365 subscription with Azure. To learn how to associate and manage
 the directory for your Office 365 subscription in Azure, see <a href="http://msdn.microsoft.com/en-us/library/azure/dn629580.aspx" target="_blank">
Manage the directory for your Office 365 subscription in Azure</a>.</p>
</div>
<a name="O15Readme_RegisteringApps"></a>
<h2 class="heading">Registering an app and getting client ID in Azure Management Portal</h2>
<p>This code sample demonstrates how you can authenticate into the Office 365 APIs using an Office 365 Organizational account. This Windows 8 app needs to have a registered client ID so that the authentication process trusts the app.</p>
<div id="sectionSection4" class="section">
<p><a name="O15Readme_ToRegisterApp" style="background-color:#ffffff"></a></p>
<h2 class="heading" style="display:inline!important">To register and add an app using Organizational accounts</h2>
Following are the steps to add and register an app for Office 365 accounts:</div>
<div id="sectionSection5" class="section">
<ol>
<li>
<p>Sign into your Microsoft Azure account via Azure Management Portal</p>
</li><li>
<p>Click the <strong>Active Directory</strong> icon from the left menu, as shown in the following Figure 1.<br>
<br>
Figure 1. Windows Azure active directory&nbsp;</p>
<img id="133097" width="417" src="133097-platform365_how_to_use_platform_365_dscovery_servce_fig1.png" alt="Microsoft Azure Active directory" height="372">
</li><li>
<p>Click the desired directory, which in this case is Office 365 tenant.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>You'll only see your Office 365 tenant in the directory after you've added your Office 365 subscription to Azure AD. If haven't, see the previous
<strong>Managing the directory for your Office 365 subscription in Azure</strong> section to learn how.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>Select <strong>Applications</strong> tab, as shown below in Figure 2.<br>
<br>
Figure 2. Applications tab</p>
<img id="133099" width="523" src="133099-office365apisgetdocumentsmailscaledarinwin8appfig2.png" alt="Microsoft Azure applications" height="495">
</li><li>Click the <span class="ui">Add</span> button at the bottom of the page to add a new native application.
</li><li>
<p>On the <strong>What do you want to do</strong> page, click on the link to <span class="ui">
Add an application my organization is developing</span> option.</p>
</li><li>
<p>On the <strong>Tell us about your application</strong> page, you must specify a name for your application as well as indicate the type of application you are registering with Azure AD. Give a name to your application and choose
<span class="ui">Native Client Application</span> from <strong>Type</strong> option, as shown below in Figure 3.<br>
<br>
Figure 3. Add application dialog box</p>
</li><li><img id="133100" width="417" src="133100-platform365_how_to_use_platform_365_dscovery_servce_fig3.png" alt="Microsoft Azure Active directory add application" height="253">
<p>Once finished, click the arrow icon on the bottom-right corner of the page.</p>
</li><li>
<p>On the <strong>App information</strong> page, provide a redirect URI, for example, http://contoso.com. Then click the checkbox in the bottom-right hand corner of the page.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>For more information about adding apps to Azure AD, see <a href="http://msdn.microsoft.com/en-us/library/azure/dn132599.aspx" target="_blank">
Adding, Updating, and Removing an Application</a>.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li></ol>
<p>You should now see the app you just added listed in the <strong>Applications</strong> tab page of your Office 365 tenant in the directory as shown in Figure 4.</p>
<div class="caption">Figure 4. Adding an app to Azure AD</div>
<br>
<img id="133101" width="686" src="133101-5f8359c2-5168-43ac-8852-a8f6d05243a8_fig4.gif" alt="Add App To Azure AD" height="349"></div>
<a name="sectionSection6"></a>
<h2 class="heading">To get a client ID for Organizational accounts</h2>
<p>Following are the steps to get client ID for Office 365 accounts:</p>
<ol>
<li>
<p>Sign into your Microsoft Azure account via Azure Management Portal</p>
</li><li>
<p>Click the <span class="ui">Active Directory</span> icon from the left menu.</p>
</li><li>
<p>Click the desired directory, which in this case is Office 365 tenant.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>You'll only see your Office 365 tenant in the directory after you've added your Office 365 subscription to Azure AD. If haven't, see the previous
<strong>Managing the directory for your Office 365 subscription in Azure</strong> section to learn how.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>Select <span class="ui">Applications</span> tab to display all the applications in the Office 365 directory.</p>
</li><li>
<p>Select the app that you just registered and added in the previous <strong>To register and add an app using Organizational accounts</strong> section.</p>
</li><li>
<p>Select <span class="ui">Configure</span> tab as shown in Figure 5.</p>
<div class="caption">Figure 5. Configure page showing client ID and redirect URI values</div>
</li><li>
<p>Copy the client ID value and the redirect URI value.</p>
</li></ol>
</div>
<a name="O15Readme_ConfiguringSample"></a>
<h2 class="heading">Configuring the sample</h2>
<p>Follow these steps to configure the sample:</p>
<ol>
<li>
<p>Open the <strong>DiscoveryWin8.sln</strong> file using Visual Studio 2012 or 2013.</p>
</li><li>
<p>In the <strong>config.cs</strong> file, update the client ID, and redirect URI for the Organizational account. For example:</p>
<div class="code">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th>&nbsp;</th>
<th>&nbsp;</th>
</tr>
<tr>
<td colspan="2">
<pre>   // App registration for Organizational account (Office 365 account)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; public string OrganizationalAccountClientId = &quot;81839e2b-5902-4721-b62-39c8bce47eae&quot;;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; public string OrganizationalAccountRedirectUri = &quot;http://contoso.com&quot;;
</pre>
</td>
</tr>
</tbody>
</table>
</div>
</li></ol>
<p>Rebuild the solution.</p>
</div>
<p><a name="sectionSection8"></a></p>
<h2 class="heading">Configuring permissions for the app to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal</h2>
<p>Follow these steps to configure permissions to Azure Active Directory (AD), Office 365 SharePoint Online and Office 365 Exchange Online in Azure Management Portal:</p>
<ol>
<li>
<p>Sign into your Microsoft Azure account via Azure Management Portal</p>
</li><li>
<p>Click the <span class="ui">Active Directory</span> icon from the left menu.</p>
</li><li>
<p>Click the desired directory, which in this case is Office 365 tenant as shown in Figure 6.</p>
<div class="caption">Figure 6. Office 365 tenant in Azure AD</div>
<br>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>You'll only see your Office 365 tenant in the directory after you've added your Office 365 subscription to Azure AD. If haven't, see the previous
<span class="ui">Managing the directory for your Office 365 subscription in Azure</span> section to learn how.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>Select <span class="ui">Applications</span> tab to display all the applications in the Office 365 directory.</p>
</li><li>
<p>Select the app that you just registered and added in the previous <strong>To register and add an app using Organizational accounts</strong> section.</p>
</li><li>
<p>Select <span class="ui">Configure</span> tab.</p>
</li><li>
<p>Scroll down to the <span class="ui">Permissions to other applications</span> section. The first column allows you to select from the available applications in your directory that expose a web API. Once selected, you may select application and delegation
 permissions that the web API exposes.</p>
</li><li>
<p>Add the applications you want to set permissions for. Make sure you add the following applications from the
<span class="ui">Select application</span> drop down list as shown in Figure 7.</p>
<ul>
<li>
<p>Azure Active Directory (AD)</p>
</li><li>
<p>Office 365 SharePoint Online</p>
</li><li>
<p>Office 365 Exchange Online</p>
</li></ul>
<div class="caption">Figure 7. Selecting applications to set permissions to in Azure AD</div>
</li><li>
<p>Set the permissions you want to give to the app in each of the following application. Set the permissions in the
<span class="ui">Delegated Permissions</span> drop down list. Do this for the following as shown in Figure 8:</p>
<ul>
<li>
<p>Azure Active Directory (AD): Set <span class="ui">Read directory data permission</span></p>
</li><li>
<p>Office 365 SharePoint Online: Set <span class="ui">Read users' files</span></p>
</li><li>
<p>Office 365 Exchange Online: Set <span class="ui">Read user's contacts</span></p>
</li></ul>
</li><li>
<div class="caption">Figure 8. Setting permissions to applications in Azure AD</div>
<br>
<img id="133103" width="697" src="133103-9f2f3cc9-b147-4976-99ab-2db5cad4f6c1_fig5.gif" alt="Delegate Permissions in Azure AD" height="356">
<p>Once the applications are selected and permissions are set, click the <span class="ui">
Save</span> button on the command bar.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>For more information about how the consent experience works for both the application developer and user in Azure AD, see the
<strong>Overview of the Consent Framework</strong> section in <a href="http://msdn.microsoft.com/en-us/library/azure/dn132599.aspx#BKMK_Consent" target="_blank">
Adding, Updating, and Removing an Application</a>.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li></ol>
<p><a name="O15Readme_build"></a></p>
<h2 class="heading">To run the sample app</h2>
<p>Press F5 to run the app on the local machine.</p>
<p><a name="O15Readme_DiscoveryService"></a></p>
<h2 class="heading">Discovery Service flow with Organization account</h2>
<p>The following images depict the discovery service flow with Microsoft account:</p>
<p>It is a single point where the developer can pass the user identity, get information to initiate authorization flows, get the hosting location of services (for the user), and get the endpoints to the service.</p>
<ol>
<li>
<p>First Sign-in: When a user clicks &quot;First Sign-In&quot;, the email of the user is requested. The app contacts the Discovery service and passes in the email address and the set of scopes that the app wants to access, as shown in the following Figure 9 and 10.</p>
<div class="caption">Figure 9: Log-in window with Organization ID</div>
<br>
<img id="133105" width="369" src="133105-discoveryservicereadme_fig12.png" alt="Log-in window with Organization ID" height="203">
<p>&nbsp;</p>
<div class="caption">Figure 10: First Sign-in return values with Organization ID</div>
<br>
<img id="133106" width="483" src="133106-discoveryservicereadme_fig13.png" alt="First Sign-in return values with Organization ID" height="243">
</li></ol>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>Microsoft account type returns &quot;1&quot; whereas Organizational ID returns &quot;2&quot; as account type.</p>
</td>
</tr>
</tbody>
</table>
<div id="sectionSection10" class="section">
<ol>
<li>
<p>Get authorized: The app goes to the authorization endpoint and the user is prompted with a single consent UI for all the services that the app wants to access. The app is granted permissions to access the resources and gets a generic refresh token, as shown
 in the following Figure 11.</p>
<div class="caption">Figure 11: Get Authorize return values with Organization ID</div>
<br>
<img id="133107" width="791" src="133107-discoveryservicereadme_fig14.png" alt="Get Authorize return values with Organization ID" height="497">
</li><li>
<p>Discover: The app contacts the Discovery service, presenting an access token to talk to this service, which includes the user on whose behalf it is acting and a list of resources it wants to access, as shown in the following Figure 12.</p>
</li></ol>
</div>
<p>&nbsp;</p>
<p>&nbsp;</p>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>With Organizational ID, you can talk to any of the four services i.e. Files, Mail, Calendar, or Contacts by Office 365</p>
</td>
</tr>
</tbody>
</table>
<li>
<div class="caption">Figure 12: Discovery service return values with Organization ID</div>
<br>
<img id="133109" width="706" src="133109-discoveryservicereadme_fig15.png" alt="Discovery service return values with Org ID" height="531">
</li><li>
<p>Get token: The app contacts the service endpoints with the access tokens and is authorized for the discovered services, as shown in the following Figure 13.</p>
<div class="caption">Figure 13: Get Token values with Organization ID</div>
<br>
<img id="133110" width="750" src="133110-discoveryservicereadme_fig16.png" alt="GetToken values with Organization ID" height="560">
</li><li>
<p>Get Files: The app contacts the resource server using the access token to access the desired resource or files, as shown in the Figure 14.</p>
<div class="caption">Figure 14: File return with Organization ID</div>
<br>
<img id="133111" width="737" src="133111-discoveryservicereadme_fig17.png" alt="File return with Organization ID" height="549">
<p><a name="O15Readme_ChangeLog"></a></p>
<h2 class="heading">Change log</h2>
<ul>
<li>
<p>First version: March, 2014</p>
</li><li>
<p>2nd version: November 2014</p>
</li></ul>
<p><a name="O15Readme_RelatedContent"></a></p>
<h2 class="heading">Related content</h2>
<ul>
<li><a href="http://msdn.microsoft.com/en-us/office/office365/howto/discover-service-endpoints" target="_blank">Discover service endpoints for your Office 365 app</a>
</li><li><a href="http://dev.office.com/getting-started" target="_blank">Office Dev Center - Getting Started</a>
</li><li><a href="http://msdn.microsoft.com/library/office/fp179930.aspx" target="_blank">Overview of Apps for SharePoint</a>
</li><li><a href="http://msdn.microsoft.com/library/office/fp161179.aspx" target="_blank">How to: Set up an environment for developing apps for SharePoint on Office 365</a>
</li></ul>
</li>