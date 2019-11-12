# ChatterBox VoIP sample app
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Windows Phone 8
## Topics
- Background Agent
- VOIP
- video chat
- audio chat
- call integration
## Updated
- 02/20/2014
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>ChatterBox is an app that uses loopback audio and video to demonstrate the VoIP capabilities of Windows Phone 8. This sample shows you how to:
</p>
<ul>
<li>
<p>Structure a VoIP app for Windows Phone.</p>
</li><li>
<p>Capture and render audio and video.</p>
</li><li>
<p>Integrate streaming video into the app’s UI.</p>
</li><li>
<p>Implement the VoIP background agents and manage the background process life cycle.</p>
</li><li>
<p>Set up a Microsoft Push Notification Service channel and use it to initiate a VoIP call.</p>
</li><li>
<p>Enable inter-process communication using the Windows Phone Runtime.</p>
</li><li>
<p>Use the Windows Phone Runtime VoIP APIs, including:</p>
<ul>
<li>
<p>VideoRenderer</p>
</li><li>
<p>VoipPhoneCall</p>
</li><li>
<p>VoipCallCoordinator</p>
</li><li>
<p>AudioRoutingManager</p>
</li></ul>
</li></ul>
<p>For more info about creating VoIP apps for Windows Phone, see <a href="http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206983(v=vs.105).aspx">
VoIP apps for Windows Phone 8</a>.</p>
<p><b>Build the sample</b> </p>
<ol>
<li>
<p>Start Visual Studio Express 2012 for Windows&nbsp;Phone and select <span class="ui">
File</span> &gt; <span class="ui">Open</span> &gt; <span class="ui">Project/Solution</span>.
</p>
</li><li>
<p>Go to the directory in which you unzipped the sample. Double-click the Visual Studio Express 2012 for Windows&nbsp;Phone solution (<span class="label">.sln</span>) file.
</p>
</li><li>
<p>Use <span class="ui">Build</span> &gt; <span class="ui">Rebuild Solution</span> to build the sample.
</p>
</li></ol>
<p><b>Run the sample</b> </p>
<ol>
<li>
<p>Before running the sample, you must set the UI project to be the startup project. In
<span class="ui">Solution Explorer</span>, right-click the UI project icon and then choose
<span class="ui">Set as Startup Project</span>. </p>
</li><li>
<p>To debug the app and then run it, press F5 or use <span class="ui">Debug</span> &gt;
<span class="ui">Start Debugging</span>. To run the app without debugging, press Ctrl&#43;F5 or use
<span class="ui">Debug</span> &gt; <span class="ui">Start Without Debugging</span>.</p>
</li><li>
<p>The main screen has four buttons:</p>
<ul>
<li>
<p><b>Make out going call</b> – Takes you first to a dialing screen and, after you click
<span class="ui">Dial</span>, takes you to the VoIP call experience. This example doesn’t actually dial an external number. Instead, it uses loopback video that streams the video being captured by the camera back to the UI with a slight delay. You will need
 a physical device to view actual video, but the emulator simulates camera input so the app can still be used on the emulator.</p>
</li><li>
<p><b>Simulate incoming call</b> – Takes you to a screen that lets you simulate an incoming call by sending a push notification to the app. When the push notification is received on the phone, the built-in phone UI launches and prompts you to answer the call
 with audio, or with audio and video. Once you accept the call, you return to the VoIP app and the simulated call with loopback video.</p>
</li><li>
<p><b>View call status</b> – Takes you to the active call UI. This button is only enabled when a call is active.</p>
</li><li>
<p><b>Email push uri</b> – Emails the URI that’s used to send incoming call notifications to the phone. This will allow you to initiate VoIP calls from another machine. You must have an email account set up on the device or emulator before you can send the
 URI.</p>
</li></ul>
</li><li>
<p>The app’s <b>call in progress</b> screen is the screen users will see when they are in a video call. At the top of the screen are buttons to switch the audio routing to different end points. At the bottom of the screen are buttons to end the call, put the
 call on hold, or switch video input between the front camera and the back camera. Behind the UI, the live video feed from the camera is shown in a small inset rectangle. Behind this, loopback video simulating incoming network video is shown.</p>
</li><li>
<p>Exiting the app while a call is in progress will demonstrate how the VoIP call status is displayed in the phone UI.</p>
</li></ol>
<p><b>Sample description</b> </p>
<p>The ChatterBox VoIP sample solution is made up of four projects:</p>
<ul>
<li>
<p><b>UI</b> – This project implements the main foreground app that provides the UI for the app. This is a typical Windows Phone app that uses XAML and managed code. The foreground app also is responsible for setting up the push notifications channel and launching
 the background process.</p>
</li><li>
<p><b>Agents</b> – This project implements the background agents that help manage the background process. These agents are written in managed code and run at various stages of the VoIP app life cycle such as when an incoming call arrives or when the foreground
 app is running.</p>
</li><li>
<p><b>BackEnd</b> – This project performs the work of managing call state, integrating with the phone’s built-in phone call processes, and managing audio and video streams. This project is written using the Windows Phone Runtime VoIP APIs like the
<span value="VoIPCallCoordinator"><span class="keyword">VoIPCallCoordinator</span></span> and the
<span value="AudioRoutingManager"><span class="keyword">AudioRoutingManager</span></span>.</p>
</li><li>
<p><b>BackEndProxyStub</b> – This is an out-of-process server written in C that enables communication between the foreground app and the background process.</p>
</li></ul>
<p><b>See also</b> </p>
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206983(v=vs.105).aspx">VoIP apps for Windows Phone 8</a>
</p>
</li></ul>
</div>
</div>
