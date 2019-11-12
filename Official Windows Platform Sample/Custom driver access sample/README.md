# Custom driver access sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Devices and sensors
- Windows Store device app
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows how to use the <a href="http://msdn.microsoft.com/library/windows/apps/dn263667">
<b>Windows.Devices.Custom</b></a> namespace to access a custom driver from a Windows Store device app.
</p>
<p class="note"><b>Important</b>&nbsp;&nbsp;Starting in Windows&nbsp;8.1, you can use Windows Runtime APIs to access USB, Human Interface Device (HID), Bluetooth GATT, Bluetooth RFCOMM, Point of Service (POS) and scanning devices from your Windows Store app. Apps written
 for Windows&nbsp;8.1 cannot use custom driver access with devices that support these APIs. For more info about these APIs, see
</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/dn263667"><b>Windows.Devices.Custom</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn278466"><b>Windows.Devices.Usb</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn264174"><b>Windows.Devices.HumanInterfaceDevice</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn297685"><b>Windows.Devices.Bluetooth.GenericAttributeProfile</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn263529"><b>Windows.Devices.Bluetooth.Rfcomm</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn298071"><b>Windows.Devices.PointOfService</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn264250"><b>Windows.Devices.Scanners</b></a>
</li></ul>
<p></p>
<p><b>Building and Using the FX2 Samples</b></p>
<p>The Custom Driver Access (MoFX2) sample application demonstrates how a Windows Store device app can interact with a USB device using a Windows Driver Framework (WDF) driver.</p>
<p>This sample is written for OSRUSB-FX2 Learning Kit device. Full functionality of the sample application requires an actual FX2 device. You can order the device
<a href="http://go.microsoft.com/fwlink/p/?linkid=227221">here</a>.</p>
<p>For more info about this device, see the device specification for the <a href="http://go.microsoft.com/fwlink/p/?linkid=227224">
OSR USB-FX2 Learning Kit</a>.</p>
<p>This document presumes you want to initially build and run the sample application.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;</p>
<p class="note">To run this sample with hardware, you must download an additional sample that provides the driver and metadata for the FX2 device. Depending on whether you would like to use a Kernel-Mode Driver Framework (KMDF) driver or a User-Mode Driver
 Framework (UMDF) driver, you can download either the <a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">
