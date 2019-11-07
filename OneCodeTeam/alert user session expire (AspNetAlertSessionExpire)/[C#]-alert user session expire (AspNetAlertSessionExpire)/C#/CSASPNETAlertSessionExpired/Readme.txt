========================================================================
                   CSASPNETAlertSessionExpired Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The project illustrates how to design a simple user control, which is used to 
alert the user when the session is about to expired. 


/////////////////////////////////////////////////////////////////////////////
Demo the Sample. 

Open the CSASPNETAlertSessionExpired.sln with Visual Studio2010, extend the 
CSASPNETAlertSessionExpired web application node and press F5 to test the application.

If you want to have a further test, please follow the demonstration step below.

Step 1: Press F5 to open the default.aspx.

Step 2: By default, we can see a "GetSessionState" button, you can click the 
button to query the SessionState.

Step 3: The session is set expired in 1 minute, So idle the page for 30 seconds.

Step 4: After 30 seconds, you will see an alert message "Your session will be expired
after 30 seconds, would you like to extend the session time?" You can choose 
"Yes" or "No".

Step 5: If you choose "Yes", the session will be reset. You can click the "GetSessionState" 
button to query the session status, you will see the SessionState: Existing.

Step 6: If you choose "No", the "GetSessionState" button will be disabled temporarily.
When the session is really expired you can click the "GetSessionState" button, 
the page will redirect to the SessionExpired page.


/////////////////////////////////////////////////////////////////////////////
Code Logical:

We use jQuery, ASP.NET AJAX at client side.
In this sample, we add a SessionExpired user control and Script Manager on
the Master page. It will display an alert message if user idle for long time, user 
can choose whether to extend the session before it's expired or not.

Step 1.  Create an ASP.NET Web Application. In this sample it is 
         "CSASPNETAlertSessionExpired".

Step 2.  Right-click the CSASPNETAlertSessionExpired , and click Add -> New Item ->
         New Folder "Controls".
		 Right-click The Controls directory, and click Add-> New Item -> Web
		  User Control name as "SessionExpired.ascx":
		 * SessionExpired.ascx.cs: 
			 1、The SessionExpired class inherits from 
			 ICallHandlerEvent which is used to indicate that the control can be
			 the target of a callback event on the server. Then it will extend
			 the Session's lifetime. 
			 2、Register the Session's timeout value to the client so that user
			 can extend the Session's lifetime before it expired.
			 3、Verify the Session is expired or not.
	     * SessionExpired.ascx:
		     It will get the Session's timeout value and request the server
	         interval by async method.
			 [CODE]
		     if (!IsPostBack)
             {
                Session["SessionForTest"] = "SessionForTest";
             }
		     [/CODE]

		     Add the Session state judgment like:
		     [CODE]
		     if (Session["SessionForTest"] == null)
             {

                lbSessionState.Text = SessionStates.Expired.ToString();
             }
             else
             {
                lbSessionState.Text = SessionStates.Exist.ToString();

             }
		     [/CODE]

		  
Step 3.  Right-click the CSASPNETAlertSessionExpired , and click Add -> New Item ->
         New Folder "Util".
		 Create a menu type which is used to output the session state consistent.

Step 4.  Open the Site.Master,(If there is no Site.Master, create one.)
         add a user control reference and
		 a script reference like below.
		 [CODE]
		  <uc:SessionExpired ID="ucSessionExpired" runat="server" />
		 [/CODE]

Step 5.  Open the Default.aspx(If there is no Default.aspx, create one which 
         is a Web Form using Master Page)
		 Add the scripts and css reference  like below:
		 <script  type="text/javascript"  language="javascript" src="Scripts/jquery-1.4.1-vsdoc.js">
		 </script>
         <script type="text/javascript" language="javascript" src="Scripts/SessionExpired.js">
		 </script>
         <link href="Styles/SessionExpired.css"  rel="stylesheet" type="text/css" />
		  We use these references to make this sample to write code easier
		 and looks better.

Step 6.  Open the Web.config(If there is no Web.config, create one.)
         Set the Session timeout value like below:
		 [CODE]
		 <system.web>
            <sessionState timeout="1"></sessionState>
         </system.web>
		 [/CODE]


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: How to: Implement Callbacks in ASP.NET Web Pages
http://msdn.microsoft.com/en-us/library/ms366518.aspx

MSDN: How to: Save Values in Session State
http://msdn.microsoft.com/en-us/library/6ad7zeeb(VS.90).aspx

MSDN: ASP.NET Session State Overview
http://msdn.microsoft.com/en-us/library/ms178581.aspx

MSDN: Working with ASP.NET Master Pages Programmatically
http://msdn.microsoft.com/en-us/library/c8y19k6h.aspx


/////////////////////////////////////////////////////////////////////////////