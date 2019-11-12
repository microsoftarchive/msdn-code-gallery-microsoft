<%@ Page Title="" Language="C#" MasterPageFile="~/User.master" AutoEventWireup="true"
    CodeBehind="NewItem.aspx.cs" Inherits="CSASPNETPrivateMessagesModule.NewItem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphWord" runat="server">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <div id="Main">
        <div>
            <table cellpadding="5" style="width: 100%;">
                <tr>
                    <td style="width: 91px; text-align: right;">
                        To：
                    </td>
                    <td>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:TextBox runat="server" ID="txtTo"></asp:TextBox>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px; text-align: right;">
                        Title：
                    </td>
                    <td>
                        <asp:TextBox ID="txtTitle" runat="server" Width="260px" MaxLength="100"></asp:TextBox>
                        &nbsp;<asp:RequiredFieldValidator ID="rfvOldPass" runat="server" ErrorMessage="*"
                            ControlToValidate="txtTitle" ValidationGroup="msg"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px; text-align: right;">
                        Content：
                    </td>
                    <td>
                        <asp:TextBox ID="txtContent" runat="server" Width="380px" TextMode="MultiLine" Height="150px"
                            MaxLength="2000"></asp:TextBox>
                        &nbsp;<asp:RequiredFieldValidator ID="rfvNewPass" runat="server" ErrorMessage="*"
                            ControlToValidate="txtContent" ValidationGroup="msg"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td style="width: 91px">
                        &nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnEnter" runat="server" OnClick="btnEnter_Click" Text="Sent" ValidationGroup="msg" />
                        &nbsp;
                        <asp:Button ID="btnDraft" runat="server" OnClick="btnDraft_Click" Text="Save Draft"
                            ValidationGroup="msg" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="divUser" style="display: block; height: 220px;">
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <asp:CheckBoxList ID="chlUser" runat="server" RepeatDirection="Horizontal" RepeatColumns="4"
                    DataTextField="UserName" DataValueField="ProviderUserKey">
                </asp:CheckBoxList>
                <p style="text-align: center;">
                    <asp:Button runat="server" ID="btnAdd" Text="Add to list" OnClick="btnAdd_Click"
                        CausesValidation="false" /></p>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</asp:Content>
