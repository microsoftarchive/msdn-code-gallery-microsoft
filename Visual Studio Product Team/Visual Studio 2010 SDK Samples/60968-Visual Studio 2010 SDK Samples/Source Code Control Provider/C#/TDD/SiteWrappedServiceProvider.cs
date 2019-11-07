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
using Microsoft.VsSDK.UnitTestLibrary;
using System.ComponentModel;
using Microsoft.VisualStudio;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    class SiteWrappedServiceProvider : ISite
    {
        private OleServiceProvider _sp;

        public SiteWrappedServiceProvider(OleServiceProvider sp)
        {
            _sp = sp;
        }

        //Support the ISite interface.
        public virtual IComponent Component { get { return null; } }
        public virtual IContainer Container { get { return null; } }
        public virtual bool DesignMode { get { return false; } }
        public virtual string Name
        {
            get { return "SiteWrappedServiceProvider"; }
            set { ;}
        }

        //Support the IServiceProvider interface.
        public virtual object GetService(Type serviceType)
        {
            // Query IUnknown from the service provider
            IntPtr ppvObject = (IntPtr)0;
            Guid guidService = serviceType.GUID;
            Guid guidIntf = VSConstants.IID_IUnknown;
            int iResult = _sp.QueryService(ref guidService, ref guidIntf, out ppvObject);
            if (iResult != VSConstants.S_OK)
            {
                return null;
            }

            // Return the object to the caller
            return System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(ppvObject);
        }
    };
}
