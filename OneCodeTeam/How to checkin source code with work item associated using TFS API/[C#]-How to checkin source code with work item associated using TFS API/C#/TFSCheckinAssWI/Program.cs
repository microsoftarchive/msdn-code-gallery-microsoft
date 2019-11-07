using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections;

namespace DemoCheckinAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Collection URL.
            string serverurl = "http://tfs-2015:8080/tfs/tfsdemo";
            //Workspace name.
            string workspaceName = "Checkin";
            //ID of Work Item that need to be associated. 
            int workItemIdLink = 9;
            //Comment for check-in
            string checkinComment = "DemoCheckin";
            //Local path of the file
            var localPath = new DirectoryInfo("C:\\Users\\zhawan2\\Documents\\Workspaces\\ProjectForTest2015TFS");
            var file = Path.Combine(localPath.FullName, "test.txt");
            try
            {
                TfsTeamProjectCollection tfsCollect = new TfsTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(serverurl));
                WorkItemStore WIStore = tfsCollect.GetService<WorkItemStore>();
                VersionControlServer versionControl = tfsCollect.GetService<VersionControlServer>();
                string user = versionControl.AuthorizedUser;
                Workspace workSpace = versionControl.GetWorkspace(workspaceName, user);
                WorkItem workItem = WIStore.GetWorkItem(workItemIdLink);
                var WIChecInInfo = new[]
                {
                    new WorkItemCheckinInfo(workItem, WorkItemCheckinAction.Associate)
                };
                workSpace.PendEdit(file);
                int changesetNo = workSpace.CheckIn(workSpace.GetPendingChanges(), checkinComment, null, WIChecInInfo, null);
                if (changesetNo > 0)
                {
                    Console.Write("Checkin succeeded, changeset is: " + changesetNo + "\n");
                    //validate work item
                    ArrayList invalidFields = workItem.Validate();
                    if (invalidFields.Count > 0)
                    {
                        foreach (Field field in invalidFields)
                        {
                            string fieldStatus = field.Status.ToString();

                            Console.WriteLine("check-in succeeded, but associating work item failed \n" + fieldStatus);
                        }
                    }
                    else
                    {
                        Console.WriteLine("no validate error \n");
                    }
                }
                else
                {
                    Console.Write("no pending changes, please press Enter to exit \n");
                }
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                var dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "logs"));
                if (!dir.Exists)
                {
                    dir.Create();
                }
                var logFile = Path.Combine(dir.FullName, "log-" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".txt");
                using (var stream = File.CreateText(logFile))
                {
                    stream.Write("Time: {0}; Exception: {1}", DateTime.Now, ex.Message + "\n" + ex.StackTrace);
                }
                Console.Write("Exception: " + ex.Message + "\n" + ex.StackTrace);
                Console.ReadLine();
            }
        }

    }
}