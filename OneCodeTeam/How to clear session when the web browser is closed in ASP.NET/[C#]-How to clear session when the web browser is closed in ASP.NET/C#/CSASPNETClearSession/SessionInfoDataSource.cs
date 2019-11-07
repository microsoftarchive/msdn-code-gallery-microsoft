using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CSASPNETClearSession
{
    /// <summary>
    /// DataSource class to write into XML file
    /// </summary>
    public class SessionInfoDataSource
    {
        private static string filePath = HttpContext.Current.Server.MapPath("~/App_Data/SessionInfo.xml");

        private static XDocument SessionInfoXDoc;

        public SessionInfoDataSource()
        {
            SessionInfoXDoc = XDocument.Load(filePath);
        }

        /// <summary>
        /// Insert SessionInfo message to XML file
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        public void InsertSessionInfo(SessionInfo sessionInfo)
        {
            SessionInfoXDoc.Root.Add(convertSessionInfoToXElement(sessionInfo));
        }

        /// <summary>
        /// Save data source changes
        /// </summary>
        public void Save()
        {
            SessionInfoXDoc.Save(filePath);
        }
            
        /// <summary>
        /// Convert Class to XML message
        /// </summary>
        /// <param name="sessionInfo"></param>
        /// <returns></returns>
        private XElement convertSessionInfoToXElement(SessionInfo sessionInfo)
        {
            if (sessionInfo != null)
            {
                XElement xDoc = new XElement("SessionInfo",
                    new XElement("SessionValue", sessionInfo.SessionValue),
                    new XElement("BrowserCloseTime", sessionInfo.BrowserClosedTime.ToString("MM/dd/yyyy HH:mm:ss")));
                return xDoc;
            }
            return null;
        }
    }
}