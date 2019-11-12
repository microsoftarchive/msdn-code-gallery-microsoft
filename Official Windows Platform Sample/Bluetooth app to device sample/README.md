# Bluetooth app to device sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Windows Phone 8
## Topics
- Bluetooth
- peerfinder
- streamsocket
## Updated
- 03/05/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>When this sample runs, it looks for all paired devices. A paired device is a device you have paired with your phone through the Bluetooth control panel on your phone through
<span class="ui">Settings</span> &gt; <span class="ui">Bluetooth</span>. For example, you may have paired a Bluetooth headset or an in-car Bluetooth speaker. This sample shows you how to:
</p>
<ul>
<li>
<p>Detect if the Bluetooth radio is on or off.</p>
</li><li>
<p>Find all paired devices.</p>
</li><li>
<p>List those devices.</p>
</li><li>
<p>Attempt to connect to a device.</p>
</li></ul>
<p>This sample uses the <span class="label">PeerFinder</span> and <span class="label">
StreamSocket</span> Windows Phone Runtime APIs. For more information on Bluetooth for Windows&nbsp;Phone&nbsp;8, see
<a href="http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj207007(v=vs.105).aspx">
Bluetooth for Windows Phone 8</a>.</p>
<h3 class="procedureSubHeading">Build the sample</h3>
<div class="subSection">
<ol>
<li>
<p>Start Visual Studio Express 2012 for Windows&nbsp;Phone and select <span class="ui">
File</span> &gt;<span class="ui">Open</span> &gt; <span class="ui">Project/Solution</span>.</p>
</li><li>
<p>Go to the directory in which you unzipped the sample. Double-click the Visual Studio Express 2012 for Windows&nbsp;Phone Solution (<span class="label">.sln</span>) file.</p>
</li><li>
<p>Use <span class="ui">Build</span> &gt; <span class="ui">Rebuild Solution</span> to build the sample</p>
</li></ol>
</div>
<h3 class="procedureSubHeading">Run the sample</h3>
<div class="subSection">
<ul>
<li>
<p>To debug the app and then run it, press F5 or use <span class="ui">Debug</span> &gt;
<span class="ui">Start Debugging</span>. To run the app without debugging, press Ctrl&#43;F5 or use
<span class="ui">Debug</span> &gt; <span class="ui">Start Without Debugging</span>.</p>
</li></ul>
</div>
</div>
<h1 class="heading"><span>Notes</span> </h1>
<div id="sectionSection0" class="section" name="collapseableSection" style="">
<p>Because Bluetooth is not available on the Windows&nbsp;Phone&nbsp;8&nbsp;Emulator, you should test this sample on a Windows&nbsp;Phone&nbsp;8 phone.</p>
<p>To find all paired Bluetooth devices this sample adds a value to the <a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.networking.proximity.peerfinder.alternateidentities.aspx">
PeerFinder.AlternateIdentities</a> using <span class="code">PeerFinder.AlternateIdentities[&quot;Bluetooth:Paired&quot;] = &quot;&quot;</span>. The call to
<span class="label">PeerFinder.FindAllPeersAsync</span> will then return all paired devices. If you have not paired any devices to your phone, the call will return an empty list.
</p>
<p>To establish a socket connection with one of the paired devices, you must supply a HostName and a ServiceName. In most cases, the ServiceName is the port on the device over which Bluetooth communication is allowed. The HostName is set for you in this app,
 but you must supply the service, or port, name. </p>
</div>
<h1 class="heading"><span><a name="seeAlsoToggle">See Also</span> </h1>
<div id="seeAlsoSection" class="section" name="collapseableSection" style="">
<h4 class="subHeading">Other Resources</h4>
<div class="seeAlsoStyle"></a><a href="http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj207007(v=vs.105).aspx">Bluetooth for Windows Phone 8</a>
</div>
<div class="seeAlsoStyle"><a href="http://go.microsoft.com/fwlink/?LinkId=262288">Bluetooth app to app sample</a>
</div>
</div>
</div>
