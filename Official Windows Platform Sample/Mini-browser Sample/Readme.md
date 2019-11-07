# Mini-browser Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
- Windows Phone 8
## Topics
- User Interface
- Browser
- HTML
## Updated
- 05/03/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>This sample shows how to use the WebBrowser control in your Windows&nbsp;Phone app. The sample takes a URL from the user and navigates to the specified page. It also automatically adjusts to portrait and landscape modes.</p>
<p>You need to install the Windows&nbsp;Phone&nbsp;SDK to run this sample. To get started, go to the
<a href="http://go.microsoft.com/fwlink/?LinkId=259204">Windows Phone Dev Center</a>.</p>
<p>For more info about building this app, see <a href="http://go.microsoft.com/fwlink/?LinkID=185886">
How to create your first app for Windows Phone</a> in the MSDN Library.</p>
</div>
<a name="BKMK_Buildinganddebuggingtheapp">
<h1 class="heading"><span>Building and debugging the app</span> </h1>
<div id="sectionSection0" class="section" name="collapseableSection" style="">
<p>The following steps show you how to build and debug this sample.</p>
<div class="alert">
<table width="100%" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<th align="left"><b>Important Note:</b> </th>
</tr>
<tr>
<td>
<p>Before you test the app, make sure that your computer has internet access to be able to test the Web browser control.</p>
</td>
</tr>
</tbody>
</table>
</div>
<h3 class="procedureSubHeading">To build and debug the app</h3>
<div class="subSection">
<ol>
<li>
<p>Build the solution by selecting the <span class="ui">BUILD | Build Solution</span> menu command. The project should build without any errors in the
<span class="ui">Error List</span> window. You can open the <span class="ui">
Error List</span> window, if it is not already open, by selecting the <span class="ui">
VIEW | Error List</span> menu command. If there are errors, review the steps above, correct any errors, and then build the solution again.</p>
</li><li>
<p>On the standard toolbar, make sure the deployment target for the app is set to one of the values for the Windows&nbsp;Phone&nbsp;Emulator, for example,
<span class="ui">Emulator WVGA</span>.</p>
</li><li>
<p>Run the app by pressing <b>F5</b> or by selecting the <span class="ui">DEBUG | Start Debugging</span> menu command. This opens the emulator window and launches the app.</p>
</li><li>
<p>To test your running app, click the <span class="ui">Go</span> button and verify that the browser goes to the specified website.</p>
</li><li>
<p>To test the app in landscape mode, press one of the rotation controls on the emulator. These are the buttons that show a horizontal and a vertical rectangle.</p>
<p>When the emulator rotates to landscape mode, the controls resize themselves to fit the landscape screen format.</p>
</li><li>
<p>To stop debugging, you can select the <span class="ui">DEBUG | Stop Debugging</span> menu command.</p>
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
</a>