<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETVerificationImage._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="tbCode" runat="server"></asp:TextBox>
        <asp:Image ImageUrl="~/ImageHandler.ashx" runat="server" />
        <asp:Button ID="btnOK" runat="server" Text="Validate" OnClick="btnOK_Click" />
        <asp:Literal ID="ltrMessage" runat="server"></asp:Literal>
    </div>
    </form>
</body>
</html>
