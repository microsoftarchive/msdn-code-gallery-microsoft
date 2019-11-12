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
    class MockDTE : EnvDTE.DTE
    {
        readonly MockEvents _events = new MockEvents();
        readonly MockDTESolution _solution;
        readonly IServiceProvider _serviceProvider;

        public MockDTE(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _solution = new MockDTESolution(_serviceProvider);
        }

        #region _DTE Members

        public EnvDTE.Document ActiveDocument
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object ActiveSolutionProjects
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Window ActiveWindow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.AddIns AddIns
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.DTE Application
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object CommandBars
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string CommandLineArguments
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Commands Commands
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.ContextAttributes ContextAttributes
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.DTE DTE
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Debugger Debugger
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.vsDisplay DisplayMode
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public EnvDTE.Documents Documents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Edition
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Events Events
        {
            get { return _events; }
        }

        public void ExecuteCommand(string CommandName, string CommandArgs)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string FileName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Find Find
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string FullName
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public object GetObject(string Name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.Globals Globals
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.ItemOperations ItemOperations
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.wizardResult LaunchWizard(string VSZFile, ref object[] ContextParams)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int LocaleID
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Macros Macros
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.DTE MacrosIDE
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Window MainWindow
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.vsIDEMode Mode
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.ObjectExtenders ObjectExtenders
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Window OpenFile(string ViewKind, string FileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Quit()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string RegistryRoot
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string SatelliteDllPath(string Path, string Name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.SelectedItems SelectedItems
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Solution Solution
        {
            get { return _solution; }
        }

        public EnvDTE.SourceControl SourceControl
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.StatusBar StatusBar
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool SuppressUI
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public EnvDTE.UndoContext UndoContext
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool UserControl
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public string Version
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.WindowConfigurations WindowConfigurations
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Windows Windows
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool get_IsOpenFile(string ViewKind, string FileName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public EnvDTE.Properties get_Properties(string Category, string Page)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
