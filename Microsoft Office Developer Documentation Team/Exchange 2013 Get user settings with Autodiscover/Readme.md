# Exchange 2013: Get user settings with Autodiscover
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Exchange Server 2010
- Exchange Online
- Office 365
- Exchange Server 2007 SP1
- Exchange Server 2013
## Topics
- Configuration
- autodiscover
## Updated
- 05/22/2014
## Description

<div id="header">
<table id="bottomTable" cellspacing="0" cellpadding="0">
<tbody>
<tr id="headerTableRow1">
<td align="left"><span id="runningHeaderText">&nbsp;</span></td>
</tr>
<tr id="headerTableRow2">
<td align="left"><span id="nsrTitle">Exchange 2013: Get user settings with Autodiscover readme</span></td>
</tr>
</tbody>
</table>
</div>
<div id="mainSection">
<div id="mainBody">
<div>
<p>This sample shows you how to implement an Autodiscover client by using the SOAP Autodiscover and POX Autodiscover services.</p>
</div>
<h2>Description of the Exchange 2013: Get user settings with Autodiscover sample</h2>
<div id="sectionSection0">
<p>This sample authenticates an email address and password entered from the console, performs a service connection point (SCP) lookup to locate Autodiscover endpoints, then sends Autodiscover requests to retrieve user settings.</p>
</div>
<h2>Prerequisites</h2>
<div id="sectionSection1">
<p>This sample requires the following:</p>
<ul>
<li>A target server that is running a version of Exchange starting with Exchange Server 2007 Service Pack 1 (SP1), including Exchange Online.
</li><li>The .NET Framework 4.5. </li><li>Visual Studio 2012 with the Visual Web Developer and C# components and an open Visual Studio 2012 solution.
</li></ul>
<p>Or</p>
<ul>
<li>A text editor to create and edit source code files and a command prompt window to run a .NET Framework command line compiler.
</li></ul>
</div>
<h2>Key components of the sample</h2>
<div id="sectionSection2">
<p>This sample contains the following files:</p>
<ul>
<li>AutodiscoverSample.sln &mdash; The Visual Studio 2012 solution file for the AutodiscoverSample project.
</li><li>AutodiscoverSample.csproj &mdash; The Visual Studio 2012 project file for the Autodiscover sample.
</li><li>App.config &mdash; Contains configuration data for the AutodiscoverSample project.
</li><li>AssemblyInfo.cs &mdash; Contains basic information about the sample application.
</li><li>AutodiscoverRequest.cs &mdash; Contains the using statements, namespace, class, and functions to implement an Autodiscover request.
</li><li>AutodiscoverUrlList.cs &mdash; Contains the using statements, namespace, class, and functions to implement an ordered list of Autodiscover endpoint URLs.
</li><li>CommandLineArgs.cs &mdash; Contains the using statements, namespace, class, and functions to parse the command line arguments.
</li><li>POXXmlStrings.cs &mdash; Contains constant strings representing XML namespaces and element names used in POX requests and responses.
</li><li>Program.cs &mdash; Contains the using statements, namespace, class, and code to implement the sample program.
</li><li>SOAPXmlStrings.cs &mdash; Contains constant strings representing XML namespaces and element names used in SOAP requests and responses.
</li><li>Tracing.cs &mdash; Contains the using statements, namespace, class, and functions to output to a file and the console.
</li></ul>
<ul>
</ul>
</div>
<h2>Configure the sample</h2>
<div id="sectionSection3">
<p>Follow these steps to configure the Exchange 2013: Get user settings with Autodiscover sample. Specify command line parameters to be used when running the sample in the Visual Studio 2012 environment.</p>
<p>&nbsp;</p>
<ol>
<li>In Visual Studio, choose <strong><span class="ui">AutodiscoverSample Properties&hellip;</span></strong> from the
<strong><span class="ui">Project</span></strong> menu. </li><li>Select <strong><span class="ui">Debug</span></strong>. </li><li>Enter parameters in the <strong><span class="ui">Command line arguments</span></strong> text box.
</li></ol>
<p>&nbsp;</p>
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong> </th>
</tr>
<tr>
<td>
<p>For a list of parameters, see &quot;Run and test the sample&quot;.</p>
</td>
</tr>
</tbody>
</table>
</div>
<h2>Build the sample</h2>
<div id="sectionSection4">
<p>Press or select <strong><span class="ui">F6</span></strong> to build and deploy the sample.</p>
</div>
<h2>Run and test the sample</h2>
<div id="sectionSection5">
<p>The sample uses the command line arguments listed in the following table.</p>
<strong>
<div class="caption"></div>
</strong>
<div>
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<th>
<p>Argument</p>
</th>
<th>
<p>Description</p>
</th>
</tr>
<tr>
<td>
<p>emailAddress</p>
</td>
<td>
<p>Required. The email address to send to the Autodiscover server.</p>
</td>
</tr>
<tr>
<td>
<p>-skipSOAP</p>
</td>
<td>
<p>Optional. If present, the application will not send SOAP Autodiscover requests. Instead, it will only send POX Autodiscover requests.</p>
</td>
</tr>
<tr>
<td>
<p>-auth authEmailAddress</p>
</td>
<td>
<p>Optional. If present, the authEmailAddress argument will be used to authenticate the connection instead of the emailAddress argument.</p>
</td>
</tr>
</tbody>
</table>
</div>
<p>Press or select <strong><span class="ui">F5</span></strong> to run the sample.</p>
</div>
<h2>Related content</h2>
<div id="sectionSection6">
<p><a href="http://msdn.microsoft.com/en-us/library/exchange/jj900169(v=exchg.150).aspx" target="_blank">Autodiscover for Exchange 2013</a></p>
<p><a href="http://msdn.microsoft.com/library/b24228a8-5127-4bac-aef0-9c9e8843c9ff" target="_blank">How to: Find Autodisover endpoints by using SCP lookup</a></p>
<p><a href="http://msdn.microsoft.com/library/e5939ec2-1100-4174-8a88-5a6e09b60b23" target="_blank">Handling Autodiscover error messages</a></p>
</div>
<h2>Change log</h2>
<div id="sectionSection7"><strong>
<div class="caption"></div>
</strong>
<div>
<table cellspacing="2" cellpadding="5" width="50%" frame="lhs">
<tbody>
<tr>
<td>
<p>Date</p>
</td>
<td>
<p>Description</p>
</td>
</tr>
<tr>
<td>
<p>May 19, 2014</p>
</td>
<td>
<p>Corrected bug in TryAutodiscoverUrl function that failed to return null when the specified URL has already been tried.</p>
</td>
</tr>
<tr>
<td>
<p>October 4, 2013</p>
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
