/********************************** Module Header **********************************\
* Module Name:  Program.cs
* Project:      CSAutomateOutlook
* Copyright (c) Microsoft Corporation.
* 
* The CSAutomateOutlook example demonstrates the use of Visual C# code to automate 
* Microsoft Outlook to log on with your profile, enumerate contacts, send a mail, log 
* off, close the Microsoft Outlook application and then clean up unmanaged COM 
* resources. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;
#endregion


namespace CSAutomateOutlook
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AutomateOutlook();


            // Clean up the unmanaged Outlook COM resources by forcing a garbage 
            // collection as soon as the calling function is off the stack (at which 
            // point these objects are no longer rooted).

            GC.Collect();
            GC.WaitForPendingFinalizers();
            // GC needs to be called twice in order to get the Finalizers called - 
            // the first time in, it simply makes a list of what is to be finalized, 
            // the second time in, it actually is finalizing. Only then will the 
            // object do its automatic ReleaseComObject.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static void AutomateOutlook()
        {
            object missing = Type.Missing;

            Outlook.Application oOutlook = null;
            Outlook.NameSpace oNS = null;
            Outlook.Folder oCtFolder = null;
            Outlook.Items oCts = null;
            Outlook.MailItem oMail = null;

            try
            {
                // Start Microsoft Outlook and log on with your profile.

                // Create an Outlook application.
                oOutlook = new Outlook.Application();
                Console.WriteLine("Outlook.Application is started");

                Console.WriteLine("User logs on ...");

                // Get the namespace.
                oNS = oOutlook.GetNamespace("MAPI");

                // Log on by using a dialog box to choose the profile.
                oNS.Logon(missing, missing, true, true);

                // Alternative logon method that uses a specific profile.
                // If you use this logon method, change the profile name to an 
                // appropriate value. The second parameter of Logon is the password 
                // (if any) associated with the profile. This parameter exists only 
                // for backwards compatibility and for security reasons, and it is 
                // not recommended for use.
                //oNS.Logon("YourValidProfile", missing, false, true);

                Console.WriteLine("Press ENTER to continue when Outlook is ready.");
                Console.ReadLine();

                // Enumerate the contact items.

                Console.WriteLine("Enumerate the contact items");

                oCtFolder = (Outlook.Folder)oNS.GetDefaultFolder(
                    Outlook.OlDefaultFolders.olFolderContacts);
                oCts = oCtFolder.Items;

                // Enumerate the contact items. Be careful with foreach loops. 
                // See: http://tiny.cc/uXw8S.
                for (int i = 1; i <= oCts.Count; i++)
                {
                    object oItem = oCts[i];

                    if (oItem is Outlook.ContactItem)
                    {
                        Outlook.ContactItem oCt = (Outlook.ContactItem)oItem;
                        Console.WriteLine(oCt.Email1Address);
                        // Do not need to Marshal.ReleaseComObject oCt because 
                        // (Outlook.ContactItem)oItem is a simple .NET type 
                        // casting, instead of a COM QueryInterface.
                    }
                    else if (oItem is Outlook.DistListItem)
                    {
                        Outlook.DistListItem oDl = (Outlook.DistListItem)oItem;
                        Console.WriteLine(oDl.DLName);
                        // Do not need to Marshal.ReleaseComObject oDl because 
                        // (Outlook.DistListItem)oItem is a simple .NET type 
                        // casting, instead of a COM QueryInterface.
                    }

                    // Release the COM object of the Outlook item.
                    Marshal.FinalReleaseComObject(oItem);
                    oItem = null;
                }

                // Create and send a new mail item.

                Console.WriteLine("Create and send a new mail item");

                oMail = (Outlook.MailItem)oOutlook.CreateItem(
                    Outlook.OlItemType.olMailItem);

                // Set the properties of the email.
                oMail.Subject = "Feedback of All-In-One Code Framework";
                oMail.To = "codefxf@microsoft.com";
                oMail.HTMLBody = "<b>Feedback:</b><br />";

                // Displays a new Inspector object for the item and allows users to 
                // click on the Send button to send the mail manually.
                // Modal = true makes the Inspector window modal
                oMail.Display(true);
                // [-or-]
                // Automatically send the mail without a new Inspector window.
                //((Outlook._MailItem)oMail).Send();

                // User logs off and quits Outlook.

                Console.WriteLine("Log off and quit the Outlook application");
                oNS.Logoff();
                ((Outlook._Application)oOutlook).Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("AutomateOutlook throws the error: {0}", ex.Message);
            }
            finally
            {
                // Manually clean up the explicit unmanaged Outlook COM resources by  
                // calling Marshal.FinalReleaseComObject on all accessor objects. 
                // See http://support.microsoft.com/kb/317109.

                if (oMail != null)
                {
                    Marshal.FinalReleaseComObject(oMail);
                    oMail = null;
                }
                if (oCts != null)
                {
                    Marshal.FinalReleaseComObject(oCts);
                    oCts = null;
                }
                if (oCtFolder != null)
                {
                    Marshal.FinalReleaseComObject(oCtFolder);
                    oCtFolder = null;
                }
                if (oNS != null)
                {
                    Marshal.FinalReleaseComObject(oNS);
                    oNS = null;
                }
                if (oOutlook != null)
                {
                    Marshal.FinalReleaseComObject(oOutlook);
                    oOutlook = null;
                }
            }
        }
    }
}