<%@ Page Title="" Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeBehind="itemdetail.aspx.cs" Inherits="CSASPNETPrivateMessagesModule.itemdetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphWord" runat="server">
    <form id="form1" runat="server">
    <div id="Main">
        <div>
            <table cellpadding="5" style="width: 100%;">
                <tr>
                    <td style="width: 91px; text-align: right;">
                        From：
                    </td>
                    <td>
                        <asp:Literal ID="ltrFrom" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px; text-align: right;">
                        To：
                    </td>
                    <td>
                        <asp:Literal ID="ltrTo" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px; text-align: right;">
                        Title：
                    </td>
                    <td>
                        <asp:Literal ID="ltrTitle" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px; text-align: right;">
                        Content：
                    </td>
                    <td>
                        <asp:Literal ID="ltrContent" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px; text-align: right;">
                        SentTime：
                    </td>
                    <td>
                        <asp:Literal ID="ltrTime" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px">
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnReply" runat="server" OnClick="btnReply_Click" Text="Reply" ValidationGroup="msg" />
                        &nbsp;
                        <asp:Button ID="btnReplyAll" runat="server" OnClick="btnReplyAll_Click" Text="ReplyAll"
                            ValidationGroup="msg" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</asp:Content>
