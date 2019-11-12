# Input Method Editor (IME) sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Win32
## Topics
- input
## Updated
- 03/24/2015
## Description

<div id="mainSection">
<p>This sample shows how to create an Input Method Editor (IME) that works in Windows Store apps and Windows&nbsp;8.1 desktop apps.</p>
<p>&nbsp;</p>
<p>The sample IME has the following features:</p>
<ul>
<li>Uses the Text Services Framework (TSF) </li><li>Runs in base trust </li><li>Compatible with Windows Store apps </li><li>Compatible with Systray and desktop </li><li>Interacts with touch keyboard </li><li>Integrates with Search contract </li><li>Interacts with light-dismiss </li></ul>
<p>&nbsp;</p>
<p>The IME sample uses the following code to obtain the parent window:</p>
<p><code>pView-&gt;GetWnd(&amp;parentWndHandle);</code></p>
<p>This implementation works only for Windows Store apps that use the built-in edit controls. This implementation won't work if the app uses the custom edit control from the subset of Text Service Framework (TSF) APIs available in the Windows Runtime. To ensure
 that the IME gets the proper parent window, so that the owned window is set correctly and works for desktop and Windows Store apps, replace the previous code with the following code:</p>
<p><code>if (FAILED(pView-&gt;GetWnd(&amp;parentWndHandle)) || (parentWndHandle == nullptr)) { parentWndHandle = GetFocus(); }</code></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://go.microsoft.com/fwlink/?LinkId=262401">Guidelines and checklist for IME development</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://go.microsoft.com/fwlink/?LinkId=262402">Text Services Framework</a>
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
<p>Start Visual Studio and select <strong>File</strong> &gt; <strong>Open</strong> &gt;
<strong>Project/Solution</strong>.</p>
</li><li>
<p>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file titled SampleIME.sln.</p>
</li><li>
<p>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <strong>Build</strong> &gt;
<strong>Build Solution</strong> to build the sample.</p>
</li></ol>
<h3>Run the sample</h3>
<p>If you build the IME sample by using Visual Studio&nbsp;2013, create an installation experience for the IME by using a third-party installer that supports Windows&nbsp;8.1, like InstallShield from Flexera Software.</p>
<p>&nbsp;</p>
<p>The following steps show how to use InstallShield to create a setup project for your IME DLL.</p>
<ul>
<li>Install Visual Studio&nbsp;2013. </li><li>Start Visual Studio&nbsp;2013. </li><li>On the <strong>File</strong> menu, point to <strong>New</strong> and select <strong>
Project</strong>. The <strong>New Project</strong> dialog opens. </li><li>In the left pane, navigate to <strong>Templates &gt; Other Project Types &gt; Setup and Deployment</strong>, click
<strong>Enable InstallShield Limited Edition</strong>, and click <strong>OK</strong>. Follow the installation instructions.
</li><li>Restart Visual Studio&nbsp;2013. </li><li>Open the IME solution (.sln) file. </li><li>Press F6 to build the solution. </li><li>In Solution Explorer, right-click the solution, point to <strong>Add</strong>, and select
<strong>New Project</strong>. The <strong>Add New Project</strong> dialog opens. </li><li>In the left tree view control, navigate to <strong>Templates &gt; Other Project Types &gt; InstallShield Limited Edition</strong>.
</li><li>In the center window, click <strong>InstallShield Limited Edition Project</strong>.
</li><li>In the <strong>Name</strong> text box, type &quot;SetupIME&quot; and click <strong>OK</strong>.
</li><li>In the <strong>Project Assistant</strong> dialog, click <strong>Application Information</strong>.
</li><li>Fill in your company name and the other fields. </li><li>Click <strong>Application Files</strong>. </li><li>In the left pane, right-click the <strong>[INSTALLDIR]</strong> folder, and select
<strong>New Folder</strong>. Name the folder &quot;Plugins&quot;. </li><li>Click <strong>Add Files</strong>. Navigate to SampleIME.dll, which is in the C&#43;&#43;\Debug folder, and add it to the
<strong>Plugins</strong> folder. Repeat this step for the IME dictionary, which is in the C&#43;&#43;\SampleIME\Dictionary folder.
</li><li>Right-click the IME DLL and select <strong>Properties</strong>. The <strong>Properties</strong> dialog opens.
</li><li>In the <strong>Properties</strong> dialog, click the <strong>COM &amp; .NET Settings</strong> tab.
</li><li>Under <strong>Registration Type</strong>, select <strong>Self-registration</strong> and click
<strong>OK</strong>. </li><li>Build the solution. The IME DLL is built, and InstallShield creates a setup.exe file that enables users to install your IME on Windows&nbsp;8.1. The setup.exe file is located in the SetupIME\SetupIME\Express\DVD-5\DiskImages\DISK1 folder.
</li></ul>
<p>&nbsp;</p>
</div>
