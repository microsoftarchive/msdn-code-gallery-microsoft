<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="LoginPage.aspx.vb" Inherits="VBASPNETAutoRedirectLoginPage.LoginPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    Login Page<br />
        User Name:
        <asp:TextBox ID="tbUserName" runat="server">username</asp:TextBox>
        (default user name: username)<br />
        Pass Word:
        <asp:TextBox ID="tbPassword" runat="server" TextMode="Password">password</asp:TextBox>
        (default pass word: password)<br />
        <asp:Button ID="btnLogin" runat="server" Text="Login" 
            onclick="btnLogin_Click" />
    </div>
    </form>
</body>
</html>