# File access sample
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
<p>This sample shows how to create, read, write, copy and delete a file, how to retrieve file properties, and how to track a file or folder so that your app can access it again. This sample uses
<a href="http://msdn.microsoft.com/library/windows/apps/br227346"><b>Windows.Storage</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br207498"><b>Windows.Storage.AccessCache</b></a> API.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample demonstrates these tasks:</p>
<ol>
<li>
<p><b>Create a file in the Pictures library</b></p>
<p>Uses one of the <a href="http://msdn.microsoft.com/library/windows/apps/br227230">
<b>StorageFolder</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br227249"><b>CreateFileAsync</b></a> methods to create the file.</p>
</li><li>
<p><b>Get a file's parent folder</b></p>
<p>(Not supported for Windows Phone.)</p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/dn298477"><b>GetParentAsync</b></a> method to get the parent folder of the file that was created in the Picture folder. The app has the Pictures library capability, so it can access the folder
 where the file was created.</p>
</li><li>
<p><b>Write and read text in a file</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/hh701440"><b>FileIO</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/hh701505"><b>WriteTextAsync</b></a> and
<b>FileIO</b>.<a href="http://msdn.microsoft.com/library/windows/apps/hh701482"><b>ReadTextAsync</b></a> methods to write and read the file. For a walkthrough of this task, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh464978">Quickstart: reading and writing a file</a>.</p>
</li><li>
<p><b>Write and read bytes in a file</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/hh701440"><b>FileIO</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/hh701490"><b>WriteBufferAsync</b></a> and
<b>FileIO</b>.<a href="http://msdn.microsoft.com/library/windows/apps/hh701468"><b>ReadBufferAsync</b></a> methods to write and read the file. For a walkthrough of this task, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh464978">Quickstart: reading and writing a file</a>.</p>
</li><li>
<p><b>Write and read a file using a stream</b></p>
<p>Uses the following API to write and read the file using a stream.</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/br227171"><b>StorageFile</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/hh996766"><b>OpenTransactedWriteAsync</b></a> method
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br208154"><b>DataWriter</b></a> class
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br208119"><b>DataReader</b></a> class
</li></ul>
<p>For a walkthrough of this task, see <a href="http://msdn.microsoft.com/library/windows/apps/hh464978">
Quickstart: reading and writing a file</a>.</p>
</li><li>
<p><b>Display file properties</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br227171"><b>StorageFile</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/hh701737"><b>GetBasicPropertiesAsync</b></a> method and the
<b>StorageFile</b>.<a href="http://msdn.microsoft.com/library/windows/apps/br227225"><b>Properties</b></a> property to get the properties of the file.</p>
</li><li>
<p><b>Track a file or folder so that you can access it later (persisting access)</b></p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br207456"><b>StorageApplicationPermissions</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br207457"><b>FutureAccessList</b></a> and
<b>StorageApplicationPermissions</b>.<a href="http://msdn.microsoft.com/library/windows/apps/br207458"><b>MostRecentlyUsedList</b></a> properties to remember a file or folder so that it can be accessed later.</p>
<p>For a walkthrough of this task, see <a href="http://msdn.microsoft.com/library/windows/apps/hh972603">
How to track recently used files and folders</a>.</p>
</li><li>
<p><b>Copy a file</b></p>
<p>Uses one of the <a href="http://msdn.microsoft.com/library/windows/apps/br227171">
<b>StorageFile</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br227190"><b>CopyAsync</b></a> methods to copy the file.</p>
</li><li>
<p><b>Compare two files to see if they're the same</b></p>
<p>(Not supported for Windows Phone.)</p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br227171"><b>StorageFile</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/dn298484"><b>IsEqual</b></a> method to compare two files.</p>
</li><li>
<p><b>Delete a file</b></p>
<p>Uses one of the <a href="http://msdn.microsoft.com/library/windows/apps/br227171">
<b>StorageFile</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br227199"><b>DeleteAsync</b></a> methods to delete the file.</p>
</li><li>
<p><b>Try to get a file without getting an error</b></p>
<p>(Not supported for Windows Phone.)</p>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/br227230"><b>StorageFolder</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/dn251721"><b>TryGetItemAsync</b></a> method to get a file without raising an exception.</p>
</li></ol>
<p class="note"><b>Note</b>&nbsp;&nbsp;If you want to learn about accessing files using a file picker, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh771180">Quickstart: Accessing files with file pickers</a>.</p>
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
</dt><dt><b>Related samples</b> </dt><dt><a href=" http://go.microsoft.com/fwlink/p/?linkid=231464">File picker sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231512">Folder enumeration sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231532">Programmatic file search sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231522">File and folder thumbnail sample</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br227346"><b>Windows.Storage namespace</b></a>,
<a href="http://msdn.microsoft.com/library/windows/apps/br207498"><b>Windows.Storage.AccessCache namespace</b></a>,
<a href="http://msdn.microsoft.com/library/windows/apps/br207831"><b>Windows.Storage.FileProperties</b></a>,
<a href="http://msdn.microsoft.com/library/windows/apps/br241791"><b>Windows.Storage.Streams namespace</b></a>
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
<li>Select <b>FileAccess.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build FileAccess.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>FileAccess.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build FileAccess.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>FileAccess.Windows</b> in <b>Solution Explorer</b> and select <b>
Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>FileAccess.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
