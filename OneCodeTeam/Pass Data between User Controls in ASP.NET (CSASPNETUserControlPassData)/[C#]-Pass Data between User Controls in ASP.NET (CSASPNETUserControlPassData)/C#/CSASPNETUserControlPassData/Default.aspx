<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETUserControlPassData.Default" %>

<%@ Register src="UserControl/UserControl1.ascx" tagname="UserControl1" tagprefix="uc1" %>
<%@ Register src="UserControl/UserControl2.ascx" tagname="UserControl2" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>  
        <uc1:UserControl1 ID="UserControl1ID" runat="server" />
        <br />
        <uc2:UserControl2 ID="UserControl2ID" runat="server" />
    </div>
    </form>
</body>
</html>
