# XAML DirectX 3D shooting game sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- DirectX
- Windows 8.1
- Windows Phone 8.1
## Topics
- Audio and video
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates the basic end-to-end implementation of a simple first person 3-D game using DirectX (Direct3D 11.1, Direct2D, XInput, and XAudio2) and XAML in a C&#43;&#43; app. XAML is used for the heads-up display and game state messages.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample supports:</p>
<ul>
<li>A touch input model </li><li>A mouse and keyboard input model </li><li>A game controller input model </li><li>Stereoscopic 3-D display </li><li>A XAML heads-up display </li><li>A persistent game state </li><li>Sound effect playback </li></ul>
<p></p>
<p>This sample is written in C&#43;&#43; and requires some experience with graphics programming. A code walkthrough for the Direct2D-specific version of this sample is available at the following links:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/hh780567">UnderstandingDirectX game development</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780560">Windows 8 DirectX shooting game sample: about the game sample</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780566">Windows 8 DirectX shooting game sample: defining the game's app framework</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780568">Windows 8 DirectX shooting game sample: defining the main game object</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780565">Windows 8 DirectX shooting game sample: assembling the rendering framework</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780562">Windows 8 DirectX shooting game sample: adding a user interface</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780563">Windows 8 DirectX shooting game sample: adding controls</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780564">Windows 8 DirectX shooting game sample: adding sound</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh780561">Windows 8 DirectX shooting game sample: extending the game sample</a>
</li></ul>
These topics provide info about the APIs used in this sample:
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/ee663274">DirectX Graphics and Gaming</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ff476345">Direct3D 11 Overview</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ff476147">Direct3D 11 Reference</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/bb205169">DXGI reference</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh405049">XAudio2</a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/ee417001">XInput</a> </li><li><a href="http://msdn.microsoft.com/library/windows/apps/br209045"><b>Windows.UI.Xaml</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/br227716"><b>Windows.UI.Xaml.Controls</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/hh702626"><b>Windows.UI.Xaml.Controls.SwapChainBackgroundPanel</b></a>
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
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=238717">DirectX 3D shooting game sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
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
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>This sample is a Universal sample and contains both Windows&nbsp;8.1 and Windows Phone 8.1 projects, as well as the code shared between them.</p>
<p></p>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>Simple3DGameXAML.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build Simple3DGameXAML.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>Simple3DGameXAML.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build Simple3DGameXAML.WindowsPhone</b>. </li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <b>Simple3DGameXAML.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy Simple3DGameXAML.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>Simple3DGameXAML.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy Simple3DGameXAML.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>Simple3DGameXAML.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>Simple3DGameXAML.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
