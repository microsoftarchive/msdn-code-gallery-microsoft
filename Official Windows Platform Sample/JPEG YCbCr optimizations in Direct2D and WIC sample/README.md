# JPEG YCbCr optimizations in Direct2D and WIC sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- DirectX
- Windows Runtime
## Topics
- YCbCr effect
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>Demonstrates performance optimizations by in Direct2D and the Windows Imaging Component rendering image data in the JPEG YCbCr format natively.
</p>
<p>Starting in Windows&nbsp;8.1, Direct2D provides support for rendering image data in the JPEG YCbCr format so that apps can render JPEG content in its native YCbCr representation instead of decompressing to BGRA. This can significantly reduce graphics memory consumption
 and resource creation time. This sample demonstrates how to efficiently decode and render YCbCr encoded JPEG images by keeping the image data in YCbCr form instead of converting to BGRA.
</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn280624">YCbCr Effect (Direct2D)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn302093"><b>IWICPlanarBitmapSourceTransform</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8.1 app samples</a>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
