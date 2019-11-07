<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ValidateLicense.aspx.cs" Inherits="LicenseSPAppSampleWeb.Pages.ValidateLicense" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.1.js"> </script>
    <script type="text/javascript" src="SPUtils.js"> </script>


</head>
 

    
<body onload="loadChromeControl('Validate License', '<%=Session["SPHostUrl"]%>', '<%=Session["SPAppWebUrl"]%>');">
  
     <div class="ms-status-yellow" style="padding:10px 10px 10px 10px"><asp:Label ID="lblWarning" runat="server" Text=""></asp:Label></div>
    <div id="chrome_ctrl_container"></div>
     
    
    
    <form id="form1" runat="server">
    <div>
    
        App functionality goes here<br />
    
        <br />
 
        
    </div>
    </form>
</body>
</html>
