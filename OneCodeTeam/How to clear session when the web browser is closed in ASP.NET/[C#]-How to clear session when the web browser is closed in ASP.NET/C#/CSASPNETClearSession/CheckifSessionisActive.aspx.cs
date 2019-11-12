using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETClearSession
{
    public partial class CheckifSessionisActive : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("Current Session value is " + Session["SessionCreatedTime"]);  
        }
    }
}