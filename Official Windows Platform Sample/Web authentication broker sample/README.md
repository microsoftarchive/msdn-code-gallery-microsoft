# Web authentication broker sample
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
- 04/23/2014
## Description

<div id="mainSection">
<p>This sample shows how you can use the <a href="http://msdn.microsoft.com/library/windows/apps/br227025">
<b>WebAuthenticationBroker</b></a> class to connect to OAuth providers such as Facebook, Flickr, Google, and Twitter.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>This sample shows you how to:</p>
<ul>
<li>Connect to Facebook using the OAuth 2.0 protocol for authentication and authorization.
</li><li>Connect to Twitter using the OAuth protocol for authentication and authorization.
</li><li>Connect to Flickr using the OAuth protocol for authentication and authorization.
</li><li>Use Account Controls to manage account settings. </li><li>Use <a href="http://msdn.microsoft.com/library/windows/apps/dn298639"><b>Windows.Web.Http.HttpClient</b></a> and classes in the
<a href="http://msdn.microsoft.com/library/windows/apps/dn298623"><b>Windows.Web.Http.Filters</b></a> namespace to use filters to connect with OAuth providers to make development easier.
</li></ul>
<p></p>
<p><b>Network capabilities</b></p>
<p>This sample requires that network capabilities be set in the <i>Package.appxmanifest</i> file to allow the app to access the network at runtime. These capabilities can be set in the app manifest using Microsoft Visual Studio.
</p>
<p>To build the Windows version of the sample:</p>
<ul>
<li>
<p><b>Internet (Client):</b> The sample can use the Internet for client operations (outbound-initiated access). This allows the app to authenticate with servers on the Internet. This is represented by the
<b>Capability name = &quot;internetClient&quot;</b> tag in the app manifest. </p>
</li><li>
<p>If the sample is used to connect and authenticate to a server located on a home or work network (a local intranet), the
<b>Private Networks (Client &amp; Server)</b> capability must be set. This is represented by the
<b>Capability name = &quot;privateNetworkClientServer&quot;</b> tag in the app manifest. </p>
</li></ul>
<p>To build the Windows Phone version of the sample:</p>
<ul>
<li>
<p><b>Internet (Client &amp; Server):</b> This sample has complete access to the network for both client operations (outbound-initiated access) and server operations (inbound-initiated access). This allows the app to connect and authenticate to a server on
 the Internet or on a local intranet. This is represented by the <b>Capability name = &quot;internetClientServer&quot;</b> tag in the app manifest.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;On Windows Phone, there is only one network capability which enables all network access for the app.</p>
<p></p>
</li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Other resources</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/">Web Authentication Broker</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh770550">Quickstart: Connecting using XML HTTP Request (IXHR2)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465283">Setting up single sign-on using the web authentication broker (C#/C&#43;&#43;/VB)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465281">Setting up single sign-on using the web authentication broker (JavaScript)</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn279122"><b>WebAccount</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298413"><b>WebAccountCommand</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn279123"><b>WebAccountProvider</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227025"><b>WebAuthenticationBroker</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227044"><b>Windows.Security.Authentication.Web</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn279692"><b>Windows.Web.Http</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298623"><b>Windows.Web.Http.Filters</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298639"><b>Windows.Web.Http.HttpClient</b></a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 Windows Store app samples</a>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>WebAuthentication.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build WebAuthentication.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>WebAuthentication.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build WebAuthentication.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>WebAuthentication.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy WebAuthentication.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>WebAuthentication.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy WebAuthentication.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>WebAuthentication.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>WebAuthentication.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
