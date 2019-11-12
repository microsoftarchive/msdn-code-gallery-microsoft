<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NonAjaxTest.aspx.cs" Inherits="CSASPNETRemoveRegisteredScripts.NonAjaxTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
        &nbsp;<asp:Button ID="btnRemove" runat="server" Text="Remove" OnClick="btnRemove_Click" />
        &nbsp;
        <input type="button" onclick="javascript:test()" value="CallFunction" /></div>
    </form>
</body>
</html>
