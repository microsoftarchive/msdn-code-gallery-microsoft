# UI Automation simple provider sample
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
<p>This sample application creates a simple custom control that implements Microsoft UI Automation provider interfaces. By implementing these interfaces, assistive technologies such as screen readers can programmatically access information about the control
 and make the control accessible to users with disabilities. </p>
<p>The control is a simple custom button that supports the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671279">
Invoke</a> control pattern. Clicking the button causes it to change color. You can also tab to the button and click it by pressing the spacebar.
</p>
<p>To test the functionality of the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671279">
Invoke</a> control pattern, you can use Inspect (an accessibility testing tool available from the Windows Software Development Kit (SDK).
</p>
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
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671596">UI Automation Provider Programmer's Guide</a>
</dt></dl>
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
</li><li>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.
</li></ol>
<p></p>
<h3>Run the sample</h3>
<p>To run this sample after building it, go to the installation folder for this sample with Windows Explorer and run UIASimpleProvider.exe from the
<i>&lt;install_root&gt;</i>\UI Automation Simple Provider Sample\C&#43;&#43;\Debug folder.
</p>
<p>To run this sample from Microsoft Visual Studio, press the F5 key to run with debugging enabled, or Ctrl&#43;F5 to run without debugging enabled. Alternatively, select
<b>Start Debugging</b> or <b>Start Without Debugging</b> from the <b>Debug</b> menu.</p>
</div>
