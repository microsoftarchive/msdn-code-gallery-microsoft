<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CascadingDropDownListWithPostBack.aspx.cs" Inherits="CSASPNETCascadingDropDownList.CascadingDropDownListWithPostBack" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>View CascadingDropDownList With PostBack</title>
    <%-- The follow javascript is the work around of EventValidation in FireFox --%>

    <script type="text/javascript">
        var hdfDdl = null; // Save city dropdownlist selected option
        var ddlCity = null;

        // Restore the selected option in city dropdownlist when page is rendering
        window.onload = function () {
            hdfDdl = document.getElementById('hdfDdlCitySelectIndex');
            ddlCity = document.getElementById('ddlCity');
            ddlCity.selectedIndex = hdfDdl.value;
            EnableOrDisableButton(false);
        }

        // Save city dropdownlist selected option when selected option is changed in city dropdownlist
        function onChange() {
            hdfDdl.value = ddlCity.selectedIndex;
        }

        // Enable or diasble submit button
        function EnableOrDisableButton(ToF) {
            document.getElementById('btnSubmit').disabled = ToF;
        }
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        &nbsp;Country:<asp:DropDownList ID="ddlCountry" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlCountry_SelectedIndexChanged" Width="110px" onChange="EnableOrDisableButton(true);">
        </asp:DropDownList>
        <br />
        &nbsp; Region :<asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged" Width="110px" onChange="EnableOrDisableButton(true);">
        </asp:DropDownList>
        <br />
        &nbsp;&nbsp; City&nbsp;&nbsp;&nbsp; :<asp:DropDownList ID="ddlCity" runat="server"
            onChange="onChange();" Width="110px">
        </asp:DropDownList>
        <br />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click"
            Width="66px" />
    </div>
    <p>
        <asp:Label ID="lblResult" runat="server"></asp:Label>
    </p>
    <p>
        &nbsp;</p>
    <p>
        <asp:HiddenField ID="hdfDdlCitySelectIndex" runat="server" />
    </p>
    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Go Back To Default page</asp:HyperLink>
    </form>
</body>
</html>
