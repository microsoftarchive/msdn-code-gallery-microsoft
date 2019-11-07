using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using Microsoft.SharePoint.Client.WorkflowServices;

namespace RerWorkflowAppWeb.Services {
  public class RemoteEventReceiver : IRemoteEventService {
    public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties) {
      return new SPRemoteEventResult();
    }

    public void ProcessOneWayEvent(SPRemoteEventProperties properties) {
      if (properties.EventType != SPRemoteEventType.ItemAdded)
        return;

      // build client context using S2S
      using (ClientContext context = TokenHelper.CreateRemoteEventReceiverClientContext(properties)) {
        Web web = context.Web;

        // create a collection of name/value pairs to pass to the workflow upon starting
        var args = new Dictionary<string, object>();
        args.Add("RemoteEventReceiverPassedValue", "Hello from the Remote Event Receiver! - " + DateTime.Now.ToString());

        // get reference to Workflow Service Manager (WSM) in SP...
        WorkflowServicesManager wsm = new WorkflowServicesManager(context, web);
        context.Load(wsm);
        context.ExecuteQuery();

        // get reference to subscription service
        WorkflowSubscriptionService subscriptionService = wsm.GetWorkflowSubscriptionService();
        context.Load(subscriptionService);
        context.ExecuteQuery();

        // get the only workflow association on item's list
        WorkflowSubscription association = subscriptionService.EnumerateSubscriptionsByList(properties.ItemEventProperties.ListId).FirstOrDefault();

        // get reference to instance service (to start a new workflow)
        WorkflowInstanceService instanceService = wsm.GetWorkflowInstanceService();
        context.Load(instanceService);
        context.ExecuteQuery();

        // start the workflow
        instanceService.StartWorkflowOnListItem(association, properties.ItemEventProperties.ListItemId, args);
        
        // execute the CSOM request
        context.ExecuteQuery();
      }
    }
  }
}
