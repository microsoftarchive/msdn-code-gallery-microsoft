using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.UserCode;

namespace SPO_SandboxedWorkflowActivity
{
    [Guid("40DE40EF-E494-4995-822C-BF623092A1E4")]
    public class SandboxedWorkflowActivity
    {
        //The activity's main public method must return a HashTable
        public Hashtable ModifyItem(SPUserCodeWorkflowContext context)
        {
            Hashtable results = new Hashtable();
            try
            {
                //Find out about the web, the list and the item in this workflow
                using (SPSite site = new SPSite(context.SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb(context.WebUrl))
                    {
                        SPList list = web.Lists[context.ListId];
                        SPListItem workflowItem = list.GetItemById(context.ItemId);
                        //Modify the body field
                        workflowItem["Body"] += "This item was modified by a sandboxed workflow";
                        results["Result"] = "Success";
                        //Save the changes
                        workflowItem.Update();
                    }
                }
            }
            catch (Exception e)
            {
                results["Result"] = "Failure";
            }
            //Return the hashtable
            return results;
        }

    }
}
