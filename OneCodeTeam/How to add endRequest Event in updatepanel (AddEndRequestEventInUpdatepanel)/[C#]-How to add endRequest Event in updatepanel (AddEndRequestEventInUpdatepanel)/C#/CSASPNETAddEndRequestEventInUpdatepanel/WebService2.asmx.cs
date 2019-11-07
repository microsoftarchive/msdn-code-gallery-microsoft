using System;
using System.Web;
using System.Web.Services;

namespace CSASPNETAddEndRequestEventInUpdatepanel
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService2 : System.Web.Services.WebService
    {
        [WebMethod]
        public string oMoney(string sNum, string sPrice)
        {
            int iNum = int.Parse(sNum);
            double dPrice = double.Parse(sPrice);
            string str = "";
            double dSum = 0;
            if ((iNum <= 0) || (dPrice <= 0))
            {
                str = "You do not plan to buy book";
            }
            else
            {
                dSum = iNum * dPrice;
                str = "You should pay RMB:￥" + dSum;
            }
            return str;
        }
    }
}
