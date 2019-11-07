// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using System.Collections.Generic;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for ContactDetailViewModel
    /// </summary>
    [TestClass]
    public class ContactDetailViewModelTests
    {
        /// <summary>
        /// Verify BuildViewModel can create all contact detail types
        /// </summary>
        [TestMethod]
        public void BuildViewModel()
        {
            Phone p = new Phone();
            Email e = new Email();
            Address a = new Address();

            var pvm = ContactDetailViewModel.BuildViewModel(p);
            Assert.IsInstanceOfType(pvm, typeof(PhoneViewModel), "Factory method created wrong ViewModel type.");
            Assert.AreEqual(p, pvm.Model, "Underlying model object on ViewModel is not correct.");

            var evm = ContactDetailViewModel.BuildViewModel(e);
            Assert.IsInstanceOfType(evm, typeof(EmailViewModel), "Factory method created wrong ViewModel type.");
            Assert.AreEqual(e, evm.Model, "Underlying model object on ViewModel is not correct.");

            var avm = ContactDetailViewModel.BuildViewModel(a);
            Assert.IsInstanceOfType(avm, typeof(AddressViewModel), "Factory method created wrong ViewModel type.");
            Assert.AreEqual(a, avm.Model, "Underlying model object on ViewModel is not correct.");
        }

        /// <summary>
        /// Verify BuildViewModel does no throw when it processes an unrecognized type
        /// </summary>
        [TestMethod]
        public void BuildViewModelUnknownType()
        {
            var f = new FakeContactDetail();
            var fvm = ContactDetailViewModel.BuildViewModel(f);
            Assert.IsNull(fvm, "BuildViewModel should return null when it doesn't know how to handle a type.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            Utilities.CheckNullArgumentException(() => { ContactDetailViewModel.BuildViewModel(null); }, "detail", "BuildViewModel");
        }

        /// <summary>
        /// Fake contact type to test BuildViewModelUnknownType
        /// </summary>
        private class FakeContactDetail : ContactDetail
        {
            /// <summary>
            /// Gets valid values for the usage field
            /// Stub implementation, just returns null
            /// </summary>
            public override IEnumerable<string> ValidUsageValues
            {
                get { return null; }
            }
        }
    }
}
