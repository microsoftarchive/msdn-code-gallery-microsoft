<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BasicDataOperationsWeb.Pages.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <center><h2>Site Lists</h2></center>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        <asp:UpdatePanel ID="ListManagementPanel" runat="server" UpdateMode="Conditional">
        <ContentTemplate>   
            <center><asp:Button runat="server" ID="RefreshListButton" Text="Refresh Lists" OnClick="RefreshList_Click" />
            <asp:Button runat="server" ID="AddListButton" Text="Add List" OnClick="AddList_Click" /><asp:TextBox ID="AddListNameBox" runat="server"/></center>
            <center><asp:Button runat="server" ID="RetrieveListButton" Text="Retrieve List Items" OnClick="RetrieveListButton_Click"/><asp:TextBox ID="RetrieveListNameBox" runat="server" />
            <asp:Button runat="server" ID="AddItemButton" Text="Add Item" OnClick="AddItemButton_Click" /> <asp:TextBox ID="AddListItemBox" runat="server" /></center>
            <center>
                <asp:Button runat="server" ID="DeleteListButton" Text="Delete This List" OnClick="DeleteListButton_Click" Visible="false"/>
                <asp:Button runat="server" ID="ChangeListTitleButton" Text="Change List Title" OnClick="ChangeListTitleButton_Click" Visible="false" /><asp:TextBox ID="ChangeListTitleBox" runat="server" Visible="false"/>
            </center>
        <asp:Table ID="ListTable" GridLines="Both" HorizontalAlign="Center" CellPadding="18" CellSpacing="0" runat="server" > 
            <asp:TableHeaderRow><asp:TableHeaderCell>List Title</asp:TableHeaderCell><asp:TableHeaderCell></asp:TableHeaderCell></asp:TableHeaderRow>
        </asp:Table>
        </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
