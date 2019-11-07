# VShadow Volume Shadow Copy Service sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Storage
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>VShadow is a command-line tool that you can use to create and manage volume shadow copies. It is also a sample that demonstrates the use of the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb968832">Volume Shadow Copy Service</a> (VSS) COM API.
</p>
<p>For more information about the VSS tool and its command-line options, see <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb530725">
VShadow Tool and Sample</a> and <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb530726">
VShadow Tool Examples</a>.</p>
<p>This sample is written in C&#43;&#43; and requires some experience with COM.</p>
<p>This sample contains the following files:</p>
<ul>
<li>break.cpp </li><li>create.cpp </li><li>delete.cpp </li><li>expose.cpp </li><li>macros.h </li><li>query.cpp </li><li>readme.html </li><li>readme.txt </li><li>revert.cpp </li><li>select.cpp </li><li>shadow.cpp </li><li>shadow.h </li><li>stdafx.cpp </li><li>stdafx.h </li><li>tracing.cpp </li><li>tracing.h </li><li>util.h </li><li>vshadow.rc </li><li>vshadow.sln </li><li>vshadow.vcxproj </li><li>vshadow.vcxproj.filters </li><li>vssclient.cpp </li><li>vssclient.h </li><li>writer.cpp </li><li>writer.h </li></ul>
<p></p>
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
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb530725">VShadow Tool and Sample</a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb530726">VShadow Tool Examples</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb968832">Volume Shadow Copy Service</a>
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
<p>To build the sample using the command line:</p>
<ul>
<li>Open the <b>Command Prompt </b>window and navigate to the directory. </li><li>Type <b>msbuild Vshadow.sln</b>. </li></ul>
<p>To build the sample using Visual Studio&nbsp;2013 (preferred method):</p>
<ul>
<li>Open File Explorer and navigate to the directory. </li><li>Double-click the icon for the .sln (solution) file to open the file in Visual Studio.
</li><li>In the <b>Build</b> menu, select <b>Build Solution</b>. The application will be built in the default \Debug or \Release directory.
</li></ul>
<h3>Run the sample</h3>
<p>To run the sample:</p>
<ol>
<li>Navigate to the directory that contains the new executable file, using the <b>
Command Prompt</b> or <b>File Explorer</b>.
<p class="note"><b>Note</b>&nbsp;&nbsp;If you use the Command Prompt, you must run as administrator.</p>
</li><li>Type the name of the executable file (vshadow.exe by default) at the command prompt.
</li></ol>
<p></p>
</div>
