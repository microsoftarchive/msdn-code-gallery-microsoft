using System.Security;

namespace O365_SingleSignOn
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
