using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.ServiceModel;

namespace AZURE_CallingEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class CallingEventReceiver : SPItemEventReceiver
    {
       /// <summary>
       /// An item was added.
       /// </summary>
       public override void ItemAdded(SPItemEventProperties properties)
       {
           //This sample demonstrates how to call a WCF service hosted in Azure
           //from an event receiver. Note that this is only possible in
           //server-side code when the event receiver is deployed OUTSIDE
           //the sandbox. The WCF Service is in the WCFServiceWebRole1
           //project in this solution.

           //This event receiver calls the DayInfoService.svc WCF service in Windows Azure
           //Make sure you package and publish the service in your Window Azure account
           //before you run this event receiver

           //Get the added item
           SPListItem addedItem = properties.ListItem;
           //Create the channel factory with a Uri, binding and endpoint
           //Edit this Uri to match the Azure location where you deployed the WCF Service
           Uri serviceUri = new Uri("http://daynamercs.cloudapp.net/dayinfoservice.svc");
           BasicHttpBinding serviceBinding = new BasicHttpBinding();
           EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
           ChannelFactory<IDayInfo> channelFactory = new ChannelFactory<IDayInfo>(serviceBinding, dayNamerEndPoint);
           //Create a channel
           IDayInfo dayNamer = channelFactory.CreateChannel();
           //Now we can call the TodayIs method
           string today = dayNamer.TodayIs();
           //Add the results to the item
           addedItem["Body"] += "This item was added on a " + today;
           addedItem.Update();
           //Close the channel factory
           channelFactory.Close();
       }


    }
}
