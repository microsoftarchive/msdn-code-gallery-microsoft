using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace RibbonXOutlook14AddinCS
{
    public partial class ThisAddIn
    {
        #region Instance Variables
        Outlook.Application m_Application;             
        Outlook.Explorers m_Explorers;
        Outlook.Inspectors m_Inspectors;
        public stdole.IPictureDisp m_pictdisp = null;
        // List of tracked explorer windows         
        internal static List<OutlookExplorer> m_Windows;
        // List of tracked inspector windows         
        internal static List<OutlookInspector> m_InspectorWindows;
        // Ribbon UI reference
        internal static Office.IRibbonUI m_Ribbon;            
        #endregion

        #region VSTO Startup and Shutdown methods

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Initialize variables
            m_Application = this.Application;
            m_Explorers = m_Application.Explorers;
            m_Inspectors = m_Application.Inspectors;
            m_Windows = new List<OutlookExplorer>();
            m_InspectorWindows = new List<OutlookInspector>();

            // Wire up event handlers to handle multiple Explorer windows
            m_Explorers.NewExplorer += 
                new Outlook.ExplorersEvents_NewExplorerEventHandler(
                    m_Explorers_NewExplorer);
            // Wire up event handlers to handle multiple Inspector windows
            m_Inspectors.NewInspector += 
                new Outlook.InspectorsEvents_NewInspectorEventHandler(
                    m_Inspectors_NewInspector);
            // Add the ActiveExplorer to m_Windows
            Outlook.Explorer expl = m_Application.ActiveExplorer()
                as Outlook.Explorer;
            OutlookExplorer window = new OutlookExplorer(expl);
            m_Windows.Add(window);
            // Hook up event handlers for window
            window.Close += new EventHandler(WrappedWindow_Close);
            window.InvalidateControl += new EventHandler<
                OutlookExplorer.InvalidateEventArgs>(
                WrappedWindow_InvalidateControl);

            // Get IPictureDisp for CurrentUser on startup
            try
            {
                Outlook.AddressEntry addrEntry = 
                    Globals.ThisAddIn.Application.Session.CurrentUser.AddressEntry;
                if (addrEntry.Type == "EX")
                {
                    Outlook.ExchangeUser exchUser = 
                        addrEntry.GetExchangeUser() as Outlook.ExchangeUser;
                    m_pictdisp = exchUser.GetPicture() as stdole.IPictureDisp;
                }
            }
            catch (Exception ex)
            {
                //Write exception to debug window
                Debug.WriteLine(ex.Message);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Shutdown code here
            // Unhook event handlers
            m_Explorers.NewExplorer -= 
                new Outlook.ExplorersEvents_NewExplorerEventHandler(
                    m_Explorers_NewExplorer);
            m_Inspectors.NewInspector -=
                new Outlook.InspectorsEvents_NewInspectorEventHandler(
                m_Inspectors_NewInspector);
           
            // Dereference objects
            m_pictdisp = null;
            m_Explorers = null;
            m_Inspectors = null;
            m_Windows.Clear();
            m_Windows = null;
            m_InspectorWindows.Clear();
            m_InspectorWindows = null;
            m_Ribbon = null;
            m_Application = null;
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new RibbonXAddin(m_Application);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Looks up the window wrapper for a given window object
        /// </summary>
        /// <param name="window">An outlook explorer window</param>
        /// <returns></returns>
        internal static OutlookExplorer FindOutlookExplorer(object window)
        {
            foreach (OutlookExplorer Explorer in m_Windows)
            {
                if (Explorer.Window == window)
                {
                    return Explorer;
                }
            }
            return null;
        }

        /// <summary>
        /// Looks up the window wrapper for a given window object
        /// </summary>
        /// <param name="window">An outlook inspector window</param>
        /// <returns></returns>
        internal static OutlookInspector FindOutlookInspector(object window)
        {
            foreach (OutlookInspector Inspector in m_InspectorWindows)
            {
                if (Inspector.Window == window)
                {
                    return Inspector;
                }
            }
            return null;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// The NewExplorer event fires whenever a new Explorer is displayed. 
        /// </summary>
        /// <param name="Explorer"></param>
        private void m_Explorers_NewExplorer(Outlook.Explorer Explorer)
        {
            try
            {
                // Check to see if this is a new window 
                // we don't already track
                OutlookExplorer existingWindow =
                    FindOutlookExplorer(Explorer);
                // If the m_Windows collection does not 
                // have a window for this Explorer,
                // we should add it to m_Windows
                if (existingWindow == null)
                {
                    OutlookExplorer window = new OutlookExplorer(Explorer);
                    window.Close += new EventHandler(WrappedWindow_Close);
                    window.InvalidateControl += new EventHandler<
                        OutlookExplorer.InvalidateEventArgs>(
                        WrappedWindow_InvalidateControl);
                    m_Windows.Add(window);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// The NewInspector event fires whenever a new Inspector is displayed. 
        /// </summary>
        /// <param name="Explorer"></param>
        void m_Inspectors_NewInspector(Microsoft.Office.Interop.Outlook.Inspector Inspector)
        {
            m_Ribbon.Invalidate();

            try
            {
                // Check to see if this is a new window 
                // we don't already track
                OutlookInspector existingWindow =
                    FindOutlookInspector(Inspector);
                // If the m_InspectorWindows collection does not 
                // have a window for this Inspector,
                // we should add it to m_InspectorWindows
                if (existingWindow == null)
                {
                    OutlookInspector window = new OutlookInspector(Inspector);
                    window.Close += new EventHandler(WrappedInspectorWindow_Close);
                    window.InvalidateControl += new EventHandler<
                        OutlookInspector.InvalidateEventArgs>(
                        WrappedInspectorWindow_InvalidateControl);
                    m_InspectorWindows.Add(window);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        void WrappedInspectorWindow_InvalidateControl(object sender, 
            OutlookInspector.InvalidateEventArgs e)
        {
            if (m_Ribbon != null)
            {
                m_Ribbon.InvalidateControl(e.ControlID);
            }
        }

        void WrappedInspectorWindow_Close(object sender, EventArgs e)
        {
            OutlookInspector window = (OutlookInspector)sender;
            window.Close -= new EventHandler(WrappedInspectorWindow_Close);
            m_InspectorWindows.Remove(window);
        }

        void WrappedWindow_InvalidateControl(object sender,
            OutlookExplorer.InvalidateEventArgs e)
        {
            if (m_Ribbon != null)
            {
                m_Ribbon.InvalidateControl(e.ControlID);
            }
        }

        void WrappedWindow_Close(object sender, EventArgs e)
        {
            OutlookExplorer window = (OutlookExplorer)sender;
            window.Close -= new EventHandler(WrappedWindow_Close);
            m_Windows.Remove(window);
        }
        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
