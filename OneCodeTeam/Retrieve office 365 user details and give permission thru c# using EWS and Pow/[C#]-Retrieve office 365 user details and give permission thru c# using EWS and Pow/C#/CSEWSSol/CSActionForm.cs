/****************************** Module Header ******************************\
Module Name:  CSActionForm.cs
Project:      CSEWSSol
Copyright (c) Microsoft Corporation.

The is the main module. It facilitates to perform various activities using EWSAPI 
and Powershell on the mailboxes on Office 365 cloud.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Microsoft.Exchange.WebServices.Data;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Remoting;
using System.Security;


namespace O365Delegates
{
    public partial class CSActionForm : Form
    {
        PowerShell powershell;
        PowerShell powershell1;
        PowerShell OnPremPS;
        Runspace Onrunspace;
        Runspace runspace;
        string EWSURL;

        public CSActionForm()
        {
            InitializeComponent();

            // ES related
            dlGenButton.Enabled = false;
            addDelButton.Enabled = false;
            lstDelButton.Enabled = false;
            remDelButton.Enabled = false;
            updDelButton.Enabled = false;

            // PS Related
            getSendAsButton.Enabled = false;
            getMBXPermButton.Enabled = false;
            giveSendAsButton.Enabled = false;
            removeSendAsButton.Enabled = false;
            giveMBXPermButton.Enabled = false;
            removeMBXPermButton.Enabled = false;
            getMUSRButton.Enabled = false;
            getMBXButton.Enabled = false;
            getCASButton.Enabled = false;
            getMailUSRButton.Enabled = false;
            getLogStatButton.Enabled = false;
            getMBXStatButton.Enabled = false;
            getSyncButton.Enabled = false;

            AboutBox abt = new AboutBox();
            abt.ShowDialog();
        }

        private static bool routeURL(string newURL)
        {

            bool result = false;
            try
            {


                Uri newAddress = new Uri(newURL);

                if (newAddress.Scheme == "https")
                {
                    result = true;
                }
                return result;
            }
            catch (Exception)
            {
                MessageBox.Show("Unidentified Issue with AutoDiscover. Try Again Later.", "O365 Delegates: Error !!!");
                return result;
            }
        }

        public void wsbutton_Click(object sender, EventArgs e)
        {
            authWSButton.Text = "Processing...";
            textBox1.Text = " ";
            authWSButton.Enabled = false;

            if ((textBox10.Text == "") || (textBox13.Text == "") || (maskedTextBox1.Text == ""))
            {
                MessageBox.Show("Email ID/Password/Domain Should Not Be Blank.", "O365 Delegates: Warning !!!");
                authWSButton.Enabled = true;
                authWSButton.Text = "AutoDiscover";
            }
            else
            {
                try
                {
                    getEWS();
                    authWSButton.Enabled = true;
                    authWSButton.Text = "AutoDiscover";
                }
                catch (FormatException)
                {
                    MessageBox.Show("Check  The Email ID Format.", "O365 Delegates: Error !!!");
                    textBox10.Select();
                    authWSButton.Enabled = true;
                    authWSButton.Text = "AutoDiscover";
                    dlGenButton.Enabled = false;
                    addDelButton.Enabled = false;
                    lstDelButton.Enabled = false;
                    remDelButton.Enabled = false;
                    updDelButton.Enabled = false;
                }
            }
        }

        public void dlbutton_Click(object sender, EventArgs e)
        {
            dlGenButton.Text = "Processing...";
            dlGenButton.Enabled = false;
            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;
            string Success = "";

            if (textBox2.Text == "" || textBox3.Text == "")
            {

                MessageBox.Show("DL Alias/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox2.Select();
                dlGenButton.Enabled = true;
                dlGenButton.Text = "Generate";
            }
            else
            {
                try
                {
                    ExpandGroupResults DLMembers = ewsService.ExpandGroup(textBox2.Text);
                    TextWriter writeFile = new StreamWriter(textBox3.Text);
                    writeFile.WriteLine("Distribution List Name :{0}", textBox2.Text);
                    writeFile.WriteLine("File Name:{0}", textBox3.Text);
                    writeFile.WriteLine("Date:{0}", DateTime.Now.ToString());
                    writeFile.WriteLine("=======================================================================");
                    int DLCount = 0;
                    foreach (EmailAddress address in DLMembers.Members)
                    {
                        writeFile.WriteLine(address);
                        DLCount = DLCount + 1;
                    }
                    writeFile.WriteLine(" ");
                    writeFile.WriteLine("===== Number Of Members in DL: " + Convert.ToString(DLCount) + ". ================");
                    writeFile.Close();
                    Success = "True";
                    DLCount = 0;
                    dlGenButton.Enabled = true;
                    dlGenButton.Text = "Generate";
                    textBox2.Select();
                }
                catch (FileNotFoundException fmterr)
                {
                    Success = "False";
                    MessageBox.Show(fmterr.Message + " (Check The File Name/Path/Is it Root?)", "O365 Delegates: Error !!!");
                    dlGenButton.Enabled = true;
                    dlGenButton.Text = "Generate";
                    textBox2.Select();
                }
                catch (Exception err)
                {
                    Success = "False";
                    MessageBox.Show(err.Message + " (Check DL Name/File Name/Path/Is it Root Folder?/Net Connection)", "O365 Delegates: Error !!!");
                    dlGenButton.Enabled = true;
                    dlGenButton.Text = "Generate";
                    textBox2.Select();
                }

                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox3.Text);
                    }
                    catch (Exception fileErr)
                    {
                        MessageBox.Show("The File Was Generated, But It Could Not Open due to error: " + fileErr.Message + " You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                        textBox2.Select();
                        dlGenButton.Enabled = true;
                        dlGenButton.Text = "Generate";
                    }
                }
            }
        }


