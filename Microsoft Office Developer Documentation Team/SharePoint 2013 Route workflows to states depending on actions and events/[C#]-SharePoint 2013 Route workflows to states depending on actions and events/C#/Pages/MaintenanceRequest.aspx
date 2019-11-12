<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
  <SharePoint:ScriptLink Name="sp.runtime.js"          runat="server" OnDemand="false" LoadAfterUI="true" Localizable="false" />
  <SharePoint:ScriptLink Name="sp.js"                  runat="server" OnDemand="false" LoadAfterUI="true" Localizable="false" />
  <SharePoint:ScriptLink Name="sp.workflowservices.js" runat="server" OnDemand="false" LoadAfterUI="true" Localizable="false" />

  <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
  <script type="text/javascript" src="../Scripts/jQuery-customPlugins.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PlaceHolderMain" runat="server">
  <WebPartPages:WebPartZone runat="server" FrameType="TitleBarOnly" ID="full" Title="loc:full" />

  Enter a reason for the maintenance request:<br />
  <input type="text" id="maintenanceReason" />
  <input type="button" name="submitMaintenanceButton" value="Submit Maintenance Request" onclick="submitCustomEvent()" />

  <script type="text/javascript">
    function submitCustomEvent() {
      // init the CSOM context
      var context = SP.ClientContext.get_current();
      // get reference to the Workflow Instance Service via the Workflow Service Manager
      var serviceManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, context.get_web());
      var instanceService = serviceManager.getWorkflowInstanceService();
      context.load(instanceService);
      context.executeQueryAsync(function () {
        // get the listID & itemID from the querystring for the item in question
        var listId = decodeURIComponent($.getQueryStringValue("SPListId"));
        var itemId = decodeURIComponent($.getQueryStringValue("SPListItemId"))

        // get all instances of the workflow on the current item (should only be one)
        var instancesCollection = instanceService.enumerateInstancesForListItem(listId, itemId);
        context.load(instancesCollection);
        context.executeQueryAsync(function () {
          // enumerate through the instance(s) and submit the event
          var enumerator = instancesCollection.getEnumerator();
          while (enumerator.moveNext()) {
            instanceService.publishCustomEvent(enumerator.get_current(), "AdHocMaintenanceRequest", $("#maintenanceReason").val());
          }
          // execute the CSOM request & when complete, redirect to the original page
          context.executeQueryAsync(function () {
            window.location = decodeURIComponent($.getQueryStringValue("Source"));
          });
        });
      });
    };
  </script>
</asp:Content>
