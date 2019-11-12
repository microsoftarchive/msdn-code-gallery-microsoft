/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.IDE.ToolWindow;

namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
{
	[TestClass()]
	public class WindowListTest
	{
		[TestMethod()]
		public void WindowsProperties()
		{
			Assembly targetAssembly = typeof(PackageToolWindow).Assembly;
			string typeName = String.Format("{0}.{1}", typeof(PackageToolWindow).Namespace, "WindowList");
			Type targetType = targetAssembly.GetType(typeName);
			object instance = Activator.CreateInstance(targetType);

			// Provide our own list of mock frame
			IList<IVsWindowFrame> frameList = new List<IVsWindowFrame>();
			Dictionary<int, object> properties = GetProperties(0);
			frameList.Add(MockWindowFrameProvider.GetFrameWithProperties(properties));
			properties = GetProperties(1);
			frameList.Add(MockWindowFrameProvider.GetFrameWithProperties(properties));
			// Get our instance to use that list
			FieldInfo field = targetType.GetField("framesList", BindingFlags.NonPublic | BindingFlags.Instance);
			field.SetValue(instance, frameList);

			// We are ready to run our test so get the property
			PropertyInfo windowsProperties = targetType.GetProperty("WindowsProperties", BindingFlags.NonPublic | BindingFlags.Instance);
			object result = windowsProperties.GetValue(instance, null);

            Assert.IsNotNull(result, "WindowsProperties returned null");
            Assert.AreEqual(typeof(ArrayList), result.GetType(), "Incorrect Type returned");
			ArrayList array = (ArrayList)result;
            Assert.AreEqual(2, array.Count, "Number of windows is incorrect");
		}

		private static Dictionary<int, object> GetProperties(int index)
		{
			Dictionary<int, object> properties = new Dictionary<int, object>();
			properties.Add((int)__VSFPROPID.VSFPROPID_Caption, "Tool Window " + index.ToString());
			properties.Add((int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot, Guid.NewGuid());
			return properties;
		}
	}
}
