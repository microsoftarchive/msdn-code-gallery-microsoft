# Tic-Tac-Toe Over Sockets Sample
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Phone 7.5
- Windows Phone 8
## Topics
- Sockets
- Networking
## Updated
- 03/05/2013
## Description

<div id="mainBody">
<p></p>
<div class="introduction">
<p>This sample demonstrates how to use sockets on a Windows&nbsp;Phone device to communicate with a service hosted on the network. It creates a Tic-Tac-Toe game, with the phone as the client and your computer as the server. The sample contains both the client and
 the server components. For more info about sockets onWindows&nbsp;Phone, see <span><span class="nolink">Sockets for Windows Phone</span></span>.</p>
<p>You need to install Windows&nbsp;Phone&nbsp;SDK&nbsp;7.1 to run this sample. To get started, go to the
<a href="http://go.microsoft.com/fwlink/?LinkId=259204">Windows Phone Dev Center</a>.</p>
<h3 class="procedureSubHeading">To run the sample</h3>
<div class="subSection">
<ol>
<li>
<p>Double-click the <span value=".sln"><span class="keyword">.sln</span></span> file to open the solution.
</p>
</li><li>
<p>Select the solution in the Solution Explorer.</p>
</li><li>
<p>Select the <span class="ui">Project | Set Startup Projects…</span> menu command in the main menu.</p>
<p>The property pages dialog for the SocketsXO solution is shown.</p>
</li><li>
<p>Click <span class="ui">Multiple startup projects</span> and set the <span class="ui">
Action</span> column of both the SocketsXO_Client and SocketsXO_Server projects to
<span class="ui">Start</span>.</p>
</li><li>
<p>Click <span class="ui">Ok</span> to save your changes and exit this dialog.</p>
</li><li>
<p>Press F5 to start debugging the app.</p>
<div class="alert">
<table width="100%" cellspacing="0" cellpadding="0">
<tbody>
<tr>
<th align="left"><b>Important Note:</b> </th>
</tr>
<tr>
<td>
<p>Because the server side of this app is trying to access the network over a port, you may see a Windows Security Alert dialog. You can click
<span class="ui">Allow Access</span> to accept the default behavior for this app or you can change the settings on this dialog.
</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>When the app launches in Windows Phone Emulator, the initial page shows at the bottom of the screen that the server (host) name is empty and the port number is 0. You will not be able to play the game until a server (host) name and a port number have been
 defined.</p>
</li><li>
<p>Click the <span class="ui">Settings</span> button in the app bar to go to the
<span class="ui">Settings</span> page. In the </p>
</li><li>
<p><span class="ui">Settings</span> page, add a value for the server, or host, name. The Host Name is the name of your computer. Procedures to find your computer name include:
</p>
<ul>
<li>
<p>Right-click the <span class="ui">Computer</span> entry in the start menu and select
<span class="ui">Properties</span>.</p>
</li><li>
<p>Display the <span class="ui">System Properties</span> dialog by typing <span class="input">
sysdm.cpl</span> into the <span class="ui">Start Menu</span> search box on your computer.</p>
</li></ul>
</li><li>
<p>In the <span class="ui">Settings</span> page, add a value for the port number. The default port number defined in the app is 13001. If you enable that port on your computer, then you will not have to modify the server code. If you want to use a different
 port, you must set this in the AsynchronousSocketListener.cs class of the SocketsXO_Server project and also change the port number value in the
<span class="ui">Settings</span> page of the app.</p>
</li><li>
<p>Click the <span class="ui">Save</span> button on the app bar to save your changes and return to the main page of the app. In the main page, you should see the server name and port number you set at the bottom of the screen.</p>
</li><li>
<p>You can start a game by clicking the game board. This will enter your game piece,
<span class="ui">X</span> or <span class="ui">O</span>, into the square you selected and then attempt to retrieve a move from the server.</p>
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
