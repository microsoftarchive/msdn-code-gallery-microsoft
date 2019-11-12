/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

// SccProviderService.cs : Implementation of Sample Source Control Provider Service
//

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider
{
    [Guid("ADC98052-1000-41D1-A6C3-704E6C1A3DE2")]
    public class SccProviderService : IVsSccProvider
    {
        private bool _active = false;
        private BasicSccProvider _sccProvider = null;

        public SccProviderService(BasicSccProvider sccProvider)
        {
            _sccProvider = sccProvider;
        }

        /// <summary>
        /// Returns whether this source control provider is the active scc provider.
        /// </summary>
        public bool Active
        {
            get { return _active; }
        }

        //--------------------------------------------------------------------------------
        // IVsSccProvider specific interfaces
        //--------------------------------------------------------------------------------
        
        // Called by the scc manager when the provider is activated. 
        // Make visible and enable if necessary scc related menu commands
        public int SetActive()
        {
            Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Provider set active"));

            _active = true;
            _sccProvider.OnActiveStateChange();

            return VSConstants.S_OK;
        }

        // Called by the scc manager when the provider is deactivated. 
        // Hides and disable scc related menu commands
        public int SetInactive()
        {
            Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Provider set inactive"));

            _active = false;
            _sccProvider.OnActiveStateChange();

            return VSConstants.S_OK;
        }

        public int AnyItemsUnderSourceControl(out int pfResult)
        {
            pfResult = 0;
            return VSConstants.S_OK;
        }
    }
}