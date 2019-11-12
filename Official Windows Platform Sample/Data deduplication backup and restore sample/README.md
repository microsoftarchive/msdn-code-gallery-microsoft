# Data deduplication backup and restore sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Storage
## Updated
- 03/17/2014
## Description

<div id="mainSection">
<p>This sample demonstrates the use of the <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh449211">
Data Deduplication Backup/Restore API</a> to perform optimized backup and optimized restore that is not optimized.
</p>
<p>For more information about this scenario, see <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh769317">
Selective File Restore Using Data Deduplication</a>.</p>
<p>This sample is written in C&#43;&#43; and requires some experience with COM.</p>
<p>This sample contains the following files: </p>
<ul>
<li>DedupBackupRestore.cpp </li><li>DedupBackupRestore.sln </li><li>DedupBackupRestore.vcxproj </li></ul>
<p></p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Windows Server&nbsp;2012&nbsp;R2 and Microsoft Visual Studio&nbsp;2013. It will not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh449204">Data Deduplication API</a>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>None supported </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>To build the sample using the command line:</p>
<ul>
<li>Open the <b>Command Prompt </b>window and navigate to the directory. </li><li>Type <b>msbuild DedupBackupRestore.sln</b>. </li></ul>
<p>To build the sample using Visual Studio&nbsp;2013 (preferred method):</p>
<ul>
<li>Open File Explorer and navigate to the directory. </li><li>Double-click the icon for the .sln (solution) file to open the file in Visual Studio.
</li><li>In the <b>Build</b> menu, select <b>Build Solution</b>. The application will be built in the default \Debug or \Release directory.
</li></ul>
<h2>Run the sample</h2>
<p>To run the sample: </p>
<ol>
<li>Install the Data Deduplication component. </li><li>Set up a Data Deduplication-enabled volume with some test files, and run a Deduplication Optimization job to optimize the files first.
<p class="note"><b>Note</b>&nbsp;&nbsp;The following sequence of PowerShell cmdlets assumes that your test volume is T:.</p>
<ol>
<li><b>Enable-DedupVolume  -volume T:</b> </li><li><b>Set-DedupVolume  -volume T: -MinimumFileAgeDays 0</b> </li><li><b>Start-DedupJob  -volume T: -type Optimization -wait</b> </li></ol>
For more information, see the <a href="http://technet.microsoft.com/library/4a752894-524d-4a64-8483-f06a73ab0ed0">
Enable-DedupVolume</a> PowerShell cmdlet documentation. </li><li>Navigate to the directory that contains the new executable file, using the <b>
Command Prompt</b> or <b>File Explorer</b>.
<p class="note"><b>Note</b>&nbsp;&nbsp;If you use the Command Prompt, you must run as administrator.</p>
</li><li>Type the name of the executable file (DedupBackupRestore.exe by default) at the command prompt.
</li></ol>
<p></p>
</div>
