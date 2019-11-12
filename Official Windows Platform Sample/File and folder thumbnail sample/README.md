# File and folder thumbnail sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- User Interface
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows how to retrieve thumbnails for files and folders. It uses <a href="http://msdn.microsoft.com/library/windows/apps/br207831">
<b>Windows.Storage.FileProperties</b></a> API. </p>
<p>The sample demonstrates these tasks:</p>
<ol>
<li>
<p><b>Retrieve a thumbnail for a picture</b></p>
</li><li>
<p><b>Retrieve album art as the thumbnail for a song</b></p>
</li><li>
<p><b>Retrieve an icon as the thumbnail for a document</b></p>
</li><li>
<p><b>Retrieve a thumbnail for a folder in the file system</b> </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;You can't retrieve a thumbnail for the Pictures library itself because it is a virtual folder. You must choose a file system folder that has pictures in it in order to retrieve a thumbnail.
</p>
</li><li>
<p><b>Retrieve a thumbnail for a file group</b></p>
<p>A file group is a virtual folder where all the files in the group have the criteria that you specify in common. For example, the sample shows a thumbnail for a file group wherein the files in the group all have the same month and year.</p>
</li></ol>
<p>To learn about retrieving the appropriate thumbnail to display to the user, see
<a href="http://msdn.microsoft.com/library/windows/apps/hh465350">Guidelines and checklist for thumbnails</a>.</p>
<p>Important APIs in this sample include:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/br207802"><b>StorageItemThumbnail</b></a> class
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br207809"><b>ThumbnailMode</b></a> enumeration
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br227171"><b>StorageFile</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br227210"><b>GetThumbnailAsync</b></a> methods
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br227230"><b>StorageFolder</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/br227288"><b>GetThumbnailAsync</b></a> methods
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh701614"><b>IStorageItemProperties</b></a>.<a href="http://msdn.microsoft.com/library/windows/apps/hh701636"><b>GetThumbnailAsync</b></a> methods
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
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Related samples</b> </dt><dt><a href=" http://go.microsoft.com/fwlink/p/?linkid=231445">File access sample</a>
</dt><dt><a href=" http://go.microsoft.com/fwlink/p/?linkid=231464">File picker sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231512">Folder enumeration sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231532">Programmatic file search sample</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br227346"><b>Windows.Storage namespace</b></a>,
<a href="http://msdn.microsoft.com/library/windows/apps/br207831"><b>Windows.Storage.FileProperties namespace</b></a>
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
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File &gt; Open &gt; Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</li><li>Press F6 or use <b>Build &gt; Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
