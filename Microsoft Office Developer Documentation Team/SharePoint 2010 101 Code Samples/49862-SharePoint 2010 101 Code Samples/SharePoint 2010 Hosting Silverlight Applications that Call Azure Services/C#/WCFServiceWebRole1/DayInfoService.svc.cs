using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{

    //This class is the implementation of the IDayInfo service
    //contract. It defines two simple methods that return the 
    //names of days as strings. When you have deployed this 
    //WCF service to Windows Azure, you can call it from the 
    //silverlight client application. 

    //N.B the clientaccesspolicy.xml file in this project must be
    //deployed with this service in order for Silverlight to be
    //permitted to access the service in Azure. Use Package in the 
    //AZURE_DayNamerService project and the file will be automatically 
    //included in the service package file

    //Before you deploy this service to Azure, you must have a 
    //Azure account set up. Use Package in the AZURE_DayNamerService
    //project to create the service package file and cloud service
    //configuration file. Then, in the Azure Management Portal, create
    //a new hosted service and specify these files. Once that's
    //done you should be able to access the service at
    //http://YourHostedServiceName.cloudapp.net/DayInfoService.svc
    public class DayInfoService : IDayInfo
    {
        public string TodayIs()
        {
            //Find out what today is
            DayOfWeek today = DateTime.Today.DayOfWeek;
            //Return today's name
            return today.ToString();
        }

        public string TodayAdd(int daysToAdd)
        {
            //Add the requested number to today
            DateTime requestedDateTime = DateTime.Today.AddDays(daysToAdd);
            DayOfWeek requestedDay = requestedDateTime.DayOfWeek;
            //Return the day's name
            return requestedDay.ToString();
        }
    }
}
