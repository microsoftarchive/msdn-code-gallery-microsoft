<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Default.aspx.vb" Inherits="VBASPNETCheckSpellingWritten._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:TextBox ID="tbInput" runat="server" Height="275px" TextMode="MultiLine" 
            Width="353px"></asp:TextBox>
        <br />
        <asp:Button ID="btnCheck" runat="server" Text="Check words" 
            onclick="btnCheck_Click" />
    
        <asp:Label ID="lbMessage" runat="server"></asp:Label>
    
    </div>
    </form>
</body>
</html>