kmdf_fx2</a> sample, or the <a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">
umdf_fx2</a> sample.</p>
<p></p>
<p><b>Installing Your Development Environment</b></p>
<p>Before you can build and run this sample application, you'll need to complete the following steps:</p>
<ul>
<li>Install <a href="http://go.microsoft.com/fwlink/p/?LinkID=301696">Windows&nbsp;8.1</a>
</li><li>Install Microsoft Visual Studio Professional&nbsp;2013 or Microsoft Visual Studio Ultimate&nbsp;2013
</li><li>Install <a href="http://go.microsoft.com/fwlink/p/?linkid=309150">Windows Driver Kit for Windows&nbsp;8.1</a>
</li><li>Download either the <a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">
kmdf_fx2</a> sample, or the <a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">
umdf_fx2</a> sample </li><li>Download this Custom driver access sample </li></ul>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">kmdf_fx2</a> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">umdf_fx2</a> </dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh404244">Device Access API</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn263667"><b>Windows.Devices.Custom</b></a>
</dt><dt><b>Concepts</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=301381">Windows Store device apps</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=303944">Windows Store device apps for internal devices (including custom driver access)</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=306563">Device Metadata schema reference</a>
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
<p>To deploy the Custom driver access sample solution on a single computer, you must first install the
<a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">kmdf_fx2</a> sample or the
<a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">umdf_fx2</a> sample, and then use the
<b>Device Metadata Authoring Wizard</b> to update and deploy the device metadata that is supplied with that sample.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;To use the <b>Device Metadata Authoring Wizard</b>, you must install Visual Studio Professional&nbsp;2013, Visual Studio Ultimate&nbsp;2013, or the
<a href="http://go.microsoft.com/fwlink/?LinkID=309209">standalone SDK for Windows&nbsp;8.1</a>, before completing the steps in this topic. Installing Microsoft Visual Studio Express&nbsp;2013 for Windows installs a version of the SDK that doesn't include the wizard.</p>
<h3><a id="Build_and_install_the_Fx2_driver_sample"></a><a id="build_and_install_the_fx2_driver_sample"></a><a id="BUILD_AND_INSTALL_THE_FX2_DRIVER_SAMPLE"></a>Build and install the Fx2 driver sample</h3>
<ol>
<li>Enable test signing.
<ol>
<li>Start the <b>Device Metadata Authoring Wizard</b> from <i>%ProgramFiles(x86)%</i>\Windows Kits\8.1\bin\x86, by double-clicking
<b>DeviceMetadataWizard.exe</b> </li><li>From the <b>Tools</b> menu, select <b>Enable Test Signing</b>. </li></ol>
</li><li>Follow the build instructions for <a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">
kmdf_fx2</a> or <a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">umdf_fx2</a>.
</li></ol>
<h3><a id="Update_and_deploy_device_metadata"></a><a id="update_and_deploy_device_metadata"></a><a id="UPDATE_AND_DEPLOY_DEVICE_METADATA"></a>Update and deploy device metadata</h3>
<p>The device metadata in this step is included in the <a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">
kmdf_fx2</a> and <a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">umdf_fx2</a> samples.</p>
<ol>
<li>Start the <b>Device Metadata Authoring Wizard</b> from <i>%ProgramFiles(x86)%</i>\Windows Kits\8.1\bin\x86, by double-clicking
<b>DeviceMetadataWizard.exe</b>. </li><li>Click <b>Edit Device Metadata</b>. This will let you edit the existing device metadata package from the
<a href="http://go.microsoft.com/fwlink/p/?LinkID=256131">kmdf_fx2</a> and <a href="http://go.microsoft.com/fwlink/p/?LinkID=256202">
umdf_fx2</a> samples. </li><li>In the <b>Open</b> dialog box, locate the device metadata package. (It has a <b>
devicemetadata-ms</b> file extension.) </li><li>On the <b>Specify Windows Store device app information</b> page, enter the Windows Store app info for this sample in the
<b>Windows Store device app</b> box. Click on <b>Import Windows Store App manifest file</b> to automatically enter the
<b>Package name</b>, <b>Publisher name</b>, and <b>Windows Store App ID</b>. </li><li>Enter the <b>Privileged application</b> information and check <b>Access custom driver</b>. The app details will be the same
<b>Package name</b> and <b>Publisher name</b> that was automatically retrieved in the previous step.
<p class="note"><b>Note</b>&nbsp;&nbsp;These build instructions are only for building this sample so it can be tested locally with the Fx2 board. If you are authoring device metadata for your own device rather than the Fx2 board, you must also ensure that the Package
 name, Publisher name, and App ID are in sync with those that you registered with the Windows Store.
</p>
<p>If the name of the <b>Package name</b> value for the app in the <b>Privileged application</b> section does not match what is imported from the Windows Store app manifest file in step 4, then the app will not be allowed to access the custom kmdf or umdf driver
 file and you will receive an access denied error when running the sample.</p>
</li><li>When you're done, click <b>Next</b> until you get to the <b>Finish</b> page. </li><li>On the <b>Review the device metadata package</b> page, make sure that all of the settings are correct and select the
<b>Copy the device metadata package to the metadata store on the local computer</b> check box. Then click
<b>Save</b>. </li><li>Restart your computer </li></ol>
<h2><a id="Building_the_solution"></a><a id="building_the_solution"></a><a id="BUILDING_THE_SOLUTION"></a>Building the solution</h2>
<ol>
<li>Start Microsoft Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
<h2><a id="Usage_Notes"></a><a id="usage_notes"></a><a id="USAGE_NOTES"></a>Usage Notes</h2>
<ul>
<li>The initial launch of the MoFX2 app will show an error message if you select <b>
1) Getting and setting Seven Segment Display</b>, then select <b>Get Seven Segment Display</b>. The error message reads: &quot;The parameter is incorrect. The segment display value is undefined.&quot;
<p>If you select <b>Set Seven Segment Display</b>, it works thereafter. This is because the device initializes the segment display to a non-numeric value. The sample does this to show how to handle error conditions.</p>
</li><li>If you select <b>Show the bar state</b>, the output shown doesn't (literally) match what is displayed on the screen. There are that number of bars lit, it just is that the table displayed by app doesn't visually correspond to bar locations on the device.
 For example, the screen shows bars 0-4 as lit, there are five lit bars on device; however, the top two and bottom two are not lit. The bar graph display seems odd, but is intentional. It's how the driver and the hardware maps actual the bar line to a number.
 This is what the functional spec for the hardware says. </li><li>In the <b>Get the switch state</b> menu option in the sample application the switch number values on the screen don't match what is silk-screened onto the Revision 2 device. The switches on the Revision 2 device start at 1 instead of 0.
</li></ul>
</div>
