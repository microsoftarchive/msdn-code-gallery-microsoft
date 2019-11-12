<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CascadingDropDownListWithCallBack.aspx.cs" Inherits="CSASPNETCascadingDropDownList.CascadingDropDownListWithCallBack" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>View CascadingDropDownList With CallBack</title>

    <script src="JScript.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        &nbsp;Country:<asp:DropDownList ID="ddlCountry" runat="server" Width="110px" onChange='ddlCountryOnChange();'>
        </asp:DropDownList>
        <br />
        &nbsp; Region :<asp:DropDownList ID="ddlRegion" runat="server" Width="110px" onChange='ddlRegionOnChange();'>
        </asp:DropDownList>
        <br />
        &nbsp;&nbsp; City&nbsp;&nbsp;&nbsp; :<asp:DropDownList ID="ddlCity" runat="server"
            onChange='ddlCityOnChange();' Width="140px">
        </asp:DropDownList>
        <br />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" Width="66px" OnClick="btnSubmit_Click" />
    </div>
    <p>
        <asp:Label ID="lblResult" runat="server"></asp:Label>
        <asp:HiddenField ID="hdfRegion" runat="server" />
        <asp:HiddenField ID="hdfCity" runat="server" />
        <asp:HiddenField ID="hdfResult" runat="server" />
        <asp:HiddenField ID="hdfRegionSelectValue" runat="server" />
        <asp:HiddenField ID="hdfCitySelectValue" runat="server" Value="0" />
        <asp:HiddenField ID="hdfCountrySelectValue" runat="server" Value="0" />
    </p>
    <br />
    <br />
    <br />
    <br />
    <br />
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Go Back To Default Page</asp:HyperLink>
    </form>
</body>
</html>
