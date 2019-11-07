/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    static class MockIVsMonitorSelectionFactory
    {
        private static GenericMockFactory MonSelFactory = null;

        #region MonSel Getters
        /// <summary>
        /// Returns a monitor selection object that does not implement any methods
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetBaseMonSelInstance()
        {
            if (MonSelFactory == null)
                MonSelFactory = new GenericMockFactory("MonitorSelection", new Type[] { typeof(IVsMonitorSelection), typeof (IVsMultiItemSelect) });
            BaseMock pb = MonSelFactory.GetInstance();
            return pb;
        }

        /// <summary>
        /// Returns a monitor selection object that implement GetCurrentSelection and GetSelectionInfo/GetSelectedItems
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetMonSel()
        {
            BaseMock pb = GetBaseMonSelInstance();
            
            // Add the callback methods
            string name = string.Format("{0}.{1}", typeof(IVsMonitorSelection).FullName, "GetCurrentSelection");
            pb.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetCurrentSelectionCallback));
            name = string.Format("{0}.{1}", typeof(IVsMultiItemSelect).FullName, "GetSelectionInfo");
            pb.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetSelectionInfoCallback));
            name = string.Format("{0}.{1}", typeof(IVsMultiItemSelect).FullName, "GetSelectedItems");
            pb.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetSelectedItemsCallback));

            // Initialize selection data to empty selection
            pb["Selection"] = null;
            return pb;
        }

        #endregion

        #region Callbacks

        private static void GetCurrentSelectionCallback(object caller, CallbackArgs arguments)
        {
            // Read the current selection data
            VSITEMSELECTION[] selection = (VSITEMSELECTION[])((BaseMock)caller)["Selection"];

            // Initialize output parameters for empty selection
            arguments.SetParameter(0, IntPtr.Zero);                 // hierarchyPtr
            arguments.SetParameter(1, VSConstants.VSITEMID_NIL);    // itemid
            arguments.SetParameter(2, null);                        // multiItemSelect 
            arguments.SetParameter(3, IntPtr.Zero);                 // selectionContainer

            if (selection != null)
            {
                if (selection.Length == 1)
                {
                    if (selection[0].pHier != null)
                    {
                        IntPtr ptrHier = Marshal.GetComInterfaceForObject(selection[0].pHier, typeof(IVsHierarchy));
                        arguments.SetParameter(0, ptrHier);                // hierarchyPtr
                    }
                    arguments.SetParameter(1, selection[0].itemid);    // itemid
                }
                else
                {
                    // Multiple selection, return IVsMultiItemSelect interface
                    arguments.SetParameter(1, VSConstants.VSITEMID_SELECTION);    // itemid
                    arguments.SetParameter(2, caller as IVsMultiItemSelect);      // multiItemSelect 
                }
            }

            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void GetSelectionInfoCallback(object caller, CallbackArgs arguments)
        {
            // Read the current selection data
            VSITEMSELECTION[] selection = (VSITEMSELECTION[])((BaseMock)caller)["Selection"];

            // Initialize output parameters for empty selection
            arguments.SetParameter(0, (uint)0);    // numberOfSelectedItems
            arguments.SetParameter(1, (int)1);    // isSingleHierarchyInt

            if (selection != null)
            {
                arguments.SetParameter(0, (uint)selection.Length);    // numberOfSelectedItems
                if (selection.Length > 0)
                {
                    for (int i = 1; i < selection.Length; i++)
                    {
                        if (selection[i].pHier != selection[0].pHier)
                        {
                            arguments.SetParameter(1, (int)0);    // isSingleHierarchyInt
                            break;
                        }
                    }
                }
            }
            
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void GetSelectedItemsCallback(object caller, CallbackArgs arguments)
        {
            // Read the current selection data
            VSITEMSELECTION[] selection = (VSITEMSELECTION[])((BaseMock)caller)["Selection"];

            // Get the arguments
            uint grfGSI = (uint)arguments.GetParameter(0);
            uint cRequestedItems = (uint)arguments.GetParameter(1);
            VSITEMSELECTION[] rgItemSel = (VSITEMSELECTION[])arguments.GetParameter(2);

            if (selection == null && cRequestedItems > 0 ||
                selection.Length < cRequestedItems)
            {
                arguments.ReturnValue = VSConstants.E_INVALIDARG;
                return;
            }

            for (int i = 0; i< cRequestedItems; i++)
            {
                rgItemSel[i].itemid = selection[i].itemid;
                if ((grfGSI & (uint)__VSGSIFLAGS.GSI_fOmitHierPtrs) == 0)
                {
                    rgItemSel[i].pHier = selection[i].pHier;
                }
            }

            arguments.ReturnValue = VSConstants.S_OK;
        }

        #endregion
    }
}
