using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Lync.Model;
using System.Security;

namespace O365_Dashboard
{
    public class LyncHelper
    {

        /// <summary>
        /// Get list of online users using powershell and then get status using Lync Managed API.
        /// </summary>
        /// <param name="contactMgr"></param>
        /// <returns></returns>
        public List<O365User> GetListOfOnlineUsers(ContactManager contactMgr)
        {
            try
            {
                Collection<PSObject> results = ExcutePowershellCommands();

                if (results != null && results.Count > 0)
                {
                    List<O365User> userList = new List<O365User>();
                    foreach (PSObject itemUser in results)
                    {
                        O365User userEntry = new O365User();
                        if (itemUser.Properties["DisplayName"] != null && itemUser.Properties["DisplayName"].Value != null)
                            userEntry.DisplayName = itemUser.Properties["DisplayName"].Value.ToString();
                        if (itemUser.Properties["UserPrincipalName"] != null && itemUser.Properties["UserPrincipalName"].Value != null)
                        {
                            userEntry.UserPrincipalName = itemUser.Properties["UserPrincipalName"].Value.ToString();

                            //User must be signed in to Lync to get the presence status of a user
                            if (LyncClient.GetClient().State == ClientState.SignedIn)
                            {
                                Contact selectedContact = contactMgr.GetContactByUri("sip:" + userEntry.UserPrincipalName);
                                userEntry.UserStatus = selectedContact.GetContactInformation(ContactInformationType.Activity).ToString();
                            }
                            else
                            {
                                userEntry.UserStatus = "Unavailable";
                            }
                        }
                        if (userEntry.UserStatus != "Offline" && userEntry.UserStatus != "Presence unknown")
                        {
                            userList.Add(userEntry);
                        }
                    }
                    return userList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Runs Get-MsolUser command and returns resulted data.
        /// </summary>
        /// <returns></returns>
        private Collection<PSObject> ExcutePowershellCommands()
        {
            try
            {
                Collection<PSObject> userList = null;
                // Create Initial Session State for runspace.
                InitialSessionState initialSession = InitialSessionState.CreateDefault();
                initialSession.ImportPSModule(new[] { "MSOnline" });
                // Create credential object.
                SecureString securePass = new SecureString();

                foreach (char secureChar in ConfigurationManager.AppSettings["Password"].ToString())
                {
                    securePass.AppendChar(secureChar);
                }
                PSCredential credential = new PSCredential(ConfigurationManager.AppSettings["UserID"].ToString(), securePass);
                // Create command to connect office 365.
                Command connectCommand = new Command("Connect-MsolService");
                connectCommand.Parameters.Add((new CommandParameter("Credential", credential)));
                // Create command to get office 365 users.
                Command getUserCommand = new Command("Get-MsolUser");
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    //Iterate through each command and runs it.
                    foreach (var com in new Command[] { connectCommand, getUserCommand })
                    {
                        var pipe = psRunSpace.CreatePipeline();
                        pipe.Commands.Add(com);
                        // Run command and generate results and errors (if any).
                        Collection<PSObject> results = pipe.Invoke();
                        var error = pipe.Error.ReadToEnd();
                        if (error.Count > 0 && com == connectCommand)
                        {
                            return null;
                        }
                        if (error.Count > 0 && com == getUserCommand)
                        {
                            return null;
                        }
                        else
                        {
                            userList = results;
                        }
                    }
                    // Close the runspace.
                    psRunSpace.Close();
                }
                return userList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
