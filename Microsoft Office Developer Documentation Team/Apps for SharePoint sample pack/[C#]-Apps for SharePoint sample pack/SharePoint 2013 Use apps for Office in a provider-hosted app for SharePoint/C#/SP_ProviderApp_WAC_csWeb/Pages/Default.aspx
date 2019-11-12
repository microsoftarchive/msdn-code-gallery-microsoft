<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SP_ProviderApp_WAC_csWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!-- Custom style sheet for rendering tiles and links in interesting ways -->
    <link type="text/css" href="../CSS/point8020metro.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
     <asp:Label ID="SiteTitle" CssClass="heading" runat="server" Text="Label"></asp:Label>
        <br />
        <!-- The code in Default.aspx.cs adds a tile to the panel below for each Office document -->
        <!-- that it finds in document libraries in the hosting SharePoint site -->
        <asp:Panel ID="FileList" CssClass="scroller fl" runat="server"></asp:Panel>
    </form>
</body>
</html>
