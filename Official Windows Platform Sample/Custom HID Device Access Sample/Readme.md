# Custom HID Device Access Sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- hid
- Devices and sensors
- Human Interface Devices
## Updated
- 10/01/2014
## Description

<div id="mainSection">
<p>This sample shows how to use the <a href="http://msdn.microsoft.com/library/windows/apps/dn264174">
<b>Windows.Devices.HumanInterfaceDevices</b></a> API. It was designed to work with a programmable, USB-based, HID device called the SuperMUTT.
</p>
<p>You can use the sample to toggle an LED on the device by sending feature reports. You can also retrieve a feature report to determine the current LED blink pattern. In addition, you can use the sample to send output reports, receive input reports, and receive
 input-report interrupts.</p>
<p>If you are new to the HID protocol and concepts like: feature reports, input reports, and output reports, you can find more information
<a href="http://go.microsoft.com/fwlink/p/?linkid=296834">here</a>.</p>
<p>You can purchase a SuperMUTT device from <a href="http://go.microsoft.com/fwlink/p/?linkid=296610">
JJG Technologies</a>. (Note that you will need to update the device firmware before you can run the app.)</p>
<p>The sample demonstrates the following tasks:</p>
<ul>
<li>Enumerating available SuperMUTT devices. </li><li>Connecting to a specific device. </li><li>Issuing a feature report to retrieve, or set, the blink pattern on the device LED.
</li><li>Handling input-report events. </li><li>Receiving an input report. </li><li>Issuing an output report. </li><li>Disconnecting from a specific device. </li></ul>
<p>When the app is suspended, or the user removes the SuperMUTT, the sample explicitly closes the device. When the app is resumed, or the user reconnects the SuperMUTT, the sample re-opens the device.</p>
<p><b>Enumerating available SuperMUTT devices.</b></p>
<p>When you start the application and press the <b>Start Device Watcher</b> button, the app executes code that enumerates the available SuperMUTT devices.</p>
<p>Internally, the sample accomplishes this by retrieving a device selector and using this selector to create a device watcher (which looks for instances of the SuperMUTT device). The app retrieves a device selector by invoking the
<b>HidDevice.GetDeviceSelector</b> method. This method returns an Advanced Query Syntax (AQS) string. The app then passes the AQS string to the
<b>DeviceInformation.CreateWatcher</b> method.</p>
<p><b>Connecting to a SuperMUTT device.</b></p>
<p>When you click the corresponding device entry (identified by a string that contains the Vendor ID and Product ID) in the Select a HID Device list, the app establishes a connection to that specific device.</p>
<p><b>Issuing a feature report</b></p>
<p>When you choose the <b>Feature Reports</b> scenario the app displays a <b>Get Led Blink Pattern</b> and a
<b>Set Led Blink Pattern</b> button.</p>
<p>To retrieve the current blink pattern, click the <b>Get Led Blink Pattern</b> button and view the current setting in the adjacent control. To set the current blink pattern, enter a value in the control and click the
<b>Set Led Blink Pattern</b> button.</p>
<p><b>Handling input-report events</b></p>
<p>When you choose the <b>Input Report Events</b> scenario the app displays a <b>
Register For Event</b> and an <b>Unregister From Event</b> button.</p>
<p>To receive input-report interrupts, click the <b>Register For Event</b> button and view the output under the Scenario pane. The app will display the cumulative number of events received as well as a count of bytes received in each event.</p>
<p>To halt the transmission of asynchronous events, press the <b>Unregister From Event</b> button.</p>
<p>Note that whenever the user leaves this scenario, the app will unregister the event.</p>
<p><b>Receiving an input report</b></p>
<p>When you choose the <b>Input and Output Reports</b> scenario the app displays two
<b>Get Input Report</b> buttons. The first button lets you retrieve a numeric value issued by a device input report. The second button lets you retrieve a boolean value issued by a device input report.</p>
<p><b>Issuing an output report</b></p>
<p>When you choose the <b>Input and Output Reports</b> scenario the app displays two
<b>Get Output Report</b> buttons. The first button lets you send a numeric value via an output report. The second button lets you send a boolean value via output report.</p>
<p><b>Disconnecting from a SuperMUTT device.</b></p>
<p>When you press the <b>Stop Device Watcher</b> button, the app executes code that disconnects from the SuperMUTT device.</p>
<p>Also, whenever the app is suspended, it executes code that disconnects from the SuperMUTT device.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn264174"><b>Windows.Devices.HumanInterfaceDevices</b></a>
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
<p>To build this sample, open the solution (.sln) file titled CustomDeviceAccess.sln from Visual Studio. Press F7 or go to Build-&gt;Build Solution from the top menu after the sample has loaded.</p>
<h2>Run the sample</h2>
<h2><a id="Device_Driver_Requirements"></a><a id="device_driver_requirements"></a><a id="DEVICE_DRIVER_REQUIREMENTS"></a>Device Driver Requirements</h2>
<p>The sample runs over the inbox USB and HID device-drivers that ship in Windows&nbsp;8.1. These device drivers are installed when you first attach the SuperMUTT device.
</p>
<p>Note that the <b>Windows.Devices.HumanInterfaceDevice</b> API does not support custom, or filter, drivers.</p>
<h2><a id="Understanding_the_app_manifest"></a><a id="understanding_the_app_manifest"></a><a id="UNDERSTANDING_THE_APP_MANIFEST"></a>Understanding the app manifest</h2>
<p>The app manifest is an XML document that contains the info the system needs to deploy, display, or update a Windows Store app. This info includes package identity, package dependencies, required capabilities, visual elements, and extensibility points. Every
 app package must include one package manifest.</p>
