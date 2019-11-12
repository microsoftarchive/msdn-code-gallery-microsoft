# ASP.NET AJAX web chat application
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- AJAX
- Web chat
## Updated
- 01/15/2013
## Description

<h1>How to design a simple AJAX web chat (CSASPNETAJAXWebChat)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The project illustrates how to design a simple AJAX web chat application. We use jQuery, SP.NET AJAX at client side and LINQ to SQL at server side. In this sample, we could create a chat room and invite someone else to join in the room
 and start to chat.</p>
<h2>Running the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">Step 1:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the CSASPNETAJAXWebChat.sln. Press F5 to open the default.aspx.</p>
<p class="MsoListParagraphCxSpMiddle"><span style=""><img src="74670-image.png" alt="" width="346" height="96" align="middle">
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 2:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>By default, we could see two chat rooms in the list, you can click the button, &quot;Create chat room&quot;, to create your own chat room. Before that button, we could see a textbox, we can input our chatting alias before joining into the
 room.</p>
<p class="MsoListParagraphCxSpMiddle"><span style=""><img src="74671-image.png" alt="" width="547" height="182" align="middle">
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 3:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Click any &quot;Join&quot; button at the end of each row. You will see a popup chat room window.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 4:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open a new browser and make the same steps to impersonate another user to join in the same chat room.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 5:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>When we input one message, we will see both of the chat room windows will show the messages.</p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">Step 6:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Validation is finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal"></p>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">Step 1:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create an &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010/Visual Web Developer 2010. In this sample it is &quot;WebChat&quot;.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 2:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Right-click the App_Data directory, and click Add -&gt; New Item -&gt; SQL Server DataBase. In this sample it is &quot;SessionDB.mdf&quot;.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 3:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the database file and create four tables.<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>* tblChatRoom: store chat room data.<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>* tblMessagePool: store the chat message data temporarily.<br>
<span style="">&nbsp;</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>* tblSession: store the user session data.<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>* tblTalker: store the user data who in the chat rooms.<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>The details columns for these tables, please refer to the SessionDB.mdf in this sample.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 4:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a new directory, &quot;Data&quot;. Right-click the directory and click Add -&gt; New Item -&gt; Linq to SQL classes.(If you could not find that template, please click the Data node of the tree view at the left hand.) In this sample,
 it is SessionDB.dbml.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 5:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the SessionDB.dbml and double-click the SessionDB.mdf, you will see the database in the Server Explorer. Expand the SessionDB.mdf, expand the Tables folder, and select the four tables, and drag them all to the stage of the SessionDB.dbml.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 6:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a new directory, &quot;Logic&quot;. We need to create some class files.<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>* ChatManager.cs: we have some static methods to control the data in the database by using Linq.<br>
