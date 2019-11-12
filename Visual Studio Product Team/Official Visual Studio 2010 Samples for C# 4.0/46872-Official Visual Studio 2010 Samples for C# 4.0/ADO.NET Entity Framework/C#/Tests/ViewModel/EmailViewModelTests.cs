// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using System.Diagnostics.CodeAnalysis;
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tests;

    /// <summary>
    /// Unit tests for EmailViewModel
    /// </summary>
    [TestClass]
    public class EmailViewModelTests
    {
        /// <summary>
        /// Verify getters and setters on ViewModel affect underlying data and notify changes
        /// </summary>
        [TestMethod]
        public void PropertyGetAndSet()
        {
            // Test initial properties are surfaced in ViewModel
            Email em = new Email { Address = "EMAIL" };
            EmailViewModel vm = new EmailViewModel(em);
            Assert.AreEqual(em, vm.Model, "Bound object property did not return object from model.");
            Assert.AreEqual(em.ValidUsageValues, vm.ValidUsageValues, "ValidUsageValues property did not return value from model.");
            Assert.AreEqual("EMAIL", vm.Address, "Address property did not return value from model.");

            // Test changing properties updates Model and raises PropertyChanged
            string lastProperty;
            vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

            lastProperty = null;
            vm.Address = "NEW_EMAIL";
            Assert.AreEqual("Address", lastProperty, "Setting Address property did not raise correct PropertyChanged event.");
            Assert.AreEqual("NEW_EMAIL", em.Address, "Setting Address property did not update model.");
        }

        /// <summary>
        /// Verify getters reflect changes in model
        /// </summary>
        [TestMethod]
        public void ModelChangesFlowToProperties()
        {
            // Test ViewModel returns current value from model
            Email em = new Email { Address = "EMAIL" };
            EmailViewModel vm = new EmailViewModel(em);

            em.Address = "NEW_EMAIL";
            Assert.AreEqual("NEW_EMAIL", vm.Address, "Address property is not fetching the value from the model.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            Utilities.CheckNullArgumentException(() => { new EmailViewModel(null); }, "detail", "ctor");
        }
    }
}
