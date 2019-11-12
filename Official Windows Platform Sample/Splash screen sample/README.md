# Splash screen sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- User Interface
- universal app
## Updated
- 07/22/2014
## Description

<div id="mainSection">
<p>This sample shows how to imitate the splash screen that Windows displays for your app by positioning a similar image correctly when Windows dismisses the splash screen that it displays. This sample uses the
<a href="http://msdn.microsoft.com/library/windows/apps/br224763"><strong>SplashScreen</strong></a> class in the
<a href="http://msdn.microsoft.com/library/windows/apps/br224766"><strong>Windows.ApplicationModel.Activation</strong></a> namespace.</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info
 about how to build apps that target Windows and Windows Phone with Visual Studio, see
<a href="http://msdn.microsoft.com/library/windows/apps/dn609832">Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>The sample demonstrates these tasks:</p>
<ol>
<li>
<p><strong>Responding when the splash screen dismissed event fires</strong></p>
</li><li>
<p><strong>Extending the splash screen</strong></p>
</li></ol>
<p>Important APIs in this sample include:</p>
<ul>
<li>JavaScript: <a href="http://msdn.microsoft.com/library/windows/apps/br212679">
<strong>onactivated</strong></a> event </li><li>C#/C&#43;&#43;/VB: <a href="http://msdn.microsoft.com/library/windows/apps/br225018">
<strong>Activated</strong></a> event </li><li><a href="http://msdn.microsoft.com/library/windows/apps/br224763"><strong>SplashScreen</strong></a> class
</li></ul>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465332">Adding a splash screen (Windows Runtime apps using JavaScript and HTML)</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465331">Adding a splash screen (Windows Runtime apps using C#/VB/C&#43;&#43; and XAML)</a>
</dt><dt><strong>Related samples</strong> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231617">App activate and suspend using WinJS sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=231474">App activated, resume, and suspend using the WRL sample</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br224763"><strong>Windows.ApplicationModel.Activation.SplashScreen class</strong></a>
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
<p>&nbsp;</p>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 Update&nbsp;2 and select <strong>File</strong> &gt;
<strong>Open</strong> &gt; <strong>Project/Solution</strong>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select <strong>SplashScreenSample.Windows</strong> in <strong>Solution Explorer</strong>.
</li><li>Press Ctrl&#43;Shift&#43;B, or use <strong>Build</strong> &gt; <strong>Build Solution</strong>, or use
<strong>Build</strong> &gt; <strong>Build SplashScreenSample.Windows</strong>. </li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select <strong>SplashScreenSample.WindowsPhone</strong> in <strong>Solution Explorer</strong>.
</li><li>Press Ctrl&#43;Shift&#43;B or use <strong>Build</strong> &gt; <strong>Build Solution</strong> or use
<strong>Build</strong> &gt; <strong>Build SplashScreenSample.WindowsPhone</strong>.
</li></ol>
</li></ul>
</li></ol>
<p>&nbsp;</p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><strong>Deploying the sample</strong></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select <strong>SplashScreenSample.Windows</strong> in <strong>Solution Explorer</strong>.
</li><li>Use <strong>Build</strong> &gt; <strong>Deploy Solution</strong> or <strong>Build</strong> &gt;
<strong>Deploy SplashScreenSample.Windows</strong>. </li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select <strong>SplashScreenSample.WindowsPhone</strong> in <strong>Solution Explorer</strong>.
</li><li>Use <strong>Build</strong> &gt; <strong>Deploy Solution</strong> or <strong>Build</strong> &gt;
<strong>Deploy SplashScreenSample.WindowsPhone</strong>. </li></ol>
</li></ul>
<p><strong>Deploying and running the sample</strong></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click <strong>SplashScreenSample.Windows</strong> in <strong>Solution Explorer</strong> and select
<strong>Set as StartUp Project</strong>. </li><li>To debug the sample and then run it, press F5 or use <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click <strong>SplashScreenSample.WindowsPhone</strong> in <strong>Solution Explorer</strong> and select
<strong>Set as StartUp Project</strong>. </li><li>To debug the sample and then run it, press F5 or use <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>. </li></ol>
</li></ul>
</div>
