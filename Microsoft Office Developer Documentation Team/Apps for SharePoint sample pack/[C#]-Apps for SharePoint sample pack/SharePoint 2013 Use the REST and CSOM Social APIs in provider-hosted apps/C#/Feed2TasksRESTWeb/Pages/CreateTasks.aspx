<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateTasks.aspx.cs" Inherits="Feed2TasksRESTWeb.Pages.CreateTasks" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="Stylesheet" href="../Styles/styles.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/sp.ui.controls.js"></script>
    <script type="text/javascript" src="../Scripts/chromecontrol.js"></script>
    <title>Create Tasks from Feed</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdnContextToken" runat="server" />
        <asp:HiddenField ID="hdnAccessToken" runat="server" />
        <asp:HiddenField ID="hdnHostWeb" runat="server" />
        <asp:HiddenField ID="hdnAppWeb" runat="server" />
        <asp:HiddenField ID="hdnDisplayName" runat="server" />
        <asp:HiddenField ID="hdnUserId" runat="server" />

        <div id="chrome_ctrl_placeholder"></div>
        <div class="tasks-section">
            <asp:DataGrid ID="assignmentPosts" runat="server" AutoGenerateColumns="False" GridLines="None" OnSelectedIndexChanged="assignmentPosts_SelectedIndexChanged" CellPadding="4" ForeColor="#333333">
                <AlternatingItemStyle BackColor="White" ForeColor="#284775" />
                <Columns>
                    <asp:ButtonColumn ButtonType="PushButton" Text="Create Task" CommandName="Select" />
                    <asp:BoundColumn DataField="CreatedDate" HeaderText="Created" />
                    <asp:BoundColumn DataField="Body" HeaderText="Body" />
                    <asp:BoundColumn DataField="Requester" HeaderText="Requester" />
                </Columns>
                <EditItemStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <ItemStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedItemStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            </asp:DataGrid>
            <asp:Label ID="messages" runat="server" />
        </div>
    </form>
</body>
</html>
