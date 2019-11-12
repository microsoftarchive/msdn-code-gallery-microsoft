<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateContentOrganizerRule.aspx.cs" Inherits="RM_CreateContentOrganizerRule.Layouts.RM_CreateContentOrganizerRule.CreateContentOrganizerRule" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h1>Creating Content Organizer Rules in Code</h1>
    <p>
        This application page demonstrates how to create a routing rule for the Content
        Organizer in a SharePoint list. The page creates a routing rule for the Shared
        Documents library. This rule moves any item of the Document content type to the
        sub folder Document Destination. Make sure this folder and library exist before 
        you run this code. Also ensure that the site-level Content Organizer feature
        is enabled.
    </p>
    <p>
        <asp:Button ID="createRuleButton" OnClick="createRuleButton_Click" runat="server" Text="Create Rule" />
        <asp:Label ID="resultsLabel" runat="server" Text=""></asp:Label>
    </p>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    RM Content Organizer Rule
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    RM Content Organizer Rule
</asp:Content>
