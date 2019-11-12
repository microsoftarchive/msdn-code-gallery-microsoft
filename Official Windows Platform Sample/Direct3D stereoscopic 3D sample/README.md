# Direct3D stereoscopic 3D sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- DirectX
## Topics
- Audio and video
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to add a stereoscopic 3-D effect to C&#43;&#43; apps by using Direct3D. It also demonstrates how to respond to system stereo changes in Direct3D.
</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The stereoscopic 3-D effect requires a display that supports stereo 3-D.</p>
<p></p>
<p>You can use the code in this sample to add stereoscopic 3-D effects to your Direct3D 11.1 app. It shows the best practices and usage of stereo 3-D support APIs that Direct3D 11.1 supports for apps.</p>
<p>The process demonstrated in this sample is as follows:</p>
<ul>
<li>Call <a href="http://msdn.microsoft.com/library/windows/apps/hh404561"><b>IDXGIFactory2::IsWindowedStereoEnabled</b></a> to verify that the computer on which the app runs supports stereo 3-D.
</li><li>Use the following code to register for <a href="http://msdn.microsoft.com/library/windows/apps/br226155">
<b>StereoEnabledChanged</b></a> notifications whenever the stereo 3D support on the computer changes. The
<a href="http://msdn.microsoft.com/library/windows/apps/br226144"><b>DisplayPropertiesEventHandler</b></a> delegate handles these notifications. These are Windows Runtime APIs from the
<a href="http://msdn.microsoft.com/library/windows/apps/br226166"><b>Windows.Graphics.Display</b></a> namespace.
<div class="code"><span>
<table>
<tbody>
<tr>
<th></th>
</tr>
<tr>
<td>
<pre>
DisplayProperties::StereoEnabledChanged &#43;=
     ref new DisplayPropertiesEventHandler(this, &amp;View::OnStereoEnableChanged);</pre>
</td>
</tr>
</tbody>
</table>
</span></div>
</li><li>Create the device with the <a href="direct3d11.overviews_direct3d_11_devices_downlevel_intro#Overview">
feature level</a> set at least at the minimum requirement for stereo 3-D support, that is, a Direct3D 11.1 device with feature level 10.0 or higher.
</li><li>Create stereo 3-D supported swap chains on which to render stereo content for left and right eye views.
</li><li>Create and set stereo projection matrices for rendering stereo content. </li><li>Set up rendering state and pipeline for rendering stereo content; that is, left and right render-target views.
</li><li>Render Direct2D content on the stereo 3-D swap chain. That is, obtain the back buffer of a swap chain as a DXGI resource and call the
<a href="http://msdn.microsoft.com/library/windows/apps/hh404627"><b>IDXGIResource1::CreateSubresourceSurface</b></a> API to obtain the left and right views.
</li></ul>
<p>For more info about the concepts and APIs demonstrated in this sample, see these topics:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/br229580">Create an app using DirectX</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ff476080">Direct3D 11 graphics</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dd370987">Direct2D graphics</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/bb205169">DXGI reference</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/bb509561">DirectX HLSL</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780567">UnderstandingDirectX game development</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh452744">Developing games</a>
</li></ul>
<p></p>
<p>This sample is written in C&#43;&#43; and requires some experience with graphics programming and DirectX.</p>
<p><b>Requirements</b></p>
<p>In addition to the minimum supported operating systems being Windows&nbsp;8.1 and Windows Server&nbsp;2012&nbsp;R2, a WDDM 1.2 display driver must be installed on the computer, and the graphics hardware must support
<a href="direct3d11.overviews_direct3d_11_devices_downlevel_intro#Overview">feature level</a> 10 or higher.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
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
<p>To debug the app and then run it, press F5 or use <b>Debug &gt; Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug &gt; Start Without Debugging</b>.</p>
<p>You can adjust the stereoscopic visual effect by pressing the 'up' or 'down' arrow keys.</p>
</div>
