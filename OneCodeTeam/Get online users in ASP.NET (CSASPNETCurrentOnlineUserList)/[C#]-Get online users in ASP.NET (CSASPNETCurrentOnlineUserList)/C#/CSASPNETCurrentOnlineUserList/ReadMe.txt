===========================================================================
            CSASPNETCurrentOnlineUserList Project Overview
===========================================================================

Use:

 The Membership.GetNumberOfUsersOnline Method can get the number of online
 users,however many asp.net projects are not using membership.This project
 shows how to display a list of current online users' information without 
 using membership provider.

/////////////////////////////////////////////////////////////////////////////
Demo the Sample.

Step1: Browse the Login.aspx page from the sample and you will find two 
textboxes which used to let user input "UserName" and "TrueName" and a
button which used to sign in.

Step2: Enter "UserName" and "TrueName" in textboxes and click the sign in 
button.If user do not enter all the textboxes before clicking the sign in
button,page will shows the error message under the button.

Step3: After user sign in,the page will redirect to CurrentOnlineUserList.aspx
page.There is a gridview control which used to show the information of 
current on line users and under the gridview contorl,there is a hyper
link which used to redirect user to sign out page.

Step4: Browse the Login.aspx page again(It is best to browse the page on another
computer),sign in another user.The gridview will show two pieces information of
current online users in CurrentOnlineUserList.aspx page.

Step5: If you browse the Login.aspx page at the same computer.You can refresh the
CurrentOnlineUserList.aspx page after one minute.The remaining number of the
current online users will become one.The gridview will just show the information
of user who is login later.

Step6: If you browse the login.aspx page and sign in at the different computer,
please close the page at one of the computers,and at the other computer please
refresh the CurrentOnlineUserList.aspx page after one minute.You will see there is
only one record in gridview control.

Step6: You can also try to close the page in any other ways after sign in.Then 
refresh the remain page after one minute,for my experience ,you can also get the 
list of current on line users.

/////////////////////////////////////////////////////////////////////////////
Code Logical:

Step1: Create a C# ASP.NET Empty Web Application in Visual Studio 2010.

Step2: Add a C# class file which named 'UserEntity' in Visual Studio 2010.
You can find the complete code in UserEntity.cs file.

Step3: Add a C# class file which named 'DataTableForCurrentOnlineUser' in Visual 
Studio 2010.You can find the complete code in DataTableForCurrentOnlineUser.cs file.
It is used to initialize the datatable which store the information of current online 
user.

Step4: Add a C# class file which named 'CheckUserOnline' in Visual Studio 2010.
You can find the complete code in CheckUserOnline.cs file.It is used to 
add JavaScript code to the page.The JavaScript function can check the user's
active time and post a request to the CheckUserOnlinePage.aspx page.
The project will auto delete the off line users'record from user table by 
checking the last active time.

Step5: Add a Login ASP.NET page into the Web Application as the page
which used to let the user sign in.

Step6: Add two textboxes ,three labels and a button into the page as the .aspx
code below.

	<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center">
        <table style="width: 50%">
            <tr>
                <td>
                    <asp:Label ID="lbUserName" runat="server" Text="UserName:">
					</asp:Label><asp:TextBox ID="tbUserName" runat="server">
					</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lbTrueName" runat="server" Text="TrueName:">
					</asp:Label><asp:TextBox ID="tbTrueName" runat="server">
					</asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:Button ID="btnLogin" runat="server"
					 Text="Sign in" OnClick="btnLogin_Click" /><br />
                    <asp:Label ID="lbMessage" runat="server"
					 Text="Label" Visible="False" ForeColor="Red">
					 </asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
	
Step7: Open the C# behind code view to write login and check the value of 
user's input data functions.
You can find the complete version in the Login.aspx.cs file.

    protected void btnLogin_Click(object sender, EventArgs e)
        {
            string _error;

            // Check the value of user's input data.
            if (check_text(out _error))
            {
                // Initialize the datatable which used to store the
                // information of current online user.
                DataTableForCurrentOnlineUser onLineTable = new DataTableForCurrentOnlineUser();

                // An instance of user's entity.
                UserEntity _user = new UserEntity();
                _user.Ticket = DateTime.Now.ToString("yyyyMMddHHmmss");
                _user.UserName = tbUserName.Text.Trim();
                _user.TrueName = tbTrueName.Text.Trim();
                _user.ClientIP = this.Request.UserHostAddress;
                _user.RoleID = "MingXuGroup";

                // Use session variable to store the ticket.
                this.Session["Ticket"] = _user.Ticket;

                // Log in.
                onLineTable.Login(_user, true);
                Response.Redirect("CurrentOnlineUserList.aspx");
            }
            else
            {
                this.lbMessage.Visible = true;
                this.lbMessage.Text = _error;
            }
        }
        public bool check_text(out string error)
        {
            error = "";
            if (this.tbUserName.Text.Trim() == "")
            {
                error = "Please enter the username";
                return false;
            }
            if (this.tbTrueName.Text.Trim() == "")
            {
                error = "Please enter the truename";
                return false;
            }
            return true;
        }

Step8: Add a CurrentOnlineUserList ASP.NET page into the Web Application as
the page which used to show the current online user list.

Step9: Add a gridview control and a hyperlink into the page as the .aspx code 
below.

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <cc1:CheckUserOnline ID="CheckUserOnline1" runat="server" />
        <table border="1" style="width: 98%; height: 100%">
            <tr>
                <td style="text-align: center">
                    Current Online User List
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:GridView ID="gvUserList" runat="server" Width="98%">
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                    <asp:HyperLink ID="hlk" runat="server" 
					NavigateUrl="~/LogOut.aspx">sign out</asp:HyperLink>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>


Step10: Open the C# behind code view to write CheckLogin function.
You can find the complete version in the CurrentOnlineUserList.aspx.cs file.

        public void CheckLogin()
        {
            string _userticket = "";
            if (Session["Ticket"] != null)
            {
                _userticket = Session["Ticket"].ToString();
            }
            if (_userticket != "")
            {
                // Initialize the datatable which used to store the information
                // of current online user.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // Check whether the user is online by using ticket.
                if (_onlinetable.IsOnline_byTicket(this.Session["Ticket"].ToString()))
                {
                    // Update the last active time.
                    _onlinetable.ActiveTime(Session["Ticket"].ToString());

                    // Bind the datatable which used to store the information of 
                    // current online user to gridview control.
                    gvUserList.DataSource = _onlinetable.ActiveUsers;
                    gvUserList.DataBind();
                }
                else
                {
                    // If the current User is not exist in the table,then redirect
                    // the page to LogoOut.
                    Response.Redirect("LogOut.aspx");
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }



Step11: Add a Logout ASP.NET page into the Web Application as the page which used
to let the user login out.

Step12: Add a hyperlink into the page as the .aspx code below.
<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Login.aspx">
Sign in again</asp:HyperLink>

Step13: Open the C# behind code view to write logout function.
You can find the complete version in the Logout.aspx.cs file.
 
 protected void Page_Load(object sender, EventArgs e)
        {
            // Initialize the datatable which used to store the information
            // of current online user.
            DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();
            if (this.Session["Ticket"] != null)
            {
                // Log out.
                _onlinetable.Logout(this.Session["Ticket"].ToString());
                this.Session.Clear();
            }
        }

Step14: Add a CheckUserOnlinePage ASP.NET page into the Web Application as the page which used
to check whether the user is online or not.

Step15: Open the C# behind code view to write Check function.
You can find the complete version in the CheckUserOnlinePage.aspx.cs file.
       
	    protected void Check()
        {
            string _myTicket = "";
            if (System.Web.HttpContext.Current.Session[this.SessionName] != null)
            {
                _myTicket = System.Web.HttpContext.Current.Session[this.SessionName].ToString();
            }
            if (_myTicket != "")
            {
                // Initialize the datatable which used to store the information of
                // current online user.
                DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();

                // Update the time when the page refresh or the page get a request.
                _onlinetable.RefreshTime(_myTicket);
                Response.Write("OK：" + DateTime.Now.ToString());
            }
            else
            {
                Response.Write("Sorry：" + DateTime.Now.ToString());
            }
        }


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: 
# ASP.NET Session State
http://msdn.microsoft.com/en-us/library/ms178581(VS.100).aspx

MSDN:
# DataTable Class
http://msdn.microsoft.com/en-us/library/system.data.datatable.aspx

/////////////////////////////////////////////////////////////////////////////