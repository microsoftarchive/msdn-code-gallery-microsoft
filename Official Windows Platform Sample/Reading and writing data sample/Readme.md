# Reading and writing data sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- HTML5
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Data Access
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows how to use the <a href="http://msdn.microsoft.com/library/windows/apps/br208119">
<b>DataReader</b></a> and <a href="http://msdn.microsoft.com/library/windows/apps/br208154">
<b>DataWriter</b></a> classes to store and retrieve data. </p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p></p>
<p>This sample demonstrates the following scenarios:</p>
<ul>
<li>Creating a memory-backed stream by using the <a href="http://msdn.microsoft.com/library/windows/apps/br241720">
<b>InMemoryRandomAccessStream</b></a> class and storing strings by using a <a href="http://msdn.microsoft.com/library/windows/apps/br208154">
<b>DataWriter</b></a> object. When the write operation completes, a <a href="http://msdn.microsoft.com/library/windows/apps/br208119">
<b>DataReader</b></a> object extracts the stored strings from the stream and displays them.
</li><li>Opening a sequential-access stream over an image by using the <a href="http://msdn.microsoft.com/library/windows/apps/hh701853">
<b>OpenSequentialReadAsync</b></a> and <a href="http://msdn.microsoft.com/library/windows/apps/br208139">
<b>ReadBytes</b></a> methods to retrieve and display its binary data. </li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1 </a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013 </a>. </p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208119"><b>DataReader</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br208154"><b>DataWriter</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh701853"><b>OpenSequentialReadAsync</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241720"><b>InMemoryRandomAccessStream</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br241791"><b>Streams</b></a>
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
<li>Select <b>DataReaderWriter.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build DataReaderWriter.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>DataReaderWriter.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build DataReaderWriter.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>DataReaderWriter.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>DataReaderWriter.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
