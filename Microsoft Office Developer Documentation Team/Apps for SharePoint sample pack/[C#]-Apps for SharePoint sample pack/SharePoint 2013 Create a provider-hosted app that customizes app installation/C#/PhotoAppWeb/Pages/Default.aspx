<%@ Page Language="C#" MasterPageFile="~/Pages/RemoteWeb.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PhotoAppWeb.Pages.Default" %>



<asp:Content ID="Content2" ContentPlaceHolderID="PlaceholderMain" runat="server">

  <p>Inspect the Photos Picture library in the host web and verify that it exists and contains several photos.</p>

  <p>
    <asp:HyperLink ID="linkPhotos" runat="server">Photos Picture library in host web</asp:HyperLink>
  </p>

</asp:Content>