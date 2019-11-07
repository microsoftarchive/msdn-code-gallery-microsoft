using System;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.CommandTargetRGB
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("0bdb1e08-ed8b-47e8-91b2-e9bd814b4ebb")]
    public class RGBToolWindow : ToolWindowPane, IOleCommandTarget
    {
        private RGBControl control;
        private uint latchedCommand = PkgCmdIDList.cmdidRed;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public RGBToolWindow() :  
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            control = new RGBControl();
            base.Content = control;
        }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            // Force initialization of the control
            control.InitializeComponent();
            CreateToolBar();
        }

        private void CreateToolBar()
        {
            // Retrieve the shell UI object
            IVsUIShell4 shell4 = ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell)) as IVsUIShell4;
            if (shell4 != null)
            {
                // Create the toolbar tray
                IVsToolbarTrayHost host = null;
                if (ErrorHandler.Succeeded(shell4.CreateToolbarTray(this, out host)))
                {
                    IVsUIElement uiElement;
                    Object uiObject, frameworkElement;

                    // Add the toolbar as defined in vsct
                    host.AddToolbar(GuidList.guidCommandTargetRGBCmdSet, PkgCmdIDList.RGBToolbar);
                    host.GetToolbarTray(out uiElement);

                    // Get the WPF element
                    uiElement.GetUIObject(out uiObject);
                    IVsUIWpfElement wpfe = uiObject as IVsUIWpfElement;

                    // Retrieve and set the toolbar tray
                    wpfe.GetFrameworkElement(out frameworkElement);
                    control.SetTray(frameworkElement as ToolBarTray);
                }
            }
        }

        // Handle Exec commands for commands in the guidCommandTargetRGBCmdSet
        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == GuidList.guidCommandTargetRGBCmdSet)
            {
                // Determine the command and call the appropriate method on the RGBControl
                if (nCmdID == PkgCmdIDList.cmdidRed)
                {
                    control.Color = RGBControlColor.Red;
                }
                else if (nCmdID == PkgCmdIDList.cmdidGreen)
                {
                    control.Color = RGBControlColor.Green;
                }
                else if (nCmdID == PkgCmdIDList.cmdidBlue)
                {
                    control.Color = RGBControlColor.Blue;
                }
                else
                {
                    // We don't support any other commands, including cmdidShowToolWindow, because we want
                    // the package to handle the command to create new tool windows
                    return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
                }

                // Set latched command
                latchedCommand = nCmdID;
            }
            else
            {
                // Requested command group is unknown
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_UNKNOWNGROUP;
            }

            return VSConstants.S_OK;
        }

        // Handle QueryStatus for commands in the guidCommandTargetRGBCmdSet
        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            // All commands in guidCommandTargetRGBCmdSet are supported except cmdidShowToolWindow
            if (pguidCmdGroup == GuidList.guidCommandTargetRGBCmdSet)
            {
                for (int i = 0; i < cCmds; i++)
                {
                    if (prgCmds[i].cmdID == PkgCmdIDList.cmdidShowToolWindow)
                    {
                        // We do not support the cmdidShowToolWindow command
                        return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
                    }
                    else
                    {
                        // Mark this command as supported and enabled
                        prgCmds[i].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED | (uint)OLECMDF.OLECMDF_ENABLED;

                        // Display highlight for latched command
                        if (prgCmds[i].cmdID == latchedCommand)
                        {
                            prgCmds[i].cmdf |= (uint)OLECMDF.OLECMDF_LATCHED;
                        }
                    }
                }
            }
            else
            {
                // Requested command group is unknown
                return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_UNKNOWNGROUP;
            }

            return VSConstants.S_OK;
        }
    }
}
