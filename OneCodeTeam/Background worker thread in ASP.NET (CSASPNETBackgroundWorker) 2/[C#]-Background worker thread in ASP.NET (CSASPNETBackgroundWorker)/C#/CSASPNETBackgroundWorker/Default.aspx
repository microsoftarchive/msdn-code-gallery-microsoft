<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CSASPNETBackgroundWorker.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Background Worker which is specified to current user</title>
</head>
<body>
    <form id="form1" runat="server">

    <p><b>Note:</b><br />
    Try to open this page in two browsers which use different sessions and then run the operation at the same time.
    You will learn that each session has its own Background Worker. It is not shared by all users.
    </p><br />

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

    <!-- UpdateUpanel let the progress can be updated without updating the whole page (partial update). -->
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            
            <!-- The timer which used to update the progress. -->
            <asp:Timer ID="Timer1" runat="server" Interval="100" Enabled="false" ontick="Timer1_Tick"></asp:Timer>

            <!-- The Label which used to display the progress and the result -->
            <asp:Label ID="lbProgress" runat="server" Text=""></asp:Label><br />

            <!-- Start the operation by inputting value and clicking the button -->
            Input a parameter: <asp:TextBox ID="txtParameter" runat="server" Text="Hello World"></asp:TextBox><br />
            <asp:Button ID="btnStart" runat="server" Text="Click to Start the Background Worker" onclick="btnStart_Click" />

        </ContentTemplate>
    </asp:UpdatePanel>

    <br /><p><b>Another page: </b><br />
    That page shows the current status of the Application Level Backgroun Worker.</p>
    <a href="GlobalBackgroundWorker.aspx" target="_blank">GlobalBackgroundWorker.aspx</a>

    </form>
</body>
</html>
