# Advanced Camera Sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows Runtime
- Windows Phone 8.1
## Topics
- camera
- Focus
- media capture
- variable photo sequence
- low lag photo sequence
- camera flash
## Updated
- 10/22/2014
## Description

<div id="mainSection">
<div id="mainSection">
<div class="clsServerSDKContent">
<h1><a id="gallery_samples.advancedcamera_gallery"></a>Advanced Camera Sample</h1>
</div>
<p>This sample is an advanced example camera app that implements several features of the
<a href="file:///C:/devdocs/gallery_phone/w_media_capture/mediacapture.xml"><strong>MediaCapture</strong></a> API.</p>
<p>This sample implements the following features of a typical camera app.</p>
<ul>
<li>Initializing and cleaning up capture resources. </li><li>Capturing still images. </li><li>Variable photo sequence capture and individual frame settings. </li><li>Adjusting focus, flash, and exposure settings. </li></ul>
<p>This sample covers many of the same areas as the <a href="http://go.microsoft.com/fwlink/p/?linkid=241428">
Media capture sample</a>, with a few important differences. This sample includes features, like variable photo sequence capture, which were introduced in Windows Phone 8.1 and are not currently available for Windows apps. Also, instead of being organized as
 a code sample, this app is designed using the best practices for building a high-quality camera app, including a heavy reliance on the ModelViewViewModel (MVVM) design pattern. If you are interested in building a fully functional camera app - as opposed to
 just adding minor camera capabilities to a more general app - it is recommended that you use this sample as the starting point for your app development.</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;</p>
<p class="note">Variable photo sequence capture is not supported on all hardware devices, including all Windows&nbsp;Phone&nbsp;8 devices. This sample will show a warning message when switching to variable photo sequence mode on unsupported devices. Variable
 photo sequence capture is supported on the Windows Phone emulator.</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;This sample requires Windows&nbsp;8.1 and Microsoft Visual Studio&nbsp;2013 with Update 2 or later.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013 , go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. After you install Visual Studio&nbsp;2013, update your installation with Update 2 or later.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="file:///C:/devdocs/gallery_phone/m_media_mca/quickstart__recording_the_screen_with_screenrecorder.xml">Quickstart: recording the screen with ScreenCapture</a>
</dt></dl>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>None supported </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>None supported </dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Visual Studio Express&nbsp;2013 for Windows --&gt; and select <strong>File</strong> &gt;
<strong>Open</strong> &gt; <strong>Project/Solution</strong>. </li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio Express&nbsp;2013 for Windows Solution (.sln) file.
</li><li>Press F7 or use <strong>Build</strong> &gt; <strong>Build Solution</strong> to build the sample.
</li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <strong>Debug</strong> &gt; <strong>
Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use <strong>
Debug</strong> &gt; <strong>Start Without Debugging</strong>.</p>
</div>
</div>
