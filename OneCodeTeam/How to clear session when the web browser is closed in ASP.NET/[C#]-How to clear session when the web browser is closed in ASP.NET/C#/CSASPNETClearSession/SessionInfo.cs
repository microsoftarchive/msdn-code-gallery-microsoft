using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;
using System.Linq;

namespace CSASPNETClearSession
{    
    public class SessionInfo
    {
        // Dummy session variable value
        private string _sessionValue;

        // Closed time of the browser
        private DateTime _closedTime;

        public string SessionValue
        {
            get
            {
                return _sessionValue;
            }
            set
            {
                _sessionValue = value;
            }
        }
    

        public DateTime BrowserClosedTime
        {
            get
            {
                return _closedTime;
            }
            set
            {
                _closedTime = value;
            }
        }
  
    }
}
