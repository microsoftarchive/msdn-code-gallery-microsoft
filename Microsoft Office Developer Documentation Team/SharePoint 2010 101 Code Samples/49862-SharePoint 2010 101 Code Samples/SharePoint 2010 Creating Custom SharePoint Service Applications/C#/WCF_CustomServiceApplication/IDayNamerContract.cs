using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace WCF_CustomServiceApplication.Server
{
    //This interface defines the WCF service contract for
    //the custom service application. 
    [ServiceContract]
    public interface IDayNamerContract
    {
        [OperationContract]
        string TodayIs();
        [OperationContract]
        string TodayAdd(int daysToAdd);
    }
}