<span style="">&nbsp; </span><span style="">&nbsp;</span>* ChatRoom.cs: it is a DataContract used to send the chat room data<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>to the client side by WCF service.<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp; </span>* Message.cs: it is a DataContract used to send the message data<span style="">&nbsp;
</span>to the client side by WCF service.<br>
<span style="">&nbsp;</span>* RoomTalker.cs: it is a DataContract used to send the talker data in one chat room to the client side by WCF service.<br>
For the details of these classes, please refer to these four files in this sample.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 7:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a new directory, &quot;Services&quot;. Right-click the directory and click<br>
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>Add -&gt; New Item -&gt; AJAX-enabled WCF service. In this sample, it is Transition.svc.
</p>
<p class="MsoListParagraphCxSpLast">Add these two attributes for that class to make sure that the session is allowed.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
[ServiceContract(Namespace = &quot;http://CSASPNETAJAXWebChat&quot;, SessionMode = SessionMode.Allowed)]
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst">In this file, we create some WCF service method which could be used by<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>the client side to execute.<br>
<span style="">&nbsp;</span>For the details of these classes, please refer to the Transition.svc in this sample.<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 8:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a new directory, &quot;Scripts&quot;. Right-click the directory and click
</p>
<p class="MsoListParagraphCxSpLast"><span style="">&nbsp; </span>Add -&gt; New Item -&gt; Jscript File. We need to create some js files to call the WCF service from client side. And there are some page logic codes for this sample; they could be defined by user
 requirement.<br>
<span style="">&nbsp;</span>ASP.NET AJAX allows us to add some service references. So the ScriptManager will generate the client service contract scripts automatically. And what we need to do is just call the service method like we use them at server side. For example,
 we call the LeaveChatRoom method in the Transition.svc, we could write the JavaScript function like this:
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
csaspnetajaxwebchat.transition.LeaveChatRoom(RoomID,SuccessCallBack,FailCallBack);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style="">&nbsp;</span>* csaspnetajaxwebchat is the namespace for this application.
</p>
<p class="MsoListParagraphCxSpMiddle">* transition is the service name. </p>
<p class="MsoListParagraphCxSpMiddle"><span style="">&nbsp;</span>* LeaveChatRoom is the method name.
</p>
<p class="MsoListParagraphCxSpMiddle"><span style="">&nbsp;</span>Because of this method has one parameter, the RoomID stands for that parameter, if we have two or more parameters for one methed, just write them before SuccessCallBack.<br>
<span style="">&nbsp;</span>* SuccessCallBack is a function used to be fired when the service called successfully.<br>
* FailCallBack is a function used to be fired when the service called unsuccessfully.
</p>
<p class="MsoListParagraphCxSpMiddle">For more details about these script functions, please refer to the files in this sample.(chatbox.js, chatMessage.js, chatRoom.js).</p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">Step 9:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the Default.aspx,(If there is no Default.aspx, create one.) Create a ScriptManager control and add a service reference and a script reference like below.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;asp:ScriptManager ID=&quot;ScriptManager1&quot; runat=&quot;server&quot;&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Services&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;asp:ServiceReference Path=&quot;~/Services/Transition.svc&quot; /&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/Services&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;Scripts&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;asp:ScriptReference Path=&quot;~/Scripts/chatbox.js&quot; /&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/Scripts&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/asp:ScriptManager&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style=""><span style=""><span style="">Step 10:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In the Head block, add some js and css references from the CDN.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>

<pre id="codePreview" class="html">
&lt;script type=&quot;text/javascript&quot; src=&quot;http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.min.js&quot;&gt;&lt;/script&gt;&lt;script type=&quot;text/javascript&quot; src=&quot;http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/jquery-ui.min.js&quot;&gt;&lt;/script&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;script type=&quot;text/javascript&quot; src=&quot;scripts/chatRoom.js&quot;&gt;&lt;/script&gt;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;link rel=&quot;Stylesheet&quot; type=&quot;text/css&quot; href=&quot;http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/themes/dark-hive/jquery-ui.css&quot; /&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst">We use these references to make this sample to write code easier and looks better.
</p>
<p class="MsoListParagraphCxSpMiddle">There are some other UI markups, please refer to the Default.aspx in this sample.
</p>
<p class="MsoListParagraphCxSpMiddle"><span style="">&nbsp;</span>For more details about CDN, please view the links listed in the References part at the end of this ReadMe file.</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">Step 11:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a new web page. In this sample, it is &quot;ChatBox.aspx&quot;.
<span style="">&nbsp;</span>In that page, we create some UI controls to send and receive messages. For more details, please refer to the ChatBox.aspx in this sample.</p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">Step 12:<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Build the application and you can debug it.</p>
<h2>More Information</h2>
<p class="MsoNormal">MSDN: How to: Create an AJAX-Enabled WCF Service and an ASP.NET Client that Accesses the Service<br>
<a href="http://msdn.microsoft.com/en-us/library/bb924552.aspx">http://msdn.microsoft.com/en-us/library/bb924552.aspx</a><br>
MSDN: LINQ to SQL: .NET Language-Integrated Query for Relational Data<br>
<a href="http://msdn.microsoft.com/en-us/library/bb425822.aspx">http://msdn.microsoft.com/en-us/library/bb425822.aspx</a><br>
MSDN: An Introduction to JavaScript Object Notation (JSON) in JavaScript and .NET<br>
<a href="http://msdn.microsoft.com/en-us/library/bb299886.aspx">http://msdn.microsoft.com/en-us/library/bb299886.aspx</a><br>
MSDN: Explore Rich Client Scripting With jQuery<br>
<a href="http://msdn.microsoft.com/en-us/magazine/dd453033.aspx">http://msdn.microsoft.com/en-us/magazine/dd453033.aspx</a><br>
ASP.NET: Microsoft Ajax Content Delivery Network(Microsoft Ajax CDN)<br>
<a href="http://www.asp.net/ajaxlibrary/cdn.ashx">http://www.asp.net/ajaxlibrary/cdn.ashx</a><br style="">
<br style="">
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
