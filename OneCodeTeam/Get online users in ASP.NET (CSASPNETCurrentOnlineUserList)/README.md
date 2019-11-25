# Get online users in ASP.NET (CSASPNETCurrentOnlineUserList)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Session Management
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>CSASPNETCurrentOnlineUserList Project Overview</h2>
<p style="font-family:Courier New"><br>
Use:<br>
<br>
The Membership.GetNumberOfUsersOnline Method can get the number of online<br>
users,however many asp.net projects are not using membership.This project<br>
shows how to display a list of current online users' information without <br>
using membership provider.<br>
<br>
Demo the Sample.<br>
<br>
Step1: Browse the Login.aspx page from the sample and you will find two <br>
textboxes which used to let user input &quot;UserName&quot; and &quot;TrueName&quot; and a<br>
button which used to sign in.<br>
<br>
Step2: Enter &quot;UserName&quot; and &quot;TrueName&quot; in textboxes and click the sign in
<br>
button.If user do not enter all the textboxes before clicking the sign in<br>
button,page will shows the error message under the button.<br>
<br>
Step3: After user sign in,the page will redirect to CurrentOnlineUserList.aspx<br>
page.There is a gridview control which used to show the information of <br>
current on line users and under the gridview contorl,there is a hyper<br>
link which used to redirect user to sign out page.<br>
<br>
Step4: Browse the Login.aspx page again(It is best to browse the page on another<br>
computer),sign in another user.The gridview will show two pieces information of<br>
current online users in CurrentOnlineUserList.aspx page.<br>
<br>
Step5: If you browse the Login.aspx page at the same computer.You can refresh the<br>
CurrentOnlineUserList.aspx page after one minute.The remaining number of the<br>
current online users will become one.The gridview will just show the information<br>
of user who is login later.<br>
<br>
Step6: If you browse the login.aspx page and sign in at the different computer,<br>
please close the page at one of the computers,and at the other computer please<br>
refresh the CurrentOnlineUserList.aspx page after one minute.You will see there is<br>
only one record in gridview control.<br>
<br>
Step6: You can also try to close the page in any other ways after sign in.Then <br>
refresh the remain page after one minute,for my experience ,you can also get the <br>
list of current on line users.<br>
</p>
<h3>Code Logical:</h3>
<p style="font-family:Courier New"><br>
Step1: Create a C# ASP.NET Empty Web Application in Visual Studio 2010.<br>
<br>
Step2: Add a C# class file which named 'UserEntity' in Visual Studio 2010.<br>
You can find the complete code in UserEntity.cs file.<br>
<br>
Step3: Add a C# class file which named 'DataTableForCurrentOnlineUser' in Visual <br>
Studio 2010.You can find the complete code in DataTableForCurrentOnlineUser.cs file.<br>
It is used to initialize the datatable which store the information of current online
<br>
user.<br>
<br>
Step4: Add a C# class file which named 'CheckUserOnline' in Visual Studio 2010.<br>
You can find the complete code in CheckUserOnline.cs file.It is used to <br>
add JavaScript code to the page.The JavaScript function can check the user's<br>
active time and post a request to the CheckUserOnlinePage.aspx page.<br>
The project will auto delete the off line users'record from user table by <br>
checking the last active time.<br>
<br>
Step5: Add a Login ASP.NET page into the Web Application as the page<br>
which used to let the user sign in.<br>
<br>
Step6: Add two textboxes ,three labels and a button into the page as the .aspx<br>
code below.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&lt;html xmlns=&quot;<a target="_blank" href="&lt;a target=" href="http://www.w3.org/1999/xhtml&quot;&gt;">http://www.w3.org/1999/xhtml&quot;&gt;</a>'&gt;<a target="_blank" href="http://www.w3.org/1999/xhtml&quot;&gt;">http://www.w3.org/1999/xhtml&quot;&gt;</a><br>
&lt;head id=&quot;Head1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;title&gt;&lt;/title&gt;<br>
&lt;/head&gt;<br>
&lt;body&gt;<br>
&nbsp; &nbsp;&lt;form id=&quot;form1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;div style=&quot;text-align: center&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;table style=&quot;width: 50%&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Label ID=&quot;lbUserName&quot; runat=&quot;server&quot; Text=&quot;UserName:&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/asp:Label&gt;&lt;asp:TextBox ID=&quot;tbUserName&quot; runat=&quot;server&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/asp:TextBox&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Label ID=&quot;lbTrueName&quot; runat=&quot;server&quot; Text=&quot;TrueName:&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/asp:Label&gt;&lt;asp:TextBox ID=&quot;tbTrueName&quot; runat=&quot;server&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/asp:TextBox&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td style=&quot;text-align: center&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Button ID=&quot;btnLogin&quot; runat=&quot;server&quot;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Text=&quot;Sign in&quot; OnClick=&quot;btnLogin_Click&quot; /&gt;&lt;br /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:Label ID=&quot;lbMessage&quot; runat=&quot;server&quot;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Text=&quot;Label&quot; Visible=&quot;False&quot; ForeColor=&quot;Red&quot;&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;/asp:Label&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/table&gt;<br>
&nbsp; &nbsp;&lt;/div&gt;<br>
&nbsp; &nbsp;&lt;/form&gt;<br>
&lt;/body&gt;<br>
&lt;/html&gt;<br>
&nbsp;&nbsp;&nbsp;&nbsp;<br>
Step7: Open the C# behind code view to write login and check the value of <br>
user's input data functions.<br>
You can find the complete version in the Login.aspx.cs file.<br>
<br>
&nbsp; &nbsp;protected void btnLogin_Click(object sender, EventArgs e)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string _error;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Check the value of user's input data.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (check_text(out _error))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Initialize the datatable which used to store the<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// information of current online user.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataTableForCurrentOnlineUser onLineTable = new DataTableForCurrentOnlineUser();<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// An instance of user's entity.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;UserEntity _user = new UserEntity();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_user.Ticket = DateTime.Now.ToString(&quot;yyyyMMddHHmmss&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_user.UserName = tbUserName.Text.Trim();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_user.TrueName = tbTrueName.Text.Trim();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_user.ClientIP = this.Request.UserHostAddress;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_user.RoleID = &quot;MingXuGroup&quot;;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Use session variable to store the ticket.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.Session[&quot;Ticket&quot;] = _user.Ticket;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Log in.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;onLineTable.Login(_user, true);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Redirect(&quot;CurrentOnlineUserList.aspx&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.lbMessage.Visible = true;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.lbMessage.Text = _error;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;public bool check_text(out string error)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;error = &quot;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (this.tbUserName.Text.Trim() == &quot;&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;error = &quot;Please enter the username&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return false;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (this.tbTrueName.Text.Trim() == &quot;&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;error = &quot;Please enter the truename&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return false;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return true;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
Step8: Add a CurrentOnlineUserList ASP.NET page into the Web Application as<br>
the page which used to show the current online user list.<br>
<br>
Step9: Add a gridview control and a hyperlink into the page as the .aspx code <br>
below.<br>
<br>
&lt;html xmlns=&quot;<a target="_blank" href="&lt;a target=" href="http://www.w3.org/1999/xhtml&quot;&gt;">http://www.w3.org/1999/xhtml&quot;&gt;</a>'&gt;<a target="_blank" href="http://www.w3.org/1999/xhtml&quot;&gt;">http://www.w3.org/1999/xhtml&quot;&gt;</a><br>
&lt;head id=&quot;Head1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;title&gt;&lt;/title&gt;<br>
&lt;/head&gt;<br>
&lt;body&gt;<br>
&nbsp; &nbsp;&lt;form id=&quot;form1&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;div&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;cc1:CheckUserOnline ID=&quot;CheckUserOnline1&quot; runat=&quot;server&quot; /&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;table border=&quot;1&quot; style=&quot;width: 98%; height: 100%&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td style=&quot;text-align: center&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Current Online User List<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td style=&quot;text-align: center&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:GridView ID=&quot;gvUserList&quot; runat=&quot;server&quot; Width=&quot;98%&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/asp:GridView&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;td style=&quot;text-align: center&quot;&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;asp:HyperLink ID=&quot;hlk&quot; runat=&quot;server&quot;
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NavigateUrl=&quot;~/LogOut.aspx&quot;&gt;sign out&lt;/asp:HyperLink&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/td&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;/tr&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;/table&gt;<br>
&nbsp; &nbsp;&lt;/div&gt;<br>
&nbsp; &nbsp;&lt;/form&gt;<br>
&lt;/body&gt;<br>
&lt;/html&gt;<br>
<br>
<br>
Step10: Open the C# behind code view to write CheckLogin function.<br>
You can find the complete version in the CurrentOnlineUserList.aspx.cs file.<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;public void CheckLogin()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string _userticket = &quot;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (Session[&quot;Ticket&quot;] != null)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_userticket = Session[&quot;Ticket&quot;].ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (_userticket != &quot;&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Initialize the datatable which used to store the information<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// of current online user.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Check whether the user is online by using ticket.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (_onlinetable.IsOnline_byTicket(this.Session[&quot;Ticket&quot;].ToString()))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Update the last active time.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_onlinetable.ActiveTime(Session[&quot;Ticket&quot;].ToString());<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Bind the datatable which used to store the information of
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// current online user to gridview control.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;gvUserList.DataSource = _onlinetable.ActiveUsers;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;gvUserList.DataBind();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// If the current User is not exist in the table,then redirect<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// the page to LogoOut.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Redirect(&quot;LogOut.aspx&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Redirect(&quot;Login.aspx&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
<br>
<br>
Step11: Add a Logout ASP.NET page into the Web Application as the page which used<br>
to let the user login out.<br>
<br>
Step12: Add a hyperlink into the page as the .aspx code below.<br>
&lt;asp:HyperLink ID=&quot;HyperLink1&quot; runat=&quot;server&quot; NavigateUrl=&quot;~/Login.aspx&quot;&gt;<br>
Sign in again&lt;/asp:HyperLink&gt;<br>
<br>
Step13: Open the C# behind code view to write logout function.<br>
You can find the complete version in the Logout.aspx.cs file.<br>
<br>
protected void Page_Load(object sender, EventArgs e)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Initialize the datatable which used to store the information<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// of current online user.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (this.Session[&quot;Ticket&quot;] != null)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Log out.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_onlinetable.Logout(this.Session[&quot;Ticket&quot;].ToString());<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.Session.Clear();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
Step14: Add a CheckUserOnlinePage ASP.NET page into the Web Application as the page which used<br>
to check whether the user is online or not.<br>
<br>
Step15: Open the C# behind code view to write Check function.<br>
You can find the complete version in the CheckUserOnlinePage.aspx.cs file.<br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;protected void Check()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string _myTicket = &quot;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (System.Web.HttpContext.Current.Session[this.SessionName] != null)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_myTicket = System.Web.HttpContext.Current.Session[this.SessionName].ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (_myTicket != &quot;&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Initialize the datatable which used to store the information of<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// current online user.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;DataTableForCurrentOnlineUser _onlinetable = new DataTableForCurrentOnlineUser();<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Update the time when the page refresh or the page get a request.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_onlinetable.RefreshTime(_myTicket);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Write(&quot;OK：&quot; &#43; DateTime.Now.ToString());<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Response.Write(&quot;Sorry：&quot; &#43; DateTime.Now.ToString());<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: <br>
# ASP.NET Session State<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms178581(VS.100).aspx">http://msdn.microsoft.com/en-us/library/ms178581(VS.100).aspx</a><br>
<br>
MSDN:<br>
# DataTable Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.data.datatable.aspx">http://msdn.microsoft.com/en-us/library/system.data.datatable.aspx</a><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
