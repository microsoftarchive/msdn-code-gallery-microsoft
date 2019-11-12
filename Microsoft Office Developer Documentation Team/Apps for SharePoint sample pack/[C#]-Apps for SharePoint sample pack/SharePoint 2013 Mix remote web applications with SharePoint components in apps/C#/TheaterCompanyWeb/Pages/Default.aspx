<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TheaterCompanyWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    
<style type="text/css">body {
    background-color: #008080;
    background-image: url(comedyandtragedy.jpg);
    background-position: 100% 100%;
    background-repeat: repeat;}
</style>
</head>

<body>
    <form id="form1" runat="server">
    <div>
    <h2 style="font-family: Castellar; font-size: xx-large; font-weight: normal; font-style: normal; color: #C0C0C0">Local Theater</h2>
    </div>
    <asp:Literal ID="Literal1" runat="server"><br /><br /></asp:Literal>

    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" 
        Text="Get the Cast" BackColor="Navy" ForeColor="White"/>

    <asp:Literal ID="Literal2" runat="server"><br /><br /></asp:Literal>

    <asp:GridView ID="GridView1" runat="server" BackColor="#66CCFF" 
        BorderColor="#0033CC" BorderStyle="Solid" Caption="The Cast" 
        CaptionAlign="Left" CellPadding="5" Font-Names="Verdana" GridLines="None" 
        HorizontalAlign="Left">
        <AlternatingRowStyle BackColor="Silver" />
    </asp:GridView>
    </form>
</body>

</html>
