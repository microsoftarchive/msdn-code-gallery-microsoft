/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderStorage and is intended
    ///to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderStorage Unit Tests
    ///</summary>
    [TestClass()]
    public class SccProviderStorageTest
    {
        /// <summary>
        ///A test for SccProviderStorage (string)
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            SccProviderStorage target = new SccProviderStorage("Dummy.txt");
            Assert.IsNotNull(target, "SccProviderStorage cannot be created");
        }

        /// <summary>
        ///A test for AddFilesToStorage (IList&lt;string&gt;)
        ///</summary>
        [TestMethod()]
        public void AddFilesToStorageTest()
        {
            string projectFile = Path.GetTempFileName();
            string storageFile = projectFile + ".storage";
            if (File.Exists(storageFile))
            {
                File.Delete(storageFile);
            }

            SccProviderStorage target = new SccProviderStorage(projectFile);

            IList<string> files = new List<string>();
            files.Add(projectFile);
            files.Add("dummy.txt");
            target.AddFilesToStorage(files);
            // Test that project file is now controlled
            Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(projectFile), "Failed to add the project file");
            // Test the storage file was written
            Assert.IsTrue(File.Exists(storageFile), "Storage file was not created");

            // Cleanup the files written by the test
            File.SetAttributes(projectFile, FileAttributes.Normal);
            File.Delete(projectFile);
            File.Delete(storageFile);
        }

        /// <summary>
        ///A test for CheckinFile (string)
        ///</summary>
        [TestMethod()]
        public void CheckinCheckoutFileTest()
        {
            string projectFile = Path.GetTempFileName();
            SccProviderStorage target = new SccProviderStorage(projectFile);

            target.CheckinFile(projectFile);
            // Test the file is readonly
            Assert.AreEqual(File.GetAttributes(projectFile) & FileAttributes.ReadOnly, FileAttributes.ReadOnly, "Checkin failed");

            // Cleanup the files written by the test
            File.SetAttributes(projectFile, FileAttributes.Normal);
            File.Delete(projectFile);
        }

        /// <summary>
        ///A test for CheckoutFile (string)
        ///</summary>
        [TestMethod()]
        public void CheckoutFileTest()
        {
            string projectFile = Path.GetTempFileName();
            SccProviderStorage target = new SccProviderStorage(projectFile);

            target.CheckinFile(projectFile);
            // Test the file is readonly
            Assert.AreEqual(File.GetAttributes(projectFile) & FileAttributes.ReadOnly, FileAttributes.ReadOnly, "Checkin failed");

            target.CheckoutFile(projectFile);
            // Test the file is readwrite
            Assert.AreEqual(File.GetAttributes(projectFile) & FileAttributes.ReadOnly, (FileAttributes)0, "Checkout failed");

            // Cleanup the files written by the test
            File.Delete(projectFile);
        }

        /// <summary>
        ///A test for GetFileStatus (string)
        ///</summary>
        [TestMethod()]
        public void GetFileStatusTest()
        {
            string projectFile = Path.GetTempFileName();
            string storageFile = projectFile + ".storage";
            if (File.Exists(storageFile))
            {
                File.Delete(storageFile);
            }

            SccProviderStorage target = new SccProviderStorage(projectFile);

            IList<string> files = new List<string>();
            files.Add(projectFile);
            target.AddFilesToStorage(files);
            // Test that project file is now controlled
            Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus(projectFile), "GetFileStatus failed for project file");
            // Checkout the file and test status again
            target.CheckoutFile(projectFile);
            Assert.AreEqual(SourceControlStatus.scsCheckedOut, target.GetFileStatus(projectFile), "GetFileStatus failed for project file");
            // Test that a dummy file is uncontrolled
            Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus("Dummy.txt"), "GetFileStatus failed for uncontrolled file");

            // Cleanup the files written by the test
            File.Delete(projectFile);
            File.Delete(storageFile);
        }

        /// <summary>
        ///A test for RenameFileInStorage (string, string)
        ///</summary>
        [TestMethod()]
        public void RenameFileInStorageTest()
        {
            string projectFile = Path.GetTempFileName();
            string storageFile = projectFile + ".storage";
            string newStorageFile = "dummy.txt.storage";
            if (File.Exists(storageFile))
            {
                File.Delete(storageFile);
            }
            if (File.Exists(newStorageFile))
            {
                File.Delete(newStorageFile);
            }

            SccProviderStorage target = new SccProviderStorage(projectFile);

            IList<string> files = new List<string>();
            files.Add(projectFile);
            target.AddFilesToStorage(files);
            target.RenameFileInStorage(projectFile, "dummy.txt");
            // Test that project file is now uncontrolled
            Assert.AreEqual(SourceControlStatus.scsUncontrolled, target.GetFileStatus(projectFile), "GetFileStatus failed for old name");
            // Test that dummy file is now controlled (and checked in since it's missing from disk)
            Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus("Dummy.txt"), "GetFileStatus failed for new name");

            // Cleanup the files written by the test
            File.SetAttributes(projectFile, FileAttributes.Normal);
            File.Delete(projectFile);
            File.Delete(newStorageFile);
        }

        /// <summary>
        ///A test for ReadStorageFile ()
        ///</summary>
        [TestMethod()]
        public void ReadStorageFileTest()
        {
            string projectFile = Path.GetTempFileName();
            string storageFile = projectFile + ".storage";
            if (File.Exists(storageFile))
            {
                File.Delete(storageFile);
            }

            StreamWriter objWriter = new StreamWriter(storageFile, false, System.Text.Encoding.Unicode);
            objWriter.Write("dummy.txt\r\n");
            objWriter.Close();
            objWriter.Dispose();

            SccProviderStorage target = new SccProviderStorage(projectFile);
            // Test that dummy file is now controlled (and checked in since it's missing from disk)
            Assert.AreEqual(SourceControlStatus.scsCheckedIn, target.GetFileStatus("Dummy.txt"), "GetFileStatus failed for new name");

            // Cleanup the files written by the test
            File.SetAttributes(projectFile, FileAttributes.Normal);
            File.Delete(projectFile);
            File.Delete(storageFile);
        }
    }
}
