using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETClearSession
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["SessionCreatedTime"] = DateTime.Now.ToString();
            Response.Write("Session Intialized at " + Session["SessionCreatedTime"]);  
        }
    }
}