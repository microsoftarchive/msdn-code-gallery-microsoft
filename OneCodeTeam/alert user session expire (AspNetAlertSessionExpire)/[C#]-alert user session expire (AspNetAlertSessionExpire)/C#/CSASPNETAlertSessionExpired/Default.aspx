<%@ Page Title="Alert Session is about to expired" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="CSASPNETAlertSessionExpired._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<script  type="text/javascript"  language="javascript" src="Scripts/jquery-1.4.1-vsdoc.js"></script>
<script type="text/javascript" language="javascript" src="Scripts/SessionExpired.js"></script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<table id="tbTestSessionState">
<tr>
<td>
<asp:Button ID="btnSessionState" runat="server" Text="GetSessionState" OnClick="SessionState_Click" />
</td>
</tr>
<tr>
<td>

Session State: <asp:Label ID="lbSessionState" runat="server"></asp:Label>
</td>
</tr>
</table>
</asp:Content>
