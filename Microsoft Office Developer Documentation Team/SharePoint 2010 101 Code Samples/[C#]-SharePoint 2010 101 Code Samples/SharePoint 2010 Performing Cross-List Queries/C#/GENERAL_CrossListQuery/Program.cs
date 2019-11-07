using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace GENERAL_CrossListQuery
{
    class Program
    {
        //This console application searches for all entries in all Tasks folders
        //that are assigned to the current user. Try creating tasks in several sites
        //within the site collection, assign them to your own account then run this
        //project. All tasks should be returned in a single operation
        static void Main(string[] args)
        {
            //Start by formulating the query in CAML
            SPSiteDataQuery query = new SPSiteDataQuery();

            //Get IDs for the SharePoint built-in fields we want to use
            string assignedToID = SPBuiltInFieldId.AssignedTo.ToString("B");
            string taskDueDateID = SPBuiltInFieldId.TaskDueDate.ToString("B");
            string titleID = SPBuiltInFieldId.Title.ToString("B");
            string taskStatusID = SPBuiltInFieldId.TaskStatus.ToString("B");
            string percentCompleteID = SPBuiltInFieldId.PercentComplete.ToString("B");

            //This is the selection creterion
            string whereClause = "<Where><Eq><FieldRef ID='" + assignedToID + "' />"
                + "<Value Type='Integer'><UserID/></Value>"
                + "</Eq></Where>";

            //This is the sort order
            string orderByClause = "<OrderBy><FieldRef ID='" + taskDueDateID + "' /></OrderBy>";

            //Set the query CAML
            query.Query = whereClause + orderByClause;

            //We will query all the Tasks lists
            query.Lists = "<Lists ServerTemplate='107' />";

            //Define the view fields in the result set
            string viewFields = "<FieldRef ID='" + titleID + "' />"
                + "<FieldRef ID='" + taskDueDateID + "' Nullable='TRUE' />"
                + "<FieldRef ID='" + taskStatusID + "' Nullable='TRUE' />"
                + "<FieldRef ID='" + percentCompleteID + "' Nullable='TRUE' />";
            query.ViewFields = viewFields;

            //Query all the SPWebs in this SPSite
            query.Webs = "<Webs Scope='SiteCollection'>";

            //Get the SPSite and SPWeb, ensuring correct disposal
            //Replace the URL with your own site collection
            using (SPSite site = new SPSite("http://intranet.contoso.com/"))
            {

                using (SPWeb web = site.OpenWeb())
                {

                     //Run the query
                    DataTable resultsTable = web.GetSiteData(query);

                    //Print a heading line
                    Console.WriteLine("{0, -10} {1, -30} {2, -20} {3}", "Date Due", "Task", "Status", "% Complete");

                    //Loop through the results and print them
                    foreach (DataRow currentRow in resultsTable.Rows)
                    {
                        //Extract various values
                        string dueDate = (string)currentRow[taskDueDateID];
                        string task = (string)currentRow[titleID];
                        string status = (string)currentRow[taskStatusID];
                        string percentComplete = (string)currentRow[percentCompleteID];

                        //Format the due date
                        DateTime dueDateTime;
                        bool hasDate = DateTime.TryParse(dueDate, out dueDateTime);
                        if (hasDate)
                        {
                            dueDate = dueDateTime.ToShortDateString();
                        }
                        else
                        {
                            dueDate = String.Empty;
                        }

                        //Format the percent complete string
                        decimal pctDec;
                        bool hasValue = decimal.TryParse(percentComplete, out pctDec);
                        if (hasValue)
                        {
                            percentComplete = pctDec.ToString("P0");
                        }
                        else
                        {
                            percentComplete = "0%";
                        }

                        //Print a line for this row
                        Console.WriteLine("{0, -10} {1, -30} {2, -20} {3, 10}", dueDate, task, status, percentComplete);

                    }

                }

            }
            //Wait for the user to press a key before closing
            Console.ReadKey();

        }
    }
}
