# Video Recorder Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
- Windows Phone 8
## Topics
- video
- Media
- camera
## Updated
- 05/03/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>Learn how to use the device camera to record video in your own app. This sample demonstrates video preview, recording, and playback. It uses the
<a href="http://go.microsoft.com/fwlink/?LinkID=225479">FileSink</a> class to write video to isolated storage, from which it can be played afterward. For detailed information about how this sample works, see
<a href="http://msdn.microsoft.com/library/windowsphone/develop/hh394041(v=vs.105).aspx">
How to record video in a camera app for Windows Phone</a>.</p>
<p>You need to install Windows&nbsp;Phone&nbsp;SDK&nbsp;7.1 to run this sample. To get started, go to the
<a href="http://go.microsoft.com/fwlink/?LinkId=259204">Windows Phone Dev Center</a>.</p>
<h3 class="procedureSubHeading">To run the sample on the device</h3>
<div class="subSection">
<ol>
<li>
<p>Double-click the <span class="ui">.sln</span> file to open the solution.</p>
</li><li>
<p>Press F5 to start debugging the app.</p>
</li><li>
<p>When the app launches on the device, the UI provides the following app bar buttons for recording and playing video:</p>
<ul>
<li>
<p><span class="ui">Record</span>: Starts video recording.</p>
</li><li>
<p><span class="ui">Stop</span>: Stops video recording or video playback, depending on the state of the app.</p>
</li><li>
<p><span class="ui">Play</span>: Plays the recorded video. You must record a video before this button is available.</p>
</li><li>
<p><span class="ui">Pause</span>: Pauses playback. This button is not available during preview or recording.</p>
</li></ul>
</li></ol>
</div>
<div class="alert">
<table width="100%" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<th align="left"><b>Note:</b> </th>
</tr>
<tr>
<td>
<p>This sample is packaged as a Windows&nbsp;Phone&nbsp;7.5 project. It can be converted to a Windows&nbsp;Phone&nbsp;8 project, by changing the target Windows Phone OS version of the project. To create a Windows&nbsp;Phone&nbsp;8 project, you must be running the Windows&nbsp;Phone&nbsp;SDK&nbsp;8.0 on
 Visual Studio 2012. You can download the latest version of the SDK from <a href="http://dev.windowsphone.com/downloadsdk">
http://dev.windowsphone.com/downloadsdk</a>.</p>
<p>To convert the sample to a Windows&nbsp;Phone&nbsp;8 project:</p>
<ol>
<li>
<p>Double-click the <span class="ui">.sln</span> file to open the solution in Visual Studio.</p>
</li><li>
<p>Right-click the project in the <span class="ui">Solution Explorer</span> and select
<span class="ui">Properties</span>. This opens the <span class="ui">Project Properties</span> window.</p>
</li><li>
<p>In the <span class="ui">Application</span> tab of the Project Properties window, select
<span class="ui">Windows Phone OS 8.0</span> from the <span class="ui">Target Windows Phone OS Version</span> dropdown. A dialog will appear asking if you want to upgrade this project to Windows Phone OS 8.0.</p>
</li><li>
<p>Select <span class="ui">Yes</span> to upgrade the project.</p>
</li></ol>
</td>
</tr>
</tbody>
</table>
</div>
</div>
</div>
