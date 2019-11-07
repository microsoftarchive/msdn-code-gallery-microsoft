using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_ServiceHostedInSharePoint
{
    //This project shows you how to create a WCF service hosted in SharePoint
    //That way, you can use the full SharePoint server-side object model
    //then call the service from a client application.

    //Note that, as well as the Service Contract (ISharePointAnnouncementAdder.cs) and
    //the service implementation (this file), this project deploys the AddAnnouncement.svc
    //file to the ISAPI folder in SharePoint. That makes the service available in _vti_bin folders

    //Add required namespaces
    using System.ServiceModel.Activation;
    using Microsoft.SharePoint.Client.Services;
    using Microsoft.SharePoint;


    //These attributes instruct the SharePoint service factory to create a metadata exchange end point for the service
    //Without these attributes, we would have to use the SharePoint web.config file to configure the WCF service
    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class AnnouncementAdderService : ISharePointAnnoucementAdder
    {
        //Implement the AddAnnouncement method
        public bool AddAnnouncement(string Title, string Body)
        {
            try
            {
                //Get the announcements list
                SPList announcementsList = SPContext.Current.Web.Lists["Announcements"];
                //Add a new announcement
                SPListItem newAnnouncement = announcementsList.AddItem();
                newAnnouncement["Title"] = Title;
                newAnnouncement["Body"] = Body;
                newAnnouncement.Update();
                //Let the client know everything went well
                return true;
            }
            catch (Exception e)
            {
                //The method did not succeed
                return false;
            }

        }
    }
}
