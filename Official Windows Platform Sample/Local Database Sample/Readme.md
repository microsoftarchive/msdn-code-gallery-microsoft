# Local Database Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
- Windows Phone 8
## Topics
- Data
- Database
- Storage
## Updated
- 05/03/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>In Windows&nbsp;Phone OS&nbsp;7.1, you can use LINQ to SQL to store relational data in a local database that resides in your app’s isolated storage container. This sample is a to-do list app that uses a multi-table local database. Items that appear in the list are
 added, updated, and deleted from a local database, where they persist between app launches. For info about how to develop this app step-by-step, see
<a href="http://msdn.microsoft.com/library/windowsphone/develop/hh286405(v=vs.105).aspx">
How to create a local database app with MVVM for Windows Phone</a>.</p>
<p>You need to install Windows&nbsp;Phone&nbsp;SDK&nbsp;7.1 to run this sample. To get started, go to the
<a href="http://go.microsoft.com/fwlink/?LinkId=259204">Windows Phone Dev Center</a>.</p>
<div class="alert">
<table width="100%" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<th align="left"><b>Note:</b> </th>
</tr>
<tr>
<td>
<p>When the app launches for the first time, no to-do items appear in the list. Isolated storage is not persisted between launches of Windows&nbsp;Phone Emulator. Run the app on a Windows&nbsp;Phone OS&nbsp;7.1 device to observe data being stored in the database between app
 launches.</p>
</td>
</tr>
</tbody>
</table>
</div>
<p>This sample includes the February 2011 version of the <span value=" Windows Phone Toolkit">
<span class="keyword">Windows Phone Toolkit</span></span>. For the latest version of the toolkit and more info, see the
<a href="http://go.microsoft.com/fwlink/?LinkID=218352">Windows Phone Toolkit website on Codeplex</a>.</p>
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
