# WebSocket sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Networking
- universal app
## Updated
- 05/13/2014
## Description

<div id="mainSection">
<p>This sample shows you how to send and receive date using the WebSocket classes in the
<a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a> namespace in your Windows Runtime app. The sample covers basic features that include making a WebSocket connection, sending and receiving data, and closing
 the connection. The sample also shows recommended ways of handling both trusted (hard coded) URI inputs and unvalidated (user-entered) URI inputs.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Windows&nbsp;8.1 provides support for the client use of WebSockets in a Windows Runtime. The
<a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a> namespace defines two types of WebSocket objects for use by clients in Windows Store apps.
</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/br226842"><b>MessageWebSocket</b></a> is suitable for typical scenarios where messages are not extremely large. Both UTF-8 and binary messages are supported.
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br226923"><b>StreamWebSocket</b></a> is more suitable for scenarios in which large files (such as photos or movies) are being transferred, allowing sections of a message to be read with each read operation
 rather than reading the entire message at once. Only binary messages are supported.
</li></ul>
Both <a href="http://msdn.microsoft.com/library/windows/apps/br226842"><b>MessageWebSocket</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br226923"><b>StreamWebSocket</b></a> connections are demonstrated in this sample. This sample shows how to use the following features:
<p></p>
<p></p>
<ul>
<li>Use a <a href="http://msdn.microsoft.com/library/windows/apps/br226842"><b>MessageWebSocket</b></a> to send UTF-8 text messages. The server will echo the messages back.
</li><li>Use a <a href="http://msdn.microsoft.com/library/windows/apps/br226923"><b>StreamWebSocket</b></a> to send binary data. The server will echo the binary data back.
</li></ul>
<p></p>
<p>Versions of this sample are provided in JavaScript, C#, VB, and C&#43;&#43;. </p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows version of this sample by default requires network access using the loopback interface.</p>
<p><b>Network capabilities</b></p>
<p>This sample requires that network capabilities be set in the <i>Package.appxmanifest</i> file to allow the app to access the network at runtime. These capabilities can be set in the app manifest using Microsoft Visual Studio.
</p>
<p>To build the Windows version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Private Networks (Client &amp; Server)</b>: The sample has inbound and outbound network access on a home or work network (a local intranet). Even though this sample by default runs on loopback, it needs either the
<b>Private Networks (Client &amp; Server)</b> or <b>Internet (Client)</b> capability in order to access the network and send and receive data. The
<b>Private Networks (Client &amp; Server)</b> capability is represented by the <b>
Capability name = &quot;privateNetworkClientServer&quot;</b> tag in the app manifest. </p>
</li><li>
<p>If the sample is modified to connect to the server component running on a different device on the Internet (a more typical app), the
<b>Internet (Client)</b> capability must be set for the client component. This is represented by the
<b>Capability name = &quot;internetClient&quot;</b>. </p>
</li></ul>
<p>To build the Windows Phone version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Internet (Client &amp; Server):</b> This provides the sample with complete access to the network for both client operations (outbound-initiated access) and server operations (inbound-initiated access). This allows the app to download various types of
 content from a WebSocket server and upload content to a WebSocket server located on the Internet. This allows the app to make network connections using the
<a href="http://msdn.microsoft.com/library/windows/apps/br226842"><b>MessageWebSocket</b></a> or
<a href="http://msdn.microsoft.com/library/windows/apps/br226923"><b>StreamWebSocket</b></a> object to a WebSocket server on the Internet or to a WebSocket server on the local intranet. This is represented by the
<b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;On Windows Phone, there is only one network capability which enables all network access for the app.</p>
<p></p>
</li></ul>
<p></p>
<p>For a sample that shows how to use a WebSocket to send and receive data so that the app is always connected and always reachable using background network notifications in a Windows Store app, download the
<a href="http://go.microsoft.com/fwlink/?LinkID=251353">ControlChannelTrigger StreamWebSocket sample</a>.
</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other resources</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452752">Adding support for networking</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761442">Connecting with WebSockets</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770532">How to configure network capabilities</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761443">How to connect with a MessageWebSocket</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh761445">How to connect with a StreamWebSocket</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226842"><b>MessageWebSocket</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226923"><b>StreamWebSocket</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a>
<h2>Operating system requirements</h2>
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
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>WebSocket.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build HttpClient.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>WebSocket.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build WebSocket.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>WebSocket.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy WebSocket.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>WebSocket.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy WebSocket.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the Windows version of the sample</b></p>
<p>For the app to attempt a WebSocket connection, this sample requires that a web server is available that supports WebSockets. The web server must also have a
<i>WebSocketSample</i> path available to access. The web server must be started before the app is run. The sample includes a PowerShell script that will install and enable IIS on a local computer and enable WebSocket connections. The easiest way to run the
 sample is to use the provided web server scripts. The PowerShell script that will install IIS on the local computer, create the
