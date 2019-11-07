# Office 365: Office 365 Dashboard
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Office 365
## Topics
- EWS Managed API
## Updated
- 07/30/2013
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">Office 365: Office 365 Dashboard</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>Learn how to use the Exchange Web Services (EWS) Managed API, Microsoft Lync 2013 SDK, and Windows PowerShell to make an Office 365 communication dashboard application that shows Microsoft Outlook 2013 mailbox usage and Microsoft Lync 2013 user presence.</p>
</div>
<div>
<p><em><span>Applies to: </span></em></p>
</div>
<h1>Description of the sample</h1>
<div id="sectionSection0">
<p>This C# desktop application showcases the integration of Microsoft API technologies with Office 365 service version 2013-V1. It shows three important API integrations:</p>
<ul>
<li>
<p>The EWS Managed API provides the functionality to enable client applications to communicate with Microsoft Exchange Server 2007 Service Pack 1 (SP1), Exchange Server 2010, Exchange Server 2013, and Exchange Online.</p>
</li><li>
<p>The Windows PowerShell 2.0 SDK contains sample code and reference assemblies that allow you to build applications based on Windows PowerShell.</p>
</li><li>
<p>The Microsoft Lync 2013 API includes a set of managed classes with methods that you can use to add collaboration functionality to your application.</p>
</li></ul>
<p>This code sample helps you understand how to use the cross-product technologies with Office 365. The sample shows the following:</p>
<ul>
<li>
<p>The Office 365 users who are online - the list includes current availability and contact information.</p>
</li><li>
<p>The total number of read and unread email messages in a user's Outlook Inbox - the data is shown in a pie chart.</p>
</li><li>
<p>The mailbox usage for a particular user - the statistics are shown in progress-bar form.</p>
</li></ul>
</div>
<h1>Prerequisites</h1>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p><a href="http://www.microsoft.com/en-us/download/details.aspx?id=34673" target="_blank">Microsoft Visual Studio 2012 Express</a> and associated requirements for Windows Forms applications.</p>
</li><li>
<p>Office 365 Enterprise with the onmicrosoft.com domain.</p>
</li><li>
<p>Office 365 Enterprise domain user account with administrative privileges.</p>
</li><li>
<p>Microsoft Lync 2013 client. The client must be installed on the local development computer and on all computers that run your application.</p>
</li><li>
<p><a href="http://www.microsoft.com/en-us/download/confirmation.aspx?id=35371" target="_blank">Microsoft Exchange Web Services Managed API</a>.</p>
</li><li>
<p><a href="http://www.microsoft.com/en-us/download/details.aspx?id=36824" target="_blank">Microsoft Lync 2013 SDK</a>.</p>
</li><li>
<p>Office 365 cmdlets.</p>
<p>Before you use the Office 365 cmdlets, they need to be installed from the <a href="http://technet.microsoft.com/en-us/library/jj151815.aspx" target="_blank">
Windows Azure Active Directory Module for Windows PowerShell</a> download link. Office 365 cmdlets have the following prerequisites:</p>
<ul>
<li>
<p>Windows 7 or Windows Server 2008 R2.</p>
</li><li>
<p>Windows PowerShell and .NET Framework 3.5.1.</p>
</li><li>
<p>Microsoft Online Services Sign-In Assistant. Download and install from the Microsoft Download Center:
<a href="http://www.microsoft.com/en-in/download/details.aspx?id=28177" target="_blank">
Microsoft Online Services Sign-In Assistant for IT Professionals RTW</a>.</p>
</li></ul>
</li><li>
<p>Valid script execution policy for your development computer. Run the <span>Get-ExecutionPolicy</span>Windows PowerShell command to check the script execution policy of your system. If the value returned is not
<span>RemoteSigned</span>, then run the <span>Set-ExecutionPolicy RemoteSigned</span>Windows PowerShell command so that your computer can run Windows PowerShell scripts against your Office 365 Enterprise.</p>
</li></ul>
<h3>Install the Office 365 application suite</h3>
<div>
<p>To install the Office 365 application suite on your computer, follow these steps:</p>
<ol>
<li>
<p>In the right pane of the dashboard, click the <span>Download software</span> link, as shown in the following figure.</p>
<img src="93304-image.png" alt=""> </li><li>
<p>On the next page, in the left pane, click <span>desktop setup</span>.</p>
<img src="93301-image.png" alt=""> </li><li>
<p>Click <span>set up</span> and confirm when you are prompted to run Office 365 Desktop Setup.</p>
</li></ol>
</div>
<h3>Verify Office 365 user account settings</h3>
<div>
<p>The sample depends on the fact that at least one user configured on your Office 365 domain has user administration privileges.</p>
<p>Log in to your Office 365 portal and follow these steps:</p>
<ol>
<li>
<p>On the Office 365 ribbon, choose the <span>Admin</span> option, and then click
<span>Office 365</span>.</p>
</li><li>
<p>On the left side of the Office 365 admin center page, click <span>Users and groups</span>.</p>
</li><li>
<p>On the <span>Active users</span> page, select the check box next to the names of the users whose admin roles you want to change, and then click the edit button.</p>
<img src="93302-image.png" alt=""> </li><li>
<p>On the selected user page, on the left tab, click <span>settings</span>.</p>
<img src="93298-image.png" alt=""> </li><li>
<p>On the <span>Settings</span> page, under <span>Assign role</span>, click <span>
yes</span>.</p>
</li><li>
<p>Choose a <span>user management admin</span> role from the list.</p>
</li><li>
<p>Click <span>save</span> to return to the <span>users and groups</span> page.</p>
</li><li>
<p>On the menu on the left side of the Office 365 admin center page, click <span>
dashboard</span> to return to the dashboard.</p>
</li></ol>
<p>&nbsp;</p>
</div>
</div>
<h1>Key components of the sample</h1>
<div id="sectionSection2">
<p>The sample Windows Forms solution contains the following:</p>
<ul>
<li>
<p>O365_Dashboard_cs project file</p>
</li><li>
<p><strong>ShowDashboard</strong> class</p>
</li><li>
<p><strong>OutlookHelper</strong> class</p>
</li><li>
<p><strong>O365User</strong> class</p>
</li><li>
<p><strong>LyncHelper</strong> class</p>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection3">
<p>To configure the Office 365 dashboard sample, follow these steps:</p>
<ol>
<li>
<p>Open Microsoft Visual Studio 2012 by using the <span>Run as Administrator</span> option.</p>
</li><li>
<p>Click <span>File</span>, select <span>Open</span>, and then click <span>Project</span>.</p>
</li><li>
<p>Browse to the location of the <span>O365_Dashboard_cs</span> solution folder and select the
<span>O365_Dashboard_cs.sln</span> file.</p>
</li><li>
<p>The solution will look like the following figure.</p>
<img src="93303-image.png" alt=""> </li><li>
<p>Open the App.config file and enter the following values:</p>
<ul>
<li>
<p>UserID - The organizational account of the user. For example: &Prime;anac@contoso.com&Prime; where Contoso, Ltd. is the tenant in the Office 365 service.</p>
</li><li>
<p>Password - The password for the account you chose.</p>
</li><li>
<p>LiveIDConnectionUri - A string that contains the URI of the server that will authenticate the sample application. For example: https://pod51037psh.outlook.com/PowerShell-LiveID?PSVersion=2.0\0</p>
</li><li>
<p>AutodiscoverUrl - An optional parameter. The Autodiscover service determines the best endpoint for a specific user. For example: https://sixpr02.outlook.com/EWS/Exchange.asmx</p>
</li></ul>
</li></ol>
</div>
<h1>Run the sample</h1>
<div id="sectionSection4">
<p>Before running the sample, be sure you have assigned yourself as a user administrator for your Office 365 Enterprise domain. In Visual Studio, press
<span>F5</span> to start debugging the sample. The sample window shows a list of Office 365 users who are online (obscured in the following figure).</p>
<img src="93300-image.png" alt="">
<p>&nbsp;</p>
<p>Click the <span>Outlook Details</span> tab to view Outlook-related information. The tab shows the Outlook email activity details in a pie chart.</p>
<img src="93299-image.png" alt=""></div>
<h1>Troubleshooting</h1>
<div id="sectionSection5">
<p>The following table lists the common configuration and environment errors that help troubleshoot issues preventing the sample from building or deploying successfully.</p>
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
<p>The request failed. The remote server returned one of the following errors:</p>
<ul>
<li>
<p>(401) Unauthorized.</p>
</li><li>
<p>A valid SMTP address must be specified.</p>
</li><li>
<p>Cannot process argument because the value of argument &quot;userName&quot; is invalid.</p>
</li></ul>
</td>
<td>
<p>Verify your credentials entered in the App.config file.</p>
</td>
</tr>
<tr>
<td>
<p>Invalid URI: The host name could not be parsed.</p>
</td>
<td>
<p>Verify the <span>LiveIDConnectionUri</span> value in the App.config file.</p>
</td>
</tr>
<tr>
<td>
<p>No Office 365 users are listed in the table.</p>
</td>
<td>
<p>Sign in to Lync with the same credentials that you used to sign in to the Office 365 portal.</p>
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
<p>July 17, 2013</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
</div>
<p>&nbsp;</p>
