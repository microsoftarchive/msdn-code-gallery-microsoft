# Firmware Update USB Device sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- usb
- Devices and sensors
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows how a Windows Store app can update the firmware of a USB device. The update operation runs as a background task. The sample demonstrates the use of the
<a href="http://msdn.microsoft.com/library/windows/apps/dn278466"><b>Windows.Devices.Usb</b></a> namespace.
</p>
<p>To run this sample you need:</p>
<ul>
<li>A SuperMUTT device. You can purchase the device from <a href="http://jjgtechnologies.com/mutt.htm">
JJG Technologies</a>. </li></ul>
<p>
<table>
<tbody>
<tr>
<th>Windows runtime class</th>
<th>Description</th>
</tr>
<tr>
<td>FirmwareUpdate</td>
<td>
<p>The class implements methods that updates firmware in a background process that is separate from the process in which the app runs.
</p>
<ul>
<li>
<p>When the user navigates to MainPage, the app checks whether a background task is already registered. If a task is found, then most likely the app either closed or stopped working. In that case, the app unregisters the previous task.</p>
</li><li>When the user clicks <b>Perform a firmware update on the first available device</b>, the app finds all SuperMUTT devices and gets the
<a href="http://msdn.microsoft.com/library/windows/apps/br225393"><b>DeviceInformation</b></a> object for the first device. The app then attempts to opens the device in order for the consent prompt to be shown to the user. When the user clicks the
<b>Allow</b> button on the consent prompt, the app closes the device (discussed later) and creates and registers a background task for updating the firmware with the trigger. The app also registers its completion and progress routines to get notified about
 the task.
<p class="note"><b>Note</b>&nbsp;&nbsp;A background task cannot show the consent prompt.</p>
</li><li>The background task runs in a separate process. Because only one process can access the device at any given time, the app releases the
<a href="http://msdn.microsoft.com/library/windows/apps/dn263883"><b>UsbDevice</b></a> object so that the background task can open the device for the update operation.
</li><li>The task is canceled if it's not completed within two minutes. </li><li>Every time the task writes a sector of firmware data to the device, the progress callback routine is invoked. Within that callback, the app updates the progress bar. After all sectors are written to the device, the task releases the
<a href="http://msdn.microsoft.com/library/windows/apps/dn263883"><b>UsbDevice</b></a> object.
</li><li>When the background task completes, the completion callback is invoked. Within that callback the app reopens the device so that it can obtain the latest firmware version. It then shows the firmware version information and the status of the update operation
 is displayed. </li></ul>
