========================================================================
                   VBASPNETAJAXWebChat Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

The project illustrates how to design a simple AJAX web chat application. 
We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
In this sample, we could create a chat room and invite someone
else to join in the room and start to chat.

/////////////////////////////////////////////////////////////////////////////
Demo the Sample. 

Open the VBASPNETAJAXWebChat.sln directly, expand the 
WebChat web application node and press F5 to test the application.

If you want to have a further test, please follow the demonstration step below.

Step 1: Press F5 to open the default.aspx.

Step 2: By default, we could see two chat rooms in the list, you can click the 
button, "Create chat room", to create your own chat room. Before that button,
we could see a textbox, we can input our chatting alias before joining into the
room.

Step 3: Click any "Join" button at the end of each row. You will see a popup
chat room window.

Step 4: Open a new browser and make the same steps to impersonate another user
to join in the same chat room.

Step 5: When we input one message, we will see both of the chat room windows
will show the messages.

/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step 1.  Create an ASP.NET Web Application. In this sample it is "WebChat".

Step 2.  Right-click the App_Data directory, and click Add -> New Item ->
         SQL Server DataBase. In this sample it is "SessionDB.mdf".

Step 3.  Open the database file and create four tables.
         * tblChatRoom: store chat room data.
         * tblMessagePool: store the chat message data temporarily.
         * tblSession: store the user session data.
         * tblTalker: store the user data who in the chat rooms.
         The details columns for these tables, please refer to the SessionDB.mdf
         in this sample.

Step 4.  Create a new directory, "Data". Right-click the directory and click
         Add -> New Item -> Linq to SQL classes.(If you could not find that 
         template, please click the Data node of the tree view at the left
         hand.) In this sample, it is SessionDB.dbml.

Step 5.  Open the SessionDB.dbml and double-click the SessionDB.mdf, you will 
         see the database in the Server Explorer. Expand the SessionDB.mdf,
         expand the Tables folder, and select the four tables, and drag them
         all to the stage of the SessionDB.dbml.

Step 6.  Create a new directory, "Logic". We need to create some class files.
         * ChatManager.vb: we have some static methods to control the data in 
		                   the database by using Linq.
		 * ChatRoom.vb: it is a DataContract used to send the chat room data
		                to the client side by WCF service.
         * Message.vb: it is a DataContract used to send the message data
		                to the client side by WCF service.
		 * RoomTalker.vb: it is a DataContract used to send the talker data in
		                one chat room to the client side by WCF service.
		For the details of these classes, please refer to these four files in
		this sample.

Step 7.  Create a new directory, "Services". Right-click the directory and click
         Add -> New Item -> AJAX-enabled WCF service. In this sample, it is 
		 Transition.svc.
		 Add these two attributes for that class to make sure that the session is
		 allowed.
		 [CODE]
		 [ServiceContract(Namespace = "http://VBASPNETAJAXWebChat", SessionMode = SessionMode.Allowed)]
         [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
		 [/CODE]
		 In this file, we create some WCF service method which could be used by
		 the client side to execute.
		 For the details of these classes, please refer to the Transition.svc
		 in this sample.

Step 8.  Create a new directory, "Scripts". Right-click the directory and click
         Add -> New Item -> Jscript File. We need to create some js files to
		 call the WCF service from client side. And there are some page logic 
		 codes for this sample; they could be defined by user requirement.
		 ASP.NET AJAX allows us to add some service references. So the 
		 ScriptManager will generate the client service contract scripts
		 automatically. And what we need to do is just call the service method
		 like we use them at server side. For example, we call the LeaveChatRoom
		 method in the Transition.svc, we could write the JavaScript function like
		 this:
		 [CODE]
		 vbaspnetajaxwebchat.transition.LeaveChatRoom(RoomID,SuccessCallBack,FailCallBack);
		 [/CODE]
		 * vbaspnetajaxwebchat is the namespace for this application.
		 * transition is the service name.
		 * LeaveChatRoom is the method name.
		 Because of this method has one parameter, the RoomID stands for
		 that parameter, if we have two or more parameters for one
		 methed, just write them before SuccessCallBack.
		 * SuccessCallBack is a function used to be fired when the service
		 called successfully.
		 * FailCallBack is a function used to be fired when the service
		 called unsuccessfully.
		 For more details about these script functions, please refer to the 
		 files in this sample.(chatbox.js, chatMessage.js, chatRoom.js)

Step 9.  Open the Default.aspx,(If there is no Default.aspx, create one.)
         Create a ScriptManager control and add a service reference and
		 a script reference like below.
		 [CODE]
		  <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/Transition.svc" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/chatbox.js" />
            </Scripts>
         </asp:ScriptManager>
		 [/CODE]
		 In the Head block, add some js and css references from the CDN.
		 [CODE]
		 <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jQuery/jquery-1.4.2.min.js"></script>
         <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/jquery-ui.min.js"></script>
         <script type="text/javascript" src="scripts/chatRoom.js"></script>
	     <link rel="Stylesheet" type="text/css" href="http://ajax.microsoft.com/ajax/jquery.ui/1.8.5/themes/dark-hive/jquery-ui.css" />
	     [/CODE]
		 We use these references to make this sample to write code easier
		 and looks better.
		 There are some other UI markups, please refer to the Default.aspx
		 in this sample.
		 For more details about CDN, please view the links listed in the
		 References part at the end of this ReadMe file.

Step 10. Create a new web page. In this sample, it is "ChatBox.aspx". 
         In that page, we create some UI controls to send and recieve
		 messages. For more details, please refer to the ChatBox.aspx
		 in this sample.

Step 11. Everything is ready, test the application and hope you can get a 
         smile.
   



/////////////////////////////////////////////////////////////////////////////
References:

MSDN: How to: Create an AJAX-Enabled WCF Service and an ASP.NET Client that Accesses the Service
http://msdn.microsoft.com/en-us/library/bb924552.aspx

MSDN: LINQ to SQL: .NET Language-Integrated Query for Relational Data
http://msdn.microsoft.com/en-us/library/bb425822.aspx

MSDN: An Introduction to JavaScript Object Notation (JSON) in JavaScript and .NET
http://msdn.microsoft.com/en-us/library/bb299886.aspx

MSDN: Explore Rich Client Scripting With jQuery
http://msdn.microsoft.com/en-us/magazine/dd453033.aspx

ASP.NET: Microsoft Ajax Content Delivery Network(Microsoft Ajax CDN)
http://www.asp.net/ajaxlibrary/cdn.ashx

/////////////////////////////////////////////////////////////////////////////