========================================================================
                  CSASPNETUserControlPassData Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

This project illustrates how to pass data from one user control to another.
A user control can contain other controls like TextBoxes or Labels, These 
control are declared as protected members, We cannot get the use control from
another one directly.

/////////////////////////////////////////////////////////////////////////////
Demo the Sample. 

Please follow these demonstration steps below.

Step 1: Open the CSASPNETUserControlPassData.sln.

Step 2: Expand the CSASPNETUserControlPassData web application and press 
        Ctrl + F5 to show the Default.aspx.

Step 3: You can find two messages on Default page. The messages be outputted by 
        UserControl2 user control, UserControl2 can retrieve the data that come from UserControl1
		user control.

Step 4: Validation finished.

/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step 1. Create a C# "ASP.NET Empty Web Application" in Visual Studio 2010 or
        Visual Web Developer 2010. Name it as "CSASPNETUserControlPassData".

Step 2. Add one web form in the root directory, name it as "Default.aspx". This
        page is use to show user controls.

Step 3. Add a folder named "UserControl" with two user controls, "UserControl1.ascx",
        "UserControl2.ascx".

Step 4. The UserControl1 user control provide public property as shown below:
        [code]
        private string strCallee;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                strCallee = "I come from UserControl1.";
                lbPublicVariable.Text = strCallee;
            }
            
        }

        /// <summary>
        /// UserControl1 message.
        /// </summary>
        public string StrCallee
        {
            get
            {
                return strCallee;
            }
            set
            {
                strCallee = value;
            }
        }
		[/code]
		
Step 5  The UserControl2 user control is use to output private messages and the data of 
        UserControl1 user control.
	    [code]
    public partial class UserControl2 : System.Web.UI.UserControl
    {
        private string strCaller = "I come from UserControl2.";
        private UserControl1 userControl1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Output UserControl2 user control message.
                lbPublicVariable2.Text = strCaller;

                // Find UserControl1 user control.
                Control control = Page.FindControl("UserControl1ID");
                userControl1= (UserControl1)control;
                if (userControl1 != null)
                {
                    // Output UserControl1 user control message.
                    Label lbUserControl1 = userControl1.FindControl("lbPublicVariable") as Label;
                    if (lbUserControl1 != null)
                    {
                        lbUserControl1.Text = userControl1.StrCallee;
                        tbModifyUserControl1.Text = userControl1.StrCallee;
                    }
                }
            }
        }

        /// <summary>
        /// UserControl2 message.
        /// </summary>
        public string StrCaller
        {
            get
            {
                return strCaller;
            }
            set
            {
                strCaller = value;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!tbModifyUserControl1.Text.Trim().Equals(string.Empty))
            {
                Control control = Session["UserControl1"] as Control;
                userControl1 = control as UserControl1;
                if (userControl1 != null)
                {
                    // Set UserControl1 user control message.
                    lbFormatMessage.Text = string.Format("forward message: {0} ", userControl1.StrCallee);
                    userControl1.StrCallee = tbModifyUserControl1.Text;
                    Session["UserControl1"] = userControl1;
                    UserControl1 pageUserControl1 = Page.FindControl("UserControl1ID") as UserControl1;
                    Label lbUserControl1 = pageUserControl1.FindControl("lbPublicVariable") as Label;
                    lbUserControl1.Text = tbModifyUserControl1.Text;
                }
                else
                {
                    control = Page.FindControl("UserControl1ID");
                    userControl1 = (UserControl1)control;
                    userControl1.StrCallee = tbModifyUserControl1.Text.Trim();
                    Label lbUserControl1 = userControl1.FindControl("lbPublicVariable") as Label;
                    lbUserControl1.Text = userControl1.StrCallee;
                    Session["UserControl1"] = userControl1;
                }
            }
            else
            {
                lbMessage.Text = "The message can not be null.";
            }
        }
    }
	    [/code]
	   
Step 6. You need drag and drop the user controls in default page.

Step 7. Build the application and you can debug it.
/////////////////////////////////////////////////////////////////////////////
References:

MSDN: ASP.NET User Controls Overview
http://msdn.microsoft.com/en-us/library/fb3w5b53.aspx

MSDN: Page.FindControl Method (String)
http://msdn.microsoft.com/en-us/library/31hxzsdw.aspx

MSDN: Page.LoadControl Method 
http://msdn.microsoft.com/en-us/library/system.web.ui.page.loadcontrol.aspx
/////////////////////////////////////////////////////////////////////////////