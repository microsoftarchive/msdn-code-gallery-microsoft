# Basic Camera Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
- Windows Phone 8
## Topics
- Media
- camera
## Updated
- 05/03/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>This sample demonstrates how to activate the camera shutter and auto focus, configure picture resolution and flash settings, and use the hardware shutter button. This sample also demonstrates touch focus and a front-facing camera for the devices that support
 those features. You can develop this application step-by-step by following along with a series of topics, starting with
<a href="http://msdn.microsoft.com/library/windowsphone/develop/hh202956(v=vs.105).aspx">
How to create a base camera app for Windows Phone</a>. </p>
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
<p>When the app launches on the device, the initial page will show the camera viewfinder and the following buttons:</p>
<ul>
<li>
<p><span class="ui">SH</span>: The software shutter button. Tapping this button triggers the camera to take a picture and save it to the media library and the isolated storage container of the app.</p>
</li><li>
<p><span class="ui">Fl:Off</span>: The flash selection button. Tapping this button changes the flash settings. There are up to four possible settings:
<span class="ui">Fl:Off</span>, <span class="ui">Fl:On</span>, <span class="ui">
Fl:RER</span> (Red Eye Reduction), and <span class="ui">Fl:Auto</span>.</p>
</li><li>
<p><span class="ui">Res</span>: The photo resolution selection button. The selected resolution is displayed on the screen after you select it.</p>
<p></p>
</li></ul>
</li><li>
<p>Change the photo settings by tapping the <span class="ui">Fl</span> and <span class="ui">
Res</span> buttons.</p>
</li><li>
<p>Take a picture by tapping the <span class="ui">SH</span> button. A <span value="TextBlock">
<span class="keyword">TextBlock</span></span> on the camera viewfinder indicates the status of the saving operations to isolated storage.</p>
</li><li>
<p>After the picture has been saved, exit the app.</p>
</li><li>
<p>In the Photos Hub, find the picture that you took in the <span class="ui">Saved Pictures</span> folder.</p>
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
