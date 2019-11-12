using System;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace RibbonXOutlook14AddinCS
{

    /// <summary>
    /// This class tracks the state of an Outlook Inspector window for your
    /// add-in and ensures that what happens in this window is handled correctly.
    /// </summary>
    class OutlookInspector
    {
        #region Instance Variables

        private Outlook.Inspector m_Window;             // wrapped window object
        // Use these instance variables to handle item-level events
        private Outlook.MailItem m_Mail;                // wrapped MailItem
        private Outlook.AppointmentItem m_Appointment;  // wrapped AppointmentItem
        private Outlook.ContactItem m_Contact;          // wrapped ContactItem
        private Outlook.ContactItem m_Task;             // wrapped TaskItem
        // Define other class-level item instance variables as needed

        #endregion

        #region Events

        public event EventHandler Close;
        public event EventHandler<InvalidateEventArgs> InvalidateControl;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of the tracking class for a particular 
        /// inspector and custom task pane.
        /// </summary>
        /// <param name="inspector">A new inspector window to track</param>
        ///<remarks></remarks>
        public OutlookInspector(Outlook.Inspector inspector)
        {
            m_Window = inspector;

            // Hookup the close event
            ((Outlook.InspectorEvents_Event)inspector).Close +=
                new Outlook.InspectorEvents_CloseEventHandler(
                OutlookInspectorWindow_Close);

            // Hookup item-level events as needed
            // For example, the following code hooks up PropertyChange
            // event for a ContactItem
            //OutlookItem olItem = new OutlookItem(inspector.CurrentItem);
            //if(olItem.Class==Outlook.OlObjectClass.olContact)
            //{
            //    m_Contact = olItem.InnerObject as Outlook.ContactItem;
            //    m_Contact.PropertyChange +=
            //        new Outlook.ItemEvents_10_PropertyChangeEventHandler(
            //        m_Contact_PropertyChange);
            //}

        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler for the inspector close event.
        /// </summary>
        private void OutlookInspectorWindow_Close()
        {
            // Unhook events from any item-level instance variables
            //m_Contact.PropertyChange -= 
            //    Outlook.ItemEvents_10_PropertyChangeEventHandler(
            //    m_Contact_PropertyChange);

            // Unhook events from the window
            ((Outlook.InspectorEvents_Event)m_Window).Close -=
                new Outlook.InspectorEvents_CloseEventHandler(
                OutlookInspectorWindow_Close);

            // Raise the OutlookInspector close event
            if (Close != null)
            {
                Close(this, EventArgs.Empty);
            }

            // Unhook any item-level instance variables
            //m_Contact = null;
            m_Window = null;
        }


        //void  m_Contact_PropertyChange(string Name)
        //{
        //    // Implement PropertyChange here
        //}
        #endregion

        #region Methods
        private void RaiseInvalidateControl(string controlID)
        {
            if (InvalidateControl != null)
                InvalidateControl(this, new InvalidateEventArgs(controlID));
        }
        #endregion

        #region Properties

        /// <summary>
        /// The actual Outlook inspector window wrapped by this instance
        /// </summary>
        internal Outlook.Inspector Window
        {
            get { return m_Window; }
        }

        #endregion

        #region Helper Class
        public class InvalidateEventArgs : EventArgs
        {
            private string m_ControlID;

            public InvalidateEventArgs(string controlID)
            {
                m_ControlID = controlID;
            }

            public string ControlID
            {
                get { return m_ControlID; }
            }
        }
        #endregion
    }
}
