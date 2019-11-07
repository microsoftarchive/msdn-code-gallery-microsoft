using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;

#region Readme and Usage
/// <summary>
/// Helper class to access common properties of Outlook Items.  This class uses
/// reflection to call the common members of all Outlook items.  Because of the use 
/// of reflection, all calls through this class will be significantly slower than
/// executing the same call against a strongly typed object.
/// </summary>
/// <remarks>
/// Uses the IDispatch interface of the Outlook object model 
/// to access the common properties of
/// Outlook items regardless of the actual item type.  
/// This prevents having to test for and cast to 
/// a specific object in order to access common properties.
///
/// Usage example:
/// 
///    OutlookItem myItem = new OutlookItem(olApp.ActiveExplorer.Selection[1]);
///    Debug.WriteLine(myItem.EntryID);
#endregion
namespace RibbonXOutlook14AddinCS
{
    class OutlookItem
    {
        private object m_item;  // the wrapped Outlook item
        private Type m_type;  // type for the Outlook item 
        private object[] m_args;  // dummy argument array
        private System.Type m_typeOlObjectClass;

        #region OutlookItem Constants

        private const string OlActions = "Actions";
        private const string OlApplication = "Application";
        private const string OlAttachments = "Attachments";
        private const string OlBillingInformation = "BillingInformation";
        private const string OlBody = "Body";
        private const string OlCategories = "Categories";
        private const string OlClass = "Class";
        private const string OlClose = "Close";
        private const string OlCompanies = "Companies";
        private const string OlConversationIndex = "ConversationIndex";
        private const string OlConversationTopic = "ConversationTopic";
        private const string OlCopy = "Copy";
        private const string OlCreationTime = "CreationTime";
        private const string OlDisplay = "Display";
        private const string OlDownloadState = "DownloadState";
        private const string OlEntryID = "EntryID";
        private const string OlFormDescription = "FormDescription";
        private const string OlGetInspector = "GetInspector";
        private const string OlImportance = "Importance";
        private const string OlIsConflict = "IsConflict";
        private const string OlItemProperties = "ItemProperties";
        private const string OlLastModificationTime = "LastModificationTime";
        private const string OlLinks = "Links";
        private const string OlMarkForDownload = "MarkForDownload";
        private const string OlMessageClass = "MessageClass";
        private const string OlMileage = "Mileage";
        private const string OlMove = "Move";
        private const string OlNoAging = "NoAging";
        private const string OlOutlookInternalVersion = "OutlookInternalVersion";
        private const string OlOutlookVersion = "OutlookVersion";
        private const string OlParent = "Parent";
        private const string OlPrintOut = "PrintOut";
        private const string OlPropertyAccessor = "PropertyAccessor";
        private const string OlSave = "Save";
        private const string OlSaveAs = "SaveAs";
        private const string OlSaved = "Saved";
        private const string OlSensitivity = "Sensitivity";
        private const string OlSession = "Session";
        private const string OlShowCategoriesDialog = "ShowCategoriesDialog";
        private const string OlSize = "Size";
        private const string OlSubject = "Subject";
        private const string OlUnRead = "UnRead";
        private const string OlUserProperties = "UserProperties";
        #endregion

        #region Constructor
        public OutlookItem(object item)
        {
            m_item = item;
            m_type = m_item.GetType();
            m_args = new Object[] { };
        }
        #endregion

        #region Public Methods and Properties
        public Outlook.Actions Actions
        {
            get
            {
                return this.GetPropertyValue(OlActions) as Outlook.Actions;
            }
        }

        public Outlook.Application Application
        {
            get
            {
                return this.GetPropertyValue(OlApplication) as Outlook.Application;
            }
        }

        public Outlook.Attachments Attachments
        {
            get
            {
                return this.GetPropertyValue(OlAttachments) as Outlook.Attachments;
            }
        }

        public string BillingInformation
        {
            get
            {
                return this.GetPropertyValue(OlBillingInformation).ToString();
            }
            set
            {
                SetPropertyValue(OlBillingInformation, value);
            }
        }

        public string Body
        {
            get
            {
                return this.GetPropertyValue(OlBody).ToString();
            }
            set
            {
                SetPropertyValue(OlBody, value);
            }
        }

