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

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockDTEProjects : EnvDTE.Projects
    {
        readonly IServiceProvider _serviceProvider;
        readonly Dictionary<string, MockDTEProject> _projects = new Dictionary<string, MockDTEProject>();

        public MockDTEProjects(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #region Projects Members

        public int Count
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.DTE DTE
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            MockSolution solution = _serviceProvider.GetService(typeof(SVsSolution)) as MockSolution;
            foreach (MockIVsProject project in solution.Projects)
            {
                if (!_projects.ContainsKey(project.FullPath))
                {
                    _projects.Add(project.FullPath, new MockDTEProject(project));
                }
                yield return _projects[project.FullPath];
            }
        }

        public EnvDTE.Project Item(object index)
        {
            return Utilities.ListFromEnum(_projects.Values)[(int)index];
        }

        public string Kind
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.DTE Parent
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public EnvDTE.Properties Properties
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
