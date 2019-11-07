<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETDetectBrowserCloseEvent.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        </asp:UpdatePanel>
        <asp:Timer ID="Timer1" runat="server" Interval="3000">
        </asp:Timer>
    </div>
    </form>
    <iframe id="Detect" src="DetectBrowserClosePage.aspx" style="border-width: 0px; border-style:none;
        width: 0px; height: 0px; overflow: hidden"></iframe>
</body>
</html>
