# Sample motion-sensor, firmware and Windows Runtime app for HID
## Requires
- Visual Studio 2013
## License
- Custom
## Technologies
- Windows Runtime
- Windows Store app
## Topics
- Devices and sensors
- HID device
- sample HID device for Windows
- sample HID device
## Updated
- 04/09/2014
## Description

<h1><span style="font-size:small">This end-to-end solution includes a tutorial for building a simple passive-infrared sensor that supports the HID protocol. The tutorial is provided as a whitepaper titled
</span><a href="http://go.microsoft.com/fwlink/?LinkId=323542" style="font-size:small">Building a motion sensor.docx</a><span style="font-size:small">. Included with this tutorial is a Visual Studio C# project, with source code, for creating the sensor firmware.</span></h1>
<p><span style="font-size:small">In addition to the tutorial for creating a device, this solution includes a sample Store App that monitors the sensor for motion and captures a short five-second video each time motion is detected. (The app is described in a
 second tutorial titled <a href="http://go.microsoft.com/fwlink/?LinkId=323544">Developing a Human Interface Device (HID) app</a>.)</span></p>
<p><span style="font-size:small">You can view a video of the complete end-to-end solution
<a href="http://channel9.msdn.com/Blogs/One-Dev-Minute/Building-a-HID-motion-sensor">
here</a>.</span></p>
<p><span style="font-size:small"><strong>Note:</strong> This end-to-end solution uses firmware produced under the the Creative Commons Attribution-ShareAlike 3.0 Unported License. For license terms see:
<a href="http://creativecommons.org/licenses/by-sa/3.0/">http://creativecommons.org/licenses/by-sa/3.0/</a></span></p>
<h2><span style="font-size:small">Installing Your Development Environment for the firmware</span></h2>
<p><span style="font-size:small">Before you can download the firmware onto your Netduino Plus board, you&rsquo;ll need to complete the following steps:</span></p>
<ul>
<li><span style="font-size:small">Install Microsoft Visual C# Express 2010 on a development machine</span>
</li><li><span style="font-size:small">Install .NET Micro Framework SDK v4.2</span> </li><li><span style="font-size:small">Install the Netduino Beta firmware v4.1.1 on your Netduino Plus board</span>
</li><li><span style="font-size:small">Download the device firmware project and open in Visual C# Express</span>
</li></ul>
<p><span style="font-size:small">Note that the video <a href="https://www.youtube.com/watch?v=RkjAmrXIRuo">
here</a> provides a great explanation for installing the Beta firmware on your Netduino board. (See the accompanying tutorial for more details about building the device and installing the firmware.)</span></p>
<h2>Building the HID-based motion sensor</h2>
<p><span style="font-size:small">The motion sensor consists of a Netduino Plus development board with an attached passive-infrared (PIR) sensor. You&rsquo;ll find details for building this sensor in the whitepaper titled
<a href="http://go.microsoft.com/fwlink/?LinkId=323542">Building a motion sensor.docx</a>. Once you build this sensor, you can attach it to your Windows 8.1 laptop or tablet and start the sample app.</span></p>
<h2>Installing Your Development Environment for the Store App</h2>
<p><span style="font-size:small">Before you can build and run the sample app, you&rsquo;ll need to complete the following steps:</span></p>
<ul>
<li><span style="font-size:small">Install Windows 8.1 on your development machine.</span>
</li><li><span style="font-size:small">Install Microsoft Visual Studio Professional 2013 or Microsoft Visual Studio Ultimate 2013 on the machine that you&rsquo;ll use to build the Store App.</span>
</li><li><span style="font-size:small">Install Microsoft Visual Studio </span></li><li><span style="font-size:small">Download the Windows Modern SDK</span> </li><li><span style="font-size:small">Download the sample app project</span> </li></ul>
<p><span style="font-size:small">See the accompanying tutorial titled <a href="http://go.microsoft.com/fwlink/?LinkId=323544">
Developing a Human Interface Device (HID) app</a> for more details about the sample App. (If your development machine doesn&rsquo;t support a video camera; you&rsquo;ll want to deploy the app onto a machine that does.)</span></p>
<h2>Building the sample app</h2>
<p><span style="font-size:small">After you build the motion sensor, you can begin testing it with the sample app. You&rsquo;ll find details for building the app (as well as details about the app&rsquo;s functionality) in the app tutorial.</span></p>
<p><span style="font-size:small">Once you&rsquo;ve read the whitepaper and installed the app, you can build it:</span></p>
<ol>
<li><span style="font-size:small">Start Microsoft Visual Studio&nbsp;2013 and select
<strong>File</strong> &gt; <strong>Open</strong> &gt; <strong>Project/Solution</strong>.</span>
</li><li><span style="font-size:small">Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Preview Solution (.sln) file.</span>
</li><li><span style="font-size:small">Press F7 or use <strong>Build</strong> &gt; <strong>
Build Solution</strong> to build the sample. </span></li><li><span style="font-size:small">Power the Netduino board using an external power supply.</span>
</li><li><span style="font-size:small">Wait several seconds for the sensor to initialize. (The infrared sensor requires apx. 10 seconds to determine the ambient level of infrared light.)</span>
</li><li><span style="font-size:small">Attach the Netduino board to your tablet or laptop using a USB cable</span>
</li><li><span style="font-size:small">With Visual Studio running and the sample project open, you can either debug the app, or run it without debugging. (To debug the app and then run it, press F5 or use
<strong>Debug</strong> &gt; <strong>Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>.)</span> </li></ol>
<h2><span style="font-size:small">Run the sample</span></h2>
<ol>
<li><span style="font-size:small">Power the Netduino board using an external power supply.</span>
</li><li><span style="font-size:small">Wait several seconds for the sensor to initialize. (The infrared sensor requires apx. 10 seconds to determine the ambient level of infrared light.)</span>
</li><li><span style="font-size:small">Attach the Netduino board to your tablet or laptop using a USB cable</span>
</li><li><span style="font-size:small">With Visual Studio running and the sample project open, you can either debug the app, or run it without debugging. (To debug the app and then run it, press F5 or use
<strong>Debug</strong> &gt; <strong>Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use
<strong>Debug</strong> &gt; <strong>Start Without Debugging</strong>.)</span> </li></ol>
