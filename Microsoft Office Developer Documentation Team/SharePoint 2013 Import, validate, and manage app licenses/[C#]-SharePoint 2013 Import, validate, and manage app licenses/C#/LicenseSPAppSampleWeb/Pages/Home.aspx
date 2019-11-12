<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="LicenseSPAppSampleWeb.Pages.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.1.js"> </script>
    <script type="text/javascript" src="SPUtils.js"> </script>
</head>
 

    
<body  onload="loadChromeControl('License Tools', '<%=Session["SPHostUrl"]%>', '<%=Session["SPAppWebUrl"]%>');">
    <div id="chrome_ctrl_container"></div>
    <div style="margin-left:20px">
    <h1><a href="ImportLicense.aspx" target="_blank">Import Test Licenses</a></h1>
    Enables site collection administrators to import test licenses. Use this tool to simulate purchases; you should then manually launch your app to verify license checks work appropiatelly. Is is strongly recommended to verify your license checks work <strong>before submitting your apps to the store.</strong><p>
        &nbsp;</p>
<h1><a href="ValidateLicense.aspx" target="_blank">Validate License Sample</a></h1>
    Simple ASP.Net web form example of how to retrieve and validate a license in your own apps (both production and test modes).&nbsp; The sample retrieve the top most license and display a warning message at the top of the page. You have to view the source code to identify key APIs involved.<p>
    </p>
&nbsp;<form id="form1" runat="server">
    <div>
    <h1><asp:HyperLink ID="viewLicenses" runat="server" Target="_blank">Manage existing licenses</asp:HyperLink></h1>
        Shortcut to launch SharePoint license management UI. View license details, assign seats, delete licenses.
    </div>
    </form>
    <br />
    <br />

    For detailed articles that explain how licensing works please refer to: <a href="http://msdn.microsoft.com/en-us/library/office/apps/jj163257(v=office.15)">http://msdn.microsoft.com/en-us/library/office/apps/jj163257(v=office.15)</a>
</div>
</body>
</html>
