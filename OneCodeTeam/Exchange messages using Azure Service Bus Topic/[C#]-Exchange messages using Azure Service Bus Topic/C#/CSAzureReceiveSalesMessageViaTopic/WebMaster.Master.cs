using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalesSendMessagesViaTopic
{
    public partial class WebMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["menu"] != null)
            {
                int intIndex = 0;
                if (int.TryParse(Session["menu"].ToString(), out intIndex))
                {
                    showMenu(intIndex);
                }
            }

        }
        public void showMenu(int n)
        {
            switch (n)
            {
                case 1:
                    this.li1.Style.Add("background-color", "#d3d3d3");
                    break;
                case 2:
                    this.li2.Style.Add("background-color", "#d3d3d3");
                    break;
                default:
                    break;
            }
        }
    }
}