using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
//This is the main WCF namespace
using System.ServiceModel;

namespace WCF_CallServiceFromEventReceiver.WCF_CallServiceFromEventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class WCF_CallServiceFromEventReceiver : SPItemEventReceiver
    {
       /// <summary>
       /// An item was added.
       /// </summary>
       public override void ItemAdded(SPItemEventProperties properties)
       {
           //This sample demonstrates how to call a WCF service 
           //from an event receiver. Note that this is only possible in
           //server-side code when the event receiver is deployed OUTSIDE
           //the sandbox. The WCF Service is in the WCF_ExampleService
           //project in this solution.

           //To use this sample, configure the WCF_CallServiceFromEventReceiver
           //project to deploy to your SharePoint site (the default is
           //http://intranet.contoso.com) then start that project for 
           //debugging. Run the WCF_ExampleService project and wait until the 
           //prompt tells you the service is ready. Then add an item to the 
           //Announcements list. The day name will be added to the bottom 
           //of the body.

           //I used svcutil.exe to generate the proxy class for the service
           //in the generatedDayNamerProxy.cs file. I'm going to configure this
           //in code by using a channel factory.

           //Get the added item
           SPListItem addedItem = properties.ListItem;
           //Create the channel factory with a Uri, binding and endpoint
           Uri serviceUri = new Uri("http://localhost:8088/WCF_ExampleService/Service/DayNamerService");
           WSHttpBinding serviceBinding = new WSHttpBinding();
           EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
           ChannelFactory<httpWCF_ExampleService> channelFactory = new ChannelFactory<httpWCF_ExampleService>(serviceBinding, dayNamerEndPoint);
           //Create a channel
           httpWCF_ExampleService dayNamer = channelFactory.CreateChannel();
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