        public string Categories
        {
            get
            {
                return this.GetPropertyValue(OlCategories).ToString();
            }
            set
            {
                SetPropertyValue(OlCategories, value);
            }
        }


        public void Close(Outlook.OlInspectorClose SaveMode)
        {
            object[] MyArgs = { SaveMode };
            this.CallMethod(OlClose);
        }

        public string Companies
        {
            get
            {
                return this.GetPropertyValue(OlCompanies).ToString();
            }
            set
            {
                SetPropertyValue(OlCompanies, value);
            }
        }

        public Outlook.OlObjectClass Class
        {
            get
            {
                if (m_typeOlObjectClass == null)
                {
                    // Note: instantiate dummy ObjectClass enumeration to get type.
                    //       type = System.Type.GetType("Outlook.OlObjectClass") doesn't seem to work
                    Outlook.OlObjectClass objClass = Outlook.OlObjectClass.olAction;
                    m_typeOlObjectClass = objClass.GetType();
                }
                return (Outlook.OlObjectClass)System.Enum.ToObject(m_typeOlObjectClass, this.GetPropertyValue(OlClass));
            }
        }

        public string ConversationIndex
        {
            get
            {
                return this.GetPropertyValue(OlConversationIndex).ToString();
            }
        }

        public string ConversationTopic
        {
            get
            {
                return this.GetPropertyValue(OlConversationTopic).ToString();
            }
        }

        public object Copy()
        {
            return (this.CallMethod(OlCopy));
        }

        public System.DateTime CreationTime
        {
            get
            {
                return (System.DateTime)this.GetPropertyValue(OlCreationTime);
            }
        }

        public void Display()
        {
            this.CallMethod(OlDisplay);
        }

        public Outlook.OlDownloadState DownloadState
        {
            get
            {
                return (Outlook.OlDownloadState)this.GetPropertyValue(OlDownloadState);
            }
        }

        public string EntryID
        {
            get
            {
                return this.GetPropertyValue(OlEntryID).ToString();
            }
        }

        public Outlook.FormDescription FormDescription
        {
            get
            {
                return (Outlook.FormDescription)this.GetPropertyValue(OlFormDescription);
            }
        }


        public Object InnerObject
        {
            get
            {
                return this.m_item;
            }
        }

        public Outlook.Inspector GetInspector
        {
            get
            {
                return this.GetPropertyValue(OlGetInspector) as Outlook.Inspector;
            }
        }

        public Outlook.OlImportance Importance
        {
            get
            {
                return (Outlook.OlImportance)this.GetPropertyValue(OlImportance);
            }
            set
            {
                SetPropertyValue(OlImportance, value);
            }
        }

        public bool IsConflict
        {
            get
            {
                return (bool)this.GetPropertyValue(OlIsConflict);
            }
        }

        public Outlook.ItemProperties ItemProperties
        {
            get
            {
                return (Outlook.ItemProperties)this.GetPropertyValue(OlItemProperties);
            }
        }

        public System.DateTime LastModificationTime
        {
            get
            {
                return (System.DateTime)this.GetPropertyValue(OlLastModificationTime);
            }
        }

        public Outlook.Links Links
        {
            get
            {
                return this.GetPropertyValue(OlLinks) as Outlook.Links;
            }
        }

        public Outlook.OlRemoteStatus MarkForDownload
        {
            get
            {
                return (Outlook.OlRemoteStatus)this.GetPropertyValue(OlMarkForDownload);
            }
            set
            {
                SetPropertyValue(OlMarkForDownload, value);
            }
        }

        public string MessageClass
        {
            get
            {
                return this.GetPropertyValue(OlMessageClass).ToString();
            }
            set
            {
                SetPropertyValue(OlMessageClass, value);
            }
        }

        public string Mileage
        {
            get
            {
                return this.GetPropertyValue(OlMileage).ToString();
            }
            set
            {
                SetPropertyValue(OlMileage, value);
            }
        }

        public object Move(Outlook.Folder DestinationFolder)
        {
            object[] myArgs = { DestinationFolder };
            return this.CallMethod(OlMove, myArgs);
        }

