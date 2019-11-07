# File picker sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Storage
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows how to access files and folders by letting the user choose them through the file pickers and how to save a file so that the user can specify the name, file type, and location of a file to save. This sample uses
<a href="http://msdn.microsoft.com/library/windows/apps/br207928"><b>Windows.Storage.Pickers</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br227346"><b>Windows.Storage</b></a> API.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample demonstrates these tasks:</p>
<ol>
<li>
<p><b>Let the user pick one file to access</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br207847"><b>FileOpenPicker</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/jj635275"><b>PickSingleFileAsync</b></a> method to call a file picker window and let the user pick
 a single file. For a walkthrough of this task, see <a href="http://msdn.microsoft.com/library/windows/apps/hh465199">
Quickstart: accessing files with file pickers</a>.</p>
</li><li>
<p><b>Let the user pick multiple files to access</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br207847"><b>FileOpenPicker</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br207851"><b>PickMultipleFilesAsync</b></a> method to call a file picker window and let the user
 pick multiple files. For a walkthrough of this task, see <a href="http://msdn.microsoft.com/library/windows/apps/hh465199">
Quickstart: accessing files with file pickers</a>.</p>
</li><li>
<p><b>Let the user pick one folder to access</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br207881"><b>FolderPicker</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br207885"><b>PickSingleFolderAsync</b></a> method to call a file picker window and let the user pick
 multiple files. For a walkthrough of this task, see <a href="http://msdn.microsoft.com/library/windows/apps/hh465199">
Quickstart: accessing files with file pickers</a>.</p>
</li><li>
<p><b>Let the user save a file and specify the name, file type, and/or save location</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br207871"><b>FileSavePicker</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br207876"><b>PickSaveFileAsync</b></a> method to call a file picker window and let the user pick
 multiple files. For a walkthrough of this task, see <a href="http://msdn.microsoft.com/library/windows/apps/jj150595">
How to save files through file pickers</a>.</p>
</li><li>
<p><b>Let the user pick a locally cached file to access</b></p>
<p>The user may choose access a file that is provided by another app (the providing app) that participates in the Cached File Updater contract. Like the first scenario, this scenario uses the
<a href="http://msdn.microsoft.com/library/windows/apps/br207847"><b>FileOpenPicker</b></a> to access these files and to show how the providing app (the
<a href="http://go.microsoft.com/fwlink/p/?linkid=231536">File picker contracts sample</a>) can interact with the user through the file picker if necessary, for example if credentials are required to access the file.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This scenario requires the <a href="http://go.microsoft.com/fwlink/p/?linkid=231536">
File picker contracts sample</a>.</p>
</li><li>
<p><b>Let the user save a locally cached file</b></p>
<p>The user may choose save a file that was provided by another app (the providing app) that participates in the Cached File Updater contract. This scenario uses the
<a href="http://msdn.microsoft.com/library/windows/apps/br207871"><b>FileSavePicker</b></a> and the
<a href="http://msdn.microsoft.com/library/windows/apps/hh701431"><b>CachedFileManager</b></a> to let the user save a file to another app (the
<a href="http://go.microsoft.com/fwlink/p/?linkid=231536">File picker contracts sample</a>) and how the providing app can interact with the user through the file picker if necessary, for example if there is a version conflict.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This scenario requires the <a href="http://go.microsoft.com/fwlink/p/?linkid=231536">
File picker contracts sample</a>.</p>
</li></ol>
<p>To learn more about accessing and saving files and folders through file pickers, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh465182">Guidelines and checklist for file pickers</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Related samples</b> </dt><dt><a href=" http://go.microsoft.com/fwlink/p/?linkid=231445">File access sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231615">Using a Blob to save and load content sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231522">File and folder thumbnail sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231536">File picker contracts sample</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br207928"><b>Windows.Storage.Pickers namespace</b></a>
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
<li>Select <b>FilePicker.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build FilePicker.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>FilePicker.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build FilePicker.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>FilePicker.Windows</b> in <b>Solution Explorer</b> and select <b>
Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>FilePicker.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
