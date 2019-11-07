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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    class MockTrackProjectDocumentsProvider
    {
        private static GenericMockFactory tpdFactory = null;

        #region TPD Getters

        /// <summary>
        /// Return a IVsTrackProjectDocuments2 without any special implementation
		/// </summary>
		/// <returns></returns>
        internal static BaseMock GetBaseTrackProjectDocuments()
		{
            if (tpdFactory == null)
                tpdFactory = new GenericMockFactory("TPDProvider", new Type[] { typeof(IVsTrackProjectDocuments2) });

            BaseMock tpd = tpdFactory.GetInstance();
            return tpd;
		}

        /// <summary>
        /// Get an IVsTrackProjectDocuments2 that implement AdviseTrackProjectDocumentsEvents and UnadviseTrackProjectDocumentsEvents.
        /// </summary>
        /// <returns></returns>
        internal static BaseMock GetTrackProjectDocuments()
        {
            BaseMock tpd = GetBaseTrackProjectDocuments();
            string name = string.Format("{0}.{1}", typeof(IVsTrackProjectDocuments2).FullName, "AdviseTrackProjectDocumentsEvents");
            tpd.AddMethodCallback(name, new EventHandler<CallbackArgs>(AdviseTrackProjectDocumentsEventsCallBack));
            name = string.Format("{0}.{1}", typeof(IVsTrackProjectDocuments2).FullName, "UnadviseTrackProjectDocumentsEvents");
            tpd.AddMethodCallback(name, new EventHandler<CallbackArgs>(UnadviseTrackProjectDocumentsEventsCallBack));
            return tpd;
        }
        
        #endregion

		#region Callbacks

        private static void AdviseTrackProjectDocumentsEventsCallBack(object caller, CallbackArgs arguments)
        {
            uint tpdTrackProjectDocumentsCookie = 1;
            arguments.SetParameter(1, tpdTrackProjectDocumentsCookie);
            arguments.ReturnValue = VSConstants.S_OK;
        }

        private static void UnadviseTrackProjectDocumentsEventsCallBack(object caller, CallbackArgs arguments)
        {
            Assert.AreEqual((uint)1, (uint)arguments.GetParameter(0), "Incorrect cookie unregistered");
            arguments.ReturnValue = VSConstants.S_OK;
        }

    	#endregion
	}
}
