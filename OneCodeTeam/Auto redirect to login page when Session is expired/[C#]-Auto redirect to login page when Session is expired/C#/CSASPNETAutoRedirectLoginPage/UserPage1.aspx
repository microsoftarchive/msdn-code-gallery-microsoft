<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPage1.aspx.cs" Inherits="CSASPNETAutoRedirectLoginPage.UserPage" %>

<%@ Register src="UserControl/AutoRedirect.ascx" tagname="AutoRedirect" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:AutoRedirect ID="AutoRedirect1" runat="server" />
        User Page 1.
        <br />
        <asp:LinkButton ID="LinkButton1" runat="server">Refresh this page</asp:LinkButton>
        <br />
        <a href="UserPage2.aspx">User Page 2</a>
    </div>
    </form>
</body>
</html>
