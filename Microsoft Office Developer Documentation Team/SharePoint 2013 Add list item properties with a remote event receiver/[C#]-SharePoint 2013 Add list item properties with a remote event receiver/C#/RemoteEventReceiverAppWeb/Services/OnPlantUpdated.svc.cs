using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace RemoteEventReceiverAppWeb.Services
{
    public class OnPlantUpdated : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            if (properties.EventType == SPRemoteEventType.ItemAdding ||
                properties.EventType == SPRemoteEventType.ItemUpdating)
            {
                // Generate Image Search for Flower
                result.ChangedItemProperties.Add("Image", 
                    CreateLink(properties.ItemEventProperties));
                result.Status = SPRemoteEventServiceStatus.Continue;
            }
            return result;
        }

        private static string CreateLink(SPRemoteItemEventProperties properties)
        {
            string description = properties.AfterProperties["Title"] + " image search";
            string searchUrl = new Uri("http://www.bing.com/images/search?q=flower " + properties.AfterProperties["Title"].ToString()).AbsoluteUri;

            return searchUrl + ", " + description;
        }


        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }
    }
}
