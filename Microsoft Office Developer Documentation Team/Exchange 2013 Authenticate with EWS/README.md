# Exchange 2013: Authenticate with EWS
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Exchange Server 2007
- Exchange Server 2010
- Exchange Online
- Exchange 2013
- EWS Managed API
## Topics
- Authentication
## Updated
- 08/20/2013
## Description

<div id="header">
<table id="bottomTable" cellpadding="0" cellspacing="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText"></span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">Exchange 2013: Authenticate with the Exchange Web Services</span>
</td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<p></p>
<div>
<p>This sample shows you how to use the Exchange Web Services (EWS) Managed API to discover your EWS end point and authenticate with the service.</p>
</div>
<h1>Description of the Exchange 2013: Authenticate with EWS sample</h1>
<div id="sectionSection0" name="collapseableSection">
<p>This sample provides different methods for getting a user's credentials, authenticating the credentials, and discovering the EWS end point. This sample includes a method that will store your credentials in the Windows Credential Manager.
</p>
</div>
<h1>Prerequisites</h1>
<div id="sectionSection1" name="collapseableSection">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A target server that is running a version of Exchange starting with Exchange Server 2007 Service Pack 1 (SP1), including Exchange Online.</p>
</li><li>
<p>The .NET Framework 4.</p>
</li><li>
<p>The EWS Managed API assembly file, Microsoft.Exchange.WebServices.dll. You can download the assembly from the
<a href="http://go.microsoft.com/fwlink/?LinkID=255472" target="_blank">Microsoft Download Center</a>.</p>
<div>
<table width="100%" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<th align="left"><b>Note</b> </th>
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
<p>Visual Studio 2010.</p>
<p>Or</p>
</li><li>
<p>A text editor to create and edit source code files and a command prompt window to run a .NET Framework command line compiler.</p>
</li></ul>
</div>
<h1>Key components of the sample</h1>
<div id="sectionSection2" name="collapseableSection">
<p>This sample contains the following files:</p>
<ul>
<li>
<p>Ex15_Authentication_CS.sln — The Visual Studio 2010 solution file for the Ex15_Authentication_CS project.</p>
</li><li>
<p>Ex15_Authentication_CS.csproj — The Visual Studio 2010 project file for authentication code in the
<b>ConnectToService</b> function.</p>
</li><li>
<p>app.config — Contains configuration data for the Ex15_Authentication_CS project.</p>
</li><li>
<p>CredentialHelper.cs — Contains the using directives, namespace, class, and functions to prompt for credentials, verify credentials, and store credentials for an application that uses EWS.</p>
</li><li>
<p>TextFileTraceListener.cs — Contains the using directives, namespace, class, and code to write the XML request and response to a text file.</p>
</li><li>
<p>Service.cs — Contains the using directives, namespace, class, and functions necessary to acquire an
<b>ExchangeService</b> object used in the CredentialHelper.cs project.</p>
</li><li>
<p>CertificateCallback.cs — Contains the using directives, namespace, class, and code to acquire an X509 certificate.</p>
</li><li>
<p>UserData.cs — Contains the using directives, namespace, class, and functions necessary to acquire user information required by the service object.</p>
</li></ul>
</div>
<h1>Configure the sample</h1>
<div id="sectionSection3" name="collapseableSection">
<p>This sample project is intended to be used by other samples that connect to EWS. Include the Ex15_Authentication_CS project in a solution that connects to EWS and implement one of the
<b>ConnectToService</b> functions to get a valid <b>ExchangeService</b> object for your solution.</p>
<p></p>
</div>
<h1>Build the sample</h1>
<div id="sectionSection4" name="collapseableSection">
<p>Press F6 to build the sample.</p>
</div>
<h1>Related content</h1>
<div id="sectionSection5" name="collapseableSection">
<p><a href="http://msdn.microsoft.com/en-us/library/jj220499(v=exchg.80).aspx" target="_blank">Get started with the EWS Managed API</a>
</p>
<p></p>
</div>
<h1>Change log</h1>
<div id="sectionSection6" name="collapseableSection">
<p>First release.</p>
</div>
</div>
</div>
