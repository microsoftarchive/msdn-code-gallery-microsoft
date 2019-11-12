# Scaling according to DPI sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- HTML5
## Topics
- Controls
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample describes how to build an app that scales according to the dots per inch (dpi) (pixel density) of the screen by loading images of the right scale or by overriding default scaling. This sample uses the
<a href="http://msdn.microsoft.com/library/windows/apps/br226166"><b>Windows.Graphics.Display</b></a> API.
</p>
<p></p>
<p>The sample demonstrates these scenarios:</p>
<p></p>
<dl><dt><a id="Loading_images_for_different__scales"></a><a id="loading_images_for_different__scales"></a><a id="LOADING_IMAGES_FOR_DIFFERENT__SCALES"></a><b>Loading images for different dpi scales</b>
</dt><dd>
<p>When a screen’s pixel density (dpi) and resolution are very high, Windows scales images and other UI elements to maintain physical sizing across devices. We recommend that you make your app scaling aware by providing multiple versions of these assets so
 that they retain quality across different scale factors. If you don’t provide multiple versions, Windows will stretch your assets by default.</p>
</dd><dt><a id="Overriding_default_scaling_of_UI_elements"></a><a id="overriding_default_scaling_of_ui_elements"></a><a id="OVERRIDING_DEFAULT_SCALING_OF_UI_ELEMENTS"></a><b>Overriding default scaling of UI elements</b>
</dt><dd>
<p>To preserve the physical size of UI, Windows automatically scales UI elements as the scale factor (<a href="http://msdn.microsoft.com/library/windows/apps/br226165"><b>ResolutionScale</b></a>) changes. You might not want this behavior especially if your
 app doesn’t have a high-res version of an element available. This scenario demonstrates how to override the automatic scaling of text and UI as the scale factor changes from 100% to 140%. You might want to use this scenario if you don’t want Windows to automatically
 scale your images or text.</p>
</dd></dl>
<p>Important APIs in this sample include:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/hh466035"><b>onresize</b></a> event
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn264258"><b>DisplayInformation</b></a> class
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br226165"><b>ResolutionScale</b></a> enumeration
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
<li>Start Visual Studio&nbsp;2013 and select <b>File &gt; Open &gt; Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.
</li><li>Press F7 or use <b>Build &gt; Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p class="note"><b>Important</b>&nbsp;&nbsp;To see the effects of different screen dpi on the sample, you must run it using the simulator that is provided by Visual Studio Express&nbsp;2013 for Windows.
</p>
<p>Before running the sample you must go to the <b>Debug</b> tab of the <b>ScalingSample</b> project's
<b>Properties</b> dialog box and change the <b>Target device</b> to <b>Simulator</b> (by default the target device is the local machine). After building the sample and setting it to run using the simulator, press F5 (run with debugging enabled) or Ctrl&#43;F5 (run
 without debugging enabled). (Or select the corresponding options from the <b>Debug</b> menu.)</p>
<p>While the sample is running in the simulator, you can change the size and dpi of the screen to see the effects of the scale factor applied for different dpi.</p>
<p class="note"><b>Tip</b>&nbsp;&nbsp;To close the simulator you must right-click the simulator icon on your task bar and click
<b>Exit</b>.</p>
</div>
