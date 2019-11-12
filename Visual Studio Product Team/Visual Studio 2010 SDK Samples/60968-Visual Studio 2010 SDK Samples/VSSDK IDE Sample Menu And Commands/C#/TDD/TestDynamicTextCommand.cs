/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.ComponentModel.Design;
using System.Reflection;

using Microsoft.VisualStudio.Shell;
using Microsoft.Samples.VisualStudio.MenuCommands;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Samples.VisualStudio.MenuAndCommands.UnitTest {

    internal class DynamicTextCommandWrapper {
        
        // ============================================================================================
        //      Static members
        private static ConstructorInfo dynamicTextConstructor;
        private static Type dynamicTextType;
        public static OleMenuCommand GetDynamicTextCommandInstance(CommandID id, string text) {
            // Check if we have found the constructor before.
            if (dynamicTextConstructor == null) {
                Assembly asm = Assembly.GetAssembly(typeof(MenuCommandsPackage));
                foreach (Type t in asm.GetTypes()) {
                    if (t.FullName.EndsWith("DynamicTextCommand")) {
                        dynamicTextType = t;
                        break;
                    }
                }
                Assert.IsNotNull(dynamicTextType, "Can not get the type for DynamicTextCommand.");
                dynamicTextConstructor = dynamicTextType.GetConstructor(new Type[] { typeof(CommandID), typeof(string) });
            }

            // Now we must have the constructor.
            Assert.IsNotNull(dynamicTextConstructor, "Can not get the constructor for DynamicTextCommand");
            // Use the constructor to get an instance of the object.
            object command = dynamicTextConstructor.Invoke(new object[] { id, text });
            Assert.IsNotNull(command, "Can not create an instance of DynamicTextCommand");
            OleMenuCommand oleCommand = command as OleMenuCommand;
            Assert.IsNotNull(oleCommand, "DynamicTextCommand does not derive from OleMenuCommand");
            return oleCommand;
        }
        //      End of static members
        // ============================================================================================

        private OleMenuCommand innerCommand;
        public DynamicTextCommandWrapper(CommandID id, string text) {
            innerCommand = GetDynamicTextCommandInstance(id, text);
        }
    }

    [TestClass()]
    public class DynamicTextCommanTest {

        [TestMethod()]
        public void CreateInstance() {
            Guid commandGuid = new Guid();
            int commandId = 42;
            CommandID id = new CommandID(commandGuid, commandId);
            string text = "Test DynamicTextCommand";
            OleMenuCommand command = DynamicTextCommandWrapper.GetDynamicTextCommandInstance(id, text);
            Assert.AreEqual(commandGuid, command.CommandID.Guid, "Guid not correct for the DynamicTextCommand.");
            Assert.AreEqual(commandId, command.CommandID.ID, "ID not correct for the DynamicTextCommand.");
        }

        [TestMethod()]
        public void CheckText() {
            OleMenuCommand command = DynamicTextCommandWrapper.GetDynamicTextCommandInstance(
                                        new CommandID(new Guid(), 77),
                                        "Test");
            string returnedText = command.Text;
            Assert.AreEqual("Test (Clicked 0 times)", returnedText, "Text not correct before first click.");

            // Simulate the click calling invoke.
            command.Invoke(null);
            // Get the new text
            returnedText = command.Text;
            Assert.AreEqual("Test (Clicked 1 times)", returnedText, "Text not correct after first click.");
        }
    }
}
