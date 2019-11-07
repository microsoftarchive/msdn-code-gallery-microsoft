<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateDocumentSet.aspx.cs" Inherits="ECM_CreateDocumentSet.Layouts.ECM_CreateDocumentSet.CreateDocumentSet" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <h1>Document Set Creation Demo</h1>
    <p>
        This page creates a new document set in the Shared Documents folder. Document
        sets enable you to manage multiple documents as one. For example, you can submit
        an entire document set as a record in a single operation. You can also apply 
        metadata and permissions to every document in the set.  Before you
        can create and use document sets, you must enable the Document Sets feature at 
        the site collection level. Then add the Document Set content type to the document
        library where you want to use them.
    </p>
    <p>
        Name: <asp:TextBox ID="nameTextbox" runat="server"></asp:TextBox>
    </p>
    <p>
        Description: <asp:TextBox ID="descriptionTextbox" runat="server"></asp:TextBox>
    </p>
    <asp:Button ID="createDocSetButton" OnClick="createDocSetButton_Click" runat="server" Text="Create Document Set" />
    <asp:Label ID="resultLabel" runat="server" Text=""></asp:Label>
</asp:Content>


<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    ECM Document Set Creation
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    ECM Document Set Creation
</asp:Content>
