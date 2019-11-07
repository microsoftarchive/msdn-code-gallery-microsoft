using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomProxyWeb
{
    public partial class SimpleContent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // The remote web page serves a string
            string content;
            content = "Just some text.";
            Response.ContentType="text/plain";
            Response.Write(content);
            Response.End();
        }
    }
}