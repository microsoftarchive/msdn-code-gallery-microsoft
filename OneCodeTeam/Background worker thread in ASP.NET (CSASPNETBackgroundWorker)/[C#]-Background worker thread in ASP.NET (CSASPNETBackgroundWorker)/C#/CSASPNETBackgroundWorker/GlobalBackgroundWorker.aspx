<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GlobalBackgroundWorker.aspx.cs" Inherits="CSASPNETBackgroundWorker.GlobalBackgroundWorker" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Application Level Background Worker</title>
</head>
<body>
    <form id="form1" runat="server">

    <p><b>Note: </b><br />Try to close the browser and then open this page again after 10 seconds.
    You will learn that it is still working even the browser is closed.</p>

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <!-- The timer which used to update the progress. -->
            <asp:Timer ID="Timer1" runat="server" Interval="1000" ontick="Timer1_Tick"></asp:Timer>

            <!-- The Label which used to display the progress -->
            <asp:Label ID="lbGlobalProgress" runat="server" Text=""></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>

    </form>
</body>
</html>
