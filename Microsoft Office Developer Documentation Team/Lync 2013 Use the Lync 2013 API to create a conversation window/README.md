# Lync 2013: Use the Lync 2013 API to create a conversation window
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Lync 2013
- Microsoft Lync Server 2013
## Topics
- Lync 2013 Model API
## Updated
- 02/08/2013
## Description

<div id="header">Summary: Use this code sample to learn how to use the Lync 2013 API
<span class="label">Lync.Model.Conversation</span> and <span class="label">Lync.Model.Conversation.AudioVideo</span> namespaces to create a conversation window.</div>
<div id="mainSection">
<div id="mainBody">
<div class="introduction">
<p><a id="75768" href="/Lync-2013-Use-the-Lync-68eabecb/file/75768/1/Lync2013_AudioVideoConversation.zip">Lync2013_AudioVideoConversation.zip</a>&nbsp;</p>
</div>
<h1 class="heading">Description of the sample</h1>
<div class="section" id="sectionSection0">
<p>This sample demonstrates how to create a conversation window by using the Lync Model API
<span class="label">conversation</span> and <span class="label">conversation.audiovideo</span> namespaces.</p>
<p>Sample features:</p>
<ul>
<li>
<p>Create an audio-video conversation window.</p>
</li><li>
<p>Register and handle conversation manager and audio-video conversation events.</p>
</li><li>
<p>Use <span class="label">Conversation</span>, <span class="label">AvModality</span>,
<span class="label">AudioChannel</span>, and <span class="label">VideoChannel</span> features.</p>
</li></ul>
<p>This sample, AudioVideoConversation, is distributed with the Lync 2013 SDK. The Lync 2013 SDK includes three development models:</p>
<ul>
<li>
<p>Lync Controls</p>
</li><li>
<p>Lync 2013 API</p>
</li><li>
<p>OCOM Unmanaged COM API</p>
</li></ul>
<p>You can drag-and-drop Microsoft Lync Controls into existing business applications to add Lync functions and user interface. Each Lync Control provides a specific feature like search, presence, instant messaging (IM) calls, and audio calls. The appearance
 of each control replicates the Lync UI for that feature. Use a single control or multiple controls. The programming style is primarily XAML text. However, you can also use C# in the code-behind file to access the Lync 2013 API and the .NET Framework.</p>
<p>Use the Lync 2013 API to start and automate the Lync UI in your business application, or add Lync functionality to new or existing .NET Framework applications and suppress the Lync UI. Lync SDK UI Automation automates the Lync UI, and Lync Controls add separate
 pieces of Lync functionality and UI as XAML controls.</p>
<p>Use the Lync 2010 API Reference to learn about the unmanaged Office Communicator Object Model API (OCOM). The OCOM API contains a subset of the types that are in the Lync 2013 API. You cannot start or carry on conversations with OCOM. But you can access
 a contact list and get contact presence.</p>
<h3 class="subHeading">Prerequisites for running and compiling sample in Visual Studio</h3>
<div class="subsection">
<p>This sample requires the following:</p>
<ul>
<li>
<p>.NET Framework 3.5 and later versions of .NET Framework</p>
</li><li>
<p>Visual Studio 2010 and later versions of Visual Studio</p>
</li><li>
<p>Lync 2010 SDK and later versions of the Lync SDK</p>
</li></ul>
</div>
<h3 class="subHeading">Prerequisites for running installed sample on client computer</h3>
<div class="subsection">
<p>This sample requires the following:</p>
<ul>
<li>
<p>A running instance of Lync 2010 or Lync 2013</p>
</li></ul>
</div>
<h3 class="subHeading">Run and test the sample</h3>
<div class="subsection">
<p>This sample was designed to be run locally on the computer running Lync 2013.</p>
<ol>
<li>
<p>Copy the AudioVideoConversation folder to a folder outside of the Program Files folder.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>The sample should run with the same privilege level as Lync.</p>
<p>Compiling it from Program Files prevents Lync from drawing the video in the conversation window of this sample.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>Open AddCustomGroup.csproj.</p>
</li><li>
<p>Start the Lync client or (preferably) select the UISuppressionMode mode.</p>
<div class="alert">
<table cellspacing="0" cellpadding="0" width="100%">
<tbody>
<tr>
<th align="left"><strong>Note</strong></th>
</tr>
<tr>
<td>
<p>Video only works With UISuppressionMode mode. For more information, see the registry files in the root folder of this sample, and see MainWindow.cs.</p>
</td>
</tr>
</tbody>
</table>
</div>
</li><li>
<p>In Visual Studio, press F5.</p>
</li></ol>
</div>
<h3 class="subHeading">Related content</h3>
<div class="subsection">
<p>Explore the following Lync developer resources.</p>
<ul>
<li>
<p><a href="http://msdn.microsoft.com/en-us/library/gg455051.aspx" target="_blank">MSDN Library: Lync</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/lync/gg132942.aspx" target="_blank">Lync Developer Center</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/office/aa905340.aspx" target="_blank">Office Developer Center</a></p>
</li><li>
<p><a href="http://msdn.microsoft.com/en-us/office/hh506337.aspx" target="_blank">Office 365 Developer Hub</a></p>
</li><li>
<p><a href="http://gallery.technet.microsoft.com/Lync-2010-API-Reference-48d2c5c9" target="_blank">Lync 2010 API Reference</a></p>
</li></ul>
</div>
</div>
</div>
</div>
