<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DemoApplicationPage.aspx.cs" Inherits="UI_ApplicationPage.Layouts.UI_ApplicationPage.DemoApplicationPage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h2>Demo Application Page</h2>
    <p>
        This simple application page displays some site properties and enables 
        you to set the site description. Application pages are deployed to the Layouts
        directory. In this case the application page will be found at 
        http://intranet.contoso.com/_layouts/UI_ApplicationPage/DemoApplicationPage.aspx
    </p>
    <table width="100%">
        <tr>
            <td width="30%">
                Title:
            </td>
            <td width="70%">
                <asp:Label ID="titleLabel" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                Description:
            </td>
            <td>
                <asp:Label ID="descriptionLabel" runat="server" Text=""></asp:Label>                
            </td>
        </tr>
        <tr>
            <td>
                Author:
            </td>
            <td>
                <asp:Label ID="authorLabel" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                Created On:
            </td>
            <td>
                <asp:Label ID="createdOnLabel" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                Current User:
            </td>
            <td>
                <asp:Label ID="currentUserLabel" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                New Site Description:
            </td>  
            <td>
                <asp:TextBox ID="newSiteDescriptionTextBox" runat="server"></asp:TextBox>
                <asp:Button ID="setDescriptionButton" runat="server" OnClick="setDescriptionButton_Click" Text="Set Description" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Demo Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    Demo Application Page
</asp:Content>
