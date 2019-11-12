# WinHTTP proxy sample
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
<p>This sample demonstrates how to use the WinHTTP API to determine the proxy for a particular URL. It uses the core functionality provided in WinHTTP for querying the proxy settings.
</p>
<p>Both <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384097">
<b>WinHttpGetProxyForUrl</b></a> and <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh405356">
<b>WinHttpGetProxyForUrlEx</b></a> are used in this sample. These functions implement the Web Proxy Auto-Discovery (WPAD) protocol for automatically configuring the proxy settings for an HTTP request. The WPAD protocol downloads a Proxy Auto-Configuration (PAC)
 file, which is a script that identifies the proxy server to use for a given target URL. PAC files are typically deployed by the IT department within a corporate network environment. These functions can automatically discover the location of the PAC file on
 the local network.</p>
<p>This sample can be extended as needed for your application, using the proxy code from this sample as a starting point, to add additional functionality. For example, an application could add a per-URL proxy cache, awareness for network changes, a filter for
 bad proxies, or other desired functionality.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384273">Windows HTTP Services (WinHTTP)</a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384099"><b>WinHttpOpenRequest</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh405355"><b>WinHttpCreateProxyResolver</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707321"><b>WinHttpFreeProxyResult</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707322"><b>WinHttpGetProxyResult</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh405356"><b>WinHttpGetProxyForUrlEx</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa383912"><b>WINHTTP_PROXY_INFO</b></a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384273">Windows HTTP Services (WinHTTP)</a>
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
<p>To build the sample using Visual Studio&nbsp;2013:</p>
<ol>
<li>Open Windows Explorer and navigate to the <b>\cpp</b> directory. </li><li>Double-click the icon for the <b>WinhttpProxySample.sln</b> file to open the file in Visual Studio.
</li><li>In the <b>Build</b> menu, select <b>Build Solution</b>. The application will be built in the default
<b>\Debug</b> or <b>\Release</b> directory. </li></ol>
<h3>Run the sample</h3>
<p>To run the sample:</p>
<ol>
<li>Navigate to the directory that contains the new executable, using the command prompt.
</li><li>Type <b>WinhttpProxySample.exe &lt;url&gt;</b> at the command prompt. </li></ol>
</div>
