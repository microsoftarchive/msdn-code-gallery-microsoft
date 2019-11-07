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
    This is the homeage for the <strong>Approval Workflow Sample app</strong>. This app installs two <br />
    document libraries: <a href="../Lists/Drafts">Drafts</a> &amp; <a href="../Lists/Manuscripts">Manuscripts</a>. When the workflow is started, <br />
    it prompts the user to select a reviewer &amp; editor. When the workflow starts a task is assigned to the <br />
    reviewer to review the document. If they reject it, a task is assigned to the author to make additional changes. <br />
    If it is approved, a task is assigned to the editor. Once the editor approves the document, it is moved to the <br />
    <strong>Manuscripts</strong> library and both the author and editor are notified via email.
    <p>To use this sample, do the following:</p>
    <ol>
      <li>Go to the <a href="../Lists/Drafts">Drafts</a> library</li>
      <li>Add a new document to the library</li>
      <li>Open the item's workflow settings and start the only workflow option</li>
      <li>
        After a few seconds the workflow will start and you will be redirected to the <br />
        Drafts list view. Navigate to the list item’s workflow status page and keep <br />
        refreshing it to see the progress of the workflow. It should take about 10-20 <br />
        seconds, eventually you will see it add a task.
      </li>
      <li>Select the task &amp; click <strong>Proceed to Editor</strong></li>
      <li>
        You will be redirected to the item's workflow status page &amp; after a few seconds<br />
        you will see another task appear (keep refreshing the page). It should take about 10-20 <br />
        seconds, eventually you will see it add a task.
      </li>
      <li>Select the task &amp; click <strong>Proceed to Editor</strong></li>
      <li>
        You will be redirected to the item's workflow status page &amp; after a few seconds<br />
        you will see the workflow complete (keep refreshing the page). When the workflow is<br />
        complete, navigate to the <a href="../Lists/Manuscripts">Manuscripts</a> to see the copied item.
      </li>
    </ol>
  </div>

</asp:Content>
