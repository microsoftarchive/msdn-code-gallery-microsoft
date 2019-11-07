# Lync 2013: Translate a conversation from one language to another in real time
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Lync 2013
## Topics
- Translation
## Updated
- 07/15/2012
## Description

<p><span style="font-size:small">The ConversationTranslator Lync SDK sample uses the
<strong>Conversation</strong> namespace from the Lync Model API to intercept instant messages and provide translation by using Bing Web Services.</span></p>
<p><span style="font-size:small">Features:</span><br>
<span style="font-size:small">&bull;&nbsp;Uses a sample architecture that registers and handles asynchronous Lync 2010 SDK events in Silverlight.</span><br>
<span style="font-size:small">&bull;&nbsp;Registers for two <strong>Conversation</strong> related events:
<strong>ParticipantAdded</strong> and <strong>InstantMessageReceived</strong>.</span><br>
<span style="font-size:small">&bull;&nbsp;Uses the Bing Translator service.</span></p>
<p><span style="font-size:small">Uses the following classes, events, and namespaces:</span></p>
<ul>
<li><span style="font-size:small"><strong>Conversation</strong> class</span> </li><li><span style="font-size:small"><strong>ConversationService </strong>class</span>
</li><li><span style="font-size:small"><strong>MessageError </strong>event</span> </li><li><span style="font-size:small"><strong>MessageReceived </strong>event</span> </li><li><span style="font-size:small"><strong>MessageSent </strong>event</span> </li><li><span style="font-size:small"><strong>ContactInformationType </strong>class</span>
</li><li><span style="font-size:small">System.Collections.ObjectModel namespace</span>
</li><li><span style="font-size:small">System.Diagnostics namespace</span> </li><li><span style="font-size:small">System.EventHandler namespace</span> </li><li><span style="font-size:small">System.ServiceModel namespace</span> </li><li><span style="font-size:small">System.ServiceModel.Channels namespace</span> </li><li><span style="font-size:small">System.ComponentModel namespace</span> </li><li><span style="font-size:small">http://api.microsofttranslator.com/V2 namespace</span>
</li><li><span style="font-size:small">http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2</span>
</li><li><span style="font-size:small">System.Runtime.Serialization namespace</span> </li><li><span style="font-size:small">System.Windows.Controls namespace</span> </li><li><span style="font-size:small">System.Windows.Documents namespace</span> </li><li><span style="font-size:small">System.Windows.Input namespace</span> </li><li><span style="font-size:small">System.Windows.Media namespace</span> </li><li><span style="font-size:small">System.Windows.Media.Animation namespace</span>
</li><li><span style="font-size:small">System.Windows.Shapes namespace</span> </li></ul>
<p><span style="font-size:small">The Lync SDK includes three development models:</span></p>
<ul>
<li><span style="font-size:small">Lync Controls</span> </li><li><span style="font-size:small">Lync API</span> </li><li><span style="font-size:small">OCOM Unmanaged COM API</span> </li></ul>
<p><span style="font-size:small">You can drag and drop Microsoft Lync Controls into existing business applications to add Lync functions and user interface. Each Lync Control provides a specific feature like search, presence, instant messaging (IM) calls, and
 audio calls. The appearance of each control replicates the Lync UI for that feature. Use a single control or multiple controls. The programming style is primarily XAML text, but you can also use C# in the code-behind file to access the Lync API and .NET Framework.</span></p>
<p><span style="font-size:small">Use the Lync API to start and automate the Lync UI in your business application, or add Lync functionality to new or existing .NET Framework applications and suppress the Lync UI. Lync SDK UI Automation automates the Lync UI,
 and Lync Controls add separate pieces of Lync functionality and UI as XAML controls.</span></p>
<p><span style="font-size:small">Use the Lync 2010 API Reference to learn about the unmanaged Office Communicator Object Model API (OCOM). The OCOM API contains a subset of the types that are in the Lync API. You cannot start or carry on conversations with
 OCOM, but you can access a contact list and get contact presence. </span></p>
<p><span style="font-size:small">It is not recommended that you use this API, but if you are a C&#43;&#43; developer and you need to add contact and presence features to your application, then this API can work for you.</span></p>
<h1>Prerequisites</h1>
<ul>
<li><span style="font-size:small">Microsoft Visual Studio 2010</span> </li><li><span style="font-size:small">Microsoft Silverlight 4 SDK</span> </li><li><span style="font-size:small">Microsoft Lync 2010 SDK and later versions of Lync SDK.</span>
</li><li><span style="font-size:small">Microsoft Lync 2010 client and later versions of the Lync client.</span>
</li><li><span style="font-size:small">Requires Internet access for the translation content service from Bing Translation.</span>
</li></ul>
<h1>Build the sample</h1>
<p><span style="font-size:small"><strong>Note</strong>: Ignore the Visual Studio warning about Web Services without Web Projects. The ConversationTranslator project uses an external Web Service.</span></p>
<h1>Related content</h1>
<ul>
<li><span style="font-size:small">Microsoft Silverlight 4 SDK download: <a href="http://www.microsoft.com/downloads/details.aspx?FamilyID=7F7119BF-5B56-4ACF-B489-9D717AFDB11A&amp;amp;displaylang=k&displaylang=en">
http://www.microsoft.com/downloads/details.aspx?FamilyID=7F7119BF-5B56-4ACF-B489-9D717AFDB11A&amp;amp;amp;displaylang=k&amp;displaylang=en</a></span>
</li><li><span style="font-size:small">Lync 2010 SDK download: <a href="http://www.microsoft.com/en-us/download/details.aspx?id=18898">
http://www.microsoft.com/en-us/download/details.aspx?id=18898</a></span> </li><li><span style="font-size:small">Lync 2010 API Reference: <a href="http://gallery.technet.microsoft.com/Lync-2010-API-Reference-48d2c5c9">
http://gallery.technet.microsoft.com/Lync-2010-API-Reference-48d2c5c9</a> </span>
</li><li><span style="font-size:small">Office 365 Developer Hub: <a href="http://msdn.microsoft.com/en-us/office/hh506337.aspx">
http://msdn.microsoft.com/en-us/office/hh506337.aspx</a> </span></li><li><span style="font-size:small">Lync Developer Center: <a href="http://msdn.microsoft.com/en-us/lync/gg132942.aspx">
http://msdn.microsoft.com/en-us/lync/gg132942.aspx</a> </span></li><li><span style="font-size:small">Office Developer Center: <a href="http://msdn.microsoft.com/en-us/office/aa905340.aspx">
http://msdn.microsoft.com/en-us/office/aa905340.aspx</a> </span></li></ul>
