<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Main.aspx.cs" Inherits="CSASPNETPreventMultipleWindows.Main" %>

<%@ Register src="UserControls/NextPage.ascx" tagname="NextPage" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <uc1:NextPage ID="NextPage1" runat="server" />
    <div>
    <a href="NextLink.aspx">Next Page 1</a>
    <br/>
    <a href="NextLink2.aspx">Next Page 2</a>
    </div>
    </form>
</body>
</html>
