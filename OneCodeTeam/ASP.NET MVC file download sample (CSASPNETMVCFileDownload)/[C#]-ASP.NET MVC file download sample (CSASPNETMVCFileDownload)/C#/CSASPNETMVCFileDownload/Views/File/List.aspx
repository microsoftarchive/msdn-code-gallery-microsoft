<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        All files available for download:</h2>
    <table style="width: 100%">
        <thead>
            <td>
                FileName
            </td>
            <td>
                Size(bytes)
            </td>
            <td style="width: 40">
            </td>
        </thead>
        <%
            var fileList = (List<System.IO.FileInfo>)Model;
            foreach (var file in fileList)
            {
        %>
        <tr>
            <td>
                <%= Html.ActionLink(file.Name,"Download",new {Action="Download", fn=file})  %>
            </td>
            <td>
                <%=file.Length %>
            </td>
            <td>
                <a href='<%= ResolveUrl("~/File/Download/"+ file.Name) %>'>
                    <img width="30" height="30" src='<%= ResolveUrl("~/images/download-icon.gif") %>' />
                </a>
            </td>
        </tr>
        <%} %>
    </table>
</asp:Content>
