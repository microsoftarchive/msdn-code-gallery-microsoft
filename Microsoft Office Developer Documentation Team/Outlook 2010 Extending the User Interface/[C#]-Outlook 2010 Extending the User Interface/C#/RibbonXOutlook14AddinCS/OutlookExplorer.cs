using System;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace RibbonXOutlook14AddinCS
{
    /// <summary>
    /// This class tracks the state of an Outlook Explorer window for your
    /// add-in and ensures that what happens in this window is handled correctly.
    /// </summary>
    class OutlookExplorer
    {
        #region Instance Variables

        private Outlook.Explorer m_Window;   // wrapped window object

        #endregion


        #region Events

        public event EventHandler Close;
        public event EventHandler<InvalidateEventArgs> InvalidateControl;

        #endregion


        #region Constructor

        /// <summary>
        /// Create a new instance of the tracking class for a particular explorer 
        /// </summary>
        /// <param name="explorer">A new explorer window to track</param>
        ///<remarks></remarks>
        public OutlookExplorer(Outlook.Explorer explorer)
        {
            m_Window = explorer;

            // Hookup Close event
            ((Outlook.ExplorerEvents_Event)explorer).Close +=
                new Outlook.ExplorerEvents_CloseEventHandler(
                OutlookExplorerWindow_Close);

            // Hookup SelectionChange event
            m_Window.SelectionChange += 
                new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(
                    m_Window_SelectionChange);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler for SelectionChange event
        /// </summary>
        private void m_Window_SelectionChange()
        {
            RaiseInvalidateControl("MyTab");
        }

        /// <summary>
        /// Event Handler for Close event.
        /// </summary>
        private void OutlookExplorerWindow_Close()
        {
            // Unhook explorer-level events

            m_Window.SelectionChange -=
                new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(
                m_Window_SelectionChange);

            ((Outlook.ExplorerEvents_Event)m_Window).Close -=
                new Outlook.ExplorerEvents_CloseEventHandler(
                OutlookExplorerWindow_Close);

            // Raise the OutlookExplorer close event
            if (Close != null)
            {
                Close(this, EventArgs.Empty);
            }

            m_Window = null;
        }

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
        /// The actual Outlook explorer window wrapped by this instance
        /// </summary>
        internal Outlook.Explorer Window
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
