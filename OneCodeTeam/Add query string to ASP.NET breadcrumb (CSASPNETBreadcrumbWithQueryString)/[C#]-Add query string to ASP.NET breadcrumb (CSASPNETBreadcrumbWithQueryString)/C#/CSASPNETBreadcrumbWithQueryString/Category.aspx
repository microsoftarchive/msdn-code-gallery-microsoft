<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Category.aspx.cs" Inherits="CSASPNETBreadcrumbWithQueryString.Category" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Category</title>
</head>
<body>
    <form id="form1" runat="server">

    <p>This is category page.</p>

    <asp:SiteMapPath ID="SiteMapPath1" runat="server">
    </asp:SiteMapPath>

    <br /><br />

    <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:TemplateField HeaderText="Items">
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# String.Format("Item.aspx?name={0}", Container.DataItem) %>'><%# Container.DataItem %></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    </form>
</body>
</html>
