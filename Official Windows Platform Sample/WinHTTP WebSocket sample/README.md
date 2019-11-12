# WinHTTP WebSocket sample
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
<p>This sample demonstrates how to use the WinHTTP API to send and receive messages between a client and a server by using the WebSocket protocol.
</p>
<p>The sample performs each step required to use the WebSocket connection. First, it creates the session, connection and request handles to open a HTTP connection. It then requests to upgrade the protocol from HTTP to the WebSocket protocol. The WebSocket handshake
 is performed by sending a request and receiving the appropriate response from the server. Data is then sent and received using the WebSocket protocol, and checks are made to ensure the complete message is transmitted. Finally, the connection is closed, and
 the close status and reason are confirmed. </p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p>For general information about WebSocket connections and how the protocol works, see the IETF's
<a href="http://go.microsoft.com/fwlink/p/?linkid=240293">WebSocket Protocol</a> documentation.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384273">Windows HTTP Services (WinHTTP)</a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa384114"><b>WinHttpSetOption</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707326"><b>WinHttpWebSocketCompleteUpgrade</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707329"><b>WinHttpWebSocketSend</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707328"><b>WinHttpWebSocketReceive</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707325"><b>WinHttpWebSocketClose</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh707327"><b>WinHttpWebSocketQueryCloseStatus</b></a>
</dt></dl>
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
<li>Open Windows Explorer and navigate to the <b>\cpp</b> directory. </li><li>Double-click the icon for the <b>WinhttpWebsocket.sln</b> file to open the file in Visual Studio.
</li><li>In the <b>Build</b> menu, select <b>Build Solution</b>. The application will be built in the default
<b>\Debug</b> or <b>\Release</b> directory. </li></ol>
<h3>Run the sample</h3>
<p>This sample requires that a web server that supports WebSockets is available for the app to access for sending and receiving data. The web server must be started before the app is run. The web server must also have a
<i>WinHttpWebSocketSample</i> path available. The sample includes a PowerShell script that will install IIS on the local computer, create the
<i>WinHttpWebSocketSample</i> folder on the server, and copy a file to this folder.
</p>
<p>The easiest way to run the sample is to use the provided PowerShell scripts. Browse to the
<i>Server</i> folder in your sample folder to setup and start the web server for WebSockets. There are two options possible.</p>
<p></p>
<ul>
<li>Start PowerShell elevated (Run as administrator) and run the following command:
<p><b>.\SetupScript.ps1</b></p>
<p>Note that you may also need to change script execution policy. </p>
</li><li>Start an elevated Command Prompt (Run as administrator) and run following command:
<p><b>PowerShell.exe -ExecutionPolicy Unrestricted -File SetupServer.ps1</b></p>
</li></ul>
<p></p>
<p>When the web server is not needed anymore, please browse to the <i>Server</i> folder in your sample folder and run one of the following:</p>
<p></p>
<ul>
<li>Start PowerShell elevated (Run as administrator) and run the following command:
<p><b>.\RemoveScript.ps1</b></p>
<p>Note that you may also need to change script execution policy. </p>
</li><li>Start an elevated Command Prompt (Run as administrator) and run following command:
<p><b>PowerShell.exe -ExecutionPolicy Unrestricted -File RemoveScript.ps1</b></p>
</li></ul>
<p></p>
<p>The sample can run using any web server that supports WebSockets, not only the one provided with the sample. In this case, running the previous scripts are not required. However, this requires some special configuration of the server to create the
<i>WinHttpWebSocketSample</i> folder and copy a file to this folder. The sample must also be updated if run against a non-localhost web server:
</p>
<p>To configure the sample for use with a different web server:</p>
<dl><dd>Copy the <i>Server\webSite</i> directory to the <i>WinHttpWebSocketSample</i> folder on the web server and configure the server to support WebSockets.
</dd><dd>The target server name should be updated in the sources. This is changed by editing the
<i>WinhttpWebsocket.cpp</i> source file so that the <i>pcwszServerName</i> value that contains the hostname or IP address of the web server is substituted for localhost.
</dd></dl>
<p class="note"><b>Note</b>&nbsp;&nbsp;IIS is not available on ARM builds. Instead, set up the web server on a separate 64-bit or 32-bit computer and follow the steps for using the sample against a non-localhost web server.
</p>
<p></p>
<p>To run the sample:</p>
<ul>
<li>Run <b>WinhttpWebsocket</b> by clicking <b>Start Without Debugging</b> on the
<b>Debug</b> menu. </li></ul>
</div>
