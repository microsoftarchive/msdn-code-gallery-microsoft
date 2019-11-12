# Exchange 2013: Create meetings programmatically
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Exchange Server 2007
- Exchange Server 2010
- Exchange Online
- EWS Managed API
- Exchange Server 2013
## Topics
- Calendar
## Updated
- 06/26/2013
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">Exchange 2013: Create meetings programmatically</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<p>&nbsp;</p>
<div>
<p>This sample shows you how to use the Exchange Web Services (EWS) Managed API to create meetings.</p>
</div>
<h1>Description of the Exchange 2013: Create meetings programmatically sample</h1>
<div id="sectionSection0">
<p>This sample authenticates an email address and password entered from the console, creates two meetings, and then sends the request to the service to create the meetings and send meeting requests to the attendees.</p>
</div>
<h1>Prerequisites</h1>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A target server that is running a version of Exchange starting with Exchange Server 2007 Service Pack 1 (SP1), including Exchange Online as part of Office 365.</p>
</li><li>
<p>The .NET Framework 4.</p>
</li><li>
<p>The EWS Managed API assembly file, Microsoft.Exchange.WebServices.dll. You can download the assembly from the
<a href="http://go.microsoft.com/fwlink/?LinkID=255472" target="_blank">Microsoft Download Center</a>.</p>
<div>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong> </th>
</tr>
<tr>
<td>
<p>The sample assumes that the assembly is in the default download directory. You will need to verify the path before you run the solution.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>Visual Studio 2010 with the Visual Web Developer and C# components and an open Visual Studio 2010 solution.</p>
<p>Or</p>
</li><li>
<p>A text editor to create and edit source code files and a command prompt window to run a .NET Framework command line compiler.</p>
</li></ul>
</div>
<h1>Key components of the sample</h1>
<div id="sectionSection2">
<p>This sample contains the following files:</p>
<ul>
<li>
<p>Ex15_CreateMeeting_CS.sln &mdash; The Visual Studio 2010 solution file for the Ex15_CreateMeeting_CS project.</p>
</li><li>
<p>Ex15_CreateMeeting_CS.csproj &mdash; The Visual Studio 2010 project file for the
<strong>CreateMeeting</strong> function.</p>
</li><li>
<p>app.config &mdash; Contains configuration data for the Ex15_CreateMeeting_CS project.</p>
</li><li>
<p>Ex15_CreateMeeting_CS.cs &mdash; Contains the using directives, namespace, class, and functions to create meetings.</p>
</li><li>
<p>Authentication.csproj &mdash; The Visual Studio 2010 project file for the dependent authentication code.</p>
</li><li>
<p>TextFileTraceListener.cs &mdash; Contains the using directives, namespace, class, and code to write the XML request and response to a text file.</p>
</li><li>
<p>Service.cs &mdash; Contains the using directives, namespace, class, and functions necessary to acquire an
<strong>ExchangeService</strong> object used in the Ex15_CreateMeeting_CS project.</p>
</li><li>
<p>CertificateCallback.cs &mdash; Contains the using directives, namespace, class, and code to acquire an X509 certificate.</p>
</li><li>
<p>UserData.cs &mdash; Contains the using directives, namespace, class, and functions necessary to acquire user information required by the service object.</p>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection3">
<p>Follow these steps to configure the Exchange 2013: Create meetings programmatically sample.</p>
<ol>
<li>
<p>Set the startup project to Ex15_CreateMeeting_CS.csproj by selecting the project in the Solution Explorer and choosing &quot;Set as StartUp Project&quot; from the
<strong><span class="ui">Project</span></strong> menu.</p>
</li><li>
<p>Ensure that the reference path for the Microsoft.Exchange.WebServices.dll points to where the DLL is installed on your local computer.</p>
</li><li>
<p>On lines 34-36 and 44-46 in Ex15_CreateMeeting_CS.cs, set the attendees on to valid meeting attendees that can receive meeting requests.</p>
</li><li>
<p>Set the <span>demoBatchCreateMeeting</span> variable to <span><strong><span class="keyword">true</span></strong></span> if you want to perform meeting creation in a batch operation. Set the
<span>demoBatchCreateMeeting</span> variable to <span><strong><span class="keyword">false</span></strong></span> to create a single meeting.</p>
</li></ol>
<p>&nbsp;</p>
</div>
<h1>Build the sample</h1>
<div id="sectionSection4">
<p>Press F5 to build and deploy the sample.</p>
</div>
<h1>Run and test the sample</h1>
<div id="sectionSection5">
<p>Press F5 to run the sample.</p>
</div>
<h1>Related content</h1>
<div id="sectionSection6">
<ul>
<li>
<p><a href="http://go.microsoft.com/fwlink/?LinkId=301827" target="_blank">Get started with the EWS Managed API</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Cancel-ef5a6d1f" target="_blank">Exchange 2013: Cancel meetings programmatically</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Delete-a21b1a84" target="_blank">Exchange 2013: Delete appointments programmatically from Exchange servers</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Respond-to-98788452" target="_blank">Exchange 2013: Respond to meeting invitations programmatically</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Create-730bd23c" target="_blank">Exchange 2013: Create weekly recurring appointments programmatically on Exchange servers</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Accessing-d73f971d" target="_blank">Exchange 2013: Access calendar items in a recurring series programmatically</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Modify-items-9f65c57c" target="_blank">Exchange 2013: Modify items in a recurring series programmatically</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Delete-a-e1c7b89d" target="_blank">Exchange 2013: Delete a recurring series programmatically from Exchange servers</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/Exchange-2013-Update-a-51bb8fa7" target="_blank">Exchange 2013: Update a recurring series programmatically on Exchange servers</a></p>
</li></ul>
</div>
<h1>Change log</h1>
<div id="sectionSection7">
<p>First release.</p>
</div>
</div>
</div>
