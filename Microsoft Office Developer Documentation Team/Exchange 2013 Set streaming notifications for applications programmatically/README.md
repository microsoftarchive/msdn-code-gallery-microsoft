# Exchange 2013: Set streaming notifications for applications programmatically
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Exchange Server 2010
- Exchange Online
- Office 365
- EWS Managed API
- Exchange Server 2013
## Topics
- events
- Notification
## Updated
- 12/11/2013
## Description

<p id="header">This sample authenticates an email address and password entered from the console, creates a streaming subscription in the authenticated user's Inbox folder on the Exchange server, and monitors the Inbox for new mail, items created, and items
 deleted.</p>
<div id="mainSection">
<div id="mainBody">
<h1>Prerequisites</h1>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>A target server that is running a version of Exchange starting with Exchange Server 2007 Service Pack 1 (SP1), including Exchange Online as part of Office 365.
</li><li>The .NET Framework 4. </li><li>The EWS Managed API assembly file, Microsoft.Exchange.WebServices.dll. You can download the assembly from the
<a href="http://go.microsoft.com/fwlink/?LinkID=255472" target="_blank">Microsoft Download Center</a>.
<div>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong> </th>
</tr>
<tr>
<td>The sample assumes that the assembly is in the default download directory. You will need to verify the path before you run the solution.</td>
</tr>
</tbody>
</table>
</div>
</li><li>A version of Visual Studio starting with Visual Studio 2010. Or </li><li>A text editor to create and edit source code files and a command prompt window to run a .NET Framework command line compiler.
</li></ul>
</div>
<h1>Key components of the sample</h1>
<p id="sectionSection2">This sample contains the following files:</p>
<ul>
<li>Ex15_SetStreamingNotifications_CS.sln &mdash; The Visual Studio solution file for the Ex15_SetStreamingNotifications_CS project.
</li><li>Ex15_SetStreamingNotifications_CS.csproj &mdash; The Visual Studio project file for the
<strong>SubscribeToStreamingNotifications</strong> function. </li><li>app.config &mdash; Contains configuration data for the Ex15_SetPullNotifications_CS project.
</li><li>Ex15_SetStreamingNotifications_CS.cs &mdash; Contains the using statements, namespace, class, and functions to create a streaming subscription.
</li><li>Ex15_Authentication_CS.csproj &mdash; The Visual Studio project file for the dependent authentication code.
</li><li>CredentialHelper.cs &mdash; Contains the using directives, namespace, class, and functions to prompt for credentials, verify credentials, and store credentials for an application that uses EWS.
</li><li>TextFileTraceListener.cs &mdash; Contains the using statements, namespace, class, and code to write the XML request and response to a text file.
</li><li>Service.cs &mdash; Contains the using statements, namespace, class, and functions necessary to acquire an
<strong>ExchangeService</strong> object used in the Ex15_SetStreamingNotifications_CS project.
</li><li>CertificateCallback.cs &mdash; Contains the using statements, namespace, class, and code to acquire an X509 certificate.
</li><li>UserData.cs &mdash; Contains the using statements, namespace, class, and functions necessary to acquire user information required by the service object.
</li></ul>
<h1>Configure the sample</h1>
<p id="sectionSection3">Follow these steps to configure the Exchange 2013: Set streaming notifications for applications programmatically sample.</p>
<ol>
<li>Set the startup project to Ex15_SetStreamingNotifications_CS by selecting the project in the Solution Explorer and choosing &quot;Set as StartUp Project&quot; from the
<strong><span class="ui">Project</span></strong> menu. </li><li>Ensure that the reference path for the Microsoft.Exchange.WebServices.dll points to where the DLL is installed on your local computer.
</li></ol>
<h1>Build the sample</h1>
<div id="sectionSection4">
<p>Press F5 to build and deploy the sample.</p>
</div>
<h1>Run and test the sample</h1>
<div id="sectionSection5">
<p>Press F5 to run the sample.</p>
<p>When you are prompted to press or select Enter, wait to press Enter until you want the program to end. Send an email to the Inbox that you are monitoring and you will see the notifications pop up in the command window. After the timeout of one minute, you
 can choose whether you want to reestablish the connection.</p>
</div>
<h1>Related content</h1>
<div></div>
<div>
<ul>
<li><a href="http://go.microsoft.com/fwlink/?LinkID=301827" target="_blank">Get started with the EWS Managed API</a><span style="font-size:large">&nbsp;</span>
</li></ul>
</div>
<h1>Change log</h1>
<div id="sectionSection7"><span style="font-size:large"><strong>
<div class="caption"></div>
</strong></span>
<div>
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th><span style="font-size:medium">Date </span></th>
<th>Description </th>
</tr>
<tr>
<td>December 10, 2013</td>
<td>Updated the sample to accept user input after the subscription times out.</td>
</tr>
<tr>
<td>July 22, 2013</td>
<td>First release.</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
</div>
