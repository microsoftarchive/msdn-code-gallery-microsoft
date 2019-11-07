<%@ Page language="C#" Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<!-- This is a simple example of a Custom Page layout. This file is the page layout itself.
     You must place everything within <asp:Content> controls. Within these add HTML markup 
     and ASP and SharePoint controls to render fields within the content type that you 
     associate this page layout with. See Elements.xml for details of how to deploy this
     page layout and associate it with a publishing content type. -->

<asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
    <!-- Show the page title -->
	<SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
    <div style="color: red;">This is an example of a custom page layout</div>

    <!-- It's a good idea to include a Web Part Manager -->
    <WebPartPages:SPProxyWebPartManager runat="server" id="ProxyWebPartManager"></WebPartPages:SPProxyWebPartManager>

    <!-- Show the page sub title -->
    <div class="customSubTitle">
        <SharePointWebControls:NoteField FieldName="Comments" InputFieldLabel="SubTitle" DisplaySize="50" runat="server"></SharePointWebControls:NoteField>
    </div>
            
    <!-- Show the page content -->
    <PublishingWebControls:RichHtmlField FieldName="PublishingPageContent" runat="server"/>
</asp:Content>
