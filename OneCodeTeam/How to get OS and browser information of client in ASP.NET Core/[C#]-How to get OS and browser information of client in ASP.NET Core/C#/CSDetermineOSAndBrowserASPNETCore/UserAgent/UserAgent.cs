using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSDetermineOSAndBrowserASPNETCore.UserAgent
{
    public class UserAgent
    {
        private string _userAgent;

        private ClientBrowser _browser;
        public ClientBrowser Browser
        {
            get
            {
                if (_browser == null)
                {
                    _browser = new ClientBrowser(_userAgent);
                }
                return _browser;
            }
        }

        private ClientOS _os;
        public ClientOS OS
        {
            get
            {
                if (_os == null)
                {
                    _os = new ClientOS(_userAgent);
                }
                return _os;
            }
        }

        public UserAgent(string userAgent)
        {
            _userAgent = userAgent;
        }
    }
}
