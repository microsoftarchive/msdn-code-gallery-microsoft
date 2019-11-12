using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace O365_ManageUsers
{

    // This class is storage purpose of user credential and connection string.
    public static class UserCredential
    {
        private static string userName = "";
        public static string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private static SecureString password;
        public static SecureString Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
