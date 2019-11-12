<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DynamicBreadcrumb.aspx.cs" Inherits="CSASPNETBreadcrumbWithQueryString.DynamicBreadcrumb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">

    <p>In this page, the breadcrumb is created dynamically.</p>

    <asp:SiteMapPath ID="SiteMapPath1" runat="server">
    </asp:SiteMapPath>

    </form>
</body>
</html>
