# Magnification API sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- User Interface
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the Magnification API to create a full-screen magnifier that magnifies the entire screen, and a windowed magnifier that magnifies and displays a portion of the screen in a window.
</p>
<p>The full-screen magnifier sample (FullscreenMagnifierSample) includes an option for setting an input transform that maps the coordinate space of the magnified screen content to the actual (unmagnified) screen coordinate space. This enables the system to
 pass pen and touch input that is entered in magnified screen content, to the correct UI element on the screen. For the input transform option to work, you must:</p>
<ul>
<li>Set <code>UIAccess=&quot;true&quot;</code> in the application's manifest before building FullScreenMagnifierSample.
</li><li>After building, digitally sign the FullScreenMagnifierSample.exe file. </li><li>Copy the signed FullScreenMagnifierSample.exe file to a secure folder before running it.
</li></ul>
<p>For more information, see <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671610">
Security Considerations for Assistive Technologies</a>.</p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013 and will not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms692162">Magnification API</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ms692162">Magnification API</a>
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
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample. Go to the C&#43;&#43; directory and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>In <b>Solution Explorer</b>, right click <b>FullScreenMagnifierSample</b> and select
<b>Properties</b>. </li><li>In the <b>FullScreenMagnifierSample Property Pages</b> dialog box, expand the
<b>Configuration Properties</b> node, and then the <b>Linker</b> node. </li><li>Select <b>Manifest File</b>, and then set the manifest properties as follows.
<table>
<tbody>
<tr>
<th>Property</th>
<th>Setting</th>
</tr>
<tr>
<td>Enable User Account Control (UAC)</td>
<td>Yes (/MANIFESTUAC:)</td>
</tr>
<tr>
<td>UAC Execution Level</td>
<td>highestAvailable (/level='highestAvaliable')</td>
</tr>
<tr>
<td>UAC Bypass UI Protection</td>
<td>Yes (/uiAccess='true')</td>
</tr>
</tbody>
</table>
</li><li>Click <b>OK</b>. </li><li>Press F6 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. Both the full-screen magnifier (FullscreenMagnifierSample) and the windowed magnifier (MagnifierSample) will be built in the default \Debug or \Release directory.
</li></ol>
<p></p>
<h3>Run the sample</h3>
<p>To run the full-screen magnifier sample: </p>
<ol>
<li>Navigate to the <i>&lt;install_root&gt;</i>\Magnification API Sample\C&#43;&#43;\Debug folder for this sample using the command prompt or Windows Explorer.
</li><li>Use a tool such as Sign Tool (SignTool.exe) to digitally sign FullscreenMagnifierSample.exe.
</li><li>Copy FullscreenMagnifierSample.exe to a secure folder. </li><li>In the secure folder, type FullscreenMagnifierSample.exe at the command line, or double-click the icon for FullscreenMagnifierSample.exe to launch from Windows Explorer.
</li></ol>
<p></p>
<p>To run the windowed sample:</p>
<ol>
<li>Navigate to the <i>&lt;install_root&gt;</i>\Magnification API Sample\C&#43;&#43;\Debug folder for this sample using the command prompt or Windows Explorer.
</li><li>Type MagnifierSample.exe at the command line, or double-click the icon for MagnifierSample.exe to launch from Windows Explorer.
</li></ol>
<p></p>
</div>
