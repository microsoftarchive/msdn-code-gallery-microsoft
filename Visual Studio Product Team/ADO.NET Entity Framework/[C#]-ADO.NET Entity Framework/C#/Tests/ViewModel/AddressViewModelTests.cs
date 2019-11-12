// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for AddressViewModel
    /// </summary>
    [TestClass]
    public class AddressViewModelTests
    {
        /// <summary>
        /// Verify getters and setters on ViewModel affect underlying data and notify changes
        /// </summary>
        [TestMethod]
        public void PropertyGetAndSet()
        {
            // Test initial properties are surfaced in ViewModel
            Address add = new Address { LineOne = "LineOne", LineTwo = "LineTwo", City = "City", State = "State", ZipCode = "ZipCode", Country = "Country" };
            AddressViewModel vm = new AddressViewModel(add);
            Assert.AreEqual(add, vm.Model, "Bound object property did not return object from model.");
            Assert.AreEqual(add.ValidUsageValues, vm.ValidUsageValues, "ValidUsageValues property did not return value from model.");
            Assert.AreEqual("LineOne", vm.LineOne, "LineOne property did not return value from model.");
            Assert.AreEqual("LineTwo", vm.LineTwo, "LineTwo property did not return value from model.");
            Assert.AreEqual("City", vm.City, "City property did not return value from model.");
            Assert.AreEqual("State", vm.State, "State property did not return value from model.");
            Assert.AreEqual("ZipCode", vm.ZipCode, "ZipCode property did not return value from model.");
            Assert.AreEqual("Country", vm.Country, "Country property did not return value from model.");

            // Test changing properties updates Model and raises PropertyChanged
            string lastProperty;
            vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

            lastProperty = null;
            vm.LineOne = "LineOne_NEW";
            Assert.AreEqual("LineOne", lastProperty, "Setting LineOne property did not raise correct PropertyChanged event.");
            Assert.AreEqual("LineOne_NEW", add.LineOne, "Setting LineOne property did not update model.");

            lastProperty = null;
            vm.LineTwo = "LineTwo_NEW";
            Assert.AreEqual("LineTwo", lastProperty, "Setting LineTwo property did not raise correct PropertyChanged event.");
            Assert.AreEqual("LineTwo_NEW", add.LineTwo, "Setting LineTwo property did not update model.");

            lastProperty = null;
            vm.City = "City_NEW";
            Assert.AreEqual("City", lastProperty, "Setting City property did not raise correct PropertyChanged event.");
            Assert.AreEqual("City_NEW", add.City, "Setting City property did not update model.");

            lastProperty = null;
            vm.State = "State_NEW";
            Assert.AreEqual("State", lastProperty, "Setting State property did not raise correct PropertyChanged event.");
            Assert.AreEqual("State_NEW", add.State, "Setting State property did not update model.");

            lastProperty = null;
            vm.ZipCode = "ZipCode_NEW";
            Assert.AreEqual("ZipCode", lastProperty, "Setting ZipCode property did not raise correct PropertyChanged event.");
            Assert.AreEqual("ZipCode_NEW", add.ZipCode, "Setting ZipCode property did not update model.");

            lastProperty = null;
            vm.Country = "Country_NEW";
            Assert.AreEqual("Country", lastProperty, "Setting Country property did not raise correct PropertyChanged event.");
            Assert.AreEqual("Country_NEW", add.Country, "Setting Country property did not update model.");
        }

        /// <summary>
        /// Verify getters reflect changes in model
        /// </summary>
        [TestMethod]
        public void ModelChangesFlowToProperties()
        {
            // Test ViewModel returns current value from model
            Address add = new Address { LineOne = "Address", LineTwo = "LineTwo", City = "City", State = "State", ZipCode = "ZipCode", Country = "Country" };
            AddressViewModel vm = new AddressViewModel(add);

            vm.LineOne = "LineOne_NEW";
            Assert.AreEqual("LineOne_NEW", add.LineOne, "LineOne property is not fetching the value from the model.");
            vm.LineTwo = "LineTwo_NEW";
            Assert.AreEqual("LineTwo_NEW", add.LineTwo, "LineTwo property is not fetching the value from the model.");
            vm.City = "City_NEW";
            Assert.AreEqual("City_NEW", add.City, "City property is not fetching the value from the model.");
            vm.State = "State_NEW";
            Assert.AreEqual("State_NEW", add.State, "State property is not fetching the value from the model.");
            vm.ZipCode = "ZipCode_NEW";
            Assert.AreEqual("ZipCode_NEW", add.ZipCode, "ZipCode property is not fetching the value from the model.");
            vm.Country = "Country_NEW";
            Assert.AreEqual("Country_NEW", add.Country, "Country property is not fetching the value from the model.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            Utilities.CheckNullArgumentException(() => { new AddressViewModel(null); }, "detail", "ctor");
        }
    }
}
