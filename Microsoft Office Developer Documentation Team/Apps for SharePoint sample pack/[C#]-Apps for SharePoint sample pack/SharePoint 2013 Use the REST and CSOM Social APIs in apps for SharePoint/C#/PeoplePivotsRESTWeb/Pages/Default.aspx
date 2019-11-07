<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PeoplePivotsRESTWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>People Pivots</title>
    <link rel="Stylesheet" href="../Styles/styles.css" />
    <script type="text/javascript" src="../Scripts/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/sp.ui.controls.js"></script>
    <script type="text/javascript" src="../Scripts/chromecontrol.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:HiddenField ID="hdnContextToken" runat="server" />
        <asp:HiddenField ID="hdnHostWeb" runat="server" />
        <asp:HiddenField ID="hdnAccessToken" runat="server" />

        <div id="chrome_ctrl_placeholder"></div>
        <div>
            <h2 class="pivot-header">Filters</h2>
            <div class="pivot-section">
                <asp:DropDownList ID="feedFilters" runat="server" AutoPostBack="true" OnSelectedIndexChanged="feedFilters_SelectedIndexChanged">
                    <asp:ListItem Text="All People" Value="0" Selected="True" />
                    <asp:ListItem Text="People who posted in the last week" Value="1" />
                    <asp:ListItem Text="People whose posts I liked" Value="2" />
                </asp:DropDownList>
            </div>
            <h2 class="pivot-header">Following</h2>
            <div class="pivot-section">
                <asp:Repeater runat="server" ID="followedImages" OnItemCommand="ShowFeedActivity">
                    <ItemTemplate>
                        <asp:ImageButton ID="followedImage" runat="server" AlternateText='<%#Eval("AccountName") %>' ToolTip='<%#Eval("Name") %>' ImageUrl='<%#Eval("ImageUrl") %>' />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <h2 class="pivot-header">Followers</h2>
            <div class="pivot-section">
                <asp:Repeater runat="server" ID="followersImages" OnItemCommand="ShowFeedActivity">
                    <ItemTemplate>
                        <asp:ImageButton ID="followersImage" runat="server" AlternateText='<%#Eval("AccountName") %>' ToolTip='<%#Eval("Name") %>' ImageUrl='<%#Eval("ImageUrl") %>' />
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <h2 class="pivot-header">Social Feed</h2>
            <div class="pivot-section" id="feedList">
                <asp:Label ID="userFeedName" runat="server" />
                <asp:DataGrid ID="feedGrid" runat="server" AutoGenerateColumns="false" BorderStyle="None" BorderWidth="0" GridLines="Horizontal">
                    <Columns>
                        <asp:BoundColumn DataField="CreatedDate" HeaderText="Created" ReadOnly="true" HeaderStyle-Width="150" ></asp:BoundColumn>
                        <asp:BoundColumn DataField="Body" HeaderText="Post Body" ReadOnly="true" HeaderStyle-Width="400"></asp:BoundColumn>
                    </Columns>
                </asp:DataGrid>
                <asp:Label ID="messages" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>
