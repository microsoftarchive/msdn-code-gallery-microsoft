# App package information sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- App model
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample shows you how to get package info by using the Windows Runtime packaging API (<a href="http://msdn.microsoft.com/library/windows/apps/br224667"><b>Windows.ApplicationModel.Package</b></a> and
<a href="http://msdn.microsoft.com/library/windows/apps/br224668"><b>Windows.ApplicationModel.PackageId</b></a>).
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Users acquire your Windows Runtime app as an app package. The operating system uses the info in an app package to install the app on a per-user basis, and ensure that all traces of the app are gone from the device after all users who installed the app uninstall
 it. Each package consists of the files that constitute the app, along with a package manifest file that describes the app to the operating system.</p>
<p>Each package is defined by a globally unique identifier known as the package identity. Each package is described through package properties such as the display name, description, and logo.</p>
<p>The sample covers these key tasks:</p>
<ul>
<li>Getting the package identity using <a href="http://msdn.microsoft.com/library/windows/apps/br224680">
<b>Package.Id</b></a> </li><li>Getting the package directory using <a href="http://msdn.microsoft.com/library/windows/apps/br224681">
<b>Package.InstalledLocation</b></a> </li><li>Getting package dependencies using <a href="http://msdn.microsoft.com/library/windows/apps/br224679">
<b>Package.Dependencies</b></a> </li></ul>
<p>The sample covers these new tasks for Windows&nbsp;8.1:</p>
<ul>
<li>Getting the package description using <a href="http://msdn.microsoft.com/library/windows/apps/dn175742">
<b>Package.Description</b></a> </li><li>Getting the package display name using <a href="http://msdn.microsoft.com/library/windows/apps/dn175743">
<b>Package.DisplayName</b></a> </li><li>Determining whether the package is a bundle package using <a href="http://msdn.microsoft.com/library/windows/apps/dn175744">
<b>Package.IsBundle</b></a> </li><li>Determining whether the package is installed in development mode using <a href="http://msdn.microsoft.com/library/windows/apps/dn175745">
<b>Package.IsDevelopmentMode</b></a> </li><li>Determining whether the package is a resource package using <a href="http://msdn.microsoft.com/library/windows/apps/dn175746">
<b>Package.IsResourcePackage</b></a> </li><li>Getting package logo using <a href="http://msdn.microsoft.com/library/windows/apps/dn175747">
<b>Package.Logo</b></a> </li><li>Getting publisher display name of the package using <a href="http://msdn.microsoft.com/library/windows/apps/dn175748">
<b>Package.PublisherDisplayName</b></a> </li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><b>Conceptual</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh464929">App packages and deployment</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224667"><b>Windows.ApplicationModel.Package</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224668"><b>Windows.ApplicationModel.PackageId</b></a>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <b>PackageSample.Windows</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build PackageSample.Windows</b>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <b>PackageSample.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Press Ctrl&#43;Shift&#43;B or use <b>Build</b> &gt; <b>Build Solution</b> or use <b>Build</b> &gt;
<b>Build PackageSample.WindowsPhone</b>. </li></ol>
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
<li>Select <b>PackageSample.Windows</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy PackageSample.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <b>PackageSample.WindowsPhone</b> in <b>Solution Explorer</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b> or <b>Build</b> &gt; <b>Deploy PackageSample.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <b>PackageSample.Windows</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <b>PackageSample.WindowsPhone</b> in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