<p>A Windows Store app that accesses a HID device must include specific <b>DeviceCapability</b> data in the
<b>Capabilities</b> node of its manifest. This data identifies the device and its purpose (or function). Note that some devices may have multiple functions.</p>
<p>The <b>Device Id</b> element corresponds to the device identifier. This element may specify a combination
<b>Vendor Id</b> (vid) and <b>Product Id</b> (pid); or, it may specify a generic string (&quot;any&quot;). In addition, the
<b>Device ID</b> may contain an optional provider string of &quot;usb&quot; or &quot;bluetooth&quot;.</p>
<p>The <b>Function Type</b> element specifies the device function. This element contains one or more HID usage values. These values consist of a
<b>Usage Page</b> and an optional <b>Usage Id</b>, each of which are 16-bit hexadecimal values.
</p>
<p><b>The sample DeviceCapabilities</b></p>
<p>In the following vendor-defined usage data, the device is identified by the <b>
Vendor Id</b> and the <b>Product Id</b> combination.</p>
<pre class="syntax"><code>  &lt;Capabilities&gt;
     &lt;!-- There are multiple ways declare a device --&gt;
    &lt;m2:DeviceCapability Name=&quot;humaninterfacedevice&quot;&gt;
      &lt;!--SuperMutt Device--&gt;
      &lt;m2:Device Id=&quot;vidpid:045E 0610&quot;&gt;
        &lt;m2:Function Type=&quot;usage:FFAA 0001&quot; /&gt;
      &lt;/m2:Device&gt;
    &lt;/m2:DeviceCapability&gt;
  &lt;/Capabilities&gt;</code></pre>
<h2><a id="Configuring_the_SuperMUTT_firmware"></a><a id="configuring_the_supermutt_firmware"></a><a id="CONFIGURING_THE_SUPERMUTT_FIRMWARE"></a>Configuring the SuperMUTT firmware</h2>
<p>The following steps allow you to configure the SuperMUTT device to run with your sample.</p>
<ul>
<li>Download and install the MUTT Software Package. </li><li>Open a command prompt and run the MuttUtil tool included in the package. Use this tool to update the firmware. (Note that you need to repeat the following command twice.)
<pre class="syntax"><code>MuttUtil.exe –forceupdatefirmware</code></pre>
</li><li>Use the MuttUtil tool to set the HID device-mode.
<pre class="syntax"><code>MuttUtil.exe –setwinrthid</code></pre>
</li></ul>
<p>Once you've tackled these steps, the SuperMUTT device is configured to work with the sample app.</p>
<p>If needed, the you can revert the SuperMUTT from HID device-mode back to its default configuration by running the following MuttUtil tool command.</p>
<dl><dd>
<pre class="syntax"><code>MuttUtil.exe –hidtodefault</code></pre>
</dd></dl>
<h2><a id="Running_the_C___and_C__Versions_of_the_Sample"></a><a id="running_the_c___and_c__versions_of_the_sample"></a><a id="RUNNING_THE_C___AND_C__VERSIONS_OF_THE_SAMPLE"></a>Running the C&#43;&#43; and C# Versions of the Sample</h2>
<p>To run this sample after building it, press F5 (run with debugging enabled) or Ctrl&#43;F5 (run without debugging enabled) from Visual Studio. (Or select the corresponding options from the Debug menu.)</p>
<h2><a id="Running_the_JavaScript_Version_of_the_Sample"></a><a id="running_the_javascript_version_of_the_sample"></a><a id="RUNNING_THE_JAVASCRIPT_VERSION_OF_THE_SAMPLE"></a>Running the JavaScript Version of the Sample</h2>
<p>To run this sample after building it, press F5 (run with debugging enabled) or Ctrl&#43;F5 (run without debugging enabled) from Visual Studio. (Or select the corresponding options from the Debug menu.)</p>
<h2><a id="Limitations"></a><a id="limitations"></a><a id="LIMITATIONS"></a>Limitations</h2>
<p><b>Designed for Peripherals</b></p>
<p>This API is designed primarily for accessing peripheral devices. That said, you can use it to access PC internal devices. However, access to these devices is limited to a privileged app that is created by the OEM.
</p>
<p><b>Not designed for Control Panel apps</b></p>
<p>This API is intended for Windows Store apps. Because there is no way for these apps to save settings outside of application scope you should not use it to write Control Panel apps.</p>
<p><b>Host Controller Limitations</b></p>
<p>The SuperMUTT device is compatible with EHCI host controllers. However, it does not currently support interrupts with XHCI host controllers.</p>
</div>
