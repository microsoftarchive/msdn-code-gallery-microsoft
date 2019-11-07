using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace CSASPNETClearSession
{
    /// <summary>
    /// Summary description for ServiceToClearSession
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class ServiceToClearSession : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public void RecordCloseTime()
        {
            HttpContext.Current.Session.Clear();

            //Logging the data t the XML File
            SessionInfoDataSource dataSource = new SessionInfoDataSource();
            SessionInfo newSessionInfo = new SessionInfo()
                {
                    SessionValue = "Current Session value is " + Session["SessionCreatedTime"],
                    BrowserClosedTime = DateTime.Now
                };
            dataSource.InsertSessionInfo(newSessionInfo);

            dataSource.Save();
        }
    }
}
