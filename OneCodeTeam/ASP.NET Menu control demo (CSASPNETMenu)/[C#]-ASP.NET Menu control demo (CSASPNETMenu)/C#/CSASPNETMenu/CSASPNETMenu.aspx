<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CSASPNETMenu.aspx.cs" Inherits="CSASPNETMenu.CSASPNETMenu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>ASP.NET Menu Control Sample</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
         <b>ASP.NET Menu Control Sample</b>
        <asp:Menu ID="Menu1" runat="server" BackColor="#F7F6F3" 
            DynamicHorizontalOffset="2" Width="50%" 
            Font-Names="Verdana" Font-Size="0.9em" ForeColor="#7C6F57" 
            StaticSubMenuIndent="10px" StaticEnableDefaultPopOutImage="false"  Orientation="Horizontal">
            <StaticSelectedStyle BackColor="#5D7B9D" />
            <StaticMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <DynamicHoverStyle BackColor="#7C6F57" ForeColor="White" />
            <DynamicMenuStyle BackColor="#F7F6F3" />
            <DynamicSelectedStyle BackColor="#5D7B9D" />
            <DynamicMenuItemStyle HorizontalPadding="5px" VerticalPadding="2px" />
            <StaticHoverStyle BackColor="#7C6F57" ForeColor="White" />
        </asp:Menu>
        <br />
        <br />
    </div>
    </form>
</body>
</html>
