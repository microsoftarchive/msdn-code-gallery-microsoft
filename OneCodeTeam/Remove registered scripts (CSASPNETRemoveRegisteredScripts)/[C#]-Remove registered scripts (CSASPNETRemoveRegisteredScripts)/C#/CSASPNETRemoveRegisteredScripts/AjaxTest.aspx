<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AjaxTest.aspx.cs" Inherits="CSASPNETRemoveRegisteredScripts.AjaxTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="Form2" runat="server">
    <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
    &nbsp;<asp:Button ID="btnRemove" runat="server" Text="Remove" OnClick="btnRemove_Click" />
    <div>
        <br />
        <asp:ScriptManager ID="ScriptManager1" EnablePartialRendering="true" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <asp:XmlDataSource ID="XmlDataSource1" DataFile="~/App_Data/Contacts.xml" XPath="Contacts/Contact"
                    runat="server" />
                <asp:DataList ID="DataList1" DataSourceID="XmlDataSource1" CellPadding="3" GridLines="Horizontal"
                    runat="server">
                    <ItemTemplate>
                        <div style="font-size: larger; font-weight: bold; cursor: pointer;" onclick='ToggleItem(<%# Eval("ID") %>);'>
                            <span>Name:
                                <%# Eval("Name") %></span>
                        </div>
                        <div id='div<%# Eval("ID") %>' style="display: block; visibility: visible;">
                            <span>
                                <%# Eval("Age") %></span>
                            <br />
                        </div>
                    </ItemTemplate>
                    <AlternatingItemStyle BackColor="#F7F7F7" />
                    <ItemStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                </asp:DataList>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
