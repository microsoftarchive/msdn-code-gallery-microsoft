using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O365_Dashboard
{

    // This class is designed to define user object values.
    public class O365User
    {
        private string displayName = "";
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        private string userPrincipalName = "";
        public string UserPrincipalName
        {
            get { return userPrincipalName; }
            set { userPrincipalName = value; }
        }

        private string userStatus = "";
        public string UserStatus
        {
            get { return userStatus; }
            set { userStatus = value; }
        }
    }
}
