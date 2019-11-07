<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diagnostics.aspx.cs" Inherits="InstrumentationWeb.Pages.Diagnostics" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script 
        src="//ajax.aspnetcdn.com/ajax/4.0/1/MicrosoftAjax.js" 
        type="text/javascript">
    </script>
    <script 
        type="text/javascript" 
        src="//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.7.2.min.js">
    </script>      
    <script 
        type="text/javascript"
        src="../ChromeLoader.js">
    </script>
</head>
<body>
     <!-- Chrome control placeholder -->
    <div id="chrome_ctrl_placeholder"></div>


    <form id="form1" runat="server">
    <div>

    <!-- Button to toggle request tracing on/off -->
    <asp:Button ID="btnToggleTracing" runat="server" Text="Turn On Tracing" OnClick="btnToggleTracing_Click" /><br />

    <!-- Link to the server's trace log -->
    <asp:HyperLink ID="lnkToTraceLog" runat="server" NavigateUrl="~/trace.axd">Trace Log</asp:HyperLink>
    </div>
    </form>
</body>
</html>
