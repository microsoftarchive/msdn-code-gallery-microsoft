# Exchange 2013 101 Code Samples
## License
- Custom
## Technologies
- Exchange Online
- Office 365
- Microsoft Exchange Server 2007
- Microsoft Exchange Server 2010
- Microsoft Exchange Server 2013
- EWS Managed API
## Topics
- EWS
## Updated
- 12/19/2013
## Description

<div id="header">The code samples in the Exchange 2013: 101 code samples package show you how to use the Exchange Web Services (EWS) Managed API to perform specific tasks with mailbox data on an on-premises Exchange server, Exchange Online, or Exchange Online
 as part of Office 365. The package spans most Exchange feature areas and each code sample within it shows you how to perform a common Exchange development task, including how to:</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<div class="section"></div>
<div class="section">
<ul>
</ul>
</div>
<ul>
<li>Create, get, and set properties for email messages, delete email messages, delay sending email messages, attach files to email messages, add signatures to email messages, and embed images in email messages.
</li><li>Create, delete, copy, move, or empty folders, find folder identifiers, and find folders by their display names.
</li><li>Access a property by name, identifier, or property set GUID, and create, get, update, or delete custom extended properties.
</li><li>Set streaming or pull notifications. </li><li>Find contacts by their display names, and create, get, update, or delete contacts that are associated with an email account.
</li><li>Get or set OOF settings or messages for users. </li><li>Get availability information for users or rooms. </li><li>Get a set of meeting times for prospective meeting attendees, get a list of rooms, respond to meeting invitations, and create, get, update, or delete meetings or appointments.
</li><li>Get email addresses that are members of distribution or room lists. </li><li>Get user names that are associated with an email address. </li><li>Create, get, update, or delete Inbox rules. </li><li>Create search folders or filters, find conversations, and find contacts. </li><li>Create, get, update, or delete single-occurrence or recurring tasks. </li><li>Get a list of delegates for an account, add or remove delegates from an account, and set delegate access permissions for specific folders.
</li><li>Create, get, update, or delete user information and configuration details, and add or remove delegates from an email account.
</li></ul>
<p>All the samples are in the form of Visual Studio solutions.</p>
<h1 class="heading">Prerequisites</h1>
<div class="section"></div>
<p class="section">The samples require the following:</p>
<div class="section"></div>
<div class="section">
<ul>
</ul>
</div>
<ul>
<li>A target server that is running a version of Exchange starting with Exchange Server 2007 Service Pack 1 (SP1), including Exchange Online.
</li><li>The .NET Framework 4. </li><li>The EWS Managed API assembly file, Microsoft.Exchange.WebServices.dll. You can download the assembly from the
<a href="http://go.microsoft.com/fwlink/?LinkID=255472" target="_blank">Microsoft Download Center</a>. The samples assume that the assembly is in the default download directory. You will need to verify the path before you run the solution for an individual
 sample. </li><li>A version of Visual Studio starting with Visual Studio 2010.<br>
Or<br>
A text editor to create and edit source code files and a command prompt window to run a .NET Framework command line compiler.
</li></ul>
<h1>Key components of the sample</h1>
<div class="section"></div>
<p class="section">Each sample typically contains the following files:</p>
<div class="section"></div>
<div class="section">
<ul>
</ul>
</div>
<ul>
<li>*.sln &mdash; A Visual Studio solution file for the project. </li><li>*.csproj &mdash; One or more Visual Studio project files. </li><li>app.config &mdash; The configuration data for the project. </li><li>*.cs &mdash; The using statements, namespace, class, and functions that showcase a specific feature.
</li><li>Authentication.csproj &mdash; The Visual Studio project file for the dependent authentication code.
</li><li>TextFileTraceListener.cs &mdash; The using statements, namespace, class, and code for writing the XML request and response to a text file.
</li><li>Service.cs &mdash; The using statements, namespace, class, and functions that are necessary for acquiring an
<strong>ExchangeService</strong> object that is used in the project. </li><li>CertificateCallback.cs &mdash; The using statements, namespace, class, and code for acquiring an X509 certificate.
</li><li>UserData.cs &mdash; The using statements, namespace, class, and functions for acquiring the user information that is required by the service object.
</li></ul>
<p><strong>Security note&nbsp; </strong></p>
<p class="heading">The certificate callback function in the Authentication project is meant for an on-premises test installation of Exchange that uses the default, self-signed certificate. We include this callback function to make it possible to run the Exchange
 101 code samples without having to install a signed certificate. In production, the Exchange server should have a valid, signed certificate. The sample authentication code should not be used on a server that has a signed certificate and it should not be used
 on a production server. To activate the callback method, follow the instructions in the code comments for the Authentication project.&nbsp;<strong><br>
</strong></p>
<h1 class="heading">Configure the sample</h1>
<div class="section"></div>
<p class="section">Follow these steps to configure the Exchange 2013: 101 code samples package.</p>
<div class="section"></div>
<div class="section">
<ol>
</ol>
</div>
<ol>
<li>In Visual Studio, set the startup project by selecting the project in the Solution Explorer and choosing &quot;Set as StartUp Project&quot; from the
<strong><span class="ui">Project</span> </strong>menu. </li><li>Ensure that the reference path for the Microsoft.Exchange.WebServices.dll file points to the location where the DLL is installed on your local computer.
</li></ol>
<h1>Build the samples</h1>
<div id="sectionSection4" class="section">
<p>Press F5 to build and deploy the samples.</p>
</div>
<h1 class="heading">Run and test the samples</h1>
<div id="sectionSection5" class="section">
<p>Press F5 to run the samples.</p>
</div>
<h1 class="heading">Related content</h1>
<div id="sectionSection6" class="section">
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/dn467891(v=exchg.150).aspx" target="_blank">How to: Communicate with EWS by using the EWS Managed API</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/dn528373(v=exchg.150).aspx" target="_blank">How to: Reference the EWS Managed API assembly</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/library/jj900168(v=exchg.150).aspx" target="_blank">Start using web services in Exchange</a></p>
</li><li>
<p><a href="http://code.msdn.microsoft.com/exchange" target="_blank">All Exchange code samples on MSDN</a></p>
</li></ul>
</div>
<h1 class="heading">Change log</h1>
<div id="sectionSection7" class="section">
<div class="caption"></div>
<div class="tableSection">
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p style="text-align:left">Date</p>
</th>
<th>
<p style="text-align:left">Description</p>
</th>
</tr>
<tr>
<td>
<p>December 2013</p>
</td>
<td>
<p>Added 31 samples and clarified the intended use of the Authentication project.</p>
</td>
</tr>
<tr>
<td>
<p>June 2013</p>
</td>
<td>
<p>First release.</p>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
</div>
</div>
