# Windows Store device app for camera sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- HTML5
## Topics
- Devices and sensors
- Windows Store device app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample demonstrates how to create a Windows Store device app for a camera. A Windows Store device app is provided by an IHV or OEM to differentiate the capture experience for a particular camera. It can be used to adjust camera settings or to provide
 additional functionality or effects. </p>
<p>The Windows Store device app in this sample provides custom UI for adjusting camera settings when a specific camera associated with the app is being used to capture a photo or video. Device metadata is used to associate the app with a camera.</p>
<p>If the user launches this sample from the app tile in <b>Start</b>, rather than when the camera is capturing video, this sample simply displays a message.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This UI in this sample works together with the custom effect provided by the
<a href="http://go.microsoft.com/fwlink/p/?linkid=251566">Driver MFT Sample</a>. Build and install the Driver MFT Sample before running this sample, so that you can use the UI implemented in this device app to control the custom effect provided in the Driver
 MFT Sample.</p>
<p></p>
<p>This sample shows that a device app for camera can be activated in two different contexts. The context can be determined by examining the activation parameters.
</p>
<ul>
<li>
<p>The app can be activated from an attempt to adjust settings. This happens when an app calls
<a href="http://msdn.microsoft.com/library/windows/apps/br211960"><b>Windows.Media.Capture.CameraOptionsUI.Show</b></a>, or when the user taps the
<b>Camera options</b> control followed by <b>More options</b> in the Camera Capture UI (<a href="http://msdn.microsoft.com/library/windows/apps/br241030"><b>Windows.Media.Capture.CameraCaptureUI</b></a>). In this case, the activation parameter's
<b>kind</b> property is <b>Windows.ApplicationModel.Activation.ActivationKind.CameraSettings</b>.
</p>
<p>When this sample app is activated from the <b>CameraSettings</b> context, it provides a custom
<b>More options</b> UI that contains slider and button controls to adjust camera settings. The camera settings are accessed using the VideoDeviceController object obtained from the activation event's CameraSettingsActivatedEventArgs argument. A Windows Store
 device app for a camera can also provide other additional settings not shown in this sample. For more info, see
<a href="http://go.microsoft.com/fwlink/p/?LinkId=306683">Windows Store device apps for cameras</a> and the
<a href="http://go.microsoft.com/fwlink/p/?linkid=251566">Driver MFT Sample</a> sample.</p>
</li><li>
<p>The app can also be activated because the user chose the app tile in <b>Start</b>. In this case, the
<b>kind</b> property is <b>Windows.ApplicationModel.Activation.ActivationKind.Launched</b>.
</p>
<p>When a Windows Store device app for a camera is launched from the app tile in <b>
Start</b>, it should provide an interesting and engaging experience. This experience can be used to highlight related products, provide customer support and other services. This sample simply shows an informational message in place of this experience.</p>
</li></ul>
<p></p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Camera Settings declaration must be included in package.appxmanifest. You can see how this is done by double-clicking on
<b>package.appxmanifest</b> in Solution Explorer and opening the <b>Declarations</b> tab.</p>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Concepts</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=301381">Windows Store device apps</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=306683">Windows Store device apps for cameras</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=251566">Driver MFT Sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=228589">Camera Capture UI sample</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=228588">Camera Options UI sample</a>
</dt><dt><b>Tutorials</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465152">Quickstart: Capturing a photo or video using the camera dialog</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241030"><b>Windows.Media.Capture.CameraCaptureUI</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br241060"><b>Windows.Media.Capture.CameraOptionsUI</b></a>
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
<td><dt>None supported </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<h2><a id="Prerequisites"></a><a id="prerequisites"></a><a id="PREREQUISITES"></a>Prerequisites</h2>
<p>The Windows Store device app that this sample demonstrates must be linked to your camera using device metadata.</p>
<ul>
<li>
<p>You need a copy of the device metadata package for your camera, to add the device app info to it. If you don’t have device metadata, you can build it using the
<b>Device Metadata Authoring Wizard</b> as described in the topic <a href="http://go.microsoft.com/fwlink/p/?LinkId=313644">
Step 2: Create device metadata for your Windows Store device app</a>.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;To use the <b>Device Metadata Authoring Wizard</b>, you must install the
<a href="http://go.microsoft.com/fwlink/?LinkID=309209">standalone SDK for Windows&nbsp;8.1</a> before completing the steps in this topic. Installing Microsoft Visual Studio Express&nbsp;2013 for Windows installs a version of the SDK that doesn't include the wizard.</p>
</li><li>If you have an internally embedded camera, rather than an externally connected one, follow the requirements that apply to your camera, described in “Appendix A: Requirements for identifying internal cameras” in the paper Developing Windows Store device
 apps for Cameras. For more info, see <a href="http://go.microsoft.com/fwlink/p/?LinkId=306683">
