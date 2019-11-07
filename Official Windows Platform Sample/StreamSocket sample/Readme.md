# StreamSocket sample
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
<p>This sample shows you how to a use stream (TCP) socket to send and receive data using the
<a href="http://msdn.microsoft.com/library/windows/apps/br226882"><b>StreamSocket</b></a> and related classes in the
<a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a> namespace in your Windows Runtime app.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The client component of the sample creates a TCP socket to make a network connection, uses the socket to send data, and closes the socket. The server component sets up a TCP listener that provides a connected socket for every incoming network connection,
 uses the socket to receive data from the client, and closes the socket. This sample is provided in the JavaScript, C#, VB, and C&#43;&#43; programming languages.</p>
<p>The client component of the sample demonstrates the following features:</p>
<p></p>
<ul>
<li>Use the <a href="http://msdn.microsoft.com/library/windows/apps/br226882"><b>StreamSocket</b></a> class to create a TCP socket.
</li><li>Make a network connection to a TCP network server using one of the <a href="http://msdn.microsoft.com/library/windows/apps/hh701504">
<b>StreamSocket.ConnectAsync</b></a> methods. </li><li>Send data to the server using the <a href="http://msdn.microsoft.com/library/windows/apps/br208154">
<b>Streams.DataWriter</b></a> object which allows a programmer to write common types (integers and strings, for example) on any stream.
</li><li>Close the socket. </li><li>Attempt a socket connection using SSL to web server at port 443 (HTTPS), evaluate the server certificate validity, and display its properties. A certificate error is expected since the self-signed certificate is not trusted and issued to a different site
 name. </li></ul>
