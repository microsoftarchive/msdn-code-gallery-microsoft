<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FindTaggedItems.aspx.cs" Inherits="ECM_FindTaggedItems.Layouts.ECM_FindTaggedItems.FindTaggedItems" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h1>Querying Document Sets</h1>
    <p>
        This page demonstrates how to find all the items in a list that have been tagged
        with a particular term from a termstore. The text you enter in the "Search For"
        textbox is used to locate a term, then items in your specified list are searched. 
        Those tagged with the term are returned and displayed.
    </p>
    <p>
        Name of list to search: <asp:TextBox ID="nameOfListTextbox" runat="server"></asp:TextBox>
    </p>
    <p>
        Search For:
        <asp:TextBox ID="searchForTextBox" runat="server"></asp:TextBox>
        <asp:Button ID="getItemsButton" OnClick="getItemsButton_Click" runat="server" Text="Get Items" />
    </p>
    <p>
        <asp:Label ID="resultLabel" runat="server" Text=""></asp:Label>
    </p>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    ECM Find Tagged Items
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    ECM Find Tagged Items
</asp:Content>
