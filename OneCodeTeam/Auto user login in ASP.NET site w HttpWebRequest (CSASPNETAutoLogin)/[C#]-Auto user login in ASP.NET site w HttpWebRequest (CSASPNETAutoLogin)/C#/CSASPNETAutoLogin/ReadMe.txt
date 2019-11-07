=============================================================================
                     CSASPNETAutoLogin Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The project illustrates how to log a user into site using HttpWebRequest Class 
without filling username and password box.


/////////////////////////////////////////////////////////////////////////////
Demo

Click the CSASPNETAutoLogin.sln directly and open the CSASPNETAutoLogin website 
to test the page directly.

If you want to have a further test, please follow the demonstration steps 
below.

Step 1: In the Login.aspx page, you fill your username and password,and click
Login button. the page will prompt that you have login this page, and display your 
name and password,it proves that you have login the page successfully, it is 
the normal method that we login the site.

Step 2: In the AutoLogin.aspx page,you also fill your username and password, and
click the AutoLogin button,the page will display the username and password.
But in the codebehind, we take the httpwebrequest to submit the current
usename and password to the login.aspx instead of the current page. it shows
the same information with the login.aspx page, it proves that we submit the 
current username and password of autologin.aspx page to the login.aspx successfully.


/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step 1. we request the login.aspx by using HttpWebRequest get method and
get the return login page html code. 

Step 2. We substring the login page html code to get the __VIEWSTATE,
__EVENTVALIDATION parameters.

Step 3. We concat the __VIEWSTATE,__EVENTVALIDATION,Username, password and 
loginbutton to the whole string and convert it to byte array.

Step 4. Submit the request data and get the return data and display it.

 

/////////////////////////////////////////////////////////////////////////////
References:

MSDN: HttpWebRequest and HttpWebResponse Class
http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.aspx
http://msdn.microsoft.com/en-us/library/system.net.httpwebresponse.aspx

MSDN: http://msdn.microsoft.com/en-us/library/debx8sh9.aspx
http://msdn.microsoft.com/en-us/library/debx8sh9.aspx

Automatic Login into a website in C#.net
http://forums.asp.net/t/1507150.aspx

How to use HttpWebRequest to send POST request to another web server
http://www.netomatix.com/httppostdata.aspx


/////////////////////////////////////////////////////////////////////////////