<p></p>
<p>The server component of the sample demonstrates the following features:</p>
<p></p>
<ul>
<li>Use the <a href="http://msdn.microsoft.com/library/windows/apps/br226906"><b>StreamSocketListener</b></a> class to create a TCP socket to listen for an incoming TCP connection.
</li><li>Bind the socket to a local service name to listen for an incoming network connection using the
<a href="http://msdn.microsoft.com/library/windows/apps/dn298302"><b>StreamSocketListener.BindServiceNameAsync</b></a> method.
</li><li>Receive a <a href="http://msdn.microsoft.com/library/windows/apps/hh701494"><b>StreamSocketListener.ConnectionReceived</b></a> event that indicates that a connection was received on the
<a href="http://msdn.microsoft.com/library/windows/apps/br226906"><b>StreamSocketListener</b></a> object.
</li><li>Receive data from the client using the <a href="http://msdn.microsoft.com/library/windows/apps/br208119">
<b>Streams.DataReader</b></a> object which allows a programmer to read common types (integers and strings, for example) on any stream.
</li><li>Close the socket. </li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Use of this sample requires network access using the loopback interface.</p>
<p>For a sample that shows how to use a datagram (UDP) socket to send and receive data in a Windows Runtime app, download the
<a href="http://go.microsoft.com/fwlink/p/?linkid=258328">DatagramSocket sample</a>.
</p>
<p>For a sample that shows how to use a <a href="http://msdn.microsoft.com/library/windows/apps/br226882">
<b>StreamSocket</b></a> so that the app is always connected and always reachable using background network notifications in a Windows Store app, download the
<a href="http://go.microsoft.com/fwlink/p/?linkid=243039">ControlChannelTrigger TCP socket sample</a>.
</p>
<p><b>Network capabilities</b></p>
<p>This sample requires that network capabilities be set in the <i>Package.appxmanifest</i> file to allow the app to access the network at runtime. These capabilities can be set in the app manifest using Microsoft Visual Studio.
</p>
<p>To build the Windows version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Private Networks (Client &amp; Server)</b>: The sample has inbound and outbound network access on a home or work network (a local intranet). Even though this sample by default runs on loopback, it needs either the
<b>Private Networks (Client &amp; Server)</b> or <b>Internet (Client &amp; Server)</b> capability in order to accept connections using the
<a href="http://msdn.microsoft.com/library/windows/apps/br226906"><b>StreamSocketListener</b></a> object. The
<b>Private Networks (Client &amp; Server)</b> capability is represented by the <b>
Capability name = &quot;privateNetworkClientServer&quot;</b> tag in the app manifest. The <b>
Internet (Client &amp; Server)</b> capability is represented by the <b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest.
</p>
</li><li>
<p>If the sample is modified to connect to the server component running on a different device on the Internet (a more typical app), the
<b>Internet (Client)</b> capability must be set for the client component. This is represented by the
<b>Capability name = &quot;internetClient&quot;</b>. The <b>Internet (Client &amp; Server)</b> capability must be set for the server component. This is represented by the
<b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest. </p>
</li></ul>
<p>To build the Windows Phone version of the sample, set the following network capabilities:</p>
<ul>
<li>
<p><b>Internet (Client &amp; Server):</b> This sample has complete access to the network for both client operations (outbound-initiated access) and server operations (inbound-initiated access). This allows the app to accept connections using the
<a href="http://msdn.microsoft.com/library/windows/apps/br226906"><b>StreamSocketListener</b></a> object from the Internet or from a local intranet. This is represented by the
<b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;On Windows Phone, there is only one network capability which enables all network access for the app.</p>
<p></p>
</li></ul>
<p></p>
<p>For more information on network capabilities, see <a href="http://msdn.microsoft.com/library/windows/apps/hh770532">
How to set network capabilities</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;Network communications using an IP loopback address cannot normally be used for interprocess communication between a Windows Runtime app and a different process (a different Windows Runtime app or a desktop app) because this is
 restricted by network isolation. Network communication using an IP loopback address is allowed within the same process for communication purposes in a Windows Runtime app. For more information, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh770532">How to set network capabilities</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other - C#/VB/C&#43;&#43; and XAML</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452751">Adding support for networking (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452976">Connecting to network services (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj150597">How to secure socket connections with TLS/SSL (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj150599">How to send and receive network data with a stream socket (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj835817">How to set network capabilities (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj150598">How to use advanced socket controls (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465221">Proximity and tapping (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj662741">Staying connected in the background (XAML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770534">Troubleshooting and debugging network connections</a>
</dt><dt><b>Other - JavaScript and HTML</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452752">Adding support for networking (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452977">Connecting to network services (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh780595">How to secure socket connections with TLS/SSL (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452996">How to send and receive network data with a stream socket (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh771189">How to set background connectivity options (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770532">How to set network capabilities (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh780596">How to use advanced socket controls (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465229">Supporting proximity and tapping (HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770534">Troubleshooting and debugging network connections</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226882"><b>StreamSocket</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226906"><b>StreamSocketListener</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br207124"><b>Windows.Networking</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br226960"><b>Windows.Networking.Sockets</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208119"><b>Windows.Storage.Streams.DataReader</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208154"><b>Windows.Storage.Streams.DataWriter</b></a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=258328">DatagramSocket sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=243039">ControlChannelTrigger TCP socket sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt></dl>
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
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>StreamSocket.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build StreamSocket.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>StreamSocket.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build StreamSocket.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>StreamSocket.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy StreamSocket.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>StreamSocket.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy StreamSocket.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the Windows version of the sample</b></p>
<p>For the app to attempt a socket connection using SSL to a web server at port 443 (HTTPS) and view the server certificate, this sample requires that a web server is available that supports HTTPS. The web server must be started before the app is run. The sample
 includes a PowerShell script that will install and enable IIS on a local computer, generate a self-signed, untrusted certificate, and enable HTTPS connections. The easiest way to run the sample is to use the provided web server scripts.
</p>
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
<li>Copy the <i>Server</i> folder to the device where IIS will be run. </li><li>Run the above scripts to install and enable IIS, generate a self-signed certificate, and enable HTTPS connections.
</li></ul>
<p></p>
<p>The sample must also be updated when run against a non-localhost web server. To configure the sample for use with IIS on a different device:
</p>
<ul>
<li>Additional capabilities may need to be added to the app manifest for the sample. For example,
<b>Internet (Client &amp; Server)</b> if the web server is located on the Internet not on a local intranet.
</li><li>The hostname of the server to connect to also needs to be updated. This can be handled in two ways. The
<b>HostNameForConnect</b> element in the HTML or XAML files can be edited so that &quot;localhost&quot; is replaced by the hostname or IP address of the web server. Alternately when the app is run, enter the hostname or IP address of the web server instead of the default
 &quot;localhost&quot; value. </li></ul>
<p></p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;IIS is not available on ARM builds. Instead, set up the web server on a separate 64-bit or 32-bit computer and follow the steps for using the sample against non-localhost web server.
</p>
<p></p>
<p>However if a server different than IIS is used, then this requires some special configuration of the server.
</p>
<ul>
<li>Configure the web server to accept HTTPS connections. </li><li>Generate a self-signed certificate for the web server with SN=www.fabrikam.com.
</li></ul>
<p></p>
<p>To configure the sample for use with a web server different than IIS not using localhost:</p>
<ul>
<li>Additional capabilities may need to be added to the app manifest for the sample. For example,
<b>Internet (Client &amp; Server)</b> if the web server is located on the Internet not on a local intranet.
</li><li>The hostname of the server to connect to also needs to be updated. This can be handled in two ways. The
<b>HostNameForConnect</b> element in the HTML or XAML files can be edited so that &quot;localhost&quot; is replaced by the hostname or IP address of the web server. Alternately when the app is run, enter the hostname or IP address of the web server instead of the default
 &quot;localhost&quot; value. </li></ul>
<p></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>StreamSocket.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
<p><b>Deploying and running the Windows Phone version of the sample</b></p>
<p>IIS is not available on Windows Phone. For the app to attempt a socket connection using SSL to a web server, there are two options:</p>
<ul>
<li>The easiest way to run the sample is to use the provided web server scripts on a separate 64-bit or 32-bit device that can run IIS. Follow the instructions for deploying and running the Windows version of the sample using IIS on a different device.
</li><li>Use a web server different than IIS on a separate device. Follow the instructions for deploying and running the Windows version of the sample using a non-IIS web server.
</li></ul>
<p></p>
<ul>
<li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>StreamSocket.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
