# Sharing content source app sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Data Access
## Updated
- 05/13/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how an app shares content with another app. This sample uses classes from the
<a href="http://msdn.microsoft.com/library/windows/apps/br205967"><b>Windows.ApplicationModel.DataTransfer</b></a> namespace. Some of the classes you might want to review in more detail are the
<a href="http://msdn.microsoft.com/library/windows/apps/br205932"><b>DataTransferManager</b></a> class, which you use to initiate a share operation, and the
<a href="http://msdn.microsoft.com/library/windows/apps/br205873"><b>DataPackage</b></a> class, which you use to package the content. Because each share scenario usually involves two apps—the source app and a target app that receives the content—we recommend
 you install and deploy the <a href="http://go.microsoft.com/fwlink/p/?linkid=231519">
Sharing content target app sample</a> when you install and run this one. This way, you can see how sharing works from end to end.
</p>
<p>This sample covers how to share content in a variety of formats, including:</p>
<ul>
<li>Text </li><li>Web link </li><li>Application link </li><li>Images </li><li>Files </li><li>Delay-rendered files </li><li>HTML content </li><li>Custom data </li></ul>
<p></p>
<p>Also, this sample shows how to share a 30x30 logo and specify its background color, and how to share the app's package family name.</p>
<p>Also, this sample shows how to display text when your app can't support a share operation.</p>
<p>To learn more about sharing and the <a href="http://msdn.microsoft.com/library/windows/apps/br205967">
<b>Windows.ApplicationModel.DataTransfer</b></a> namespace, we recommend you take a look at the
<a href="http://msdn.microsoft.com/library/windows/apps/hh464923">Sharing and exchanging data</a> section of our documentation, which describes how sharing works and contains several how-to topics that cover how to share
<a href="http://msdn.microsoft.com/library/windows/apps/hh758313">text</a>, an <a href="http://msdn.microsoft.com/library/windows/apps/hh758305">
image</a>, files, and other formats. Our <a href="http://msdn.microsoft.com/library/windows/apps/hh465251">
Guidelines for sharing content</a> can also help you create a great user experience with the share feature.
</p>
<p>For more info about the concepts and APIs demonstrated in this sample, see these topics:</p>
<ul>
<li><a href="http://go.microsoft.com/fwlink/p/?linkid=231519">Sharing content target app sample</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh464923">Sharing and exchanging data</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh758308">How to share files (HTML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh871371">How to share files (XAML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh758310">How to share HTML (HTML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh973055">How to share HTML (XAML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh758313">How to share text (HTML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh871372">How to share text (XAML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh465261">Quickstart: Sharing content (HTML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh871368">Quickstart: Sharing content (XAML)</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh738408"><b>DataPackageView</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br205977"><b>ShareOperation</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br205967"><b>Windows.ApplicationModel.DataTransfer</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br205989"><b>Windows.ApplicationModel.DataTransfer.Share</b></a>
</li></ul>
<p></p>
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
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
