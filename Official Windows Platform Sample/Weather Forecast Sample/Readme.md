# Weather Forecast Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
- Windows Phone 8
## Topics
- Data
- Networking
- web service
## Updated
- 05/03/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>This sample shows how you can call and process data from a web service in a Windows&nbsp;Phone app. The sample uses the
<b>HttpWebRequest</b> class to make an asynchronous call to a web service that provides weather forecast information. Windows&nbsp;Phone apps should always use asynchronous operations when performing a task that can take a significant amount of time, such as performing
 a web service query, doing file operations with isolated storage, or starting up one of the devices sensors. This ensures that the user interface continues to respond to user input while the operation is taking place, providing a smooth user experience. The
 web service returns an XML document that is parsed by the app. Data binding is used to update the user interface to show the new forecast data.
</p>
<p>You must install the Windows&nbsp;Phone&nbsp;SDK to run this sample. To get started, go to the
<a href="http://go.microsoft.com/fwlink/?LinkId=259204">Windows Phone Dev Center</a>.</p>
<p>For more information about developing Windows&nbsp;Phone apps that use web services, see
<a href="http://go.microsoft.com/fwlink/?LinkID=193227">Communications for Windows Phone</a> in the MSDN Library.</p>
<p>For more info about using data binding to dynamically modify your user interface based on your app data, see
<a href="http://go.microsoft.com/fwlink/?LinkID=207966">Implementing the Model-View-ViewModel pattern in a Windows Phone app</a> in the MSDN Library.</p>
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
