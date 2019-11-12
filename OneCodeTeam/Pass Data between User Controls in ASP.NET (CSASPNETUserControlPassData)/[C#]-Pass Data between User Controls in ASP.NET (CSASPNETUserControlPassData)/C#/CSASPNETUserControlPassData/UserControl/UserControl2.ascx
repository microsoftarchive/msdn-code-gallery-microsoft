<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserControl2.ascx.cs" Inherits="CSASPNETUserControlPassData.UserControl2" %>
<style type="text/css">
    .UserControl2
    {
        border:1px solid;
        border-color:Blue;
    }
</style>
<div class="UserControl2">
   User Control 2 element:
   <br />
   User Control 2 Message: <asp:Label ID="lbPublicVariable2" runat="server"></asp:Label>
   <br />
   Modify UserControl1 public variable: <asp:TextBox ID="tbModifyUserControl1" runat="server"></asp:TextBox> 
   <asp:Button ID="btnSubmit" runat="server" Text="Commit Text" 
        onclick="btnSubmit_Click" />
   <asp:Label ID="lbFormatMessage" runat="server"></asp:Label>
   <br />
   <asp:Label ID="lbMessage" runat="server"></asp:Label>
</div>