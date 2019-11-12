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
    class MockBuildEvents : EnvDTE.BuildEvents
    {
        public int OnBuildBeginSubscriberCount
        {
            get { return (OnBuildBegin == null) ? 0 : OnBuildBegin.GetInvocationList().Length; }
        }

        public void FireOnBuildBegin(EnvDTE.vsBuildScope scope, EnvDTE.vsBuildAction action)
        {
            if (OnBuildBegin != null)
            {
                OnBuildBegin(scope, action);
            }
        }

        public void FireOnBuildDone(EnvDTE.vsBuildScope scope, EnvDTE.vsBuildAction action)
        {
            if (OnBuildDone != null)
            {
                OnBuildDone(scope, action);
            }
        }

        #region _dispBuildEvents_Event Members

        public event EnvDTE._dispBuildEvents_OnBuildBeginEventHandler OnBuildBegin;

        public event EnvDTE._dispBuildEvents_OnBuildDoneEventHandler OnBuildDone;

        // These are unused currently, but they must exist to satisfy the interface contract.
        // Disable the warning for unused variables.
#pragma warning disable 67
        public event EnvDTE._dispBuildEvents_OnBuildProjConfigBeginEventHandler OnBuildProjConfigBegin;

        public event EnvDTE._dispBuildEvents_OnBuildProjConfigDoneEventHandler OnBuildProjConfigDone;
#pragma warning restore 67

        #endregion
    }
}
