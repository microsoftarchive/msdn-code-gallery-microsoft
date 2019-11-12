<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoLogin.aspx.cs" Inherits="CSASPNETAutoLogin.AutoLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <table cellspacing="0" cellpadding="1" border="0" id="Login1" style="border-collapse:collapse;">
	<tr>
		<td><table cellpadding="0" border="0">
			<tr>
				<td align="center" colspan="2">Log In</td>
			</tr><tr>
                
				<td align="right"><label for="UserName">User Name:</label></td><td><asp:TextBox ID="btnUserName" runat="server"> </asp:TextBox></td>
			</tr><tr>			
                
				<td align="right"><label for="Password">Password:</label></td><td>
                            <asp:TextBox ID="btnPassword" runat="server" TextMode="Password" ></asp:TextBox></td>

			</tr><tr>
				<td align="right" colspan="2"><asp:Button ID="Login1" runat="server" 
                        Text="AutoLogin" onclick="autoLogin_Click" /></td>
			</tr>

		</table>
		
		</td>

	</tr>

</table>

    </form>
</body>
</html>
