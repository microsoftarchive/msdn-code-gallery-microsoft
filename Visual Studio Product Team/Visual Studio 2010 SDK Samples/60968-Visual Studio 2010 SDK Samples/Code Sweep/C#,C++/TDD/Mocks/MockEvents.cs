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
using System.Text;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockEvents : EnvDTE.Events
    {
        readonly MockBuildEvents _buildEvents = new MockBuildEvents();

        #region Events Members

        public EnvDTE.BuildEvents BuildEvents
        {
            get { return _buildEvents; }
        }

        public EnvDTE.DTEEvents DTEEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.DebuggerEvents DebuggerEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.FindEvents FindEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object GetObject(string Name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.ProjectItemsEvents MiscFilesEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.SelectionEvents SelectionEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.SolutionEvents SolutionEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.ProjectItemsEvents SolutionItemsEvents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object get_CommandBarEvents(object CommandBarControl)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.CommandEvents get_CommandEvents(string Guid, int ID)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.DocumentEvents get_DocumentEvents(EnvDTE.Document Document)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.OutputWindowEvents get_OutputWindowEvents(string Pane)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.TaskListEvents get_TaskListEvents(string Filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.TextEditorEvents get_TextEditorEvents(EnvDTE.TextDocument TextDocumentFilter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.WindowEvents get_WindowEvents(EnvDTE.Window WindowFilter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
