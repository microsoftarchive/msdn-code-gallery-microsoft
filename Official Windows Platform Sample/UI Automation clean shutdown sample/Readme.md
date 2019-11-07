# UI Automation clean shutdown sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Automation
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample has two pieces that work together to show the correct way to clean up and shut down a Microsoft UI Automation control that is hosted in a DLL. The first piece is a DLL that contains a simple control with a basic UI Automation provider. The second
 piece is an application that hosts the control. </p>
<p>When the host application starts, it displays the <b>Load DLL</b> button. When you press the button, the host application loads the DLL and uses it to create and display the control. Also, the text of the button changes to
<b>Unload DLL</b>. When you press the button again, the host application destroys the control and unloads the DLL.
</p>
<p>When the control is destroyed, the provider calls the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh437312">
<b>UiaDisconnectProvider</b></a> function. If any external UI Automation client applications were connected to the control, the call to
<b>UiaDisconnectProvider</b> ensures that the clients release their references so the DLL can unload safely.</p>
<p>A good way to see the sample work is to connect a UI Automation client application after you press the
<b>Load DLL</b> button. Two good client applications are Narrator (the screen reader application included with Windows) and Inspect (an accessibility testing tool included in the Windows Software Development Kit (SDK)).</p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013 and will not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee684009">UI Automation</a>
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
</li><li>In Solution Explorer, select UiaCleanShutdownControl to build the simple control DLL, or UiaCleanShutdownHost to build the host application. (You'll need to build both pieces to use this sample.)
</li><li>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.
</li></ol>
<p></p>
<h3>Run the sample</h3>
<p>To run the sample after building it, follow these steps:</p>
<ol>
<li>Copy both pieces—the control DLL and the host application—to the same location. You'll find the control DLL at &lt;<i>install_root</i>&gt;\UI Automation Clean Shutdown Sample\C&#43;&#43;\UiaCleanShutdownControl\Debug\UiaCleanShutdownControl.dll, and the host application
 at &lt;<i>install_root</i>&gt;\UI Automation Clean Shutdown Sample\C&#43;&#43;\UiaCleanShutdownHost\Debug\UiaCleanShutdownHost.exe.
</li><li>Navigate to the location where you copied both pieces of the sample. </li><li>Type UiaCleanShutdownHost.exe at the command line, or double-click the icon for UiaCleanShutdownHost.exe to launch it from Windows Explorer.
</li></ol>
<p></p>
</div>
