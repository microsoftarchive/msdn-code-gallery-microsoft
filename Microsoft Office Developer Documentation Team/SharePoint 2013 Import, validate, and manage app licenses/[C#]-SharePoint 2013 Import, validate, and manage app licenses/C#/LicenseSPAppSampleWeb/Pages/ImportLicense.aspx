<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportLicense.aspx.cs" Inherits="LicenseSPAppSampleWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=9" />
    <script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.1.js"> </script>
    <script type="text/javascript" src="SPUtils.js"> </script>

    <title>SP App licensing samples</title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }

        .auto-style2 {
            width: 165px; 
        }

    </style>
</head>
<body  style="overflow:scroll;" onload="loadChromeControl('Import License', '<%=Session["SPHostUrl"]%>', '<%=Session["SPAppWebUrl"]%>');">
    <div id="chrome_ctrl_container"></div>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div style="margin-left: 10%">
                    Use this page to import a test license for your app. The default values provided 
        match this Test App but you must change them to match the App whose license you 
        want to simulate.<br />
                    <br />
                    <br />
                    <strong>App Title</strong><br />
                    <asp:TextBox ID="importAppTitle" runat="server"
                        Width="263px">License Demo App</asp:TextBox>
                    <br />
                    <br />
                    <strong>ProductId</strong> 
                    <br />
                    Must match the one contained on your AppManifest.xml. Default value is the ID for this sample app. <br />
                    <asp:TextBox ID="importProductId" runat="server"
                        Width="263px">{7c617a53-6f45-4d23-ada0-28eabb744acb}</asp:TextBox>
                    <br />
                    <br />
                    <strong>Provider Name</strong><br />
                    <asp:TextBox ID="importProviderName" runat="server" Width="225px">Microsoft SDK Demo</asp:TextBox>
                    <br />
                    <br />
                    <strong>License Type</strong><br />
                    <asp:DropDownList ID="importLicenseType" runat="server" AutoPostBack="True"
                        OnSelectedIndexChanged="importLicenseType_SelectedIndexChanged">
                        <asp:ListItem>Free</asp:ListItem>
                        <asp:ListItem>Paid</asp:ListItem>
                        <asp:ListItem>Trial</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <br />
                    <strong>User limit</strong><br />
                    <asp:DropDownList ID="importUserLimit" runat="server" Enabled="False">
                        <asp:ListItem Value="0">Unlimited</asp:ListItem>
                        <asp:ListItem>10</asp:ListItem>
                        <asp:ListItem>20</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <br />
                    <strong>Expiration</strong><br />
                    <asp:DropDownList ID="importExpiration" runat="server" Enabled="False"
                        OnSelectedIndexChanged="importExpiration_SelectedIndexChanged">
                        <asp:ListItem Value="0">NA</asp:ListItem>
                        <asp:ListItem Value="30">30 days</asp:ListItem>
                        <asp:ListItem Value="-1">-1 (expired)</asp:ListItem>
                        <asp:ListItem Value="9999">Unlimited</asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <br />
                    <strong>Customer ID<br /> </strong>Simulates the identity of a purchaser; generate a new one if you want to import multiple license types for the same app (at the same time); otherwise the license will be overwritten.<br />
                   
                    &nbsp;<asp:Button ID="generateCustomerId" runat="server" OnClick="generateCustomerId_Click"
                        Text="Generate New" />
                    <br />
                     <asp:Label ID="importPurchaserId" runat="server" Text="739835AE59FDE73E"></asp:Label>
                    <br />
                    <br />
                    <strong>Execute Import License</strong><br />
                    <table class="auto-style1">
                        <tr>
                            <td class="auto-style2">
                                <asp:Button ID="ImportLicense" runat="server" Height="48px" OnClick="ImportLicense_Click" Text="Import License" Width="147px" />
                            </td>
                            <td>
                                <asp:Label ID="statusMessage" runat="server" ForeColor="Red"></asp:Label>
                                <br />
                                
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>

</body>
</html>
