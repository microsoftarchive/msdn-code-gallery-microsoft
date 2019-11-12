/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/
using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using VSConstants = Microsoft.VisualStudio.VSConstants;
using Microsoft.Samples.VisualStudio.IronPython.Project.Library;

namespace Microsoft.Samples.VisualStudio.IronPython.Project
{

    internal class TextLineEventListener : IVsTextLinesEvents, IDisposable {
        private const int defaultDelay = 2000;

        private string fileName;
        private ModuleId fileId;
        private IVsTextLines buffer;
        private bool isDirty;

        private IConnectionPoint connectionPoint;
        private uint connectionCookie;

        public TextLineEventListener(IVsTextLines buffer, string fileName, ModuleId id)
        {
            this.buffer = buffer;
            this.fileId = id;
            this.fileName = fileName;
            IConnectionPointContainer container = buffer as IConnectionPointContainer;
            if (null != container) {
                Guid eventsGuid = typeof(IVsTextLinesEvents).GUID;
                container.FindConnectionPoint(ref eventsGuid, out connectionPoint);
                connectionPoint.Advise(this as IVsTextLinesEvents, out connectionCookie);
            }
        }

        #region Properties
        public ModuleId FileID {
            get { return fileId; }
        }
        public string FileName {
            get { return fileName; }
            set { fileName = value; }
        }
        #endregion

        #region Events
        private EventHandler<HierarchyEventArgs> onFileChanged;
        public event EventHandler<HierarchyEventArgs> OnFileChanged {
            add { onFileChanged += value; }
            remove { onFileChanged -= value; }
        }

        public event TextLineChangeEvent OnFileChangedImmediate;

        #endregion

        #region IVsTextLinesEvents Members
        void IVsTextLinesEvents.OnChangeLineAttributes(int iFirstLine, int iLastLine) {
            // Do Nothing
        }

        void IVsTextLinesEvents.OnChangeLineText(TextLineChange[] pTextLineChange, int fLast) {
            TextLineChangeEvent eh = OnFileChangedImmediate;
            if (null != eh) {
                eh(this, pTextLineChange, fLast);
            }

            isDirty = true;
        }
        #endregion

        #region IDisposable Members
        public void Dispose() {
            if ((null != connectionPoint) && (0 != connectionCookie)) {
                connectionPoint.Unadvise(connectionCookie);
                System.Diagnostics.Debug.WriteLine("\n\tUnadvised from TextLinesEvents\n");
            }
            connectionCookie = 0;
            connectionPoint = null;

            this.buffer = null;
            this.fileId = null;
        }
        #endregion

        #region Idle time processing
        public void OnIdle() {
            if (!isDirty) {
                return;
            }
            if (null != onFileChanged) {
                HierarchyEventArgs args = new HierarchyEventArgs(fileId.ItemID, fileName);
                args.TextBuffer = buffer;
                onFileChanged(fileId.Hierarchy, args);
            }

            isDirty = false;
        }
        #endregion
    }
}
