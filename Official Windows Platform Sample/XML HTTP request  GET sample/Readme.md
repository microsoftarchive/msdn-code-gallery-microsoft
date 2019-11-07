# XML HTTP request  GET sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Networking
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh831163">
XML HTTP Extended Request (IXMLHTTPRequest2)</a> interfaces to asynchronously send HTTP GET requests and receive HTTP responses. The sample uses the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh831151"><b>IXMLHTTPRequest2</b></a> interface to send the GET request to an HTTP server and the
<a href="ixhr2.ixmlhttprequest2callback"><b>IXMLHTTPRequest2Callback</b></a> interface to receive the HTTP response from the HTTP server.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh770550">Quickstart: Connecting using XML HTTP Request (IXMLHTTPRequest2)</a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh831151"><b>IXMLHTTPRequest2</b></a>
</dt><dt><a href="ixhr2.ixmlhttprequest2callback"><b>IXMLHTTPRequest2Callback</b></a> </dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh831163">XML HTTP Extended Request (IXMLHTTPRequest2)</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa385331">WinINet</a>
<h3>Operating system requirements</h3>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
</tbody>
</table>
<h3>Build the sample</h3>
<ol>
<li>
<p>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.</p>
</li><li>
<p>2. Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.</p>
</li><li>
<p>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.</p>
</li></ol>
<h3>Run the sample</h3>
<p>To run the sample:</p>
<ol>
<li>Navigate to the directory that contains the new executable, using the Command Prompt.
</li><li>Type <b>XMLHttpRequestGet.exe &lt;url&gt;</b> at the command prompt </li></ol>
</div>
