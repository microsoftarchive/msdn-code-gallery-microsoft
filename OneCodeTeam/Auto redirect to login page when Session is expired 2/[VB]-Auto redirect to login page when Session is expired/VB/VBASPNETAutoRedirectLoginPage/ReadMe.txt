========================================================================
               VBASPNETAutoRedirectLoginPage Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

The project illustrates how to develop an asp.net code-sample that will be 
redirect to login page when page Session is expired or time out automatically. 
It will ask the user to extend the Session at one minutes before
expiring. If the user does not has any actions, the web page will be redirected 
to login page automatically, and also note that it need to work in one or
more browser's tabs. 

/////////////////////////////////////////////////////////////////////////////
Demo the Sample. 

Please follow these demonstration steps below.

Step 1: Open the CSASPNETAutoRedirectLoginPage.sln.

Step 2: Expand the CSASPNETAutoRedirectLoginPage web application and press 
        Ctrl + F5 to show the LoginPage.aspx page.

Step 3: You will see login page and input the user name and pass word, and then
        click the login button to redirect the user page.

Step 4: Now you can open more user pages or refresh the user page for persisting
        user's activity. 

Step 5: When you stop operating the pages and wait for 1 minute, the all web
        pages of code-sample will be redirected the login page automatically. For 
		easy testing, the code-sample set the session's timeout to 1 minute.

Step 5: Validation finished.

/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step 1. Create a VB "ASP.NET Empty Web Application" in Visual Studio 2010 or
        Visual Web Developer 2010. Name it as "VBASPNETAutoRedirectLoginPage".

Step 2. Add a web form named "LoginUser.aspx" for user login in website,
        and this page can prevent users who want to skip the login step
	    by visit specified pages.

		The login page code will be like this:
		[code]
		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' Prevent the users who try to skip the login step by visit specified page.
            If Not Page.IsPostBack Then
                Session.Abandon()
            End If
            If Request.QueryString("info") IsNot Nothing Then
                Dim message As String = Request.QueryString("info").ToString()
                If message = "0" Then
                    Response.Write("<strong>you need login first to visit user page.</strong>")
                End If
            End If

        End Sub

        ''' <summary>
        ''' User login method.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click
            Dim username As String = tbUserName.Text.Trim()
            If tbUserName.Text.Trim() = "username" AndAlso tbPassword.Text.Trim() = "password" Then
                Session("username") = username
                Response.Redirect("UserPage2.aspx")
            Else
                Response.Write("<strong>User name or pass word error.</strong>")
            End If

        End Sub
		[/code]

Step 3. Add two user pages and named them as "UserPage1.aspx", "UserPage2.aspx".
        These two user pages are use to observe the web application and redirect the login
		page automatically. You can also open multiple tabs or windows in your browser. 

Step 4. Add a user control on the root dictionary and drag and drop it in the 
        every user page for redirecting to the login page when session is expired or
		time out. The user control's JavaScript code and C# code will be like this:

		JavaScript code:
		[code]
		<script type="text/javascript">
        var timeRefresh;
        var timeInterval;
        var currentTime;
		var expressTime;

        expressTime = "<%=ExpressDate %>";
        currentTime = "<%=LoginDate %>";
        setCookie("express", expressTime);
        timeRefresh = setInterval("Refresh()", 1000);

        // Refresh this page to check session is expire or timeout.
        function Refresh() {
            var current = getCookie("express");
            var date = current.split(" ")[0];
            var time = current.split(" ")[1];
            var scriptDate = new Date();
            var year = scriptDate.getFullYear();
            var month = scriptDate.getMonth() + 1;
            var day = scriptDate.getDate();
            var hour = scriptDate.getHours();
            var min = scriptDate.getMinutes();
            var second = scriptDate.getSeconds();
            if (Date.UTC(year, month, day, hour, min, second) >=
                Date.UTC(date.split("-")[0], date.split("-")[1], date.split("-")[2],
                time.split(":")[0], time.split(":")[1], time.split(":")[2])) {
                clearInterval(timeRefresh);
                Redirect();
            }
         }
 
         function Redirect() {
             window.location.replace("LoginPage.aspx");
         }

         // Retrieve cookie by name.
         function getCookie(name) {
            var arg = name + "=";
            var aLen = arg.length;
            var cLen = document.cookie.length;
            var i = 0;
            while (i < cLen) {
                 var j = i + aLen;
                 if (document.cookie.substring(i, j) == arg) {
                 return getCookieVal(j);
                 }
                 i = document.cookie.indexOf(" ", i) + 1;
                 if (i == 0) break;
            }
             return;
         }

         function getCookieVal(offset) {
             var endStr = document.cookie.indexOf(";", offSet);
             if (endStr == -1) {
                 endStr = document.cookie.length;
             }
             return unescape(document.cookie.substring(offSet, endStr));
         }

        // Assign values to cookie variable.
        function setCookie(name, value) {
             document.cookie = name + "=" + escape(value);
        }
        </script>
		[/code]

		The VB code:
		[code]
		Public LoginDate As String
        Public ExpressDate As String

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' Check session is expire or timeout.
            If Session("username") Is Nothing Then
                Response.Redirect("LoginPage.aspx?info=0")
            End If

            ' Get user login time or last activity time.
            Dim [date] As DateTime = DateTime.Now
            LoginDate = [date].ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "")
            Dim sessionTimeout As Integer = Session.Timeout
            Dim dateExpress As DateTime = [date].AddMinutes(sessionTimeout)
            ExpressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "")
        End Sub
		[/code]

Step 5. Build the application and you can debug it.
/////////////////////////////////////////////////////////////////////////////
References:

MSDN: ASP.NET User Controls
http://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx

MSDN: HttpCookie Class
http://msdn.microsoft.com/en-us/library/system.web.httpcookie.aspx
/////////////////////////////////////////////////////////////////////////////