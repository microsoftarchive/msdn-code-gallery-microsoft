# DirectComposition layered child window sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- DirectX
## Topics
- Audio and video
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the Microsoft DirectComposition&nbsp;API to animate the bitmap of a layered child window. It consists of a simple video player that lets you apply animated 2-D transforms to the video window as a video plays.</p>
<p>Specifically, this sample shows how to:</p>
<ul>
<li>Create layered child windows by applying the <b>WS_EX_LAYERED</b> extended window style
</li><li>Animate layered child windows and apply animated 2-D transforms (translate, rotate, skew, and scale)
</li><li>Use the window cloaking feature to hide a layered child window's &quot;real&quot; window bitmap while DirectComposition animates the visual representation of the window
</li></ul>
<p></p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013 and won't compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh437348">Animation</a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh437378">How to animate the bitmap of a layered child window</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh437371">DirectComposition</a>
<h3>Operating system requirements</h3>
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
<h3>Build the sample</h3>
<p>To build this sample:</p>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample. Go to the cpp directory and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F6 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<h3>Run the sample</h3>
<p>To run this sample after building it, use Windows Explorer to go to the installation folder for this sample and run DirectComposition_LayeredChildWindow.exe from the
<i>&lt;install_root&gt;</i>\DirectComposition layered child window sample\C&#43;&#43;\Debug folder.
</p>
<p>To run this sample from Microsoft Visual Studio, press the F5 key to run with debugging enabled, or Ctrl&#43;F5 to run without debugging enabled. Alternatively, select
<b>Start Debugging</b> or <b>Start Without Debugging</b> from the <b>Debug</b> menu.</p>
<p>To see this sample in action, you need to load a media file for the sample to play in its video window. You'll find a demo media file called vc1 in the
<i>&lt;install_root&gt;</i>\DirectComposition layered child window sample\C&#43;&#43;\media folder.
</p>
</div>
