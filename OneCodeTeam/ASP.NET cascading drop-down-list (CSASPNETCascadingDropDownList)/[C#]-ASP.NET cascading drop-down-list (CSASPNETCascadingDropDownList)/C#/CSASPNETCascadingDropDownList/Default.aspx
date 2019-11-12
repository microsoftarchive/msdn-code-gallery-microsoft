<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETCascadingDropDownList.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Please choose which demo to view</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    </div>
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/CascadingDropDownListWithPostBack.aspx">View CascadingDropDown With PostBack</asp:HyperLink>
    <br />
    <br />
    <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/CascadingDropDownListWithCallBack.aspx">View CascadingDropDown With CallBack</asp:HyperLink>
    </form>
</body>
</html>
