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
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;


namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox.UnitTests
{
    static class MockServicesProvider
    {
        #region Fields
        private static GenericMockFactory uiShellFactory;
        private static GenericMockFactory runningDocFactory;
        private static GenericMockFactory windowFrameFactory;
        private static GenericMockFactory qeqsFactory;
        #endregion Fields

        #region Getter functions for the general Mock objects
        #region UiShell Getters
        /// <summary>
        /// Returns an IVsUiShell that does not implement any methods
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetUiShellInstance()
        {
            if (uiShellFactory == null)
            {
                uiShellFactory = new GenericMockFactory("UiShell", new Type[] { typeof(IVsUIShell), typeof(IVsUIShellOpenDocument) });
            }
            BaseMock uiShell = uiShellFactory.GetInstance();
            return uiShell;
        }

        /// <summary>
        /// Get an IVsUiShell that implements SetWaitCursor, SaveDocDataToFile, ShowMessageBox
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetUiShellInstance0()
        {
            BaseMock uiShell = GetUiShellInstance();
            string name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "SetWaitCursor");
            uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(SetWaitCursorCallBack));
            name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "SaveDocDataToFile");
            uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(SaveDocDataToFileCallBack));
            name = string.Format("{0}.{1}", typeof(IVsUIShell).FullName, "ShowMessageBox");
            uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(ShowMessageBoxCallBack));
            name = string.Format("{0}.{1}", typeof(IVsUIShellOpenDocument).FullName, "OpenCopyOfStandardEditor");
            uiShell.AddMethodCallback(name, new EventHandler<CallbackArgs>(OpenCopyOfStandardEditorCallBack));
            return uiShell;
        }
        #endregion

        #region RunningDocumentTable Getters

        /// <summary>
        /// Gets the IVsRunningDocumentTable mock object which implements FindAndLockIncrement, 
        /// NotifyDocumentChanged and UnlockDocument
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetRunningDocTableInstance()
        {
            if (null == runningDocFactory)
            {
                runningDocFactory = new GenericMockFactory("RunningDocumentTable", new Type[] { typeof(IVsRunningDocumentTable) });
            }
            BaseMock runningDoc = runningDocFactory.GetInstance();
            string name = string.Format("{0}.{1}", typeof(IVsRunningDocumentTable).FullName, "FindAndLockDocument");
            runningDoc.AddMethodCallback(name, new EventHandler<CallbackArgs>(FindAndLockDocumentCallBack));
            name = string.Format("{0}.{1}", typeof(IVsRunningDocumentTable).FullName, "NotifyDocumentChanged");
            runningDoc.AddMethodReturnValues(name, new object[]{VSConstants.S_OK});
            name = string.Format("{0}.{1}", typeof(IVsRunningDocumentTable).FullName, "UnlockDocument");
            runningDoc.AddMethodReturnValues(name, new object[]{VSConstants.S_OK});
            return runningDoc;
        }
        #endregion

        #region Window Frame Getters

        /// <summary>
        /// Gets an IVsWindowFrame mock object which implements the SetProperty method
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetWindowFrameInstance()
        {
            if (null == windowFrameFactory)
            {
                windowFrameFactory = new GenericMockFactory("WindowFrame", new Type[] { typeof(IVsWindowFrame) });
            }
            BaseMock windowFrame = windowFrameFactory.GetInstance();
            string name = string.Format("{0}.{1}", typeof(IVsWindowFrame).FullName, "SetProperty");
            windowFrame.AddMethodReturnValues(name, new object[] { VSConstants.S_OK });
            return windowFrame;
        }
        #endregion

        #region QueryEditQuerySave Getters

        /// <summary>
        /// Gets an IVsQueryEditQuerySave2 mock object which implements QuerySaveFile and QueryEditFiles methods
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetQueryEditQuerySaveInstance()
        {
            if (null == qeqsFactory)
            {
                qeqsFactory = new GenericMockFactory("QueryEditQuerySave", new Type[] { typeof(IVsQueryEditQuerySave2) });
            }

            BaseMock qeqs = qeqsFactory.GetInstance();

            string name = string.Format("{0}.{1}", typeof(IVsQueryEditQuerySave2).FullName, "QuerySaveFile");
            qeqs.AddMethodCallback(name, new EventHandler<CallbackArgs>(QuerySaveFileCallBack));
            name = string.Format("{0}.{1}", typeof(IVsQueryEditQuerySave2).FullName, "QueryEditFiles");
            qeqs.AddMethodCallback(name, new EventHandler<CallbackArgs>(QueryEditFilesCallBack));
            return qeqs;
        }
        #endregion
        #endregion Getter functions for the general Mock objects

        #region Callbacks
        private static void SetWaitCursorCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void SaveDocDataToFileCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;

            VSSAVEFLAGS dwSave = (VSSAVEFLAGS)arguments.GetParameter(0);
            IPersistFileFormat editorInstance = (IPersistFileFormat)arguments.GetParameter(1);
            string fileName = (string)arguments.GetParameter(2);

            //Call Save on the EditorInstance depending on the Save Flags
            switch (dwSave)
            {
                case VSSAVEFLAGS.VSSAVE_Save:
                case VSSAVEFLAGS.VSSAVE_SilentSave:
                    editorInstance.Save(fileName, 0, 0);
                    arguments.SetParameter(3, fileName);    // setting pbstrMkDocumentNew
                    arguments.SetParameter(4, 0);           // setting pfSaveCanceled
                    break;
                case VSSAVEFLAGS.VSSAVE_SaveAs:
                    String newFileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
                    editorInstance.Save(newFileName, 1, 0);     //Call Save with new file and remember=1
                    arguments.SetParameter(3, newFileName);     // setting pbstrMkDocumentNew
                    arguments.SetParameter(4, 0);               // setting pfSaveCanceled
                    break;
                case VSSAVEFLAGS.VSSAVE_SaveCopyAs:
                    newFileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(fileName));
                    editorInstance.Save(newFileName, 0, 0);     //Call Save with new file and remember=0
                    arguments.SetParameter(3, newFileName);     // setting pbstrMkDocumentNew
                    arguments.SetParameter(4, 0);               // setting pfSaveCanceled
                    break;
            }
        }
        
        private static void FindAndLockDocumentCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;

            arguments.SetParameter(2, null);        //setting IVsHierarchy
            arguments.SetParameter(3, (uint)1);           //Setting itemID
            arguments.SetParameter(4, IntPtr.Zero); //Setting DocData
            arguments.SetParameter(5, (uint)1);           //Setting docCookie
        }

        private static void QuerySaveFileCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;

            arguments.SetParameter(3, (uint)tagVSQuerySaveResult.QSR_SaveOK);   //set result
        }

        private static void QueryEditFilesCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;

            arguments.SetParameter(5, (uint)tagVSQueryEditResult.QER_EditOK);   //setting result
            arguments.SetParameter(6, (uint)0);                                 //setting outFlags
        }

        private static void ShowMessageBoxCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;

            arguments.SetParameter(10, (int)DialogResult.Yes);
        }

        private static void OpenCopyOfStandardEditorCallBack(object caller, CallbackArgs arguments)
        {
            arguments.ReturnValue = VSConstants.S_OK;
            arguments.SetParameter(2, (IVsWindowFrame)GetWindowFrameInstance());
        }
        #endregion
    }
}