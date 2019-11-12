# XAML WebView control sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Controls
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br227702">
<b>WebView</b></a> control. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample covers:</p>
<ul>
<li>Navigating a <a href="http://msdn.microsoft.com/library/windows/apps/br227702">
<b>WebView</b></a> to a specific URL by calling the <a href="http://msdn.microsoft.com/library/windows/apps/br227710">
<b>Navigate</b></a> method. </li><li>Loading your own HTML into a <a href="http://msdn.microsoft.com/library/windows/apps/br227702">
<b>WebView</b></a> by calling <a href="http://msdn.microsoft.com/library/windows/apps/br227711">
<b>NavigateToString</b></a>. </li><li>Loading HTML from the app package or from the local or temporary folders using the
<b>ms-appx-web</b> and <b>ms-appdata</b> schemes. </li><li>Loading HTML and support files (such as CSS, script, and images) by calling <a href="http://msdn.microsoft.com/library/windows/apps/dn299344">
<b>NavigateToLocalStreamUri</b></a>. </li><li>Invoking JavaScript functions in <a href="http://msdn.microsoft.com/library/windows/apps/br227702">
<b>WebView</b></a>-hosted content from your app code by calling the <a href="http://msdn.microsoft.com/library/windows/apps/dn299342">
<b>InvokeScriptAsync</b></a> method. </li><li>Receiving notifications and data in your app code sent from <a href="http://msdn.microsoft.com/library/windows/apps/br227702">
<b>WebView</b></a>-hosted script by handling the <a href="http://msdn.microsoft.com/library/windows/apps/br227713">
<b>ScriptNotify</b></a> event. </li><li>Supporting the <a href="m_ca_contracts.capabilities_and_contracts_portal#share_contract">
Share contract</a> by using the <a href="http://msdn.microsoft.com/library/windows/apps/dn299327">
<b>CaptureSelectedContentToDataPackageAsync</b></a> method to retrieve the currently-selected content in the
<a href="http://msdn.microsoft.com/library/windows/apps/br227702"><b>WebView</b></a> control. (Windows only)
</li><li>Using the <a href="http://msdn.microsoft.com/library/windows/apps/dn299326"><b>CapturePreviewToStreamAsync</b></a> method to create a thumbnail image of the
<a href="http://msdn.microsoft.com/library/windows/apps/br227702"><b>WebView</b></a> content.
</li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=243667">Windows 8.1 app samples</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn299327"><b>CaptureSelectedContentToDataPackageAsync</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn299342"><b>InvokeScriptAsync</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227710"><b>Navigate</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227711"><b>NavigateToString</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn299344"><b>NavigateToLocalStreamUri</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227713"><b>ScriptNotify</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br227702"><b>WebView</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn299326"><b>CapturePreviewToStreamAsync</b></a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br211380">Create a blog reader</a>
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
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>Controls_WebView.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build Controls_WebView.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>Controls_WebView.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build Controls_WebView.WindowsPhone</b>. </li></ol>
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
<li>Select <b>Controls_WebView.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy Controls_WebView.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>Controls_WebView.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy Controls_WebView.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>Controls_WebView.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>Controls_WebView.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