        private void addbutton_Click(object sender, EventArgs e)
        {
            addDelButton.Text = "Processing...";
            addDelButton.Enabled = false;

            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            ewsService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, textBox4.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;

            string Test1 = "Good";
            string NameCorrect = "False";
            string UsrPresent = "False";


            if ((textBox4.Text == "" || textBox5.Text == "") && (radioButton3.Checked == false || radioButton4.Checked == false))
            {
                MessageBox.Show("Select Folder Level Permission Radio Button. Also Manager ID/Delegate ID Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox4.Select();
                addDelButton.Enabled = true;
                addDelButton.Text = "Give Permission";
                Test1 = "Bad";
            }
            else
            {
                try
                {
                    NameResolutionCollection GALName = ewsService.ResolveName(textBox5.Text, ResolveNameSearchLocation.DirectoryOnly, true);
                    foreach (NameResolution NameList in GALName)
                    {
                        textBox5.Text = NameList.Mailbox.Address;
                        NameCorrect = "True";
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Check permission on impersonation or Check The ID", "O365 Delegates: Warning !!!");
                    Test1 = "Bad";
                    textBox4.Select();
                    addDelButton.Enabled = true;
                    addDelButton.Text = "Give Permission";
                    NameCorrect = "Exception";
                }

                if (NameCorrect == "False")
                {
                    MessageBox.Show("Name Is Not Found. Check The ID :" + textBox5.Text, "O365 Delegates: Warning !!!");
                    textBox4.Select();
                    addDelButton.Enabled = true;
                    addDelButton.Text = "Give Permission";
                }
                else if (NameCorrect == "True")
                {

                    Mailbox actMBX = new Mailbox(textBox4.Text);
                    DelegateInformation delegateUSR = ewsService.GetDelegates(actMBX, true);
                    foreach (DelegateUserResponse delResult in delegateUSR.DelegateUserResponses)
                    {
                        DelegateUser delegateUSER = delResult.DelegateUser;
                        if (delegateUSER == null)
                        {
                            MessageBox.Show("Previous action on the mailbox is still in process. Please try after sometime.", "O365 Delegates: Warning !!!");
                            UsrPresent = "True";
                            NameCorrect = "False";
                            textBox4.Select();
                            addDelButton.Enabled = true;
                            addDelButton.Text = "Give Permission";
                        }

                        else if ((delegateUSER.UserId.PrimarySmtpAddress != null) && (textBox5.Text == delegateUSER.UserId.PrimarySmtpAddress))
                        {
                            MessageBox.Show("Delegate already present with permission: " + delegateUSER.Permissions.CalendarFolderPermissionLevel, "O365 Delegates: Warning !!!");
                            UsrPresent = "True";
                            NameCorrect = "False";
                            textBox4.Select();
                            addDelButton.Enabled = true;
                            addDelButton.Text = "Give Permission";
                        }
                    }

                }

            }
            if ((UsrPresent == "False") && (radioButton3.Checked == true) && (Test1 == "Good"))
            {
                try
                {
                    List<DelegateUser> addDel = new System.Collections.Generic.List<DelegateUser>();
                    DelegateUser newDel = new DelegateUser(textBox5.Text);
                    newDel.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    addDel.Add(newDel);
                    Mailbox actMBX1 = new Mailbox(textBox4.Text);
                    Collection<DelegateUserResponse> delReturn = ewsService.AddDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, addDel);
                    MessageBox.Show("Editor Permission on Calendar to " + textBox5.Text + " On The Mailbox " + textBox4.Text + " Added.", "O365 Delegates: Success !!!");
                    textBox4.Select();
                    addDelButton.Enabled = true;
                    addDelButton.Text = "Give Permission";
                    NameCorrect = "False";

                }
                catch (Exception)
                {
                    MessageBox.Show("Unable To Add Permission", "O365 Delegates: Error !!!");
                    textBox4.Select();
                    addDelButton.Enabled = true;
                    addDelButton.Text = "Give Permission";
                    NameCorrect = "False";
                }

            }

            else if ((UsrPresent == "False") && (radioButton4.Checked == true) && (Test1 == "Good"))
            {
                try
                {
                    List<DelegateUser> newAllDelegates = new System.Collections.Generic.List<DelegateUser>();
                    DelegateUser AllDelegate = new DelegateUser(textBox5.Text);
                    AllDelegate.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    AllDelegate.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    AllDelegate.Permissions.TasksFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    AllDelegate.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    AllDelegate.Permissions.NotesFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    AllDelegate.Permissions.JournalFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                    newAllDelegates.Add(AllDelegate);
                    Mailbox actMBX2a = new Mailbox(textBox4.Text);
                    Collection<DelegateUserResponse> delReturn = ewsService.AddDelegates(actMBX2a, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, newAllDelegates);
                    MessageBox.Show("Editor Permission on All to " + textBox5.Text + " On The Mailbox " + textBox4.Text + " Added.", "O365 Delegates: Success !!!");
                    textBox4.Select();
                    addDelButton.Enabled = true;
                    addDelButton.Text = "Give Permission";
                    NameCorrect = "False";

                }
                catch (Exception)
                {
                    MessageBox.Show("Unable To Add Permission", "O365 Delegates: Error !!!");
                    textBox4.Select();
                    addDelButton.Enabled = true;
                    addDelButton.Text = "Give Permission";
                    NameCorrect = "False";
                }
            }
        }


        private void lstbutton_Click_1(object sender, EventArgs e)
        {
            lstDelButton.Text = "Processing... ";
            lstDelButton.Enabled = false;
            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;
            string Success = "";
            TextWriter writeFile;

            if (textBox6.Text == "" || textBox7.Text == "")
            {
                MessageBox.Show("Manager ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox6.Select();
                lstDelButton.Enabled = true;
                lstDelButton.Text = "Generate";
            }
            else
            {
                try
                {
                    ewsService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, textBox6.Text);
                    Mailbox actMBX = new Mailbox(textBox6.Text);
                    writeFile = new StreamWriter(textBox7.Text);
                    DelegateInformation delegateUSR = ewsService.GetDelegates(actMBX, true);
                    writeFile.WriteLine("Manager ID :{0}", textBox6.Text);
                    writeFile.WriteLine("File Name:{0}", textBox7.Text);
                    writeFile.WriteLine("Date:{0}", DateTime.Now.ToString());
                    writeFile.WriteLine("=======================================================================");
                    foreach (DelegateUserResponse delResult in delegateUSR.DelegateUserResponses)
                    {
                        DelegateUser delegateUSER = delResult.DelegateUser;
                        if (delegateUSER == null)
                        {
                            Success = "False";
                            MessageBox.Show("Previous action on the mailbox is still in process. Please try after sometime", "O365 Delegates: Warning !!!");
                        }
                        else
                        {
                            writeFile.WriteLine("Name: {0}", delegateUSER.UserId.DisplayName);
                            writeFile.WriteLine("Email ID: {0}", delegateUSER.UserId.PrimarySmtpAddress);
                            writeFile.WriteLine("Calendar Permission: {0}", delegateUSER.Permissions.CalendarFolderPermissionLevel);
                            writeFile.WriteLine("Task Permission: {0}", delegateUSER.Permissions.TasksFolderPermissionLevel);
                            writeFile.WriteLine("Inbox Permission: {0}", delegateUSER.Permissions.InboxFolderPermissionLevel);
                            writeFile.WriteLine("Contacts Permission: {0}", delegateUSER.Permissions.ContactsFolderPermissionLevel);
                            writeFile.WriteLine("Notes Permission: {0}", delegateUSER.Permissions.NotesFolderPermissionLevel);
                            writeFile.WriteLine("Journal Permission: {0}", delegateUSER.Permissions.JournalFolderPermissionLevel);
                            writeFile.WriteLine(" ");
                            Success = "True";
                        }
                    }

                    writeFile.Close();
                    textBox6.Select();
                    lstDelButton.Enabled = true;
                    lstDelButton.Text = "Generate";

                }
                catch (FileNotFoundException badfileerr)
                {
                    Success = "False";
                    MessageBox.Show(badfileerr.Message + " (Check The File Name/Path/Is it Root?)", "O365 Delegates: Error !!!");
                    textBox6.Select();
                    lstDelButton.Enabled = true;
                    lstDelButton.Text = "Generate";
                }
                catch (FormatException fmterr)
                {
                    Success = "False";
                    MessageBox.Show(fmterr.Message + " (Check The File Name/Path/Is it Root?)", "O365 Delegates: Error !!!");
                    textBox6.Select();
                    lstDelButton.Enabled = true;
                    lstDelButton.Text = "Generate";
                }
                catch (Exception err)
                {
                    Success = "False";
                    MessageBox.Show(err.Message + " (Check ID/ File Name / Path/Is it Root Folder?/Net Connection)", "O365 Delegates: Error !!!");
                    textBox6.Select();
                    lstDelButton.Enabled = true;
                    lstDelButton.Text = "Generate";
                }

            }

            if (Success == "True")
            {
                try
                {

                    Process.Start(textBox7.Text);
                }
                catch (Exception fileErr)
                {
                    MessageBox.Show("The File Was Generated, But It Could Not Open due to error: " + fileErr.Message + " You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                    textBox6.Select();
                    lstDelButton.Enabled = true;
                    lstDelButton.Text = "Generate";
                }
            }
        }


        private void rembutton_Click(object sender, EventArgs e)
        {
            remDelButton.Text = "Processing...";
            remDelButton.Enabled = false;
            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            ewsService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, textBox8.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;

            string Test1 = "Good";
            string NameCorrect = "False";
            string UsrPresent = "False";
            string empty = "No";
            if (textBox8.Text == "" || textBox9.Text == "")
            {
                MessageBox.Show("Manager ID/Delegate ID Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox8.Select();
                remDelButton.Enabled = true;
                remDelButton.Text = "Remove";
                Test1 = "Bad";
            }
            else if (Test1 == "Good")
            {
                try
                {
                    NameResolutionCollection GALName = ewsService.ResolveName(textBox9.Text, ResolveNameSearchLocation.DirectoryOnly, true);
                    foreach (NameResolution NameList in GALName)
                    {
                        textBox9.Text = NameList.Mailbox.Address;
                        NameCorrect = "True";
                    }
                }
                catch (Exception err1)
                {

                    MessageBox.Show(err1.Message + " (Check permission on impersonation or Check The ID:" + textBox8.Text + " )", "O365 Delegates: Warning !!!");
                    textBox8.Select();
                    remDelButton.Enabled = true;
                    remDelButton.Text = "Remove";
                    NameCorrect = "Exception";
                }

                if (NameCorrect == "False")
                {
                    MessageBox.Show("Name Is Not Found. Check The ID :" + textBox9.Text);
                    textBox8.Select();
                    remDelButton.Enabled = true;
                    remDelButton.Text = "Remove";
                }
                else if (NameCorrect == "True")
                {
                    Mailbox actMBX = new Mailbox(textBox8.Text);
                    DelegateInformation delegateUSR = ewsService.GetDelegates(actMBX, true);

                    foreach (DelegateUserResponse delResult in delegateUSR.DelegateUserResponses)
                    {
                        DelegateUser delegateUSER = delResult.DelegateUser;
                        if (delegateUSER == null)
                        {
                            MessageBox.Show("Previous action on the mailbox is in progress. Please try after sometime.", "O365 Delegates: Error !!!");
                            UsrPresent = "False";
                            empty = "Yes";
                            NameCorrect = "False";
                            textBox8.Select();
                            remDelButton.Enabled = true;
                            remDelButton.Text = "Remove";
                        }
                        else if ((delegateUSER.UserId.PrimarySmtpAddress != null) && (textBox9.Text == delegateUSER.UserId.PrimarySmtpAddress))
                        {
                            UsrPresent = "True";
                            NameCorrect = "False";
                            textBox8.Select();
                            remDelButton.Enabled = true;
                            remDelButton.Text = "Remove";
                        }

                    }

                    if ((UsrPresent == "True") && (empty == "No"))
                    {
                        try
                        {
                            List<UserId> removeDel = new System.Collections.Generic.List<UserId>();
                            Mailbox actMBX1 = new Mailbox(textBox8.Text);
                            DelegateInformation delDetail = ewsService.GetDelegates(actMBX1, true);
                            foreach (DelegateUserResponse delResult in delDetail.DelegateUserResponses)
                            {
                                DelegateUser delegateUSER = delResult.DelegateUser;
                                if ((delegateUSER.UserId.PrimarySmtpAddress != null) && (textBox9.Text == delegateUSER.UserId.PrimarySmtpAddress))
                                {
                                    removeDel.Add(delResult.DelegateUser.UserId);
                                    Collection<DelegateUserResponse> delReturn = ewsService.RemoveDelegates(actMBX1, removeDel);
                                    MessageBox.Show("Delegate: " + textBox9.Text + " On The Mailbox " + textBox8.Text + " Removed.", "O365 Delegates: Success !!!");
                                    UsrPresent = "True";
                                    NameCorrect = "False";
                                    textBox8.Select();
                                    remDelButton.Enabled = true;
                                    remDelButton.Text = "Remove";

                                }

                            }
                            textBox8.Select();
                            remDelButton.Enabled = true;
                            remDelButton.Text = "Remove";
                            NameCorrect = "False";
                        }
                        catch (Exception err2)
                        {
                            MessageBox.Show(err2.Message + " (Unable To Remove Delegate)", "O365 Delegates: Error !!!");
                            textBox8.Select();
                            remDelButton.Enabled = true;
                            remDelButton.Text = "Remove";
                            NameCorrect = "False";

                        }
                    }
                    else if ((UsrPresent == "False") && (empty == "No"))
                    {
                        MessageBox.Show("Delegate: " + textBox9.Text + " Is Not There for " + textBox8.Text, "O365 Delegates: Warning !!!");
                        textBox8.Select();
                        remDelButton.Enabled = true;
                        remDelButton.Text = "Remove";
                        NameCorrect = "False";
                    }
                }
            }
        }

        private void updbutton_Click(object sender, EventArgs e)
        {
            updDelButton.Text = "Processing...";
            updDelButton.Enabled = false;
            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            ewsService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, textBox11.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;
            string Test1 = "Good";
            string NameCorrect = "False";
            string UsrPresent = "False";

            if ((textBox11.Text == "" || textBox12.Text == "") || (radioButton11.Checked == false && radioButton12.Checked == false) || (radioButton1.Checked == false && radioButton2.Checked == false && radioButton5.Checked == false && radioButton6.Checked == false))
            {
                MessageBox.Show("Manager ID/Delegate ID Should Not Be Blank.Also Select appropriate Radio Buttons", "O365 Delegates: Warning !!!");
                textBox11.Select();
                updDelButton.Enabled = true;
                updDelButton.Text = "Update";
                Test1 = "Bad";
            }
            else if (Test1 == "Good")
            {
                try
                {
                    NameResolutionCollection GALName = ewsService.ResolveName(textBox12.Text, ResolveNameSearchLocation.DirectoryOnly, true);
                    foreach (NameResolution NameList in GALName)
                    {
                        textBox12.Text = NameList.Mailbox.Address;
                        NameCorrect = "True";
                    }
                }
                catch (Exception err3)
                {
                    MessageBox.Show(err3.Message + " (Check permission on impersonation or Check The ID.)", "O365 Delegates: Warning !!!");
                    textBox11.Select();
                    updDelButton.Enabled = true;
                    updDelButton.Text = "Update";
                    NameCorrect = "Exception";
                }
                if (NameCorrect == "False")
                {
                    MessageBox.Show("Name Is Not Found. Check The ID :" + textBox12.Text);
                    textBox11.Select();
                    updDelButton.Enabled = true;
                    updDelButton.Text = "Update";
                }
                else if (NameCorrect == "True")
                {
                    Mailbox actMBX = new Mailbox(textBox11.Text);
                    DelegateInformation delegateUSR = ewsService.GetDelegates(actMBX, true);
                    foreach (DelegateUserResponse delResult in delegateUSR.DelegateUserResponses)
                    {
                        DelegateUser delegateUSER = delResult.DelegateUser;

                        if (delegateUSER == null)
                        {
                            MessageBox.Show("Previous action on the mailbox is still in process. Please try after sometime.", "O365 Delegates: Warning !!!");
                            textBox11.Select();
                            updDelButton.Enabled = true;
                            updDelButton.Text = "Update";
                            NameCorrect = "Exception";
                        }

                        else if ((delegateUSER.UserId.PrimarySmtpAddress != null) && (textBox12.Text == delegateUSER.UserId.PrimarySmtpAddress))
                        {
                            UsrPresent = "True";
                            NameCorrect = "False";

                        }
                    }
                    if (UsrPresent == "True")
                    {
                        try
                        {
                            if (radioButton11.Checked)
                            {
                                if (radioButton1.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with None Permission on Calendar Folder", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                                else if (radioButton2.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with Author Permission on Calendar Folder", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                                else if (radioButton5.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with Reviewer Permission on Calendar Folder", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                                else if (radioButton6.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with Editor Permission on Calendar Folder", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                            }

                            if (radioButton12.Checked)
                            {
                                if (radioButton1.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    perfCorrect.Permissions.TasksFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    perfCorrect.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    perfCorrect.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    perfCorrect.Permissions.NotesFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    perfCorrect.Permissions.JournalFolderPermissionLevel = DelegateFolderPermissionLevel.None;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with None Permission on All Folders", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                                else if (radioButton2.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    perfCorrect.Permissions.TasksFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    perfCorrect.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    perfCorrect.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    perfCorrect.Permissions.NotesFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    perfCorrect.Permissions.JournalFolderPermissionLevel = DelegateFolderPermissionLevel.Author;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with Author Permission on All Folders", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                                else if (radioButton5.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    perfCorrect.Permissions.TasksFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    perfCorrect.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    perfCorrect.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    perfCorrect.Permissions.NotesFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    perfCorrect.Permissions.JournalFolderPermissionLevel = DelegateFolderPermissionLevel.Reviewer;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with Reviewer Permission on All Folders", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";

                                }
                                else if (radioButton6.Checked)
                                {
                                    List<DelegateUser> delCorrect = new System.Collections.Generic.List<DelegateUser>();
                                    DelegateUser perfCorrect = new DelegateUser(textBox12.Text);
                                    Mailbox actMBX1 = new Mailbox(textBox11.Text);
                                    perfCorrect.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    perfCorrect.Permissions.TasksFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    perfCorrect.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    perfCorrect.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    perfCorrect.Permissions.NotesFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    perfCorrect.Permissions.JournalFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                    delCorrect.Add(perfCorrect);
                                    ewsService.UpdateDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, delCorrect);
                                    MessageBox.Show("Delegate: " + textBox12.Text + " On The Mailbox " + textBox11.Text + " Updated with Editor Permission on All Folders", "O365 Delegates: Success !!!");
                                    Test1 = "Good";
                                    NameCorrect = "False";
                                    UsrPresent = "False";
                                    textBox11.Select();
                                    updDelButton.Enabled = true;
                                    updDelButton.Text = "Update";
                                }
                            }
                        }

                        catch (Exception err4)
                        {
                            MessageBox.Show(err4.Message + " (Unable To Update Delegate)", "O365 Delegates: Error !!!");
                            Test1 = "Good";
                            UsrPresent = "False";
                            textBox11.Select();
                            updDelButton.Enabled = true;
                            NameCorrect = "False";
                        }
                    }
                    else
                    {
                        MessageBox.Show("Delegate: " + textBox12.Text + " Is Not There for " + textBox11.Text, "O365 Delegates: Warning !!!");
                        textBox11.Select();
                        updDelButton.Enabled = true;
                        NameCorrect = "False";
                        Test1 = "Good";
                        UsrPresent = "False";
                    }

                }
            }
        }



        private void blgbutton_Click(object sender, EventArgs e)
        {
            Process.Start("http://blogs.technet.com/b/sukum/archive/2013/01/23/coming-soon-office-365-delegates-tool.aspx");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void gsasbutton_Click(object sender, EventArgs e)
        {

            giveSendAsButton.Text = "Processing...";
            giveSendAsButton.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string outPath = Convert.ToString(curDir + @"\Result.txt");
            string Success = "";
            if (textBox14.Text == "" || textBox15.Text == "")
            {
                MessageBox.Show("Identity/Trustee Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox14.Select();
                giveSendAsButton.Enabled = true;
                giveSendAsButton.Text = "Grant";
            }
            else
            {
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox14.Text;
                string tst = textBox15.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Add-RecipientPermission -identity $usr -AccessRights SendAs -Trustee $tst  -Confirm:$False ");
                        writeFile.WriteLine(@"Get-RecipientPermission -identity $usr | FL | out-file $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }
                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Add-RecipientPermission -identity $usr -AccessRights SendAs -Trustee $tst  -Confirm:$False ");
                    writeFile.WriteLine(@"Get-RecipientPermission -identity $usr | FL | out-file $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();

                }
                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                    textBox14.Select();
                    giveSendAsButton.Enabled = true;
                    giveSendAsButton.Text = "Grant";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "True";
                    textBox14.Select();
                    giveSendAsButton.Enabled = true;
                    giveSendAsButton.Text = "Grant";
                }
                if (Success == "True")
                {
                    try
                    {
                        Process.Start(outPath);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The File Was Generated, But It Could Not Open. You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                        textBox14.Select();
                        giveSendAsButton.Enabled = true;
                        giveSendAsButton.Text = "Grant";
                    }
                }
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void rsasbutton_Click(object sender, EventArgs e)
        {

            removeSendAsButton.Text = "Processing...";
            removeSendAsButton.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string outPath = Convert.ToString(curDir + @"\Result.txt");

            string Success = "";
            if (textBox14.Text == "" || textBox15.Text == "")
            {

                MessageBox.Show("Identity/Trustee Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox14.Select();
                removeSendAsButton.Enabled = true;
                removeSendAsButton.Text = "Remove";
            }
            else
            {

                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox14.Text;
                string tst = textBox15.Text;
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {


                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Remove-RecipientPermission -identity $usr -AccessRights SendAs -Trustee $tst  -Confirm:$False ");
                        writeFile.WriteLine(@"Get-RecipientPermission -identity $usr | FL | out-file $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {

                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Remove-RecipientPermission -identity $usr -AccessRights SendAs -Trustee $tst  -Confirm:$False ");
                    writeFile.WriteLine(@"Get-RecipientPermission -identity $usr | FL | out-file $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();

                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }




                if (Success == "True")
                {
                    try
                    {

                        Process.Start(outPath);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The File Was Generated, But It Could Not Open. You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                        textBox14.Select();
                        removeSendAsButton.Enabled = true;
                        removeSendAsButton.Text = "Remove";
                    }
                }

                textBox14.Select();
                removeSendAsButton.Enabled = true;
                removeSendAsButton.Text = "Remove";
            }
        }





        private void psbutton_Click(object sender, EventArgs e)
        {
            authPSButton.Text = "Processing...";
            textBox1.Text = " ";
            authPSButton.Enabled = false;
            //authWSPSButton.Enabled = false;


            if ((textBox10.Text == "") || (textBox13.Text == "") || (maskedTextBox1.Text == ""))
            {
                MessageBox.Show("Email ID/Password/Domain Should Not Be Blank.", "O365 Delegates: Warning !!!");
                authPSButton.Enabled = true;
                //authWSPSButton.Enabled = true;
                authPSButton.Text = "PowerShell (PS)";
            }

            else
            {
                try
                {
                    createPSSession();
                    textBox1.Text = "Tabs with title PS can be used.";
                    getSendAsButton.Enabled = true;
                    getMBXPermButton.Enabled = true;
                    giveSendAsButton.Enabled = true;
                    removeSendAsButton.Enabled = true;
                    giveMBXPermButton.Enabled = true;
                    removeMBXPermButton.Enabled = true;
                    getMUSRButton.Enabled = true;
                    getMBXButton.Enabled = true;
                    getCASButton.Enabled = true;
                    getMailUSRButton.Enabled = true;
                    getLogStatButton.Enabled = true;
                    getMBXStatButton.Enabled = true;
                    getSyncButton.Enabled = true;
                    authPSButton.Enabled = false;
                    authPSButton.Text = "PowerShell (PS)";
                    //authWSPSButton.Enabled = false;
                    closePSButton.Enabled = true;
                    MessageBox.Show("Powershell Session Created.Tabs with title PS can be used.", "O365 Delegates !!!");

                }
                catch (Exception)
                {
                    MessageBox.Show("Issue in creating Remote Powershell session", "O365 Delegates: Error !!!");
                    textBox10.Select();
                    authPSButton.Enabled = true;
                    authPSButton.Text = "PowerShell (PS)";
                    getSendAsButton.Enabled = false;
                    getMBXPermButton.Enabled = false;
                    giveSendAsButton.Enabled = false;
                    removeSendAsButton.Enabled = false;
                    giveMBXPermButton.Enabled = false;
                    removeMBXPermButton.Enabled = false;
                    getMUSRButton.Enabled = false;
                    getMBXButton.Enabled = false;
                    getCASButton.Enabled = false;
                    getMailUSRButton.Enabled = false;
                    getLogStatButton.Enabled = false;
                    getMBXStatButton.Enabled = false;
                    getSyncButton.Enabled = false;
                    //authWSPSButton.Enabled = true;
                    closePSButton.Enabled = false;
                }
            }
        }



        private void cpsbutton_Click(object sender, EventArgs e)
        {
            try
            {
                runspace.Dispose();
                runspace = null;
                powershell1.Dispose();
                powershell1 = null;
                MessageBox.Show("Powershell Session Closed", "O365 Delegates");
                textBox1.Text = "Powershell Session Closed";
            }
            catch (Exception)
            {
                MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
            }

            getSendAsButton.Enabled = false;
            getMBXPermButton.Enabled = false;
            giveSendAsButton.Enabled = false;
            removeSendAsButton.Enabled = false;
            giveMBXPermButton.Enabled = false;
            removeMBXPermButton.Enabled = false;
            getMUSRButton.Enabled = false;
            getMBXButton.Enabled = false;
            getCASButton.Enabled = false;
            getMailUSRButton.Enabled = false;
            getLogStatButton.Enabled = false;
            getMBXStatButton.Enabled = false;
            getSyncButton.Enabled = false;
            authWSButton.Enabled = true;
            authPSButton.Enabled = true;
            //authWSPSButton.Enabled = true;
            closePSButton.Enabled = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void mbxbutton_Click(object sender, EventArgs e)
        {
            getMBXButton.Text = "Processing...";
            getMBXButton.Enabled = false;
            string Success = "";
            if (textBox20.Text == "" || textBox21.Text == "")
            {

                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox21.Select();
                getMBXButton.Enabled = true;
                getMBXButton.Text = "Get-Mailbox";
            }
            else
            {
                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox20.Text;
                string usr = textBox21.Text;
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"get-mailbox -identity $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"get-mailbox -identity $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }

                if (Success == "True")
                {
                    try
                    {

                        Process.Start(textBox20.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox21.Select();
                        getMBXButton.Enabled = true;
                        getMBXButton.Text = "Get-Mailbox";
                    }
                }
                textBox21.Select();
                getMBXButton.Enabled = true;
                getMBXButton.Text = "Get-Mailbox";
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void musrbutton_Click(object sender, EventArgs e)
        {
            getMUSRButton.Text = "Processing...";
            getMUSRButton.Enabled = false;
            string Success = "";
            if (textBox18.Text == "" || textBox19.Text == "")
            {

                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox18.Select();
                getMUSRButton.Enabled = true;
                getMUSRButton.Text = "Get-MsolUser";
            }
            else
            {

                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox19.Text;
                string usr = textBox18.Text;
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-MsolUser -UserPrincipalName $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {

                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-MsolUser -UserPrincipalName $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();

                }

                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }


                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox19.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox18.Select();
                        getMUSRButton.Enabled = true;
                        getMUSRButton.Text = "Get-MsolUser";
                    }
                }
                textBox18.Select();
                getMUSRButton.Enabled = true;
                getMUSRButton.Text = "Get-MsolUser";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void casbutton_Click(object sender, EventArgs e)
        {
            getCASButton.Text = "Processing...";
            getCASButton.Enabled = false;
            string Success = "";
            if (textBox25.Text == "" || textBox24.Text == "")
            {
                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox25.Select();
                getCASButton.Enabled = true;
                getCASButton.Text = "Get-CASMailbox";
            }
            else
            {
                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox24.Text;
                string usr = textBox25.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-CASMailbox -Identity $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-CASMailbox -Identity $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }

                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox24.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox25.Select();
                        getCASButton.Enabled = true;
                        getCASButton.Text = "Get-CASMailbox";
                    }
                }
                textBox25.Select();
                getCASButton.Enabled = true;
                getCASButton.Text = "Get-CASMailbox";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void mausrbutton_Click(object sender, EventArgs e)
        {
            getMailUSRButton.Text = "Processing...";
            getMailUSRButton.Enabled = false;

            string Success = "";
            if (textBox26.Text == "" || textBox27.Text == "")
            {

                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox27.Select();
                getMailUSRButton.Enabled = true;
                getMailUSRButton.Text = "Get-MailUser";
            }
            else
            {

                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox26.Text;
                string usr = textBox27.Text;
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-MailUser -Identity $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {

                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-MailUser -Identity $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();

                }

                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }


                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox26.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox27.Select();
                        getMailUSRButton.Enabled = true;
                        getMailUSRButton.Text = "Get-CASMailbox";
                    }
                }
                textBox27.Select();
                getMailUSRButton.Enabled = true;
                getMailUSRButton.Text = "Get-MailUser";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void logbbutton_Click(object sender, EventArgs e)
        {
            getLogStatButton.Text = "Processing...";
            getLogStatButton.Enabled = false;

            string Success = "";
            if (textBox28.Text == "" || textBox29.Text == "")
            {

                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox29.Select();
                getLogStatButton.Enabled = true;
                getLogStatButton.Text = "Get-LogonStatistics";
            }
            else
            {
                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox28.Text;
                string usr = textBox29.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-LogonStatistics -Identity $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-LogonStatistics -Identity $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }


                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }



                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox28.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox29.Select();
                        getLogStatButton.Enabled = true;
                        getLogStatButton.Text = "Get-LogonStatistics";
                    }
                }
                textBox29.Select();
                getLogStatButton.Enabled = true;
                getLogStatButton.Text = "Get-LogonStatistics";
            }
        }

        private void mbxsbutton_Click(object sender, EventArgs e)
        {
            getMBXStatButton.Text = "Processing...";
            getMBXStatButton.Enabled = false;
            string Success = "";
            if (textBox30.Text == "" || textBox31.Text == "")
            {

                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox31.Select();
                getMBXStatButton.Enabled = true;
                getMBXStatButton.Text = "Get-MailboxFolderStatistics";
            }
            else
            {
                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox30.Text;
                string usr = textBox31.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-MailboxFolderStatistics -Identity $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-MailboxFolderStatistics -Identity $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }



                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox30.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox31.Select();
                        getMBXStatButton.Enabled = true;
                        getMBXStatButton.Text = "Get-MailboxFolderStatistics";
                    }
                }
                textBox31.Select();
                getMBXStatButton.Enabled = true;
                getMBXStatButton.Text = "Get-MailboxFolderStatistics";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void syncbutton_Click(object sender, EventArgs e)
        {
            getSyncButton.Text = "Processing...";
            getSyncButton.Enabled = false;

            string Success = "";
            if (textBox32.Text == "" || textBox33.Text == "")
            {

                MessageBox.Show("Email ID/File Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox33.Select();
                getSyncButton.Enabled = true;
                getSyncButton.Text = "Get-ActiveSyncDeviceStatistics";
            }
            else
            {
                string curDir = Directory.GetCurrentDirectory();
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string outPath = textBox32.Text;
                string usr = textBox33.Text;
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-ActiveSyncDeviceStatistics  -Mailbox $usr | FL  | Out-File $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-ActiveSyncDeviceStatistics  -Mailbox $usr | FL  | Out-File $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runCMDlet(psPath, usr, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }



                if (Success == "True")
                {
                    try
                    {
                        Process.Start(textBox32.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Issue in generating output. Please check the file name/path/Is it in Root?", "O365 Delegates: Warning !!!");
                        textBox33.Select();
                        getSyncButton.Enabled = true;
                        getSyncButton.Text = "Get-ActiveSyncDeviceStatistics";
                    }
                }
                textBox33.Select();
                getSyncButton.Enabled = true;
                getSyncButton.Text = "Get-ActiveSyncDeviceStatistics";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void gmprmbutton_Click(object sender, EventArgs e)
        {
            giveMBXPermButton.Text = "Processing...";
            giveMBXPermButton.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string outPath = Convert.ToString(curDir + @"\Result.txt");
            string Success = "";
            if (textBox17.Text == "" || textBox16.Text == "")
            {
                MessageBox.Show("Identity/Trustee Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox11.Select();
                giveMBXPermButton.Enabled = true;
                giveMBXPermButton.Text = "Grant";
            }
            else
            {
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox17.Text;
                string tst = textBox16.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Add-MailboxPermission -Identity  $usr -AccessRights FullAccess -User $tst -InheritanceType All -Confirm:$False ");
                        writeFile.WriteLine(@"Get-MailboxPermission -identity $usr | FL | out-file $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }
                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Add-MailboxPermission -Identity  $usr -AccessRights FullAccess -User $tst -InheritanceType All -Confirm:$False ");
                    writeFile.WriteLine(@"Get-MailboxPermission -identity $usr | FL | out-file $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                    textBox17.Select();
                    giveMBXPermButton.Enabled = true;
                    giveMBXPermButton.Text = "Grant";
                }
                catch (Exception)
                {
                    Success = "True";
                    textBox17.Select();
                    giveMBXPermButton.Enabled = true;
                    giveMBXPermButton.Text = "Grant";
                }

                if (Success == "True")
                {
                    try
                    {
                        Process.Start(outPath);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The File Was Generated, But It Could Not Open. You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                        textBox17.Select();
                        giveMBXPermButton.Enabled = true;
                        giveMBXPermButton.Text = "Grant";
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void rmprmbutton_Click(object sender, EventArgs e)
        {
            removeMBXPermButton.Text = "Processing...";
            removeMBXPermButton.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string outPath = Convert.ToString(curDir + @"\Result.txt");
            string Success = "";
            if (textBox17.Text == "" || textBox16.Text == "")
            {
                MessageBox.Show("Identity/Trustee Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox17.Select();
                removeMBXPermButton.Enabled = true;
                removeMBXPermButton.Text = "Remove";
            }
            else
            {
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox17.Text;
                string tst = textBox16.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Remove-MailboxPermission -identity $usr -AccessRights FullAccess  -User $tst  -InheritanceType All -Confirm:$False ");
                        writeFile.WriteLine(@"Get-MailboxPermission -identity $usr  | FL | out-file $outPath");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Remove-MailboxPermission -identity $usr -AccessRights FullAccess  -User $tst  -InheritanceType All -Confirm:$False ");
                    writeFile.WriteLine(@"Get-MailboxPermission -identity $usr  | FL | out-file $outPath");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }


                if (Success == "True")
                {
                    try
                    {
                        Process.Start(outPath);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The File Was Generated, But It Could Not Open. You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                        textBox17.Select();
                        removeMBXPermButton.Enabled = true;
                        removeMBXPermButton.Text = "Remove";
                    }
                }
                textBox17.Select();
                removeMBXPermButton.Enabled = true;
                removeMBXPermButton.Text = "Remove";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void sasbutton_Click(object sender, EventArgs e)
        {
            getSendAsButton.Text = "Processing...";
            getSendAsButton.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string outPath = Convert.ToString(curDir + @"\Result.txt");
            string Success = "";
            if (textBox34.Text == "" && textBox35.Text == "")
            {
                MessageBox.Show("Identity/Trustee Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox34.Select();
                getSendAsButton.Enabled = true;
                getSendAsButton.Text = "Get SendAs";
            }
            else if (textBox34.Text != "" && textBox35.Text == "")
            {
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox34.Text;
                string tst = textBox35.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-RecipientPermission -identity $usr | FL | out-file $outPath -ea stop");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }
                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-RecipientPermission -identity $usr | FL | out-file $outPath -ea stop");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }

            }
            else if (textBox34.Text != "" && textBox35.Text != "")
            {
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox34.Text;
                string tst = textBox35.Text;
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {

                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-RecipientPermission -identity $usr -Trustee $tst | FL  | out-file $outPath -ea stop  ");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-RecipientPermission -identity $usr -Trustee $tst | FL  | out-file $outPath -ea stop  ");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }
            }
            else if (textBox34.Text == "" && textBox35.Text != "")
            {
                MessageBox.Show("Identity Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox34.Select();
                getSendAsButton.Enabled = true;
                getSendAsButton.Text = "Get SendAs";
            }

            if (Success == "True")
            {
                try
                {

                    Process.Start(outPath);
                }
                catch
                {
                    MessageBox.Show("The File Was Generated, But It Could Not Open. You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                    textBox34.Select();
                    getSendAsButton.Enabled = true;
                    getSendAsButton.Text = "Get SendAs";
                }
            }
            textBox34.Select();
            getSendAsButton.Enabled = true;
            getSendAsButton.Text = "Get SendAs";

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void mprmbutton_Click(object sender, EventArgs e)
        {

            getMBXPermButton.Text = "Processing...";
            getMBXPermButton.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string outPath = Convert.ToString(curDir + @"\Result.txt");

            string Success = "";
            if (textBox34.Text == "" && textBox35.Text == "")
            {
                MessageBox.Show("Identity/Trustee Name Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox34.Select();
                getMBXPermButton.Enabled = true;
                getMBXPermButton.Text = "Get MBX Perm.";
            }
            else if (textBox34.Text != "" && textBox35.Text == "")
            {

                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox34.Text;
                string tst = textBox35.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-MailboxPermission -identity $usr | FL | out-file $outPath -ea stop");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }
                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-MailboxPermission -identity $usr | FL | out-file $outPath -ea stop");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }




            }
            else if (textBox34.Text != "" && textBox35.Text != "")
            {
                string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
                string usr = textBox34.Text;
                string tst = textBox35.Text;
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine(@"Get-MailboxPermission  -identity $usr -user $tst | FL  | out-file $outPath -ea stop  ");
                        writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                        writeFile.Close();
                    }

                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine(@"Get-MailboxPermission  -identity $usr -user $tst | FL  | out-file $outPath -ea stop  ");
                    writeFile.WriteLine("if($error -ne $null){$error > $outPath }");
                    writeFile.Close();
                }

                try
                {
                    runSetCMDlet(psPath, usr, tst, outPath);
                    Success = "True";
                }
                catch (Exception)
                {
                    MessageBox.Show("It seems session timed out. It could be, the tool was idle for long time OR issue in net connection.Restart the tool to recreate the session", "O365 Delegates: Error !!!");
                    Success = "False";
                }

            }
            else if (textBox34.Text == "" && textBox35.Text != "")
            {
                MessageBox.Show("Identity Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox34.Select();
                getMBXPermButton.Enabled = true;
                getMBXPermButton.Text = "Get MBX Perm.";
            }

            if (Success == "True")
            {
                try
                {

                    Process.Start(outPath);
                }
                catch
                {
                    MessageBox.Show("The File Was Generated, But It Could Not Open. You Can Open It From Respective Application Directly.", "O365 Delegates: Warning !!!");
                    textBox34.Select();
                    getMBXPermButton.Enabled = true;
                    getMBXPermButton.Text = "Get MBX Perm.";
                }
            }
            textBox34.Select();
            getMBXPermButton.Enabled = true;
            getMBXPermButton.Text = "Get MBX Perm.";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private void button20_Click(object sender, EventArgs e)
        {
            button20.Text = "Processing...";
            button20.Enabled = false;
            string curDir = Directory.GetCurrentDirectory();
            string psPath = Convert.ToString(curDir + @"\getmbx.ps1");
            string outPath = Convert.ToString(curDir + @"\Result.txt");
            string usr = textBox23.Text;
            string Success = "false";
            string psstatus = "fail";

            if (((textBox23.Text == "") || (textBox36.Text == "") || (textBox37.Text == "") || (maskedTextBox2.Text == "")) || ((radioButton7.Checked == false) && (radioButton8.Checked == false) && (radioButton9.Checked == false)))
            {
                MessageBox.Show("FQDN/User/PWD/Radio option Should Not Be Blank.", "O365 Delegates: Warning !!!");
                button20.Text = "Get LDP";
                button20.Enabled = true;
                textBox37.Focus();
            }

            else
            {
                try
                {
                    string adminUPN = textBox37.Text;
                    string adminPWD = maskedTextBox2.Text;
                    string FQDN = "http://" + textBox36.Text + "/powershell/";
                    Uri psURL = new Uri(FQDN);
                    createOnPremPSSession(FQDN, adminUPN, adminPWD);
                    psstatus = "pass";
                }
                catch (Exception)
                {
                    MessageBox.Show("Issue in creating Powershell Runspace. Check server name and credential", "O365 Delegates: Warning !!!");
                    psstatus = "fail";
                    button20.Text = "Get LDP";
                    button20.Enabled = true;
                    textBox37.Focus();
                }
            }

            if ((radioButton7.Checked == true) && (psstatus == "pass"))
            {
                if (!File.Exists(psPath))
                {

                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {

                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine("try");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$uDN=get-user -identity $usr | ft DistinguishedName -hidetableheaders | out-string");
                        writeFile.WriteLine("$ldpDN=\"[ADSI]'LDAP://$uDN'\"");
                        writeFile.WriteLine("$ldpDump=Invoke-Expression \"$ldpDN\"");
                        writeFile.WriteLine(@"if((Test-Path -path $outpath) -eq $true)");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine(@"clear-content $outpath");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("foreach($pN in $ldpDump.psbase.properties.Propertynames)");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$val=\"$($ldpDump.psbase.properties[$pN].count):$($pN):$($ldpDump.psbase.properties[$pN])\" | out-string");
                        writeFile.WriteLine(@"$val >> $outpath");
                        writeFile.WriteLine(@"if($error -ne $null){$error > $outpath }");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("catch");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$error > $outpath");
                        writeFile.WriteLine("}");
                        writeFile.Close();

                    }

                }
                else
                {

                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine("try");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$uDN=get-user -identity $usr | ft DistinguishedName -hidetableheaders | out-string");
                    writeFile.WriteLine("$ldpDN=\"[ADSI]'LDAP://$uDN'\"");
                    writeFile.WriteLine("$ldpDump=Invoke-Expression \"$ldpDN\"");
                    writeFile.WriteLine(@"if((Test-Path -path $outpath) -eq $true)");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine(@"clear-content $outpath");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("foreach($pN in $ldpDump.psbase.properties.Propertynames)");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$val=\"$($ldpDump.psbase.properties[$pN].count):$($pN):$($ldpDump.psbase.properties[$pN])\" | out-string");
                    writeFile.WriteLine(@"$val >> $outpath");
                    writeFile.WriteLine(@"if($error -ne $null){$error > $outpath }");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("catch");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$error > $outpath");
                    writeFile.WriteLine("}");
                    writeFile.Close();

                }

            }
            else if ((radioButton8.Checked == true) && (psstatus == "pass"))
            {
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine("try");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$uDN=get-contact -identity $usr | ft DistinguishedName -hidetableheaders | out-string");
                        writeFile.WriteLine("$ldpDN=\"[ADSI]'LDAP://$uDN'\"");
                        writeFile.WriteLine("$ldpDump=Invoke-Expression \"$ldpDN\"");
                        writeFile.WriteLine(@"if((Test-Path -path $outpath) -eq $true)");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine(@"clear-content $outpath");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("foreach($pN in $ldpDump.psbase.properties.Propertynames)");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$val=\"$($ldpDump.psbase.properties[$pN].count):$($pN):$($ldpDump.psbase.properties[$pN])\" | out-string");
                        writeFile.WriteLine(@"$val >> $outpath");
                        writeFile.WriteLine(@"if($error -ne $null){$error > $outpath }");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("catch");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$error > $outpath");
                        writeFile.WriteLine("}");
                        writeFile.Close();
                    }
                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine("try");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$uDN=get-contact -identity $usr | ft DistinguishedName -hidetableheaders | out-string");
                    writeFile.WriteLine("$ldpDN=\"[ADSI]'LDAP://$uDN'\"");
                    writeFile.WriteLine("$ldpDump=Invoke-Expression \"$ldpDN\"");
                    writeFile.WriteLine(@"if((Test-Path -path $outpath) -eq $true)");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine(@"clear-content $outpath");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("foreach($pN in $ldpDump.psbase.properties.Propertynames)");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$val=\"$($ldpDump.psbase.properties[$pN].count):$($pN):$($ldpDump.psbase.properties[$pN])\" | out-string");
                    writeFile.WriteLine(@"$val >> $outpath");
                    writeFile.WriteLine(@"if($error -ne $null){$error > $outpath }");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("catch");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$error > $outpath");
                    writeFile.WriteLine("}");
                    writeFile.Close();
                }
            }
            else if ((radioButton9.Checked == true) && (psstatus == "pass"))
            {
                if (!File.Exists(psPath))
                {
                    using (StreamWriter writeFile = File.CreateText(psPath))
                    {
                        writeFile.WriteLine("$error.clear()");
                        writeFile.WriteLine("try");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$uDN=get-group -identity $usr | ft DistinguishedName -hidetableheaders | out-string");
                        writeFile.WriteLine("$ldpDN=\"[ADSI]'LDAP://$uDN'\"");
                        writeFile.WriteLine("$ldpDump=Invoke-Expression \"$ldpDN\"");
                        writeFile.WriteLine(@"if((Test-Path -path $outpath) -eq $true)");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine(@"clear-content $outpath");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("foreach($pN in $ldpDump.psbase.properties.Propertynames)");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$val=\"$($ldpDump.psbase.properties[$pN].count):$($pN):$($ldpDump.psbase.properties[$pN])\" | out-string");
                        writeFile.WriteLine(@"$val >> $outpath");
                        writeFile.WriteLine(@"if($error -ne $null){$error > $outpath }");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("}");
                        writeFile.WriteLine("catch");
                        writeFile.WriteLine("{");
                        writeFile.WriteLine("$error > $outpath");
                        writeFile.WriteLine("}");
                        writeFile.Close();
                    }
                }
                else
                {
                    TextWriter writeFile = new StreamWriter(psPath);
                    writeFile.WriteLine("$error.clear()");
                    writeFile.WriteLine("try");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$uDN=get-group -identity $usr | ft DistinguishedName -hidetableheaders | out-string");
                    writeFile.WriteLine("$ldpDN=\"[ADSI]'LDAP://$uDN'\"");
                    writeFile.WriteLine("$ldpDump=Invoke-Expression \"$ldpDN\"");
                    writeFile.WriteLine(@"if((Test-Path -path $outpath) -eq $true)");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine(@"clear-content $outpath");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("foreach($pN in $ldpDump.psbase.properties.Propertynames)");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$val=\"$($ldpDump.psbase.properties[$pN].count):$($pN):$($ldpDump.psbase.properties[$pN])\" | out-string");
                    writeFile.WriteLine(@"$val >> $outpath");
                    writeFile.WriteLine(@"if($error -ne $null){$error > $outpath }");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("}");
                    writeFile.WriteLine("catch");
                    writeFile.WriteLine("{");
                    writeFile.WriteLine("$error > $outpath");
                    writeFile.WriteLine("}");
                    writeFile.Close();
                }
            }

            try
            {
                runOnCMDlet(psPath, usr, outPath);
                Success = "True";
            }
            catch
            {
                MessageBox.Show("Issue in running PowerShell Command", "O365 Delegates: Error !!!");
                Success = "False";
            }


            if (psstatus == "pass")
            {
                Onrunspace.Dispose();
                Onrunspace = null;
                OnPremPS.Dispose();
                OnPremPS = null;
            }

            if ((Success == "True") && (psstatus == "pass"))
            {
                try
                {
                    Process.Start(outPath);
                }
                catch
                {
                    MessageBox.Show("Issue in generating the file.", "O365 Delegates: Warning !!!");
                    textBox14.Select();
                    giveSendAsButton.Enabled = true;
                    giveSendAsButton.Text = "Get LDP";
                }
            }
            textBox21.Select();
            button20.Text = "Get LDP";
            button20.Enabled = true;
        }

        private void blgbutton_Click_1(object sender, EventArgs e)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Help - revised (1).docx");
            Process.Start("http://blogs.technet.com/b/sukum/archive/2013/01/23/coming-soon-office-365-delegates-tool.aspx");
        }

        private void pathbbutton_Click(object sender, EventArgs e)
        {
            string curDir = Directory.GetCurrentDirectory();
            MessageBox.Show("Installation Path: " + curDir, "O365 Delegates !!!");
        }

        private void helpbbutton_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Please refer the Help.pdf file included in setup compressed file or you can also download from the blog.", "O365 Delegates !!!");
        }

        // Functions


        // Function AddCalDelegates
        public Collection<DelegateUserResponse> AddCalDelegates()
        {

            addDelButton.Enabled = false;
            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            ewsService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, textBox4.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;
            string NameCorrect = "False";

            if ((textBox4.Text == "" || textBox5.Text == "") && (radioButton3.Checked == false || radioButton4.Checked == false))
            {
                MessageBox.Show("Manager ID/Delegate ID Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox4.Select();
                addDelButton.Enabled = true;
            }
            else
            {

                NameResolutionCollection CheckName = ewsService.ResolveName(textBox5.Text, ResolveNameSearchLocation.DirectoryOnly, true);
                foreach (NameResolution NameList in CheckName)
                {

                    NameCorrect = "True";
                    addDelButton.Enabled = true;
                }
            }

            if (NameCorrect == "True")
            {
                Mailbox actMBX = new Mailbox(textBox4.Text);
                DelegateInformation delegateUSR = ewsService.GetDelegates(actMBX, true);
                MessageBox.Show(textBox5.Text + "textbox");

                foreach (DelegateUserResponse delResult in delegateUSR.DelegateUserResponses)
                {
                    DelegateUser delegateUSER = delResult.DelegateUser;
                    if ((delegateUSER.UserId.PrimarySmtpAddress != null) && (textBox5.Text == delegateUSER.UserId.PrimarySmtpAddress))
                    {
                        MessageBox.Show("Delegate already present with permission: " + delegateUSER.Permissions.CalendarFolderPermissionLevel + ". Suggest You Remove The Delegate And Re-Add");
                        NameCorrect = "False";
                    }
                }
                return null;
            }
            else
            {
                MessageBox.Show("Name Is Not Found. Check The ID :" + textBox5.Text);
                NameCorrect = "False";
                textBox5.Select();
                addDelButton.Enabled = true;
                return null;
            }
        }

        // Function AddCalInboxDelegates

        public Collection<DelegateUserResponse> AddCalInboxDelegates()
        {
            addDelButton.Enabled = false;
            ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
            ewsService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, textBox4.Text);
            Uri ewsServiceURL = new Uri(EWSURL);
            ewsService.Url = ewsServiceURL;

            if ((textBox4.Text == "" || textBox5.Text == "") && (radioButton3.Checked == false || radioButton4.Checked == false))
            {
                MessageBox.Show("Manager ID/Delegate ID Should Not Be Blank.", "O365 Delegates: Warning !!!");
                textBox4.Select();
                addDelButton.Enabled = true;

            }
            else
            {
                string NameCorrect1 = "False";
                NameResolutionCollection CheckName = ewsService.ResolveName(textBox5.Text, ResolveNameSearchLocation.DirectoryOnly, true);
                foreach (NameResolution NameList in CheckName)
                {
                    NameCorrect1 = "True";
                }
                if (NameCorrect1 == "True")
                {
                    Mailbox actMBX = new Mailbox(textBox4.Text);
                    DelegateInformation delegateUSR = ewsService.GetDelegates(actMBX, true);
                    foreach (DelegateUserResponse delResult in delegateUSR.DelegateUserResponses)
                    {
                        DelegateUser delegateUSER = delResult.DelegateUser;
                        if ((delegateUSER.UserId.PrimarySmtpAddress != null) && (textBox5.Text == delegateUSER.UserId.PrimarySmtpAddress))
                        {
                            MessageBox.Show("Delegate already present with permission: " + delegateUSER.Permissions.CalendarFolderPermissionLevel + ". Suggest You Remove The Delegate Ans Re-Add");
                            NameCorrect1 = "False";
                        }
                        else
                        {
                            try
                            {
                                List<DelegateUser> newAllDelegates = new System.Collections.Generic.List<DelegateUser>();
                                DelegateUser AllDelegate = new DelegateUser(textBox5.Text);
                                AllDelegate.Permissions.CalendarFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                AllDelegate.Permissions.InboxFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                AllDelegate.Permissions.TasksFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                AllDelegate.Permissions.ContactsFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                AllDelegate.Permissions.NotesFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                AllDelegate.Permissions.JournalFolderPermissionLevel = DelegateFolderPermissionLevel.Editor;
                                newAllDelegates.Add(AllDelegate);
                                Mailbox actMBX1 = new Mailbox(textBox4.Text);
                                Collection<DelegateUserResponse> delReturn = ewsService.AddDelegates(actMBX1, MeetingRequestsDeliveryScope.DelegatesAndSendInformationToMe, newAllDelegates);
                                MessageBox.Show("Editor Permission on All to " + textBox5.Text + " On The Mailbox " + textBox4.Text + " Added.", "O365 Delegates: Success !!!");
                                textBox4.Select();
                                addDelButton.Enabled = true;
                                NameCorrect1 = "False";
                                return delReturn;
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Unable To Add Permission", "O365 Delegates: Error !!!");
                                textBox4.Select();
                                addDelButton.Enabled = true;
                                NameCorrect1 = "False";
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Name Is Not Found. Check The ID :" + textBox5.Text);
                    NameCorrect1 = "False";
                    textBox5.Select();
                    addDelButton.Enabled = true;
                }
            }
            return null;
        }

        // Function getEWS
        public void getEWS()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ExchangeService ewsService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                ewsService.Credentials = new NetworkCredential(textBox10.Text, maskedTextBox1.Text, textBox13.Text);
                ewsService.TraceEnabled = true;
                ewsService.TraceFlags = TraceFlags.All;
                this.Cursor = Cursors.Default;
                ewsService.AutodiscoverUrl(textBox10.Text, routeURL);
                MessageBox.Show("EWS Session Created.Tabs with title WS can be used.", "O365 Delegates !!!");
                textBox1.Text = "Tabs with title WS can be used.";
                EWSURL = (Convert.ToString(ewsService.Url));
                dlGenButton.Enabled = true;
                addDelButton.Enabled = true;
                lstDelButton.Enabled = true;
                remDelButton.Enabled = true;
                updDelButton.Enabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Check Details Entered And URL reachability.", "O365 Delegates: Error !!!");
                dlGenButton.Enabled = false;
                addDelButton.Enabled = false;
                lstDelButton.Enabled = false;
                remDelButton.Enabled = false;
                updDelButton.Enabled = false;
            }

        }

        // Function runCMDlet  
        public void runCMDlet(string PSPath, string usr, string OutPath)
        {
            System.Collections.ObjectModel.Collection<PSObject> outPUT = new System.Collections.ObjectModel.Collection<PSObject>();
            powershell = PowerShell.Create();
            powershell.Runspace = runspace;
            System.IO.StreamReader sr = new System.IO.StreamReader(PSPath);
            powershell.AddScript(sr.ReadToEnd());
            powershell.Runspace.SessionStateProxy.SetVariable("usr", usr);
            powershell.Runspace.SessionStateProxy.SetVariable("outPath", OutPath);
            outPUT = powershell.Invoke();
            sr.Close();
        }

        // Function runOnCMDlet
        public void runOnCMDlet(string PSPath, string usr, string OutPath)
        {
            System.Collections.ObjectModel.Collection<PSObject> outPUT = new System.Collections.ObjectModel.Collection<PSObject>();
            OnPremPS = PowerShell.Create();
            OnPremPS.Runspace = Onrunspace;
            System.IO.StreamReader sr = new System.IO.StreamReader(PSPath);
            OnPremPS.AddScript(sr.ReadToEnd());
            OnPremPS.Runspace.SessionStateProxy.SetVariable("usr", usr);
            OnPremPS.Runspace.SessionStateProxy.SetVariable("outPath", OutPath);
            outPUT = OnPremPS.Invoke();
            sr.Close();
        }

        // Function runSetCMDlet
        public void runSetCMDlet(string PSPath, string usr, string tst, string OutPath)
        {
            System.Collections.ObjectModel.Collection<PSObject> outPUT = new System.Collections.ObjectModel.Collection<PSObject>();
            powershell = PowerShell.Create();
            powershell.Runspace = runspace;
            System.IO.StreamReader sr = new System.IO.StreamReader(PSPath);
            powershell.AddScript(sr.ReadToEnd());
            powershell.Runspace.SessionStateProxy.SetVariable("usr", usr);
            powershell.Runspace.SessionStateProxy.SetVariable("tst", tst);
            powershell.Runspace.SessionStateProxy.SetVariable("outPath", OutPath);
            outPUT = powershell.Invoke();
            sr.Close();
        }

        // Function createPSSession
        public void createPSSession()
        {

            System.Uri psURL = new Uri("https://ps.outlook.com/powershell/");
            System.Security.SecureString securePassword1 = safeString(maskedTextBox1.Text);
            System.Management.Automation.PSCredential creds1 = new System.Management.Automation.PSCredential(textBox10.Text, securePassword1);
            runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace();

            powershell1 = PowerShell.Create();
            PSCommand command = new PSCommand();
            command.AddCommand("New-PSSession");
            command.AddParameter("ConfigurationName", "Microsoft.Exchange");
            command.AddParameter("ConnectionUri", psURL);
            command.AddParameter("Credential", creds1);
            command.AddParameter("Authentication", "Basic");
            command.AddParameter("AllowRedirection");

            PSSessionOption sessionOption = new PSSessionOption();
            sessionOption.SkipCACheck = true;
            sessionOption.SkipCNCheck = true;
            sessionOption.SkipRevocationCheck = true;
            command.AddParameter("SessionOption", sessionOption);
            powershell1.Commands = command;

            runspace.Open();

            powershell1.Runspace = runspace;

            Collection<PSSession> result = powershell1.Invoke<PSSession>();

            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddCommand("Set-Variable");
            command.AddParameter("Name", "ra");
            command.AddParameter("Value", result[0]);
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();


            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddScript("Import-PSSession -Session $ra");
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();


            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddScript("import-module msonline");

            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();


            powershell = PowerShell.Create();
            command = new PSCommand();
            command.AddCommand("connect-msolservice");
            command.AddParameter("Credential", creds1);
            powershell.Commands = command;
            powershell.Runspace = runspace;
            powershell.Invoke();

        }

        // Function createOnPremPSSession
        public void createOnPremPSSession(string sURI, string admin, string pwd)
        {
            System.Uri psURL = new Uri(sURI);
            System.Security.SecureString securePassword1 = safeString(pwd);
            System.Management.Automation.PSCredential creds1 = new System.Management.Automation.PSCredential(admin, securePassword1);
            Onrunspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace();
            OnPremPS = PowerShell.Create();

            PSCommand command = new PSCommand();
            command.AddCommand("New-PSSession");
            command.AddParameter("ConfigurationName", "Microsoft.Exchange");
            command.AddParameter("ConnectionUri", psURL);
            command.AddParameter("Credential", creds1);
            command.AddParameter("Authentication", "kerberos");
            command.AddParameter("AllowRedirection");

            PSSessionOption sessionOption = new PSSessionOption();
            sessionOption.SkipCACheck = true;
            sessionOption.SkipCNCheck = true;
            sessionOption.SkipRevocationCheck = true;
            command.AddParameter("SessionOption", sessionOption);
            OnPremPS.Commands = command;
            Onrunspace.Open();

            OnPremPS.Runspace = Onrunspace;

            Collection<PSSession> result = OnPremPS.Invoke<PSSession>();
            OnPremPS = PowerShell.Create();
            command = new PSCommand();
            command.AddCommand("Set-Variable");
            command.AddParameter("Name", "OnPremSess");
            command.AddParameter("Value", result[0]);
            OnPremPS.Commands = command;
            OnPremPS.Runspace = Onrunspace;
            OnPremPS.Invoke();


            OnPremPS = PowerShell.Create();
            command = new PSCommand();
            command.AddScript("Import-PSSession -Session $OnPremSess");
            OnPremPS.Commands = command;
            OnPremPS.Runspace = Onrunspace;
            OnPremPS.Invoke();

        }

        // Function safeString
        public static SecureString safeString(string password)
        {
            SecureString remotePassword = new SecureString();
            for (int i = 0; i < password.Length; i++)
                remotePassword.AppendChar(password[i]);
            return remotePassword;
        }


    }
}
