# DXGI desktop duplication sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- DirectX
## Topics
- Graphics
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the desktop duplication behaviors exposed by DXGI in DirectX.
</p>
<p>This sample is written in C&#43;&#43;. You also need some experience with DirectX.</p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013 and will not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p>For info about how to use the desktop duplication API, see <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh404487">
Desktop Duplication API</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
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
<p>To build this sample, open the solution (.sln) file titled DesktopDuplication.sln from Visual Studio&nbsp;2013 for Windows&nbsp;8.1 (any SKU). Press F7 (or F6 for Visual Studio&nbsp;2013) or go to Build-&gt;Build Solution from the top menu after the sample has loaded.</p>
<h3>Run the sample</h3>
<p>To run this sample after building it, perform the following:</p>
<ol>
<li>Navigate to the directory that contains the new executable, using the command prompt.
</li><li>Type one of the following commands to run the executable.
<ol>
<li>From the command-line, run <b>desktopduplication.exe parameter \bitmap [interval in seconds]</b> to produce a bitmap every [interval in seconds] seconds
</li><li>From the command-line, run <b>desktopduplication.exe \output [all, #]</b> where &quot;all&quot; duplicates the desktop to all outputs, and [#] specifies the number of outputs.
</li></ol>
</li></ol>
</div>
