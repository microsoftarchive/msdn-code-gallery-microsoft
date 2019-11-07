# XAML ListView and GridView essentials sample
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
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br242705">
<b>GridView</b></a> and <a href="http://msdn.microsoft.com/library/windows/apps/br242878">
<b>ListView</b></a> controls for Windows Runtime apps on Windows and Windows Phone. For Windows, it includes performance enhancements introduced in Windows&nbsp;8.1.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample covers:</p>
<ul>
<li>Instantiating a <a href="http://msdn.microsoft.com/library/windows/apps/br242705">
<b>GridView</b></a>, setting its data source, and displaying data items using the
<a href="http://msdn.microsoft.com/library/windows/apps/dn298914"><b>ContainerContentChanging</b></a> event.
</li><li>Responding to <a href="http://msdn.microsoft.com/library/windows/apps/br242904">
<b>ItemClick</b></a> events. </li><li>Instantiating a <a href="http://msdn.microsoft.com/library/windows/apps/br242878">
<b>ListView</b></a>. </li><li>Retemplating <a href="http://msdn.microsoft.com/library/windows/apps/hh738501">
<b>GridViewItem</b></a> instances while retaining the performance improvements of
<a href="http://msdn.microsoft.com/library/windows/apps/dn279298"><b>GridViewItemPresenter</b></a>. (Windows only)
</li><li>Retemplating <a href="http://msdn.microsoft.com/library/windows/apps/br242919">
<b>ListViewItem</b></a> instances while retaining the performance improvements of
<a href="http://msdn.microsoft.com/library/windows/apps/dn298500"><b>ListViewItemPresenter</b></a>. (Windows only)
</li><li>How to efficiently create a custom item container that doesn't use <a href="http://msdn.microsoft.com/library/windows/apps/dn298500">
<b>ListViewItemPresenter</b></a> or <a href="http://msdn.microsoft.com/library/windows/apps/dn279298">
<b>GridViewItemPresenter</b></a>. (Windows only) </li></ul>
<p></p>
<p>This sample is written in XAML. For the HTML version, see the <a href="http://go.microsoft.com/fwlink/p/?linkid=242398">
ListView basic usage sample (HTML)</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=226564">XAML GridView grouping and SemanticZoom sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=227147">XAML ListView and GridView customizing interactivity sample</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242705"><b>GridView</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242878"><b>ListView</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn279298"><b>GridViewItemPresenter</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298500"><b>ListViewItemPresenter</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242830"><b>ItemTemplate</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242817"><b>ItemContainerStyle</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242826"><b>ItemsPanel</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242828"><b>ItemsSource</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242904"><b>ItemClick</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298914"><b>ContainerContentChanging</b></a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh780650">QuickStart: adding ListView and GridView controls</a>
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
<li>Select <b>ListViewSimple.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build ListViewSimple.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>ListViewSimple.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build ListViewSimple.WindowsPhone</b>. </li></ol>
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
<li>Select <b>ListViewSimple.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy ListViewSimple.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>ListViewSimple.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy ListViewSimple.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>ListViewSimple.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>ListViewSimple.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
