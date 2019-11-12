<%@ Page Title="" Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeBehind="MessageList.aspx.cs" Inherits="CSASPNETPrivateMessagesModule.MessageList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphWord" runat="server">
    <form id="form1" runat="server">
    <div id="Main">
        <asp:GridView ID="gdvView" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="MessageTitle" HeaderText="Title" />
                <asp:BoundField DataField="Message" HeaderText="Content" />
                <asp:BoundField DataField="CreateDate" HeaderText="CreateDate" />
                <asp:TemplateField HeaderText="Operate" ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkDel" runat="server" CommandName="lnDelete" Text="Delete" OnClientClick="return confirm('To confirm that you want to delete?');"></asp:LinkButton>
                        <a href="itemdetail.aspx?id=<%# Eval("MessageID") %>">detail</a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:HiddenField ID="hifIds" runat="server" />
    </div>
    </form>
</asp:Content>
