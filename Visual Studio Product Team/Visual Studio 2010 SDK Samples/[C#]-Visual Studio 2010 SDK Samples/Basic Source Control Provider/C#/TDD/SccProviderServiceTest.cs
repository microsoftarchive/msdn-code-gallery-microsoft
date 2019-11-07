/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
{
    /// <summary>
    /// Wee need this fake implementation of BasicSccProvider in order to override
    /// the behavior of OnActiveState change
    /// </summary>
    internal class FakeSccProvider : BasicSccProvider
    {
        public override void OnActiveStateChange()
        {
            return;
        }
    }

    /// <summary>
    ///This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService and is intended
    ///to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService Unit Tests
    ///</summary>
    [TestClass()]
    public class SccProviderServiceTest
    {
        /// <summary>
        /// Creates a SccProviderService object
        ///</summary>
        public SccProviderService GetSccProviderServiceInstance
        {
            get
            {
                // Create a provider package
                FakeSccProvider sccProvider = new FakeSccProvider();

                OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

                // Need to mock a service implementing IVsRegisterScciProvider, because the scc provider will register with it
                IVsRegisterScciProvider registerScciProvider = MockRegisterScciProvider.GetBaseRegisterScciProvider();
                serviceProvider.AddService(typeof(IVsRegisterScciProvider), registerScciProvider, true);

                // Site the package
                IVsPackage package = sccProvider as IVsPackage;
                package.SetSite(serviceProvider);

                //  Get the source control provider service object
                FieldInfo sccServiceMember = typeof(BasicSccProvider).GetField("sccService", BindingFlags.Instance | BindingFlags.NonPublic);
                SccProviderService target = sccServiceMember.GetValue(sccProvider) as SccProviderService;

                return target;
            }
        }

        /// <summary>
        ///A test for SccProviderService creation and interfaces
        ///</summary>
        [TestMethod()]
        public void ConstructorTest()
        {
            BasicSccProvider sccProvider = new BasicSccProvider();
            SccProviderService target = new SccProviderService(sccProvider);

            Assert.AreNotEqual(null, target, "Could not create provider service");
            Assert.IsNotNull(target as IVsSccProvider, "The object does not implement IVsPackage");
        }

        /// <summary>
        ///A test for Active
        ///</summary>
        [TestMethod()]
        public void ActiveTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            // After the object is created, the provider is inactive
            Assert.AreEqual(false, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.Active was not reported correctly.");

            // Activate the provider and test the result
            target.SetActive();
            Assert.AreEqual(true, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.Active was not reported correctly.");

            // Deactivate the provider and test the result
            target.SetInactive();
            Assert.AreEqual(false, target.Active, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.Active was not reported correctly.");
        }

        /// <summary>
        ///A test for AnyItemsUnderSourceControl (out int)
        ///</summary>
        [TestMethod()]
        public void AnyItemsUnderSourceControlTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            int pfResult = 0;
            int actual = target.AnyItemsUnderSourceControl(out pfResult);

            // The method is not supposed to fail, and the basic provider cannot control any projects
            Assert.AreEqual(VSConstants.S_OK, pfResult, "pfResult_AnyItemsUnderSourceControl_expected was not set correctly.");
            Assert.AreEqual(0, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.AnyItemsUnderSourceControl did not return the expected value.");
        }

        /// <summary>
        ///A test for SetActive ()
        ///</summary>
        [TestMethod()]
        public void SetActiveTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            int actual = target.SetActive();
            Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.SetActive failed.");
        }

        /// <summary>
        ///A test for SetInactive ()
        ///</summary>
        [TestMethod()]
        public void SetInactiveTest()
        {
            SccProviderService target = GetSccProviderServiceInstance;

            int actual = target.SetInactive();
            Assert.AreEqual(VSConstants.S_OK, actual, "Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.SccProviderService.SetInactive failed.");
        }
    }
}
