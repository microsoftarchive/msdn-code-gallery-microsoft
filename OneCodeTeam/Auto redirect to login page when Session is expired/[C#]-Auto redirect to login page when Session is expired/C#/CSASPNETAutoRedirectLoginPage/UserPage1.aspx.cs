using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace CSASPNETAutoRedirectLoginPage
{
    public partial class UserPage : System.Web.UI.Page
    {
        public string loginDate;
        public string expressDate;
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check session is expire or timeout.
            if (Session["username"] == null)
            {
                Response.Redirect("LoginPage.aspx?info=0");
            }

            // Get user login time or last activity time.
            DateTime date = DateTime.Now;
            loginDate = date.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
            int sessionTimeout = Session.Timeout;
            DateTime dateExpress = date.AddMinutes(sessionTimeout);
            expressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
        }

    }
}