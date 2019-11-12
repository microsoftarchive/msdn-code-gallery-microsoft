using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    //This is the interface that defines the WCF service contract
    [ServiceContract]
    public interface IDayInfo
    {
        //Two simple operation contracts that return strings
        [OperationContract]
        string TodayIs();

        [OperationContract]
        string TodayAdd(int daysToAdd);
    }


}
