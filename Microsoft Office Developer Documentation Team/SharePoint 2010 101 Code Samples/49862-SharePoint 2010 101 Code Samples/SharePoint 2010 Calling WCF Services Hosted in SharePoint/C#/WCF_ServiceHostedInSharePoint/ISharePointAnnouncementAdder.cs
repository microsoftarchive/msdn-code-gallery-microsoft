using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_ServiceHostedInSharePoint
{
    //Define the service contract for the WCF service
    [ServiceContract]
    public interface ISharePointAnnoucementAdder
    {
        //One method that adds an item to the Announcements list
        [OperationContract]
        bool AddAnnouncement(string Title, string Body);
    }

}
