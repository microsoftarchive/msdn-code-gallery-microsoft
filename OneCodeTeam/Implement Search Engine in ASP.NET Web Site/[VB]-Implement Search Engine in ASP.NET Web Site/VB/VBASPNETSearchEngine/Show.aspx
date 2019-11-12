<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Show.aspx.vb" Inherits="VBASPNETSearchEngine.Show" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title><%= Data.Title %></title>
</head>
<body>
    <form id="form1" runat="server">

    <!-- This page shows an individual article according to a Query String parameter id. -->
    <h1><%= Data.Title %></h1>
    <div class="content"><%= Data.Content%></div>

    </form>
</body>
</html>
