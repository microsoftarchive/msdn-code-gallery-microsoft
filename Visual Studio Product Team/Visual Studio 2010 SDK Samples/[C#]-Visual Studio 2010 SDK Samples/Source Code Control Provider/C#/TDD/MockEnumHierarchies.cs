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
using Microsoft.VisualStudio;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    class MockEnumHierarchies : IEnumHierarchies
    {
        List<MockIVsProject> _projects;
        int _next = 0;

        public MockEnumHierarchies(IEnumerable<MockIVsProject> projects)
        {
            _projects = new List<MockIVsProject>(projects);
        }

        #region IEnumHierarchies Members

        public int Clone(out IEnumHierarchies ppenum)
        {
            ppenum = new MockEnumHierarchies(_projects);
            return VSConstants.S_OK;
        }

        public int Next(uint celt, IVsHierarchy[] rgelt, out uint pceltFetched)
        {
            pceltFetched = 0;

            while (pceltFetched < celt && _next < _projects.Count)
            {
                rgelt[pceltFetched] = _projects[_next];
                pceltFetched++;
                ++_next;
            }

            if (pceltFetched == celt)
            {
                return VSConstants.S_OK;
            }
            else
            {
                return VSConstants.S_FALSE;
            }
        }

        public int Reset()
        {
            _next = 0;
            return VSConstants.S_OK;
        }

        public int Skip(uint celt)
        {
            IVsHierarchy[] items = new IVsHierarchy[celt];
            uint fetched;

            return Next(celt, items, out fetched);
        }

        #endregion
    }
}
