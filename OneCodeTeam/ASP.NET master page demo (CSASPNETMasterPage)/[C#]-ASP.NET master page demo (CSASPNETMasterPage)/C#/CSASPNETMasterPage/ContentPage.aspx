<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeBehind="ContentPage.aspx.cs" Inherits="CSASPNETMasterPage.ContentPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentHolder" runat="server">
    <div style="background-color: #D4D0C8; width: 500px;">
        <p>
            <h3>Content Page</h3>
            Enter Your Name Please:<br />
            <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            <asp:Button ID="Button1" runat="server" Text="Button" 
            onclick="Button1_Click" />
        </p>
    </div>
</asp:Content>
