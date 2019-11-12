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
using Microsoft.Samples.VisualStudio.IDE.ToolWindow;

namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
{
	class MockWindowFrameProvider
	{
		const string propertiesName = "properties";

		private static GenericMockFactory frameFactory = null;
		private static GenericMockFactory trackSelectionFactory = null;

        private static GenericMockFactory FrameFactory
        {
            get
            {
                if (null == frameFactory)
                {
                    Type[] interfaces = new Type[] {
                        typeof(IVsWindowFrame),
                        typeof(IVsWindowFrame2)
                    };
                    frameFactory = new GenericMockFactory("WindowFrame", interfaces);
                }
                return frameFactory;
            }
        }

		/// <summary>
		/// Return a IVsWindowFrame without any special implementation
		/// </summary>
		/// <returns></returns>
		internal static IVsWindowFrame GetBaseFrame()
		{
            IVsWindowFrame frame = (IVsWindowFrame)FrameFactory.GetInstance();
			return frame;
		}

		/// <summary>
		/// Return an IVsWindowFrame implements GetProperty
		/// The peopertiesList will be used too look up PropertyIDs to find values for
		/// requested properties
		/// </summary>
		/// <param name="propertiesList">The dictionary contains PropertyID/Value pairs</param>
		/// <returns></returns>
		internal static IVsWindowFrame GetFrameWithProperties(Dictionary<int, object> propertiesList)
		{
			BaseMock frame = (BaseMock)FrameFactory.GetInstance();
			frame[propertiesName] = propertiesList;
			// Add support for GetProperty
			string name = string.Format("{0}.{1}", typeof(IVsWindowFrame).FullName, "GetProperty");
			frame.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetPropertiesCallBack));
			// Add support for GetGuidProperty
			name = string.Format("{0}.{1}", typeof(IVsWindowFrame).FullName, "GetGuidProperty");
			frame.AddMethodCallback(name, new EventHandler<CallbackArgs>(GetPropertiesCallBack));

			return (IVsWindowFrame)frame;
		}

		internal static GenericMockFactory TrackSelectionFactory
		{
			get
			{
				if (trackSelectionFactory == null)
					trackSelectionFactory = new GenericMockFactory("MockTrackSelection", new Type[] { typeof(ITrackSelection) });
				return trackSelectionFactory;
			}
		}

		#region Callbacks
		private static void GetPropertiesCallBack(object caller, CallbackArgs arguments)
		{
			arguments.ReturnValue = VSConstants.S_OK;

			// Find the corresponding property
			object propertyID = arguments.GetParameter(0);
			Dictionary<int, object> properties = (Dictionary<int, object>)((BaseMock)caller)[propertiesName];
			object propertyValue = null;
			if (properties != null && propertyID != null)
				propertyValue = properties[(int)propertyID];
			// Set the value we ended up with as the return value
			arguments.SetParameter(1, propertyValue);
		}
		#endregion
	}
}