<i>WebSocketSample</i> folder on the server, copy files to this folder, and enable IIS.</p>
<p>Browse to the <i>Server</i> folder in your sample folder to setup and start the web server. There are two options possible.</p>
<p></p>
<ul>
<li>Start PowerShell elevated (Run as administrator) and run the following command:
<p><b>.\SetupServer.ps1</b></p>
<p>Note that you may also need to change script execution policy. </p>
</li><li>Start an elevated Command Prompt (Run as administrator) and run following command:
<p><b>PowerShell.exe -ExecutionPolicy Unrestricted -File SetupServer.ps1</b></p>
</li></ul>
<p></p>
<p>When the web server is not needed anymore, please browse to the <i>Server</i> folder in you sample folder and run one of the following:</p>
<p></p>
<ul>
<li>Start PowerShell elevated (Run as administrator) and run the following command:
<p><b>.\RemoveServer.ps1</b></p>
<p>Note that you may also need to change script execution policy. </p>
</li><li>Start an elevated Command Prompt (Run as administrator) and run following command:
<p><b>PowerShell.exe -ExecutionPolicy Unrestricted -File RemoveServer.ps1</b></p>
</li></ul>
<p></p>
<p>The sample can run using any web server, not only the one provided with the sample. If IIS is used on a different computer, then the previous scripts can be used with minor changes.
</p>
<ul>
<li>Copy the <i>Server</i> folder to the device where IIS will be run. </li><li>Run the above scripts to install and enable IIS and enable WebSocket connections.
</li></ul>
<p></p>
<p>The sample must also be updated when run against a non-localhost web server. To configure the sample for use with IIS on a different device:
</p>
<ul>
<li>Additional capabilities may need to be added to the app manifest for the sample. For example,
<b>Internet (Client &amp; Server)</b> if the web server is located on the Internet not on a local intranet.
</li><li>The hostname of the server to connect to also needs to be updated. This can be handled in two ways. The
<b>ServerAddressField</b> element in the HTML or XAML files can be edited so that &quot;localhost&quot; is replaced by the hostname or IP address of the web server. Alternately when the app is run, enter the hostname or IP address of the web server instead of the default
 &quot;localhost&quot; value in the <b>Server Address</b> field. </li></ul>
<p></p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;IIS is not available on ARM builds nor on Windows Phone. Instead, set up the web server on a separate 64-bit or 32-bit computer and follow the steps for using the sample against non-localhost web server.
</p>
<p></p>
<p>However if a server different than IIS is used, then this requires some special configuration of the server.
</p>
<ul>
<li>Copy the <i>Server\webSite</i> directory to the <i>WebSocketSample</i> folder on the web server.
</li><li>Configure the web server to accept WebSocket connections. </li></ul>
<p></p>
<p>To configure the sample for use with a web server different than IIS not using localhost:</p>
<ul>
<li>Additional capabilities may need to be added to the app manifest for the sample. For example,
<b>Internet (Client &amp; Server)</b> if the web server is located on the Internet not on a local intranet.
</li><li>The URI of the server to connect to also needs to be updated. This can be handled in two ways. The
<b>ServerAddressField</b> element in the HTML or XAML files can be edited so that &quot;localhost&quot; is replaced by the hostname or IP address of the web server. Alternately when the app is run, enter the hostname or IP address of the web server instead of the default
 &quot;localhost&quot; value in the <b>Server Address</b> field. </li></ul>
<p></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>WebSocket.Windows</b> in <b>Solution Explorer</b> and select <b>
Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
<p><b>Deploying and running the Windows Phone version of the sample</b></p>
<p>IIS is not available on Windows Phone. For the app to attempt a WebSocket connection to a server that supports WebSockets , there are two options:</p>
<ul>
<li>The easiest way to run the sample is to use the provided web server scripts on a separate 64-bit or 32-bit device that can run IIS. Follow the instructions for deploying and running the Windows version of the sample using IIS on a different device.
</li><li>Use a web server different than IIS on a separate device. Follow the instructions for deploying and running the Windows version of the sample using a non-IIS web server.
</li></ul>
<p></p>
<ul>
<li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>WebSocket.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
