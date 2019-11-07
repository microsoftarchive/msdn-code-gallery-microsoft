<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content2" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
  <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
  <script type="text/javascript" src="../_layouts/15/sp.runtime.js"></script>
  <script type="text/javascript" src="../_layouts/15/sp.js"></script>
  <script type="text/javascript" src="../_layouts/15/sp.workflowservices.js"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
  Page Title
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="PlaceHolderMain" runat="server">
  <table>
    <tr>
      <td>Document Reviewer:
        <SharePoint:PeopleEditor AllowEmpty="false" ValidatorEnabled="true" MultiSelect="false" ID="docReviewerUser" runat="server" />
        <br />
        <br />
      </td>
    </tr>
    <tr>
      <td>Document Editor:
        <SharePoint:PeopleEditor AllowEmpty="false" ValidatorEnabled="true" MultiSelect="false" ID="docEditorUser" runat="server" />
        <br />
        <br />
      </td>
    </tr>
    <tr>
      <td>
        <input type="button" name="startWorkflowButton" value="Start" onclick="StartWorkflow()" />
        <input type="button" name="cancelButton" value="Cancel" onclick="RedirFromInitForm()" />
        <br />
      </td>
    </tr>
  </table>
  <script type="text/javascript">
    // ---------- Start workflow ----------
    function StartWorkflow() {
      var errorMessage = "An error occured when starting the workflow.";
      var subscriptionId = "", itemId = "", redirectUrl = "";

      var urlParams = GetUrlParams();
      if (urlParams) {
        //itemGuid = urlParams["ItemGuid"];
        itemId = urlParams["ID"];
        redirectUrl = urlParams["Source"];
        subscriptionId = urlParams["TemplateID"];
      }

      if (subscriptionId == null || subscriptionId == "") {
        // Cannot load the workflow subscription without a subscriptionId, so workflow cannot be started.
        alert(errorMessage + "  Could not find the workflow subscription id.");
        RedirFromInitForm(redirectUrl);
      }
      else {
        // Set workflow in-arguments/initiation parameters
        var wfParams = new Object();

        // get reviewer loginname
        var html = $("#ctl00_PlaceHolderMain_docReviewerUser_upLevelDiv");
        wfParams['DocReviewerLoginName'] = $("#divEntityData", html).attr("key");
        // get editor loginname
        var html = $("#ctl00_PlaceHolderMain_docEditorUser_upLevelDiv");
        wfParams['DocEditorLoginName'] = $("#divEntityData", html).attr("key");

        // Get workflow subscription and then start the workflow
        var context = SP.ClientContext.get_current();
        var wfManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, context.get_web());
        var wfDeployService = wfManager.getWorkflowDeploymentService();
        var subscriptionService = wfManager.getWorkflowSubscriptionService();

        context.load(subscriptionService);
        context.executeQueryAsync(

            function (sender, args) { // Success
              var subscription = null;
              // Load the workflow subscription
              if (subscriptionId)
                subscription = subscriptionService.getSubscription(subscriptionId);
              if (subscription) {
                if (itemId != null && itemId != "") {
                  // Start list workflow
                  wfManager.getWorkflowInstanceService().startWorkflowOnListItem(subscription, itemId, wfParams);
                }
                else {
                  // Start site workflow
                  wfManager.getWorkflowInstanceService().startWorkflow(subscription, wfParams);
                }
                context.executeQueryAsync(
                    function (sender, args) {
                      // Success
                      RedirFromInitForm(redirectUrl);
                    },
                    function (sender, args) {
                      // Error
                      alert(errorMessage + "  " + args.get_message());
                      RedirFromInitForm(redirectUrl);
                    }
                )
              }
              else {
                // Failed to load the workflow subscription, so workflow cannot be started.
                alert(errorMessage + "  Could not load the workflow subscription.");
                RedirFromInitForm(redirectUrl);
              }
            },
            function (sender, args) { // Error
              alert(errorMessage + "  " + args.get_message());
              RedirFromInitForm(redirectUrl);
            }
        )
      }
    }

    // ---------- Redirect from page ----------
    function RedirFromInitForm(redirectUrl) {
      window.location = redirectUrl;
    }

    // ---------- Returns an associative array (object) of URL params ----------
    function GetUrlParams() {
      var urlParams = null;
      if (urlParams == null) {
        urlParams = {};
        var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
          urlParams[key] = decodeURIComponent(value);
        });
      }
      return urlParams;
    }
  </script>

</asp:Content>
