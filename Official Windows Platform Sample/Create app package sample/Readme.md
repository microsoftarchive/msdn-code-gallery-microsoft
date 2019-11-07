# Create app package sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- COM
## Topics
- Packaging
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample shows you how to create an app package by using the app <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446766">
Packaging API</a>. </p>
<p>Users acquire your app as an app package. Windows uses the information in an app package to install the app on a per-user basis, and ensure that all traces of the app are gone from the device after all users who installed the app uninstall it. Each package
 consists of the files that constitute the app, along with a package manifest file that describes the app to Windows.</p>
<p>The sample covers the following tasks:</p>
<ul>
<li>Use <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446679">
<b>IAppxFactory::CreatePackageWriter</b></a> to create a package writer </li><li>Use <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446763">
<b>IAppxPackageWriter::AddPayloadFile</b></a> to add the payload files to the package
</li><li>Create an input stream and use <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446764">
<b>IAppxPackageWriter::Close</b></a> to write the manifest at the end of the package
</li></ul>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013; it doesn't compile with Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><b>Sample</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=106455">Extract app package contents sample</a>
</dt><dt><b>Tasks</b> </dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446616">How to create a package</a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh464929">App packages and deployment</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446679"><b>IAppxFactory::CreatePackageWriter</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446762"><b>IAppxPackageWriter</b></a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh446593">App packaging and deployment</a>
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
<h4><a id="From_the_Command_window"></a><a id="from_the_command_window"></a><a id="FROM_THE_COMMAND_WINDOW"></a>From the Command window</h4>
<ol>
<li>
<p>Open a Command window.</p>
</li><li>
<p>Go to the directory where you downloaded the CreateAppx sample.</p>
</li><li>
<p>Run the following command:</p>
<p><b>msbuild CreateAppx.sln</b></p>
</li></ol>
<h4><a id="From_Visual_Studio"></a><a id="from_visual_studio"></a><a id="FROM_VISUAL_STUDIO"></a>From Visual Studio</h4>
<ol>
<li>
<p>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.</p>
</li><li>
<p>Go to the directory where you downloaded the CreateAppx sample and double-click its Microsoft Visual Studio Solution (.sln) file.</p>
</li><li>
<p>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b>.</p>
</li></ol>
<h3>Run the sample</h3>
<ol>
<li>
<p>Open a Command window.</p>
</li><li>
<p>Go to the CreateAppx\Data directory.</p>
</li><li>
<p>Run the following command (where &lt;path&gt; is &quot;..\debug\CreateAppx&quot; or &quot;..\release\CreateAppx&quot;):</p>
<p><b>&lt;path&gt;\CreateAppx</b></p>
<p>This command creates an app package named HelloWorld.appx from the contents of the CreateAppx\Data directory.</p>
</li></ol>
</div>
