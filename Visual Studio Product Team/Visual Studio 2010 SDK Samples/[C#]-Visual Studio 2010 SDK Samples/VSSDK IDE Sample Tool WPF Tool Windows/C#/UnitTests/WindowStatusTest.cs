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
using Microsoft.Samples.VisualStudio.IDE.ToolWindow;

namespace Microsoft.Samples.VisualStudio.IDE.ToolWindow.UnitTests
{
    [TestClass()]
    public class WindowStatusTest
    {
        int count;

        [TestMethod()]
        public void OnShow()
        {
            WindowStatus windowStatus = GetWindowStatusInstance();

            int show = 1;
            int hr = windowStatus.OnShow(show);
            Assert.AreEqual(0, hr, "Method call failed");
            Assert.AreEqual(0, count, "Incorrect number of events generated");
        }

        [TestMethod()]
        public void OnClose()
        {
            WindowStatus windowStatus = GetWindowStatusInstance();

            uint saveOption = 0x100;
            int hr = windowStatus.OnClose(ref saveOption);
            Assert.AreEqual(0, hr, "Method call failed");
            Assert.AreEqual((uint)0x100, saveOption, "Save Option changed");
            Assert.AreEqual(0, count, "Incorrect number of events generated");
        }

        [TestMethod()]
        public void OnMove()
        {
            WindowStatus windowStatus = GetWindowStatusInstance();

            int x = 1, y = 2, w = 3, h = 4;
            int hr = windowStatus.OnMove(x, y, w, h);
            Assert.AreEqual(0, hr, "Method call failed");
            Assert.AreEqual(x, windowStatus.X, "Failed to set X");
            Assert.AreEqual(y, windowStatus.Y, "Failed to set Y");
            Assert.AreEqual(w, windowStatus.Width, "Failed to set W");
            Assert.AreEqual(h, windowStatus.Height, "Failed to set H");
            Assert.AreEqual(1, count, "Incorrect number of events generated");
        }

        [TestMethod()]
        public void OnSize()
        {
            WindowStatus windowStatus = GetWindowStatusInstance();

            int x = 7, y = 8, w = 9, h = 10;
            int hr = windowStatus.OnSize(x, y, w, h);
            Assert.AreEqual(0, hr, "Method call failed");
            Assert.AreEqual(x, windowStatus.X, "Failed to set X");
            Assert.AreEqual(y, windowStatus.Y, "Failed to set Y");
            Assert.AreEqual(w, windowStatus.Width, "Failed to set W");
            Assert.AreEqual(h, windowStatus.Height, "Failed to set H");
            Assert.AreEqual(1, count, "Incorrect number of events generated");
        }

        [TestMethod()]
        public void OnDockableChange()
        {
            WindowStatus windowStatus = GetWindowStatusInstance();

            int x = 71, y = 82, w = 93, h = 104, docked = 1;
            int hr = windowStatus.OnDockableChange(docked, x, y, w, h);
            Assert.AreEqual(0, hr, "Method call failed");
            Assert.AreEqual(x, windowStatus.X, "Failed to set X");
            Assert.AreEqual(y, windowStatus.Y, "Failed to set Y");
            Assert.AreEqual(w, windowStatus.Width, "Failed to set W");
            Assert.AreEqual(h, windowStatus.Height, "Failed to set H");
            Assert.AreEqual(true, windowStatus.IsDockable, "Failed to set Docked");
            Assert.AreEqual(1, count, "Incorrect number of events generated");
        }

        private WindowStatus GetWindowStatusInstance()
        {
            WindowStatus windowStatus = new WindowStatus(null, null);
            this.count = 0;
            windowStatus.StatusChange += new EventHandler<EventArgs>(this.StatusChanged);

            return windowStatus;
        }

        private void StatusChanged(object sender, EventArgs arguments)
        {
            ++count;
        }
    }
}
