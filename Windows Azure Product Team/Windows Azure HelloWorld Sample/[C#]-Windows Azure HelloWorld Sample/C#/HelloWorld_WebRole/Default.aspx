<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Microsoft.Samples.ServiceHosting.HelloWorld.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Hello World!</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="font-size: 64pt; font-family: Arial, Helvetica, sans-serif; font-weight: bolder">
        
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        
    </div>
    <div style="font-size: 14pt; font-family: Arial, Helvetica, Sans-Serif; font-weight: bolder">
        <p><asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></p>
        <p><asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink></p>
        <p><asp:HyperLink ID="HyperLink2" runat="server"></asp:HyperLink></p>
        <p><asp:HyperLink ID="HyperLink3" runat="server"></asp:HyperLink></p>
    </div>
    </form>
</body>
</html>