<p></p>
</td>
</tr>
</tbody>
</table>
</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn303355">Writing a Windows store app for a USB device</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn278466"><b>Windows.Devices.Usb</b></a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/dn278466"><b>Windows.Devices.Usb</b></a>
<p>Provides Windows Runtime classes and enumerations that a Windows store app can use to communicate with an external USB device that uses WinUSB (Winusb.sys) as the device driver.
</p>
, <a href="http://msdn.microsoft.com/library/windows/apps/br224847"><b>Windows.ApplicationModel.Background</b></a>
<p>Enables an app to schedule background tasks to run app code even when the app is suspended.
</p>
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
<h2><a id="Flash_the_SuperMUTT_device_to_load_the_driver"></a><a id="flash_the_supermutt_device_to_load_the_driver"></a><a id="FLASH_THE_SUPERMUTT_DEVICE_TO_LOAD_THE_DRIVER"></a>Flash the SuperMUTT device to load the driver</h2>
<p>To automatically load Winusb.sys as the device driver: </p>
<ol>
<li>Download and install the <a href="http://msdn.microsoft.com/en-us/library/windows/hardware/jj590752.aspx">
MUTT Software Package</a>. </li><li>Open a command prompt and run the MuttUtil tool included in the package. Use the tool to update the firmware:
<p><code>MuttUtil.exe –forceupdatefirmware</code></p>
</li><li>By using the MuttUtil tool, change the device mode to WinRTUsbPersonality:
<p><code>MuttUtil.exe –SetWinRTUsb</code></p>
<p>The SuperMUTT device when configured in WinRTUsbPersonality mode, exposes configuration, interfaces, and endpoints, that work with the sample.
</p>
</li></ol>
<h2><a id="Building_the_Sample"></a><a id="building_the_sample"></a><a id="BUILDING_THE_SAMPLE"></a>Building the Sample</h2>
<p>To build this sample, open the solution (.sln) file titled FirmwareUpdateUsbDevice.sln from Microsoft Visual Studio&nbsp;2013. Press F7 or go to
<b>Build</b>-&gt;<b>Build Solution</b> from the top menu after the sample has loaded.</p>
<h2>Run the sample</h2>
<ol>
<li>Update the device metadata to indicate that the sample is a privileged app.
<p><img src="102741-image.png" alt="" align="middle">
</p>
</li><li>Open the <b>Finish</b> tab. Select the <b>Copy packages to your system's local metadata store</b> check box.
<p class="note"><b>Note</b>&nbsp;&nbsp;Alternatively, you can copy the metadata manually to %PROGRAMDATA%/Microsoft/Windows/DeviceMetadataStore.</p>
</li><li>Connect the device to the computer. </li><li>In Control Panel, open <b>View devices and printers</b> and verify that the icon of the device is this image:
<p><img src="102742-image.png" alt="" align="middle">
</p>
</li><li>Verify that the device description is: <b>Device Metadata Package Sample for SuperMUTT</b>.
</li><li>View the properties. Notice that the description specifies the specific sample associated in the device metadata. For example, for the C&#43;&#43; sample, the description shows
<b>To be used for Updating Firmware with C&#43;&#43; sample</b>. This string is useful for identifying the sample when you are running JavaScript, C#/VB.NET samples simultaneously.
</li><li>To run this sample after building it, press F5 (run with debugging enabled) or Ctrl-F5 (run without debugging enabled), or select the corresponding options from the
<b>Debug</b> menu. To deploy the app, select <b>Build</b> &gt; <b>Deploy FirmwareUpdateUsbDevice</b>.
<p class="note"><b>Note</b>&nbsp;&nbsp;The first time you run the sample, you will be prompted whether you allow the app to use the USB device. Choose
<b>Allow</b> to proceed.</p>
</li><li>Click <b>Perform a firmware update on the first available</b> device to start updating the device.
</li><li>View the progress of the operation. </li><li>When the output string shows <b>Firmware update completed</b>, verify that the
<b>Firmware version after firmware update</b> shows the correct version. </li><li>To cancel the update, click <b>Cancel firmware update</b>. </li></ol>
<p><img src="102743-image.png" alt="" align="middle">
</p>
<h2><a id="Known_issue"></a><a id="known_issue"></a><a id="KNOWN_ISSUE"></a>Known issue</h2>
<p><b>Applies to C&#43;&#43; and C# samples</b>: The app might unexpectedly terminate when the user tries to cancel a firmware update request that is in progress.</p>
<p>Each time the sample app initiates a request to update the device firmware, the app starts a background task. As the task runs, it first waits for approximately 30 seconds while the device prepares itself for the update (implemented with a
<b>Concurrency::wait</b> call in <b>SetupDeviceForFirmwareUpdateAsync</b>). During that time, the progress bar does not move.</p>
<p>If the user cancels the request during the 30-second period, the app cannot cancel the background task because of the
<b>wait</b> call. Because there is a system-imposed time limit on completing tasks when they are canceled, the system terminates the background task and the app closes.</p>
<p>To avoid that unexpected termination, replace all <b>Concurrency::wait</b> methods in the sample with this
<b>Wait</b> implementation. Make sure you call <b>Wait().get()</b> to allow any cancellation exception to propagate through the task chain.
</p>
<p>The following workaround is suggested for C&#43;&#43; apps. </p>
<div class="code"><span>
<table>
<tbody>
<tr>
<th>C&#43;&#43;</th>
</tr>
<tr>
<td>
<pre>/// &lt;summary&gt;
/// Waits for a caller specified time, but it checks every second to determine whether the task was canceled.
///
/// Must call Wait().get() to propagate the canceled exception.
///
/// If the background task is canceled, the task must be completed within a certain amount of time
/// (See &quot;How to handle idle or hung background tasks&quot; in MSDN). Otherwise, the
/// system terminates the background task and the app.
///
/// In order to complete the task within the system-imposed time limit, the background task cannot wait for too long.
/// This implementation waits and checks whether the task was canceled in small intervals to prevent a long wait. 
/// &lt;/summary&gt;
/// &lt;param name=&quot;millisecondsToWait&quot;&gt;&lt;/param&gt;
task&lt;void&gt; UpdateFirmwareTask::Wait(uint32 millisecondsToWait)
{
    return task&lt;void&gt;([millisecondsToWait] ()
    {
        uint32 timeLeftToWait = millisecondsToWait;

        // Split waits so that the background task does not wait more than the specified amount of time
        while (timeLeftToWait &gt; 0)
        {
            if (is_task_cancellation_requested())
            {
                cancel_current_task();
            }

            uint32 timeToWait = 0;

            if (timeLeftToWait &gt; FirmwareUpdateTaskInformation::MaxWaitTimeBetweenCancellationChecks)
            {
                timeToWait = FirmwareUpdateTaskInformation::MaxWaitTimeBetweenCancellationChecks;
            }
            else
            {
                timeToWait = timeLeftToWait;
            }

            wait(timeToWait);

            timeLeftToWait -= timeToWait;
        }

     }, cancellationTokenSource.get_token());
}
</pre>
</td>
</tr>
</tbody>
</table>
</span></div>
<p>The following workaround is suggested for C# apps. When calling <b>Task.Delay</b>, include a cancellation token as an argument, as shown here:</p>
<div class="code"><span>
<table>
<tbody>
<tr>
<th>C#</th>
</tr>
<tr>
<td>
<pre>
await Task.Delay(30000, cancellationTokenSource.Token);  

</pre>
</td>
</tr>
</tbody>
</table>
</span></div>
</div>
