<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="InstrumentationWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
       <script 
        src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" 
        type="text/javascript">
    </script>
    <script 
        type="text/javascript" 
        src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.2.min.js">
    </script>      
    <script 
        type="text/javascript"
        src="../ChromeLoader.js">
    </script>
</head>
<body>
    <!-- Chrome control placeholder -->
    <div id="chrome_ctrl_placeholder"></div>


    <form id="form1" runat="server">
 
    <!-- POPULATE DATA button in "Windows Store App" style -->
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click"
        Text="POPULATE DATA" BackColor="#004E98" ForeColor="White" Font-Size="Large" 
        Style="font-family: 'Segoe UI'; border-style: none; text-wrap: normal; font-weight: normal" 
        Height="50px" Width="239px" />
    <asp:Literal ID="Literal1" runat="server"><br /><br /></asp:Literal>

    <!-- Link to Diagnostics page. Initially invisible -->
    <asp:HyperLink ID="hyperlink1" runat="server" Visible="false" Font-Bold="true">DIAGNOSTICS PAGE</asp:HyperLink>
    <asp:Literal ID="Literal2" runat="server"><br /><br /></asp:Literal>

    <!-- Grid to show the titles of SharePoint lists on the host web -->
    <asp:GridView ID="GridView1" runat="server" BackColor="#808080" ForeColor="White"
        BorderColor="#0033CC" BorderStyle="None" Caption="LISTS ON THE HOST WEB" 
        CaptionAlign="Left" CellPadding="5" Style="font-family: 'Segoe UI'" GridLines="None" 
        HorizontalAlign="Left">
        <AlternatingRowStyle BackColor="White" ForeColor="Black" />
    </asp:GridView>
    </form>
</body>
</html>
