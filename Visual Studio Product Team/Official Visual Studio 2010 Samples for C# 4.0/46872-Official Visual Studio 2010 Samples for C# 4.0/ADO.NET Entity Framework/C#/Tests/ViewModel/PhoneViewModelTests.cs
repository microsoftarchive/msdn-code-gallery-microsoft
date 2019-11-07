// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for PhoneViewModel
    /// </summary>
    [TestClass]
    public class PhoneViewModelTests
    {
        /// <summary>
        /// Verify getters and setters on ViewModel affect underlying data and notify changes
        /// </summary>
        [TestMethod]
        public void PropertyGetAndSet()
        {
            // Test initial properties are surfaced in ViewModel
            Phone ph = new Phone { Number = "NUMBER", Extension = "EXTENSION" };
            PhoneViewModel vm = new PhoneViewModel(ph);
            Assert.AreEqual(ph, vm.Model, "Bound object property did not return object from model.");
            Assert.AreEqual(ph.ValidUsageValues, vm.ValidUsageValues, "ValidUsageValues property did not return value from model.");
            Assert.AreEqual("NUMBER", vm.Number, "Number property did not return value from model.");
            Assert.AreEqual("EXTENSION", vm.Extension, "Extension property did not return value from model.");

            // Test changing properties updates Model and raises PropertyChanged
            string lastProperty;
            vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

            lastProperty = null;
            vm.Number = "NEW_NUMBER";
            Assert.AreEqual("Number", lastProperty, "Setting Number property did not raise correct PropertyChanged event.");
            Assert.AreEqual("NEW_NUMBER", ph.Number, "Setting Number property did not update model.");

            lastProperty = null;
            vm.Extension = "NEW_EXTENSION";
            Assert.AreEqual("Extension", lastProperty, "Setting Extension property did not raise correct PropertyChanged event.");
            Assert.AreEqual("NEW_EXTENSION", ph.Extension, "Setting Extension property did not update model.");
        }

        /// <summary>
        /// Verify getters reflect changes in model
        /// </summary>
        [TestMethod]
        public void ModelChangesFlowToProperties()
        {
            // Test ViewModel returns current value from model
            Phone ph = new Phone { Number = "NUMBER", Extension = "EXTENSION" };
            PhoneViewModel vm = new PhoneViewModel(ph);

            ph.Number = "NEW_NUMBER";
            ph.Extension = "NEW_EXTENSION";
            Assert.AreEqual("NEW_NUMBER", vm.Number, "Number property is not fetching the value from the model.");
            Assert.AreEqual("NEW_EXTENSION", vm.Extension, "Extension property is not fetching the value from the model.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            Utilities.CheckNullArgumentException(() => { new PhoneViewModel(null); }, "detail", "ctor");
        }
    }
}
