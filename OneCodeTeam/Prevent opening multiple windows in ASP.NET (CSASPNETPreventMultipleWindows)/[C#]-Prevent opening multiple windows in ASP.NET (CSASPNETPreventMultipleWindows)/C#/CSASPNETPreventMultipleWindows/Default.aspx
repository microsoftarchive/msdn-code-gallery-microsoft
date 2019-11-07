<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETPreventMultipleWindows.Default" %>

<%@ Register src="UserControls/DefaultPage.ascx" tagname="DefaultPage" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body onload="">
    <form id="form1" runat="server">
    <div>
        <uc1:DefaultPage ID="DefaultPage1" runat="server" /> 
        <br />
    </div>
    </form>
</body>
</html>
