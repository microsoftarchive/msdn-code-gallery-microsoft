# ASP.NET AJAX web chat application (VBASPNETAJAXWebChat)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- AJAX
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>VBASPNETAJAXWebChat Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
The project illustrates how to design a simple AJAX web chat application. <br>
We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.<br>
In this sample, we could create a chat room and invite someone<br>
else to join in the room and start to chat.<br>
<br>
Demo the Sample. <br>
<br>
Open the VBASPNETAJAXWebChat.sln directly, expand the <br>
WebChat web application node and press F5 to test the application.<br>
<br>
If you want to have a further test, please follow the demonstration step below.<br>
<br>
Step 1: Press F5 to open the default.aspx.<br>
<br>
Step 2: By default, we could see two chat rooms in the list, you can click the <br>
button, &quot;Create chat room&quot;, to create your own chat room. Before that button,<br>
we could see a textbox, we can input our chatting alias before joining into the<br>
room.<br>
<br>
Step 3: Click any &quot;Join&quot; button at the end of each row. You will see a popup<br>
chat room window.<br>
<br>
Step 4: Open a new browser and make the same steps to impersonate another user<br>
to join in the same chat room.<br>
<br>
Step 5: When we input one message, we will see both of the chat room windows<br>
will show the messages.<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step 1. &nbsp;Create an ASP.NET Web Application. In this sample it is &quot;WebChat&quot;.<br>
<br>
Step 2. &nbsp;Right-click the App_Data directory, and click Add -&gt; New Item -&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; SQL Server DataBase. In this sample it is &quot;SessionDB.mdf&quot;.<br>
<br>
Step 3. &nbsp;Open the database file and create four tables.<br>
&nbsp; &nbsp; &nbsp; &nbsp; * tblChatRoom: store chat room data.<br>
&nbsp; &nbsp; &nbsp; &nbsp; * tblMessagePool: store the chat message data temporarily.<br>
&nbsp; &nbsp; &nbsp; &nbsp; * tblSession: store the user session data.<br>
&nbsp; &nbsp; &nbsp; &nbsp; * tblTalker: store the user data who in the chat rooms.<br>
&nbsp; &nbsp; &nbsp; &nbsp; The details columns for these tables, please refer to the SessionDB.mdf<br>
&nbsp; &nbsp; &nbsp; &nbsp; in this sample.<br>
<br>
Step 4. &nbsp;Create a new directory, &quot;Data&quot;. Right-click the directory and click<br>
&nbsp; &nbsp; &nbsp; &nbsp; Add -&gt; New Item -&gt; Linq to SQL classes.(If you could not find that
<br>
&nbsp; &nbsp; &nbsp; &nbsp; template, please click the Data node of the tree view at the left<br>
&nbsp; &nbsp; &nbsp; &nbsp; hand.) In this sample, it is SessionDB.dbml.<br>
<br>
Step 5. &nbsp;Open the SessionDB.dbml and double-click the SessionDB.mdf, you will
<br>
&nbsp; &nbsp; &nbsp; &nbsp; see the database in the Server Explorer. Expand the SessionDB.mdf,<br>
&nbsp; &nbsp; &nbsp; &nbsp; expand the Tables folder, and select the four tables, and drag them<br>
&nbsp; &nbsp; &nbsp; &nbsp; all to the stage of the SessionDB.dbml.<br>
<br>
Step 6. &nbsp;Create a new directory, &quot;Logic&quot;. We need to create some class files.<br>
&nbsp; &nbsp; &nbsp; &nbsp; * ChatManager.vb: we have some static methods to control the data in
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; the database by using Linq.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * ChatRoom.vb: it is a DataContract used to send the chat room data<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;to the client side by WCF service.<br>
&nbsp; &nbsp; &nbsp; &nbsp; * Message.vb: it is a DataContract used to send the message data<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;to the client side by WCF service.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * RoomTalker.vb: it is a DataContract used to send the talker data in<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;one chat room to the client side by WCF service.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;For the details of these classes, please refer to these four files in<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;this sample.<br>
<br>
Step 7. &nbsp;Create a new directory, &quot;Services&quot;. Right-click the directory and click<br>
&nbsp; &nbsp; &nbsp; &nbsp; Add -&gt; New Item -&gt; AJAX-enabled WCF service. In this sample, it is
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Transition.svc.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Add these two attributes for that class to make sure that the session is<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; allowed.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [ServiceContract(Namespace = &quot;<a target="_blank" href="http://VBASPNETAJAXWebChat&quot;,">http://VBASPNETAJAXWebChat&quot;,</a> SessionMode = SessionMode.Allowed)]<br>
&nbsp; &nbsp; &nbsp; &nbsp; [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [/CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; In this file, we create some WCF service method which could be used by<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; the client side to execute.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; For the details of these classes, please refer to the Transition.svc<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; in this sample.<br>
<br>
Step 8. &nbsp;Create a new directory, &quot;Scripts&quot;. Right-click the directory and click<br>
&nbsp; &nbsp; &nbsp; &nbsp; Add -&gt; New Item -&gt; Jscript File. We need to create some js files to<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; call the WCF service from client side. And there are some page logic
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; codes for this sample; they could be defined by user requirement.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ASP.NET AJAX allows us to add some service references. So the
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ScriptManager will generate the client service contract scripts<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; automatically. And what we need to do is just call the service method<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; like we use them at server side. For example, we call the LeaveChatRoom<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; method in the Transition.svc, we could write the JavaScript function like<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; this:<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; vbaspnetajaxwebchat.transition.LeaveChatRoom(RoomID,SuccessCallBack,FailCallBack);<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [/CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * vbaspnetajaxwebchat is the namespace for this application.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * transition is the service name.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * LeaveChatRoom is the method name.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Because of this method has one parameter, the RoomID stands for<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; that parameter, if we have two or more parameters for one<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; methed, just write them before SuccessCallBack.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * SuccessCallBack is a function used to be fired when the service<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; called successfully.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; * FailCallBack is a function used to be fired when the service<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; called unsuccessfully.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; For more details about these script functions, please refer to the
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; files in this sample.(chatbox.js, chatMessage.js, chatRoom.js)<br>
<br>
Step 9. &nbsp;Open the Default.aspx,(If there is no Default.aspx, create one.)<br>
&nbsp; &nbsp; &nbsp; &nbsp; Create a ScriptManager control and add a service reference and<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; a script reference like below.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&lt;asp:ScriptManager ID=&quot;ScriptManager1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;Services&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:ServiceReference Path=&quot;~/Services/Transition.svc&quot; /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/Services&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;Scripts&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:ScriptReference Path=&quot;~/Scripts/chatbox.js&quot; /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/Scripts&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;/asp:ScriptManager&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [/CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; In the Head block, add some js and css references from the CDN.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;script type=&quot;text/javascript&quot; src=&quot;<a target="_blank" href="http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.min.js&quot;&gt;&lt;/script&gt;">http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.min.js&quot;&gt;&lt;/script&gt;</a><br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;script type=&quot;text/javascript&quot; src=&quot;<a target="_blank" href="http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/jquery-ui.min.js&quot;&gt;&lt;/script&gt;">http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/jquery-ui.min.js&quot;&gt;&lt;/script&gt;</a><br>
&nbsp; &nbsp; &nbsp; &nbsp; &lt;script type=&quot;text/javascript&quot; src=&quot;scripts/chatRoom.js&quot;&gt;&lt;/script&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &lt;link rel=&quot;Stylesheet&quot; type=&quot;text/css&quot; href=&quot;<a target="_blank" href="http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/themes/dark-hive/jquery-ui.css&quot;">http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/themes/dark-hive/jquery-ui.css&quot;</a>
 /&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp; [/CODE]<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; We use these references to make this sample to write code easier<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; and looks better.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; There are some other UI markups, please refer to the Default.aspx<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; in this sample.<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; For more details about CDN, please view the links listed in the<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; References part at the end of this ReadMe file.<br>
<br>
Step 10. Create a new web page. In this sample, it is &quot;ChatBox.aspx&quot;. <br>
&nbsp; &nbsp; &nbsp; &nbsp; In that page, we create some UI controls to send and recieve<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; messages. For more details, please refer to the ChatBox.aspx<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; in this sample.<br>
<br>
Step 11. Everything is ready, test the application and hope you can get a <br>
&nbsp; &nbsp; &nbsp; &nbsp; smile.<br>
&nbsp; <br>
<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: How to: Create an AJAX-Enabled WCF Service and an ASP.NET Client that Accesses the Service<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb924552.aspx">http://msdn.microsoft.com/en-us/library/bb924552.aspx</a><br>
<br>
MSDN: LINQ to SQL: .NET Language-Integrated Query for Relational Data<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb425822.aspx">http://msdn.microsoft.com/en-us/library/bb425822.aspx</a><br>
<br>
MSDN: An Introduction to JavaScript Object Notation (JSON) in JavaScript and .NET<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb299886.aspx">http://msdn.microsoft.com/en-us/library/bb299886.aspx</a><br>
<br>
MSDN: Explore Rich Client Scripting With jQuery<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/magazine/dd453033.aspx">http://msdn.microsoft.com/en-us/magazine/dd453033.aspx</a><br>
<br>
ASP.NET: Microsoft Ajax Content Delivery Network(Microsoft Ajax CDN)<br>
<a target="_blank" href="http://www.asp.net/ajaxlibrary/cdn.ashx">http://www.asp.net/ajaxlibrary/cdn.ashx</a><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
