using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace O365_Dashboard
{
    public class OutlookHelper
    {
        private ExchangeService service;

        public OutlookHelper()
        {
            // Default constructor.
        }

        public OutlookHelper(bool needServiceBinding)
        {

            // This constructor used to get binding for exchange service.
            try
            {
                service = GetBinding();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Iterate through folder to get read/unread flags.
        /// </summary>
        /// <returns>Returns integer array with two elements : read and unread count.</returns>
        public int[] GetMailCountDetails()
        {
            
            try
            {
                int[] mailCounts = new int[2];
                int unReadCount = 0;
                int readCount = 0;

                // Declare folder and item view.
                FolderView viewFolders = new FolderView(int.MaxValue)
                {
                    Traversal = FolderTraversal.Deep,
                    PropertySet =
                        new PropertySet(BasePropertySet.IdOnly)
                };

                ItemView viewEmails = new ItemView(int.MaxValue) { PropertySet = new PropertySet(BasePropertySet.IdOnly) };

                //Create read and unread email items filters
                SearchFilter unreadFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false);
                SearchFilter readFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, true);

                // Create folder filter.
                SearchFilter folderFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "AllItems");

                //Find the AllItems folder
                FindFoldersResults inboxFolders = service.FindFolders(WellKnownFolderName.MsgFolderRoot, folderFilter, viewFolders);

                // If user does not have an AllItems folder then search the Inbox and Inbox sub-folders.
                if (inboxFolders.Count() == 0)
                {
                    // Search all items in Inbox for read and unread email.
                    FindItemsResults<Item> findUnreadResults = service.FindItems(WellKnownFolderName.Inbox, unreadFilter, viewEmails);
                    FindItemsResults<Item> findReadResults = service.FindItems(WellKnownFolderName.Inbox, readFilter, viewEmails);

                    unReadCount += findUnreadResults.Count();
                    readCount += findReadResults.Count();

                    //Get all Inbox sub-folders
                    inboxFolders = service.FindFolders(WellKnownFolderName.Inbox, viewFolders);

                    //Look for read and unread email in each Inbox sub folder
                    foreach (Folder folder in inboxFolders.Folders)
                    {
                        findUnreadResults = service.FindItems(folder.Id, unreadFilter, viewEmails);
                        findReadResults = service.FindItems(folder.Id, readFilter, viewEmails);
                        unReadCount += findUnreadResults.Count();
                        readCount += findReadResults.Count();
                    }
                }

                // AllItems folder is avilable.
                else 
                {

                    foreach (Folder folder in inboxFolders.Folders)
                    {
                        FindItemsResults<Item> findUnreadResults = service.FindItems(folder.Id, unreadFilter, viewEmails);
                        FindItemsResults<Item> findReadResults = service.FindItems(folder.Id, readFilter, viewEmails);
                        unReadCount += findUnreadResults.Count();
                        readCount += findReadResults.Count();
                    }
                }

                // Fill count to the integer array.
                mailCounts[0] = readCount;
                mailCounts[1] = unReadCount;
                return mailCounts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Call windows powershell sdk to get mailbox usage details. 
        /// </summary>
        /// <param name="credentials">PSCredential object</param>
        /// <param name="liveIDConnectionUri">Live ID connection uri</param>
        /// <param name="schemaUri">Schema uri</param>
        /// <returns>Returns string array with details of ProhibitSendQuota and TotalItemSize</returns>
        public string[] GetMailBoxSizeDetails(PSCredential credentials, string liveIDConnectionUri, string schemaUri)
        {
            Runspace runspace = null;
            string[] mailBoxDetail = new string[2];
            try
            {

                // Create basic connection using PSCredential.
                WSManConnectionInfo connectionInfo = new WSManConnectionInfo(new Uri(liveIDConnectionUri), schemaUri, credentials);
                connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;
                runspace = RunspaceFactory.CreateRunspace(connectionInfo);

                // Open runspace once user authorized.
                runspace.Open();

                // Get ProhibitSendQuota value.
                Collection<PSObject> prohibitSendQuota = GetProhibitSendQuota(runspace);

                foreach (PSObject resultProhibitSendQuota in prohibitSendQuota)
                {
                    foreach (PSPropertyInfo psInfo in resultProhibitSendQuota.Properties)
                    {
                        if (psInfo.Name == "ProhibitSendQuota")
                        {
                            mailBoxDetail[0] = psInfo.Value.ToString();
                            break;
                        }
                    }
                }

                // Get TotalItemSize value.
                Collection<PSObject> usedQuota = GetUsedQuota(runspace);

                foreach (PSObject resultUsedQuota in usedQuota)
                {
                    foreach (PSPropertyInfo psInfoUsedQuota in resultUsedQuota.Properties)
                    {
                        if (psInfoUsedQuota.Name == "TotalItemSize")
                        {
                            mailBoxDetail[1] = psInfoUsedQuota.Value.ToString();
                            break;
                        }
                    }
                }

                return mailBoxDetail;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (runspace != null)
                    runspace.Close();
            }
        }

        /// <summary>
        /// Run powershell command Get-MailboxStatistics using defined runspace.
        /// </summary>
        /// <param name="runspace"></param>
        /// <returns></returns>
        public Collection<PSObject> GetUsedQuota(Runspace runspace)
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddCommand("Get-MailboxStatistics");
                powershell.AddParameter("Identity", ConfigurationManager.AppSettings["UserID"].ToString());
                powershell.Runspace = runspace;
                return powershell.Invoke();
            }
        }

        /// <summary>
        /// Run powershell command Get-Mailbox using defined runspace.
        /// </summary>
        /// <param name="runspace"></param>
        /// <returns></returns>
        public Collection<PSObject> GetProhibitSendQuota(Runspace runspace)
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.AddCommand("Get-Mailbox");
                    powershell.AddParameter("Identity", ConfigurationManager.AppSettings["UserID"].ToString());
                    powershell.Runspace = runspace;
                    return powershell.Invoke();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get bindings for exchange web service.
        /// Will take more time if AutodiscoverUrl value is not defined in app.config file.
        /// </summary>
        /// <returns></returns>
        static ExchangeService GetBinding()
        {
            try
            {
                ExchangeService service = new ExchangeService();
                service.Credentials = new WebCredentials(ConfigurationManager.AppSettings["UserID"].ToString(),
                ConfigurationManager.AppSettings["Password"].ToString());
                try
                {

                    // Check if AutodiscoverUrl is given in App.config file.
                    if (ConfigurationManager.AppSettings["AutodiscoverUrl"].ToString() == "")
                    {
                        service.AutodiscoverUrl(ConfigurationManager.AppSettings["UserID"].ToString(), RedirectionUrlValidationCallback);
                    }
                    else
                    {
                        service.Url = new System.Uri(ConfigurationManager.AppSettings["AutodiscoverUrl"]);
                    }

                }
                catch (AutodiscoverRemoteException)
                {
                    throw;
                }
                return service;
            }
            catch (Exception)
            {
                throw;
            }
        }

        static bool RedirectionUrlValidationCallback(String redirectionUrl)
        {
            // Perform validation.
            // Validation is developer dependent to ensure a safe redirect.
            return true;
        }
    }
}
