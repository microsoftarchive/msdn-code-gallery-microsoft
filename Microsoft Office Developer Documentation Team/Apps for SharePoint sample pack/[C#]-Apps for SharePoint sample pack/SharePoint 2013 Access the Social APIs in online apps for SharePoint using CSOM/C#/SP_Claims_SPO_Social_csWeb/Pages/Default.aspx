<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SP_Claims_SPO_Social_csWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Social Sample</title>
    <!-- Some simple CSS for rendering tiles to give our UI a modern feel -->
    <link rel="Stylesheet" href="../CSS/point8020metro.css" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <!-- Our UI has a simple heading, followed by a panel where we will render the three most recent posts as tiles -->
        <!-- It also has a text box and two buttons for posting replies or new posts -->
        <asp:Panel ID="Timeline" Width="200" runat="server">
            <div class="heading fl" style="margin-left:10px">Timeline</div>
        </asp:Panel>
        <asp:Panel ID="TimelinePanel" Width="200" runat="server" />
        <asp:Panel ID="PostReplier" Width="200" runat="server">
            <asp:TextBox ID="ReplyText" runat="server" TextMode="MultiLine" Height="75" style="margin-left:10px" Width="190"></asp:TextBox>
            <asp:Button ID="ReplyNow" runat="server" Width="90" Text="Reply All" style="margin-left:10px" CssClass="fl tileRed noPad" OnClick="ReplyNow_Click" />
            <asp:Button ID="PostNew" runat="server" Width="90" Text="Post New" style="margin-left:10px" CssClass="fl tileRed noPad" OnClick="PostNew_Click" />
            <asp:Label ID="errLabel" runat="server" Text=""></asp:Label>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
