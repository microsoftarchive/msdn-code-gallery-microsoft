<%-- The following 4 lines are ASP.NET directives needed when using SharePoint components --%>

<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%-- The markup and script in the following Content element will be placed in the <head> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
  <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
  <script type="text/javascript" src="/_layouts/15/sp.runtime.js"></script>
  <script type="text/javascript" src="/_layouts/15/sp.js"></script>

  <!-- Add your CSS styles to the following file -->
  <link rel="Stylesheet" type="text/css" href="../Content/App.css" />

  <!-- Add your JavaScript to the following file -->
  <script type="text/javascript" src="../Scripts/App.js"></script>
</asp:Content>

<%-- The markup in the following Content element will be placed in the TitleArea of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
  Page Title
</asp:Content>

<%-- The markup and script in the following Content element will be placed in the <body> of the page --%>
<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">

  <div>
    This is the homeage for the <strong>Remote Event Receiver &amp; Workflow app.</strong> This sample<br />
    demonstrates how to start a workflow using the managed client-side object mode (CSOM). This is done<br />
    using a remote event receiver (RER). When an item is added to an announcement list, it triggers a<br />
    RER that is handled by a web service in a remote web. The web service uses the Workflow Services CSOM<br />
    API to create a custom event and passes it to the workflow as well as starts it. The workflow<br />
    then updates the body of the announcement item that triggered the event using the message added<br />
    from the remote event receiver.
    <p>To use this sample, do the following:</p>
    <ol>
      <li>Go to the <a href="../Lists/AnnouncementsList">Announcements</a> list</li>
      <li>Add an item to the announcements list, but leave the <strong>Body</strong> field blank</li>
      <li>
        Go back to the item you added in the announcements list and keep refreshing the page until<br />
        you see the <strong>Body</strong> field get updated by the workflow.
      </li>
    </ol>
  </div>

</asp:Content>
