using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace CSAsyncUpload
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            string strPath = MapPath("~/upload/") + Path.GetFileName(e.FileName);
            AsyncFileUpload1.SaveAs(strPath);
            displayImage.ImageUrl = strPath;
        }
    }
}