<%@ Page language="C#" Inherits="Microsoft.SharePoint.Publishing.PublishingLayoutPage,Microsoft.SharePoint.Publishing,Version=14.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePointWebControls" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="PublishingWebControls" Namespace="Microsoft.SharePoint.Publishing.WebControls" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="PublishingNavigation" Namespace="Microsoft.SharePoint.Publishing.Navigation" Assembly="Microsoft.SharePoint.Publishing, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<!-- This page layout is designed to display a content type based on the Article Page
     content type with an extra Media Field column called "Article Video" -->
<asp:Content ContentPlaceholderID="PlaceHolderPageTitle" runat="server">
	<SharePointWebControls:FieldValue id="PageTitle" FieldName="Title" runat="server"/>
</asp:Content>

<asp:Content ContentPlaceholderID="PlaceHolderMain" runat="server">
    <H2>This is the Video Article page layout</H2>

    <!-- It's a good idea to include a Web Part Manager -->
    <WebPartPages:SPProxyWebPartManager runat="server" id="ProxyWebPartManager"></WebPartPages:SPProxyWebPartManager>

    <!-- Show the page sub title -->
    <div class="customSubTitle">
        <SharePointWebControls:NoteField ID="NoteField1" FieldName="Comments" InputFieldLabel="SubTitle" DisplaySize="50" runat="server"></SharePointWebControls:NoteField>
    </div>
            
    <table>
        <tr>
            <td>
                <!-- Show the page content -->
                <PublishingWebControls:RichHtmlField ID="RichHtmlField1" FieldName="PublishingPageContent" runat="server"/>
            </td>
            <td>
                <!-- Show the video control -->
                <PublishingWebControls:MediaFieldControl FieldName="Article_x0020_Video"
                    runat="server"     
                    ID="MediaFieldControl1"     
                    DisplayMode="Overlay"     
                    ErrorMessage="cannot display media field"     
                    Loop="true"     
                    PresentationLocked="false"     
                    TemplateSource=""     
                    MediaSource="" 
                    InDesign="true">
                </PublishingWebControls:MediaFieldControl>
            </td>
        </tr>    
    </table>

</asp:Content>
