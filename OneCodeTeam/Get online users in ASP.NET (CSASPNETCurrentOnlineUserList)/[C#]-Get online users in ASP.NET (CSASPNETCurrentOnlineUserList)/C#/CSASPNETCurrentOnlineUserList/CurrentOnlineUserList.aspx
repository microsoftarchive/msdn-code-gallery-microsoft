<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CurrentOnlineUserList.aspx.cs"
    Inherits="CSASPNETCurrentOnlineUserList.CurrentOnlineUserList" %>

<%@ Register Assembly="CSASPNETCurrentOnlineUserList" Namespace="CSASPNETCurrentOnlineUserList"
    TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
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
                    <asp:HyperLink ID="hlk" runat="server" NavigateUrl="~/LogOut.aspx">sign out</asp:HyperLink>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
