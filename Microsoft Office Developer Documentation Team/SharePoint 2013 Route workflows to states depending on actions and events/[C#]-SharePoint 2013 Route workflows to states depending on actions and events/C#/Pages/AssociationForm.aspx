<%@ Page Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage, Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" MasterPageFile="~masterurl/default.master" Language="C#" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="WorkflowServices" Namespace="Microsoft.SharePoint.WorkflowServices.ApplicationPages" Assembly="Microsoft.SharePoint.WorkflowServices.ApplicationPages, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="Content1" ContentPlaceHolderId="PlaceHolderAdditionalPageHead" runat="server">
  <script type="text/javascript" src="../Scripts/jquery-1.8.2.min.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderId="PlaceHolderPageTitleInTitleArea" runat="server">
    Custom Workflow Association Form
</asp:Content>

<%--    
        IMPORTANT NOTE: 
        Be sure to update the InitiationUrl property value to the URL of the custom initiation form.  
        The InitiationUrl property can be updated from the workflow's property grid.
--%>

<asp:Content ID="Content5" ContentPlaceHolderId="PlaceHolderMain" runat="server">
<%--    
        The following sample code creates a simple custom workflow Association Form that allows users to 
    set workflow parameter values when adding or editing a list workflow association. 
--%>

    <WorkflowServices:WorkflowAssociationFormContextControl ID="WorkflowAssociationFormContextControl1" runat="server" />
    
    <h1><label id="wfHeader"></label></h1>

    <div>
        <table>
            <tr><td colspan="2">Maintenance Person:<br /><SharePoint:PeopleEditor AllowEmpty="false" ValidatorEnabled="true" MultiSelect="false" ID="MaintenancePerson" runat="server" /><br /><br /></td></tr>
            <tr><td colspan="2">Edmunds API:<br /><input id="EdmundsApiKey" type="text" /><br /><br /></td></tr>
            <tr>
                <td><button id="Save" onclick="return runAssocWFTask()">Save</button></td>
                <td><button id="Cancel" onclick="location.href = cancelRedirectUrl; return false;">Cancel</button></td>
            </tr>
        </table>

        <script type="text/javascript">
            var errorMessage = "An error occured when saving the workflow association.";
            var dlg = null;
            var complete = 0;
            var CID_O15WorkflowTask = "0x0108003365C4474CAE8C42BCE396314E88E51F";

            // ---------- Entry point from Save button click ----------
            function runAssocWFTask() {
                // Resolve users in user fields before running validation
                var peoplePickerDict = SPClientPeoplePicker.SPClientPeoplePickerDict;
                for (var pickerId in peoplePickerDict) {
                    peoplePickerDict[pickerId].AddUnresolvedUserFromEditor(false);
                    peoplePickerDict[pickerId].ResolveAllUsers();
                }

                var form = SPClientForms.ClientFormManager.GetClientForm('Workflow');
                if (form.SubmitClientForm()) {
                    // Validation errors, don't submit the form yet. 
                    return false;
                }

                var button = document.getElementById("Save");
                var cb = new SP.Utilities.CommandBlock(null, associateWF, assocComplete);
                var task = new SP.Utilities.Task(button, SP.Utilities.Task.TaskType.autoCancel, 0, cb, inProgressDialog, null, null);
                task.start();

                return false;
            }

            function assocComplete() {
                if (dlg != null) {
                    dlg.close();
                }
            }

            function inProgressDialog() {
                if (dlg == null) {
                    dlg = SP.UI.ModalDialog.showWaitScreenWithNoClose("associating workflow", "custom workflow association", null, null);
                }
            }

            // ---------- Save workflow association ----------
            function associateWF(state, pauseFunction) {
                if (complete != 0)
                    return complete;

                var historyListId = "";
                var taskListId = "";
                var metadata = new Object();

                // Get form input values and set workflow in-argument values
                var html = $("#ctl00_PlaceHolderMain_MaintenancePerson_upLevelDiv");
                metadata['MaintenancePerson'] = $("#divEntityData", html).attr("key");

                var strInputValue = document.getElementById("EdmundsApiKey").value;
                if (strInputValue) {
                  metadata['EdmundsApiKey'] = strInputValue;
                }

                var context = SP.ClientContext.get_current();
                var web = context.get_web();
                var wfManager = SP.WorkflowServices.WorkflowServicesManager.newObject(context, web);

                var newHistoryList = null;
                var taskList = null;

                // Set history list id. If new list, create new first.
                if (historyListName) {
                    if (historyListName[0] == 'z') {
                        // Need to create new history list for the association
                        historyListName = historyListName.substring(1); //remove the 'z'
                        var listCreationInfo = new SP.ListCreationInformation();
                        listCreationInfo.set_templateType(SP.ListTemplateType.workflowHistory);
                        listCreationInfo.set_title(historyListName);
                        listCreationInfo.set_description(historyListDescription);
                        newHistoryList = web.get_lists().add(listCreationInfo);
                        context.load(newHistoryList, 'Id');
                    }
                    else {
                        // Get history list 
                        historyListId = historyListName;
                    }
                }

                // Set task list id. If new list, create new first. 
                if (taskListName) {
                    if (taskListName[0] == 'z') {
                        // Need to create new task list for the association
                        taskListName = taskListName.substring(1); 
                        var listCreationInfo = new SP.ListCreationInformation();
                        listCreationInfo.set_templateType(SP.ListTemplateType.tasksWithTimelineAndHierarchy);
                        listCreationInfo.set_title(taskListName);
                        listCreationInfo.set_description(taskListDescription);
                        taskList = web.get_lists().add(listCreationInfo);
                    }
                    else {
                        var listCollection = web.get_lists();
                        taskList = listCollection.getById(taskListName);
                    }
                    context.load(taskList, 'Id');
                    var contentTypeCollection = web.get_availableContentTypes();
                    var contentType = contentTypeCollection.getById(CID_O15WorkflowTask);
                    context.load(contentType, 'Name');
                    var taskListContentTypeCollection = taskList.get_contentTypes();
                    context.load(taskListContentTypeCollection, 'Include(Name)');
                }

                //  Check if task list contains the OOTB SharePoint 2013 Workflow Task content type

                context.executeQueryAsync(function (sender, args) {

                    complete = 0.66;

                    if (newHistoryList != null) {
                        historyListId = newHistoryList.get_id().toString();
                    }
                    taskListId = taskList.get_id().toString();

                    metadata["HistoryListId"] = historyListId;
                    metadata["TaskListId"] = taskListId;

                    var eventTypes = new Array();
                    if (autoStartCreate) {
                        eventTypes.push("ItemAdded");
                    }
                    if (autoStartChange) {
                        eventTypes.push("ItemUpdated");
                    }
                    if (allowManual) {
                        eventTypes.push("WorkflowStart");
                    }

                    // If workflow association exists, then we will update its subscription information. Otherwise, it's a new association, and we will add the new subscription.

                    if (subscriptionId != null && subscriptionId != "") {
                        // Updating an existing subscription
                        var subscription = wfManager.getWorkflowSubscriptionService().getSubscription(subscriptionId);
                        subscription.set_name(workflowName);
                        subscription.set_eventTypes(eventTypes);
                        for (var key in metadata) {
                            subscription.setProperty(key, metadata[key]);
                        }
                        // Publish
                        wfManager.getWorkflowSubscriptionService().publishSubscription(subscription);
                        context.executeQueryAsync(
                            function (sender, args) {
                                // Success
                                complete = 1;
                                location.href = redirectUrl;
                            },
                            function (sender, args) {
                                // Error occured
                                complete = 1;
                                alert(errorMessage + " " + args.get_message());
                            }
                        );
                    }
                    else {
                        // Add new workflow association
                        var newSubscription = SP.WorkflowServices.WorkflowSubscription.newObject(context);
                        newSubscription.set_definitionId(definitionId);
                        newSubscription.set_eventSourceId(eventSourceId);
                        newSubscription.set_eventTypes(eventTypes);
                        newSubscription.set_name(workflowName);
                        for (var key in metadata) {
                            newSubscription.setProperty(key, metadata[key]);
                        }
                        // Publish
                        wfManager.getWorkflowSubscriptionService().publishSubscriptionForList(newSubscription, listId);

                        context.executeQueryAsync(
                            function (sender, args) {
                                // Success
                                complete = 1;
                                location.href = redirectUrl;
                            },
                            function (sender, args) {
                                // Error occured
                                complete = 1;
                                alert(errorMessage + " " + args.get_message());
                            }
                        );
                    }
                },
                function (sender, args) {
                    // Error occured
                    complete = 1;
                    alert(errorMessage + " " + args.get_message());
                })

                complete = 0.33;
                return complete;
            }

            function setHeader() {
                var headerLabel = document.getElementById('wfHeader');
                if (headerLabel != null)
                    headerLabel.innerText = headerString;
            }

            Sys.Application.add_load(setHeader);
        </script>
</div>
</asp:Content>