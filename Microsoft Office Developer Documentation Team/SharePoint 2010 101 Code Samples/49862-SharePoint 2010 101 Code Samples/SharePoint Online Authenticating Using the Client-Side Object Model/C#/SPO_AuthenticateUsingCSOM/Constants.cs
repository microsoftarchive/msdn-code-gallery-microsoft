using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPO_AuthenticateUsingCSOM
{
    class Constants
    {
        public const string WR_METHOD_OPTIONS = "OPTIONS";

        public const string FED_AUTH_COOKIE_NAME = "FedAuth";

        public const string CLAIM_HEADER_RETURN_URL = "X-Forms_Based_Auth_Return_Url";
        public const string CLAIM_HEADER_AUTH_REQUIRED = "X-FORMS_BASED_AUTH_REQUIRED";

        // messages
        public const string MSG_REQUIRED_SITE_URL = "The Site URL is required.";
        public const string MSG_NOT_CLAIM_SITE = "The requested site does not appear to have claims enabled or the Login Url has not been set.";

        public const int DEFAULT_POP_UP_WIDTH = 925;
        public const int DEFAULT_POP_UP_HEIGHT = 525;
    }
}