        public bool NoAging
        {
            get
            {
                return (bool)this.GetPropertyValue(OlNoAging);
            }
            set
            {
                SetPropertyValue(OlNoAging, value);
            }
        }

        public long OutlookInternalVersion
        {
            get
            {
                return (long)this.GetPropertyValue(OlOutlookInternalVersion);
            }
        }

        public string OutlookVersion
        {
            get
            {
                return this.GetPropertyValue(OlOutlookVersion).ToString();
            }
        }

        public Outlook.Folder Parent
        {
            get
            {
                return this.GetPropertyValue(OlParent) as Outlook.Folder;
            }
        }

        public Outlook.PropertyAccessor PropertyAccessor
        {
            get
            {
                return this.GetPropertyValue(OlPropertyAccessor) as Outlook.PropertyAccessor;
            }
        }

        public void PrintOut()
        {
            this.CallMethod(OlPrintOut);
        }

        public void Save()
        {
            this.CallMethod(OlSave);
        }

        public void SaveAs(string path, Outlook.OlSaveAsType type)
        {
            object[] myArgs = { path, type };
            this.CallMethod(OlSaveAs, myArgs);
        }

        public bool Saved
        {
            get
            {
                return (bool)this.GetPropertyValue(OlSaved);
            }
        }

        public Outlook.OlSensitivity Sensitivity
        {
            get
            {
                return (Outlook.OlSensitivity)this.GetPropertyValue(OlSensitivity);
            }
            set
            {
                SetPropertyValue(OlSensitivity, value);
            }
        }

        public Outlook.NameSpace Session
        {
            get
            {
                return this.GetPropertyValue(OlSession) as Outlook.NameSpace;
            }
        }

        public void ShowCategoriesDialog()
        {
            this.CallMethod(OlShowCategoriesDialog);
        }

        public long Size
        {
            get
            {
                return (long)this.GetPropertyValue(OlSize);
            }
        }

        public string Subject
        {
            get
            {
                return this.GetPropertyValue(OlSubject).ToString();
            }
            set
            {
                SetPropertyValue(OlSubject, value);
            }
        }

        public bool UnRead
        {
            get
            {
                return (bool)this.GetPropertyValue(OlUnRead);
            }
            set
            {
                SetPropertyValue(OlUnRead, value);
            }
        }

        public Outlook.UserProperties UserProperties
        {
            get
            {
                return this.GetPropertyValue(OlUserProperties) as Outlook.UserProperties;
            }
        }

        #endregion

        #region Private Helper Functions
        private object GetPropertyValue(string propertyName)
        {
            try
            {
                // An invalid property name exception is propagated to client
                return m_type.InvokeMember(
                    propertyName,
                    BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty,
                    null,
                    m_item,
                    m_args);
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(
                    string.Format(
                    "OutlookItem: GetPropertyValue for {0} Exception: {1} ",
                    propertyName, ex.Message));
                throw;
            }
        }

        private void SetPropertyValue(string propertyName, object propertyValue)
        {
            try
            {
                m_type.InvokeMember(
                    propertyName,
                    BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty,
                    null,
                    m_item,
                    new object[] { propertyValue });
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(
                   string.Format(
                   "OutlookItem: SetPropertyValue for {0} Exception: {1} ",
                   propertyName, ex.Message));
                throw;
            }
        }

        private object CallMethod(string methodName)
        {
            try
            {
                // An invalid property name exception is propagated to client
                return m_type.InvokeMember(
                    methodName,
                    BindingFlags.Public | BindingFlags.InvokeMethod,
                    null,
                    m_item,
                    m_args);
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(
                    string.Format(
                    "OutlookItem: CallMethod for {0} Exception: {1} ",
                    methodName, ex.Message));
                throw;
            }
        }

        private object CallMethod(string methodName, object[] args)
        {
            try
            {
                // An invalid property name exception is propagated to client
                return m_type.InvokeMember(
                    methodName,
                    BindingFlags.Public | BindingFlags.InvokeMethod,
                    null,
                    m_item,
                    args);
            }
            catch (SystemException ex)
            {
                Debug.WriteLine(
                    string.Format(
                    "OutlookItem: CallMethod for {0} Exception: {1} ",
                    methodName, ex.Message));
                throw;
            }
        }
        #endregion

    }
}