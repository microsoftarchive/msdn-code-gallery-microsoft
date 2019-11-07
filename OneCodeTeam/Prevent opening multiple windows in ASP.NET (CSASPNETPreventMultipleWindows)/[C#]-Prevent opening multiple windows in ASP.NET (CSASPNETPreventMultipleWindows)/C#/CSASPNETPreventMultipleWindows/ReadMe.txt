========================================================================
            CSASPNETPreventMultipleWindows Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

The project illustrates how to detect and prevent multiple windows or 
tab usage in Web Applications.

/////////////////////////////////////////////////////////////////////////////
Demo the Sample. 

Please follow these demonstration steps below.

Step 1: Open the CSASPNETPreventMultipleWindows.sln.

Step 2: Expand the CSASPNETPreventMultipleWindows web application and press 
        Ctrl + F5 to show the Default.aspx.

Step 3: We will see two links on the page. First, you can Left click one
        of them jump to correct link(Like Nextlink.aspx and Nextlink2.aspx).

Step 4: Then, you can right click these links and choose Open In New Tab 
        or Open In New Window,and you will find the link not available.

Step 5: you can even copy the right url and Paste it in the browser address bar for 
        test.but you can not achieve your goal.

Step 6: Validation finished.

/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step 1. Create a C# "ASP.NET Empty Web Application" in Visual Studio 2010 or
        Visual Web Developer 2010. Name it as "CSASPNETPreventMultipleWindows".

Step 2. Add one folder, "UserControls".Add two User Controls in this folder,
        "DefaultPage.ascx","NextPage.ascx".

Step 3. Add five web forms in the root directory,"Default.aspx","InvalidPage.aspx"
        ,"Main.aspx","NextLink.aspx","NextLink2.aspx".

Step 4. Move DefaultPage.ascx user control on Default.aspx file and move
        NextPage.ascx user control on all other files extension name are ".aspx".
		In Defalut.aspx.cs page, we have a method generate unique random number:
		[code]
        public string GetWindowName()
        {
            string WindowName = Guid.NewGuid().ToString().Replace("-", "");
            Session["WindowName"] = WindowName;
            return WindowName;
        }
		[/code]
		And window.name will recieve this unique string,we use window.name to prevent
		mutiple windows and tabs. 
		[code]
		//set window.name property
        window.open("Main.aspx", "<%=GetWindowName() %>");

		 //If this window name not equal to sessions,will be goto InvalidPage
       if (window.name != "<%=GetWindowName()%>") {
          window.name = "InvalidPage";
          window.open("InvalidPage.aspx","_self");      
       }
	   [/code]

Step 5. Write the codes like the sample.you can get more details from comments in
        the sample file.
         
Step 6. Build the application and you can debug it.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: User control
http://msdn.microsoft.com/en-us/library/fb3w5b53.aspx

/////////////////////////////////////////////////////////////////////////////