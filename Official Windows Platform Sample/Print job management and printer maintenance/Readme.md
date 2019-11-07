# Print job management and printer maintenance
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- printer
- Devices and sensors
- Windows Store device app
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows how to create a Windows Store device app that can perform print job management and device maintenance. This sample also includes a printer extension library that can be used to access a printer from a Windows Store device app written for
 Windows&nbsp;8.1. </p>
<p>This sample only demonstrates new functionality introduced in Windows&nbsp;8.1. To learn how to extend the print settings and print notifications experience, see the
<a href="http://go.microsoft.com/fwlink/p/?linkid=242862">Print settings and print notifications</a> sample.</p>
<p>Windows Store device apps for printers let printer manufacturers create a Windows Store app that serves as a companion to their device. A Windows Store device app includes a StoreManifest.xml file that specifies a device metadata experience ID. Device metadata
 associates a specific Windows Store app with a specific printer or family of printers. For more info, see
<a href="http://go.microsoft.com/fwlink/p/?LinkId=306682">Windows Store device apps for printers</a>.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p><b>Printer Extension Library</b></p>
<p>The C# sample uses a C# printer extension library to perform print-specific tasks. The printer extension library references a standard .NET library that provides interop functionality with the COM-based printing infrastructure.</p>
<p>The Visual Studio project PrinterExtensionLibrary wraps the COM implementation of the COM interface PrinterExtensionLib. This enables code sharing between printer extensions and Windows Store device apps for printers. You should consider adding any helper
 objects, convenience methods, or data model code at this layer. For more info about these interfaces, see
<a href="http://go.microsoft.com/fwlink/p/?LinkID=299887">Printer Extension Interfaces</a>.</p>
<p>Within the PrinterExtensionLibrary project that is included with the PrinterExtension sample project, there are two C# files. These files wrap the contents of PrinterExtensionLib, but additional classes could be added at this layer in order to enable code
 sharing between printer extensions and Windows Store device apps.</p>
<ul>
<li>
<p>PrinterExtensionTypes.cs specifies a number of helpful enumerations, constants and interfaces that wrap the COM PrinterExtensionLib APIs.</p>
</li><li>
<p>PrinterExtensionAdapters.cs specifies all of the constructable classes used to wrap the COM PrinterExtensionLib APIs.</p>
</li></ul>
<p>This project may be augmented with any necessary C# files that describe common model layer code necessary to build your printer extension and/or Windows Store device app for printers. Microsoft recommends against updating the existing classes insofar as
 possible, as this will make it more difficult for manufacturers to incorporate bug fixes to the app samples as they become available.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=301381">Windows Store device apps</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=242862">Print settings and print notifications (device app for printer sample)</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
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
</tbody>
</table>
<h2>Build the sample</h2>
<h2><a id="Prerequisites"></a><a id="prerequisites"></a><a id="PREREQUISITES"></a>Prerequisites</h2>
<p>The Windows Store device app that this sample demonstrates must be linked to your printer using device metadata.</p>
<ul>
<li>
<p>You need a copy of the device metadata package for your printer, to add the device app info to it. If you don’t have device metadata, you can build it using the
<b>Device Metadata Authoring Wizard</b> as described in the topic <a href="http://go.microsoft.com/fwlink/p/?LinkId=313644">
Step 2: Create device metadata for your Windows Store device app</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;To use the <b>Device Metadata Authoring Wizard</b>, you must install Microsoft Visual Studio Professional&nbsp;2013, Microsoft Visual Studio Ultimate&nbsp;2013, or the
<a href="http://go.microsoft.com/fwlink/?LinkID=309209">standalone SDK for Windows&nbsp;8.1</a> before completing the steps in this topic. Installing Microsoft Visual Studio Express&nbsp;2013 for Windows installs a version of the SDK that doesn't include the wizard.</p>
</li></ul>
<h2><a id="Install_the_sample"></a><a id="install_the_sample"></a><a id="INSTALL_THE_SAMPLE"></a>Install the sample</h2>
<p>The following steps build the sample and install device metadata.</p>
<ol>
<li>Enable test signing.
<ol>
<li>Start the <b>Device Metadata Authoring Wizard</b> from <i>%ProgramFiles(x86)%</i>\Windows Kits\8.1\bin\x86, by double-clicking
<b>DeviceMetadataWizard.exe</b> </li><li>From the <b>Tools</b> menu, select <b>Enable Test Signing</b>. </li></ol>
</li><li>Reboot the computer </li><li>
<p>Build the solution by opening the solution (.sln) file. Press F7 or go to <b>Build-&gt;Build Solution</b> from the top menu after the sample has loaded.
</p>
</li><li>Disconnect and uninstall the printer. This step is required so that Windows will read the updated device metadata the next time the device is detected.
</li><li>Edit and save device metadata. To link the device app to your device, you must associate the device app with your device.
<p class="note"><b>Note</b>&nbsp;&nbsp;If you haven't created your device metadata yet, see
<a href="http://go.microsoft.com/fwlink/p/?LinkId=313644">Step 2: Create device metadata for your Windows Store device app</a>.</p>
<ol>
<li>If the <b>Device Metadata Authoring Wizard</b> is not open yet, start it from
<i>%ProgramFiles(x86)%</i>\Windows Kits\8.1\bin\x86, by double-clicking <b>DeviceMetadataWizard.exe</b>.
</li><li>Click <b>Edit Device Metadata</b>. This will let you edit your existing device metadata package.
</li><li>In the <b>Open</b> dialog box, locate the device metadata package associated with your Windows Store device app. (It has a
<b>devicemetadata-ms</b> file extension.) </li><li>On the <b>Specify Windows Store device app information</b> page, enter the Windows Store app info in the
<b>Windows Store device app</b> box. Click on <b>Import Windows Store App manifest file</b> to automatically enter the
<b>Package name</b>, <b>Publisher name</b>, and <b>Windows Store App ID</b>. </li><li>
<p>If your app is registering for printer notifications, fill out the <b>Notification handlers</b> box. In
<b>Event ID</b>, enter the name of the print event handler. In <b>Event Asset</b>, enter the name of the file where that code resides.</p>
</li><li>When you're done, click <b>Next</b> until you get to the <b>Finish</b> page. </li><li>On the <b>Review the device metadata package</b> page, make sure that all of the settings are correct and select the
<b>Copy the device metadata package to the metadata store on the local computer</b> check box. Then click
<b>Save</b>. </li></ol>
</li><li>Reconnect your printer so that Windows reads the updated device metadata when the device is connected.
</li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
