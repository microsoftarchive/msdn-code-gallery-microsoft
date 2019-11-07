using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//This references ServiceReference1 which connects to SharePoint
using REST_CallRESTFromDesktop.ServiceReference1;
//We need the System.Net namespace for the CredentialCache class
using System.Net;

namespace REST_CallRESTFromDesktop
{
    public partial class Form1 : Form
    {
        //This code sample connects to a team site at http://intranet.contoso.com
        //To connect to your own sites, remove ServiceReference1 from the 
        //project, then add a new data source that connects to the right URL

        //This varible holds the data context for the REST service
        //Note: The class name TeamSiteDataContext was created by Visual Studio when the ServiceReference1 was added
        //If your site is not called "Team Site" the class will be called something else.
        //The URI must point to the REST service you want to call. This example calls the List Data service
        TeamSiteDataContext dataContext = new TeamSiteDataContext(new Uri("http://intranet.contoso.com/_vti_bin/listdata.svc"));

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //In the Form_Load event handler, pass credentials to the data context object
            //CredentialCache is part of the System.Net namespace. It's DefaultCredientials
            //property passes the credentials of the current user.
            dataContext.Credentials = CredentialCache.DefaultCredentials;
            //Set the DataSource to bond to the Announcements list.
            announcementsBindingSource.DataSource = dataContext.Announcements;
            //When you load the form, the items in the Annoncements list are displayed.
        }
    }
}
