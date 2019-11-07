using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WebService1 : System.Web.Services.WebService
    {
        [WebMethod]
        public string Choose(string str)
        {
            string book = "";
            if (str == "")
            {
                book = "You have no choice or do not like book";
            }
            else
            {
                book = "Your favorite book is:" + str + " <input id=\"btBuy\" type=\"button\" value=\"Buy\" onclick=\"return ShowBuy()\" />";
            } 
            return book;
        }
    }
}
