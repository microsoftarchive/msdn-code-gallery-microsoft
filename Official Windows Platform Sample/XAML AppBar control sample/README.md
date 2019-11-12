# XAML AppBar control sample
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
<p>This sample demonstrates how to use an app bar to present navigation, commands, and tools to users.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>On Windows, the app bar is hidden by default and appears when users swipe a finger from the bottom edge of the screen. The app bar covers the content of the app and can be dismissed by the user with an edge swipe or by interacting with the app. (The Windows
 sample demonstrates both the <a href="http://msdn.microsoft.com/library/windows/apps/hh701927">
<b>AppBar</b></a> control and the lightweight <a href="http://msdn.microsoft.com/library/windows/apps/dn279427">
<b>CommandBar</b></a> control.)</p>
<p>On Windows Phone 8.1, the app bar is visible by default and has two states: &quot;closed&quot; and &quot;opened&quot;. When in the closed state, an app bar can either be &quot;minimal&quot; or &quot;compact&quot;. The app bar goes to an opened state when the user taps the ellipsis on the app bar.
 (The Windows Phone 8.1 sample demonstrates the lightweight <a href="http://msdn.microsoft.com/library/windows/apps/dn279427">
<b>CommandBar</b></a> control.) </p>
<p>This sample shows how to add an app bar, customize the app bar, and control the app bar.</p>
<p>This sample is written in Extensible Application Markup Language (XAML). For the HTML version, see the
<a href="http://go.microsoft.com/fwlink/p/?linkid=242389">HTML AppBar control sample</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8.1 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkID=393547">
Windows&nbsp;8.1 app samples pack</a>. The samples in the Windows&nbsp;8.1 app samples pack will build and run only on
<a href="http://go.microsoft.com/fwlink/p/?linkid=301697">Visual Studio&nbsp;2013</a>.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701927"><b>AppBar</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh781230">Adding app bars</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh781231">Guidelines and checklist for app bars</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=242389">HTML AppBar control sample</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh781232">QuickStart: Adding app bars</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
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
<li>Select <b>AppBarControl.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build AppBarControl.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>AppBarControl.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build AppBarControl.WindowsPhone</b>. </li></ol>
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
<li>Select <b>AppBarControl.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy AppBarControl.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>AppBarControl.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy AppBarControl.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>AppBarControl.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>AppBarControl.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
