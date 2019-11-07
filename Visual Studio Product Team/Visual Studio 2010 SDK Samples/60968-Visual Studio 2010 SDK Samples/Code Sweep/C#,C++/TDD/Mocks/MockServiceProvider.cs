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
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockServiceProvider : IServiceProvider
    {
        readonly MockTaskList _taskList = new MockTaskList();
        readonly MockShell _uiShell = new MockShell();
        readonly MockStatusBar _statusBar = new MockStatusBar();
        readonly MockDTE _dte;
        readonly MockSolution _solution = new MockSolution();
        readonly MockRDT _rdt = new MockRDT();
        readonly MockUIShellOpenDocument _uiShellOpenDoc = new MockUIShellOpenDocument();
        readonly MockTextManager _textMgr = new MockTextManager();
        readonly MockWebBrowsingService _webBrowser = new MockWebBrowsingService();
        readonly OleMenuCommandService _menuService;

        public MockServiceProvider()
        {
            _menuService = new OleMenuCommandService(this);
            _dte = new MockDTE(this);
        }

        public MockTaskList TaskList
        {
            get { return _taskList; }
        }

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (typeof(SVsTaskList).IsEquivalentTo(serviceType))
            {
                return _taskList;
            }
            else if (typeof(SVsUIShell).IsEquivalentTo(serviceType))
            {
                return _uiShell;
            }
            else if (typeof(SVsStatusbar).IsEquivalentTo(serviceType))
            {
                return _statusBar;
            }
            else if (typeof(DTE).IsEquivalentTo(serviceType))
            {
                return _dte;
            }
            else if (typeof(SVsSolution).IsEquivalentTo(serviceType))
            {
                return _solution;
            }
            else if (typeof(SVsRunningDocumentTable).IsEquivalentTo(serviceType))
            {
                return _rdt;
            }
            else if (typeof(SVsUIShellOpenDocument).IsEquivalentTo(serviceType))
            {
                return _uiShellOpenDoc;
            }
            else if (typeof(SVsTextManager).IsEquivalentTo(serviceType))
            {
                return _textMgr;
            }
            else if (typeof(SVsWebBrowsingService).IsEquivalentTo(serviceType))
            {
                return _webBrowser;
            }
            else if (typeof(IMenuCommandService).IsEquivalentTo(serviceType))
            {
                return _menuService;
            }
            else if (typeof(ISelectionService).IsEquivalentTo(serviceType))
            {
                return null;
            }
            else if (typeof(IDesignerHost).IsEquivalentTo(serviceType))
            {
                return null;
            }
            else
            {
                Debug.Fail("Service " + serviceType + " not found.");
                return null;
            }
        }

        #endregion
    }
}
