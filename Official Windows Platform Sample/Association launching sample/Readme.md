# Association launching sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- App model
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows you how to launch the user's default app for file type or a protocol. You can also learn how to enable your app to be the default app for a file type or a protocol.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>A Windows Runtime app can't set, change, or query the default apps for file types and protocols. The operating system asks the user to select which app to use as the default for each file type and protocol.</p>
<p>The sample covers these key tasks:</p>
<ul>
<li>launching the default app for a file using <a href="http://msdn.microsoft.com/library/windows/apps/hh701461">
<b>LaunchFileAsync</b></a> </li><li>handling file activation through the <b>Activated</b> event </li><li>launching the default app for a protocol using <a href="http://msdn.microsoft.com/library/windows/apps/hh701476">
<b>LaunchUriAsync</b></a> </li><li>handling protocol activation through the <b>Activated</b> event </li></ul>
<p>The sample covers this new task for Windows&nbsp;8.1:</p>
<ul>
<li>launching a target app and having the currently running source app remain on the screen for various amounts of screen space using
<a href="http://msdn.microsoft.com/library/windows/apps/dn298314"><b>LauncherOptions.DesiredRemainingView</b></a>
<p class="note"><b>Note</b>&nbsp;&nbsp;<a href="http://msdn.microsoft.com/library/windows/apps/dn298314"><b>LauncherOptions.DesiredRemainingView</b></a> isn't supported for Windows Phone.</p>
</li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Tasks</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452684">How to handle file activation (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779669">How to handle file activation (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452686">How to handle protocol activation (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779670">How to handle protocol activation (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452687">How to launch the default app for a file (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779671">How to launch the default app for a file (C#/VB/C&#43;&#43;)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452690">How to launch the default app for a protocol (JavaScript)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh779672">How to launch the default app for a protocol (C#/VB/C&#43;&#43;)</a>
</dt><dt><b>Guidelines</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh700321">Guidelines and checklist for file types and protocols</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224716"><b>Windows.ApplicationModel.Activation.FileActivatedEventArgs</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224742"><b>Windows.ApplicationModel.Activation.ProtocolActivatedEventArgs</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701461"><b>Windows.System.Launcher.LaunchFileAsync</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701476"><b>Windows.System.Launcher.LaunchUriAsync</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701781"><b>Windows.UI.WebUI.WebUIFileActivatedEventArgs</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701885"><b>Windows.UI.WebUI.WebUIProtocolActivatedEventArgs</b></a>
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
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory containing the sample in the language you desire - either C&#43;&#43;, C#, or JavaScript. Double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Select either the Windows or Windows Phone project version of the sample. Press Ctrl&#43;Shift&#43;B, or select
<b>Build</b> &gt; <b>Build Solution</b>. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ol>
<li>Select either the Windows or Windows Phone project version of the sample. </li><li>Select <b>Build</b> &gt; <b>Deploy Solution</b>. </li></ol>
<p><b>Deploying and running the sample</b></p>
<ol>
<li>Right-click either the Windows or Windows Phone project version of the sample in
<b>Solution Explorer</b> and select <b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or select <b>Debug</b> &gt; <b>
Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or select<b>Debug</b> &gt;
<b>Start Without Debugging</b>. </li></ol>
</div>
