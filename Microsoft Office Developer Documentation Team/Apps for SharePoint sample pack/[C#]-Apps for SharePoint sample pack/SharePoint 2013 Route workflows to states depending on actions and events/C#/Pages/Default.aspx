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
    This is the homepage for the <strong>Fleet Management App</strong>. This sample demonstrates<br />
    a workflow that once the user enters a new vehicle into the <a href="../Lists/Fleet Management">Fleet Management</a> list,<br />
    starts immediately and retrieves information about the vehicle from <a href="http://www.edmunds.com">Edmunds</a> public<br />
    <a href="http://developer.edmunds.com/docs/The_Vehicle_API/">Vehicle API web service</a>. 
    <p>
      Once the vehicle has been updated, the workflow sets a task that waits for 30 seconds, simulating<br />
      a 3-month delay for scheduled maintenance (such as an oil change). At this time the car is taken<br />
      out of service and a work task is assied to the maintenance owner for the vehicle. Once the task is<br />
      complete, the workflow puts the car back in service and restarts the timer.
    </p>
    <p>
      In addition, using the menu item drop down (aka: the ECB menu) for a vehicle, a user can manually<br />
      request out-of-schedule service for the vehicle in the case of a flat tire or damage. This is accomplished<br />
      using custom events that the workflow is listening for.
    </p>
    <p>
      To use this sample, do the following:
      <ol>
        <li>
          The sample automatically created an association to automatically start the workflow, but some <br />
          values need to be filled in on the association form after deploying the sample app (node this<br />
          only needs to be done one time per deployment). To do this, follow these steps:
          <ol style="list-style:lower-alpha;">
            <li>Navigate to <a href="../Lists/Fleet Management">Fleet Management</a> list</li>
            <li>Using the ribbon, select the List tab and click the <strong>Workflow Settings</strong> button</li>
            <li>On the <strong>Settings &gt; Workflow Settings</strong> page, click the <strong>Fleet Management Workflow</strong> that<br />was previously created</li>
            <li>Click the <strong>Next</strong> button at the bottom of the page</li>
            <li>
              On the association form, select a person to assign maintenance tasks &amp; enter the<br />
              API key you obtained from registering at the <a href="http://www.edmunds.com"></a>Edmunds site to use the <br />
              <a href="http://developer.edmunds.com/docs/The_Vehicle_API/">Vehicle API</a>. You must first register for an account &amp; then apply for <br />
              an API key at <a href="http://developer.edmunds.com/">developer.edmunds.com</a>.
            </li>
          </ol>
        </li>
        <li>
          To test the workflow, follow these steps:
          <ol style="list-style:lower-alpha;">
            <li>Navigate to <a href="../Lists/Fleet Management">Fleet Management</a> list</li>
            <li>Add a new vehicle using a VIN number (if it isn't valid, nothing will get updated)</li>
            <li>When you save the item you'll see the workflow start and query the Edmunds Vehicle API</li>
            <li>
              After 30 seconds the vehicle will be taken out of service and a maintenance task will<br />
              be assigned to the maintenance person
            </li>
            <li>Once completing the maintenance task is complete, the vehicle will be placed back in service</li>
          </ol>
        </li>
        <li>
          Also try taking the vehicle out of service manually:
          <ol style="list-style:lower-alpha;">
            <li>When the vehicle is in service, navigate to <a href="../Lists/Fleet Management">Fleet Management</a> list</li>
            <li>Use the ECB menu to select the vehicle and select "Submit Maintenance Request"</li>
            <li>Enter a reason to take it out of service (ie: flat tire)</li>
            <li>Notice the vehicle is taken out of service</li>
          </ol>
        </li>
      </ol>
    </p>
  </div>

</asp:Content>
