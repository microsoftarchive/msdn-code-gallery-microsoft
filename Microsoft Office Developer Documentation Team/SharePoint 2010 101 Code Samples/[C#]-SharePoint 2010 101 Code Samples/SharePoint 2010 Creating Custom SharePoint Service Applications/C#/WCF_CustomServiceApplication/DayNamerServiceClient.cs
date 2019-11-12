using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace WCF_CustomServiceApplication.Client
{
    //This class is the proxy that runs on Web-Front End
    //servers. Web parts etc use this class to call the
    //Service Application and call its methods.
    public sealed class DayNamerServiceClient
    {
        private SPServiceContext _serviceContext;

        public DayNamerServiceClient(SPServiceContext serviceContext)
        {
            if (serviceContext == null)
                throw new ArgumentNullException("serviceContext");

            _serviceContext = serviceContext;
        }

        //Implement the methods defined in the contract
        public string TodayIs()
        {
            string result = string.Empty;

            // run the call against the application proxy
            DayNamerServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.TodayIs());

            return result;
        }

        public string TodayAdd(int daysToAdd)
        {
            string result = string.Empty;

            // run the call against the application proxy
            DayNamerServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.TodayAdd(daysToAdd));

            return result;
        }
    }
}
