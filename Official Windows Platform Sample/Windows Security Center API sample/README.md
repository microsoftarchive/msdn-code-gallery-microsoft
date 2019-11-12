# Windows Security Center API sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Security
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p></p>
<p>Demonstrates how to query the Windows Security Center for product information for any products registered with the Security Center that are antivirus, antispyware, or firewall products.
</p>
<p></p>
<p class="note"><b>Important</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013. It doesn't compile in Microsoft Visual Studio Express&nbsp;2013 for Windows</p>
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
<ol>
<li>
<p>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.</p>
</li><li>
<p>Go to the directory in which you unzipped the WscApiSample sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.</p>
</li><li>
<p>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.</p>
</li></ol>
<h3>Run the sample</h3>
<ol>
<li>
<p>Open a Command Prompt window.</p>
</li><li>
<p>Go to the directory that contains WscApiSample.exe.</p>
</li><li>
<p>Run the following command:</p>
<p><b>WscApiSample </b>[<b>-av</b>|<b>-as</b>|<b>-fw</b>]<b>[-av|-as|-fw]</b> where
<b>-av</b> queries for antivirus programs, <b>-as</b> queries for antispyware programs, and
<b>-fw</b> queries for firewall programs.</p>
<p>For example, <b>WscApiSample -av</b> would provide information about antivirus programs that are registered with the Windows Security Center.</p>
</li></ol>
</div>
