using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Forms; 

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon1();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace RibbonXOutlook14AddinCS
{
    [ComVisible(true)]
    public class RibbonXAddin : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        private Outlook.Application olApplication;
        
        //Override of constructor to pass 
        // a trusted Outlook.Application object
        public RibbonXAddin(Outlook.Application outlookApplication)
        {
            olApplication = outlookApplication as Outlook.Application;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            string customUI = string.Empty;
            Debug.WriteLine(ribbonID);
            //Return the appropriate Ribbon XML for ribbonID
            switch (ribbonID)
            {
                case "Microsoft.Outlook.Explorer":
                    customUI =GetResourceText(
                        "RibbonXOutlook14AddinCS.Explorer.xml");
                    return customUI;
                case "Microsoft.Outlook.Mail.Read":
                    customUI= GetResourceText(
                        "RibbonXOutlook14AddinCS.ReadMail.xml");
                    return customUI;
                case "Microsoft.Mso.IMLayerUI":
                    customUI = GetResourceText(
                        "RibbonXOutlook14AddinCS.ContactCard.xml");
                    return customUI;
                default:
                    return string.Empty;
            }
        }

        #endregion

        #region Ribbon Callbacks
        // RibbonX callbacks

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            ThisAddIn.m_Ribbon = ribbonUI;
        }

        // Only show MyTab when Explorer Selection is 
        // a received mail or when Inspector is a read note
        public bool MyTab_GetVisible(Office.IRibbonControl control)
        {
            if (control.Context is Outlook.Explorer)
            {
                Outlook.Explorer explorer = 
                    control.Context as Outlook.Explorer;
                Outlook.Selection selection = explorer.Selection;
                if (selection.Count == 1)
                {
                    if (selection[1] is Outlook.MailItem)
                    {
                        Outlook.MailItem oMail = 
                            selection[1] as Outlook.MailItem;
                        if (oMail.Sent == true)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (control.Context is Outlook.Inspector)
            {
                Outlook.Inspector oInsp = 
                    control.Context as Outlook.Inspector;
                if (oInsp.CurrentItem is Outlook.MailItem)
                {
                    Outlook.MailItem oMail = 
                        oInsp.CurrentItem as Outlook.MailItem;
                    if (oMail.Sent == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool MyTabInspector_GetVisible(Office.IRibbonControl control)
        {
            if (control.Context is Outlook.Inspector)
            {
                Outlook.Inspector oInsp =
                    control.Context as Outlook.Inspector;
                if (oInsp.CurrentItem is Outlook.MailItem)
                {
                    Outlook.MailItem oMail =
                        oInsp.CurrentItem as Outlook.MailItem;
                    if (oMail.Sent == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        //MyBackstageTab_GetVisible hides the place in an Inspector window
        public bool MyBackstageTab_GetVisible(Office.IRibbonControl control)
        {
            if (control.Context is Outlook.Explorer)
                return true;
            else
                return false;
        }

        public stdole.IPictureDisp GetCurrentUserImage(Office.IRibbonControl control)
        {
            //stdole.IPictureDisp pictureDisp = null;
            Outlook.AddressEntry addrEntry = 
                Globals.ThisAddIn.Application.Session.CurrentUser.AddressEntry;
                if(addrEntry.Type=="EX")
                {
                    if (Globals.ThisAddIn.m_pictdisp != null)
                    {
                        return Globals.ThisAddIn.m_pictdisp;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
        }


        // OnMyButtonClick routine handles all button click events
        // and displays IRibbonControl.Context in message box
        public void OnMyButtonClick(Office.IRibbonControl control)
        {
            string msg = string.Empty;
            if (control.Context is Outlook.AttachmentSelection)
            {
                msg = "Context=AttachmentSelection" + "\n";
                Outlook.AttachmentSelection attachSel =
                    control.Context as Outlook.AttachmentSelection;
                foreach (Outlook.Attachment attach in attachSel)
                {
                    msg = msg + attach.DisplayName + "\n";
                }
            }
            else if (control.Context is Outlook.Folder)
            {
                msg = "Context=Folder" + "\n";
                Outlook.Folder folder = 
                    control.Context as Outlook.Folder;
                msg = msg + folder.Name;
            }
            else if (control.Context is Outlook.Selection)
            {
                msg = "Context=Selection" + "\n";
                Outlook.Selection selection = 
                    control.Context as Outlook.Selection;
                if (selection.Count == 1)
                {
                    OutlookItem olItem = 
                        new OutlookItem(selection[1]);
                    msg = msg + olItem.Subject
                        + "\n" + olItem.LastModificationTime; 
                }
                else
                {
                    msg = msg + "Multiple Selection Count="
                        + selection.Count;
                }
            }
            else if (control.Context is Outlook.OutlookBarShortcut)
            {
                msg = "Context=OutlookBarShortcut" + "\n";
                Outlook.OutlookBarShortcut shortcut = 
                    control.Context as Outlook.OutlookBarShortcut;
                msg = msg + shortcut.Name;
            }
            else if (control.Context is Outlook.Store)
            {
                msg = "Context=Store" + "\n";
                Outlook.Store store = 
                    control.Context as Outlook.Store;
                msg = msg + store.DisplayName;
            }
            else if (control.Context is Outlook.View)
            {
                msg = "Context=View" + "\n";
                Outlook.View view = 
                    control.Context as Outlook.View;
                msg = msg + view.Name;
            }
            else if (control.Context is Outlook.Inspector)
            {
                msg = "Context=Inspector" + "\n";
                Outlook.Inspector insp = 
                    control.Context as Outlook.Inspector;
                if (insp.AttachmentSelection.Count >= 1)
                {
                    Outlook.AttachmentSelection attachSel =
                        insp.AttachmentSelection;
                    foreach (Outlook.Attachment attach in attachSel)
                    {
                        msg = msg + attach.DisplayName + "\n";
                    }
                }
                else
                {
                    OutlookItem olItem =
                        new OutlookItem(insp.CurrentItem);
                    msg = msg + olItem.Subject;
                }
            }
            else if (control.Context is Outlook.Explorer)
            {
                msg = "Context=Explorer" + "\n";
                Outlook.Explorer explorer = 
                    control.Context as Outlook.Explorer;
                if (explorer.AttachmentSelection.Count >= 1)
                {
                    Outlook.AttachmentSelection attachSel =
                        explorer.AttachmentSelection;
                    foreach (Outlook.Attachment attach in attachSel)
                    {
                        msg = msg + attach.DisplayName + "\n";
                    }
                }
                else
                {
                    Outlook.Selection selection =
                        explorer.Selection;
                    if (selection.Count == 1)
                    {
                        OutlookItem olItem =
                            new OutlookItem(selection[1]);
                        msg = msg + olItem.Subject
                            + "\n" + olItem.LastModificationTime;
                    }
                    else
                    {
                        msg = msg + "Multiple Selection Count="
                            + selection.Count;
                    }
                }
            }
            else if (control.Context is Outlook.NavigationGroup)
            {
                msg = "Context=NavigationGroup" + "\n";
                Outlook.NavigationGroup navGroup = 
                    control.Context as Outlook.NavigationGroup;
                msg = msg + navGroup.Name;
            }
            else if (control.Context is 
                Microsoft.Office.Core.IMsoContactCard)
            {
                msg = "Context=IMsoContactCard" + "\n";
                Office.IMsoContactCard card =
                    control.Context as Office.IMsoContactCard;
                if (card.AddressType == 
                    Office.MsoContactCardAddressType.
                    msoContactCardAddressTypeOutlook)
                {
                    // IMSOContactCard.Address is AddressEntry.ID
                    Outlook.AddressEntry addr =
                        Globals.ThisAddIn.Application.Session.GetAddressEntryFromID(
                        card.Address);
                    if (addr != null)
                    {
                        msg = msg + addr.Name;
                    }
                }
            }
            else if (control.Context is Outlook.NavigationModule)
            {
                msg="Context=NavigationModule";
            }
            else if (control.Context == null)
            {
                msg="Context=Null";
            }
            else
            {
                msg="Context=Unknown";
            }
            MessageBox.Show(msg,
                "RibbonXOutlook14AddinCS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
