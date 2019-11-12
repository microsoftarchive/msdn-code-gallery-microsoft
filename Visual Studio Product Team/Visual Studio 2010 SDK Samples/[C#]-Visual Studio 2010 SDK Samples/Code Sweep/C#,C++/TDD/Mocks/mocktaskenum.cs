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

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockTaskEnum : IVsEnumTaskItems
    {
        readonly IList<IVsTaskItem> _items;
        int _next = 0;

        public MockTaskEnum(IList<IVsTaskItem> items)
        {
            _items = items;
        }

        #region IVsEnumTaskItems Members

        public int Clone(out IVsEnumTaskItems ppenum)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Next(uint celt, IVsTaskItem[] rgelt, uint[] pceltFetched)
        {
            for (pceltFetched[0] = 0; celt > 0; --celt, ++pceltFetched[0])
            {
                if (_next >= _items.Count)
                {
                    return VSConstants.S_FALSE;
                }
                rgelt[pceltFetched[0]] = _items[_next++];
            }
            return VSConstants.S_OK;
        }

        public int Reset()
        {
            _next = 0;
            return VSConstants.S_OK;
        }

        public int Skip(uint celt)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
