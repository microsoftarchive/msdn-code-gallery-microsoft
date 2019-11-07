using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Collections;    
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using SPOM = Microsoft.SharePoint; 
using SPAdmin = Microsoft.SharePoint.Administration;

// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


namespace WebPartSample.VisualWebPart1
{
    public partial class VisualWebPart1UserControl : UserControl
    {
        SPAdmin.SPFarm farm = SPAdmin.SPFarm.Local;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SPAdmin.SPWebService service = farm.Services.GetValue<SPAdmin.SPWebService>("");
                foreach (SPAdmin.SPWebApplication webapp in service.WebApplications)
                {
                    foreach (SPOM.SPSite site in webapp.Sites)
                    {
                        string url = site.Url;
                        uxSites.Items.Add(url);
                    }
                    uxSites.Items[0].Selected = true;
                    GetAllListData(uxSites.SelectedItem.ToString());
                }
            }
        }

        protected void uxSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetAllListData(uxSites.SelectedItem.ToString());
        }

        private void GetAllListData(string selectedsite)
        {
            using (SPOM.SPSite siteselected = new SPOM.SPSite(selectedsite))
            {
                List<ListProperties> listpropcollection = new List<ListProperties>();
                foreach (SPOM.SPWeb web in siteselected.AllWebs)
                {
                    foreach (SPOM.SPList list in web.Lists)
                    {
                        ListProperties listprops = new ListProperties();
                        listprops.ListTitle = list.Title;  
                        listprops.ItemCount = list.ItemCount;
                        listprops.FolderCount = list.Folders != null ? list.Folders.Count : 0;
                        listprops.FieldCount = list.Fields != null ? list.Fields.Count : 0;
                        listprops.ViewCount = list.Views != null ? list.Views.Count : 0; ;
                        listprops.WorkFlowCount = list.WorkflowAssociations != null ? list.WorkflowAssociations.Count : 0;
                        listpropcollection.Add(listprops);
                        
                    }
                    if (web != null)
                    {
                        web.Dispose();
                    }
                }
                GridView1.DataSource = listpropcollection;
                GridView1.DataBind();  
            }
        }

    }
}