Windows Store device apps for cameras</a>. </li></ul>
<h2><a id="Install_the_sample"></a><a id="install_the_sample"></a><a id="INSTALL_THE_SAMPLE"></a>Install the sample</h2>
<p>The following steps build the sample and install device metadata.</p>
<ol>
<li>Enable test signing.
<ol>
<li>Start the <b>Device Metadata Authoring Wizard</b> from <i>%ProgramFiles(x86)%</i>\Windows Kits\8.1\bin\x86, by double-clicking
<b>DeviceMetadataWizard.exe</b> </li><li>From the <b>Tools</b> menu, select <b>Enable Test Signing</b>. </li></ol>
</li><li>Reboot the computer </li><li>
<p>Build the solution by opening the solution (.sln) file titled DeviceAppForWebcam.sln. Press F7 or go to
<b>Build-&gt;Build Solution</b> from the top menu after the sample has loaded. </p>
</li><li>Disconnect and uninstall the camera. This step is required so that Windows will read the updated device metadata the next time the device is detected.
<ul>
<li>If you have an externally connected camera, disconnect and uninstall your camera.
</li><li>If you have an internal camera, use <b>Device Manager</b> to uninstall the camera.
</li></ul>
</li><li>Edit and save device metadata. To link the device app to your camera, you must associate the device app with your camera.
<p class="note"><b>Note</b>&nbsp;&nbsp;If you haven't created your device metadata yet, see
<a href="http://go.microsoft.com/fwlink/p/?LinkId=313644">Step 2: Create device metadata for your Windows Store device app</a>.</p>
<ol>
<li>If the <b>Device Metadata Authoring Wizard</b> is not open yet, start it from
<i>%ProgramFiles(x86)%</i>\Windows Kits\8.1\bin\x86, by double-clicking <b>DeviceMetadataWizard.exe</b>.
</li><li>Click <b>Edit Device Metadata</b>. This will let you edit your existing device metadata package.
</li><li>In the <b>Open</b> dialog box, locate the device metadata package associated with your Windows Store device app. (It has a
<b>devicemetadata-ms</b> file extension.) </li><li>On the <b>Specify Windows Store device app information</b> page, enter the Windows Store app info in the
<b>Windows Store device app</b> box. Click on <b>Import Windows Store App manifest file</b> to automatically enter the
<b>Package name</b>, <b>Publisher name</b>, and <b>Windows Store App ID</b>. </li><li>When you're done, click <b>Next</b> until you get to the <b>Finish</b> page. </li><li>On the <b>Review the device metadata package</b> page, make sure that all of the settings are correct and select the
<b>Copy the device metadata package to the metadata store on the local computer</b> check box. Then click
<b>Save</b>. </li></ol>
</li><li>Reconnect your camera so that Windows reads the updated device metadata when the device is connected.
<ul>
<li>If you have an external camera, simply connect the camera. </li><li>If you have an internal camera, do one of the following:
<ul>
<li>Refresh the PC in the <b>Devices and Printers</b> folder. </li><li>Use<b> Device Manager</b> to scan for hardware changes. Windows should read the updated metadata when the device is detected.
</li></ul>
</li></ul>
</li></ol>
<h2>Run the sample</h2>
<p>To run the sample as if it were launched by the user choosing the app tile in Start, press F5 (run with debugging enabled) or Ctrl&#43;F5 (run without debugging enabled), or select the corresponding options from the Debug menu.
</p>
<p></p>
<p>To run the custom user interface for <b>More options</b>, do the following: </p>
<ol>
<li>Build and install the <a href="http://go.microsoft.com/fwlink/p/?linkid=251566">
Driver MFT Sample</a>, to install a custom effect that the <b>More options</b> flyout can control. You may skip this step, but then the
<b>Effect On/Off</b> and <b>Effect</b> controls in the custom flyout will not work.
</li><li>Run an app that invokes the <b>More options</b> flyout. Here are two ways to do that:
<ul>
<li>Run an app that displays the Camera Capture UI using <a href="http://msdn.microsoft.com/library/windows/apps/br241030">
<b>Windows.Media.Capture.CameraCaptureUI</b></a>. The <a href="http://go.microsoft.com/fwlink/p/?linkid=228589">
Camera Capture UI sample</a> is an example of such an app. <a href="http://msdn.microsoft.com/library/windows/apps/hh465152">
Quickstart: Capturing a photo or video using the camera dialog</a> provides a tutorial on using the Camera Capture UI.
<p>When the Camera Capture UI is displayed, tap the options button and then choose
<b>More options</b> in the dialog box that appears.</p>
</li><li>Run an app that calls <a href="http://msdn.microsoft.com/library/windows/apps/br211960">
<b>CameraOptionsUI.Show</b></a>. The <a href="http://go.microsoft.com/fwlink/p/?linkid=228588">
Camera Options UI sample</a> is an example of such an app. </li></ul>
</li><li>Check if the custom <b>More options</b> UI is correctly installed. It should contain
<b>Effect On/Off</b> and <b>Effect</b> controls. If you don't see these controls, check the troubleshooting steps.
</li><li>If you installed the <a href="http://go.microsoft.com/fwlink/p/?linkid=251566">
Driver MFT Sample</a> in step 1, the <b>Effect On/Off</b> control toggles an effect that covers the lower part of the video with a green box. The
<b>Effect</b> control adjusts the amount of the video that is covered. </li></ol>
<p></p>
<h2><a id="Troubleshooting"></a><a id="troubleshooting"></a><a id="TROUBLESHOOTING"></a>Troubleshooting</h2>
<p>If the <b>More options</b> flyout doesn't contain <b>Effect On/Off</b> and <b>
Effect</b> controls, check the following:</p>
<ul>
<li>Make sure you enabled test signing before installing the sample. Enable test signing by running
<code>bcdedit -set testsigning on</code> in a command prompt. </li><li>Make sure Package Name, Publisher Name, and App ID in the device metadata match the fields defined in the package.appxmanifest file of this sample.
</li><li>Use the <a href="http://go.microsoft.com/fwlink/p/?linkid=249441 ">Camera Capture UI sample</a> to test.
</li><li>If you have an internal camera (rather than an externally connected one), follow the requirements for your camera described in &quot;Appendix A: Requirements for identifying internal cameras&quot; in the Developing device apps for Cameras white paper. Note that you
 don’t have to provide the PLD information described in the paper, if your camera does not expose PLD info in its ACPI tables.
</li><li>If you have an internal camera, after installing the sample, refresh the PC using the
<b>Devices and Printers</b> folder. Select the PC in the folder, and click the Refresh button as shown in the following image. The camera itself should not be visible in the
<b>Devices and Printers</b> folder. This is because internal cameras are enumerated as part of the PC device container.
</li></ul>
<p>If you have installed the <a href="http://go.microsoft.com/fwlink/p/?linkid=251566">
Driver MFT Sample</a> but don't see the video effect that covers the lower part of the video with a green box, check the following:</p>
<ul>
<li>Check that the <b>Effect On/Off</b> switch in the <b>More Options</b> flyout is set to
<b>On</b>. </li><li>Check that the SampleMFT0.DLL is registered and that you have entered the CLSID of the Driver MFT under the device registry key for the camera you are using to capture video, as described in
<b>Building the sample</b>. </li><li>Check that SampleMFT0.DLL is in a subdirectory of Program Files. </li></ul>
</div>
