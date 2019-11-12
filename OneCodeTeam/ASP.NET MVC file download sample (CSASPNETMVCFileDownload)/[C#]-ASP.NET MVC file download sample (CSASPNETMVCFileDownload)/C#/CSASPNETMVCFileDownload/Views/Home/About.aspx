<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">
    About Us
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        About</h2>
    <p>
        <ul>
            <li><b>Sample Name:</b> CSASPNETMVCFileDownload</li>
            <li><b>Language:</b> C#</li>
            <li><b>Author:</b> Steven Cheng</li>
        </ul>
    </p>
</asp:Content>
