<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETStopPostbackInJS.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    There are two ways that stop postbacks in js:<br />
    <a href="StopPostBack1.aspx">StopPostBack1.aspx: use onClientClick event to stop postbacks in JavaScript.</a><br />
    <a href="StopPostBack2.aspx">StopPostBack2.aspx: use another Hidden Button to stop postbacks in JavaScript.</a>
    </div>
    </form>
</body>
</html>
