using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Win8ServiceDiscovery
{/// <summary>
    ///
    /// </summary>
    /// 
    public class Config
    {
        // App registration credentials for Microsoft account (Live Id).
        public string MicrosoftAccountClientId = "";            //"000000004412F88B";
        public string MicrosoftAccountClientSecret = "";        //"pt5Yt6UVwxHhWzGmJPELzzWiT8J2sWBP";
        public string MicrosoftAccountRedirectUri = "";         //"http://msdn.microsoft.com/office/office365/";


        // App registration for Organizational account (Office 365 account).
        public string OrganizationalAccountClientId = "";       //"81639e2b-8902-4721-b642-39c8bce47eae";
        public string OrganizationalAccountRedirectUri = "";    //"http://msdn.microsoft.com";

    }
}


