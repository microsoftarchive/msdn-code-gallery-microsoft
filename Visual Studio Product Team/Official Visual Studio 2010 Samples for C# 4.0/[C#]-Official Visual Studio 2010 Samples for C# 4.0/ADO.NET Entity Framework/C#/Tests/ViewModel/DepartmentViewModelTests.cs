// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel
{
    using EmployeeTracker.Model;
    using EmployeeTracker.ViewModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tests;

    /// <summary>
    /// Unit tests for DepartmentViewModel
    /// </summary>
    [TestClass]
    public class DepartmentViewModelTests
    {
        /// <summary>
        /// Verify getters and setters on ViewModel affect underlying data and notify changes
        /// </summary>
        [TestMethod]
        public void PropertyGetAndSet()
        {
            // Test initial properties are surfaced in ViewModel
            Department dep = new Department { DepartmentName = "DepartmentName", DepartmentCode = "DepartmentCode" };
            DepartmentViewModel vm = new DepartmentViewModel(dep);
            Assert.AreEqual(dep, vm.Model, "Bound object property did not return object from model.");
            Assert.AreEqual("DepartmentName", vm.DepartmentName, "DepartmentName property did not return value from model.");
            Assert.AreEqual("DepartmentCode", vm.DepartmentCode, "DepartmentCode property did not return value from model.");

            // Test changing properties updates Model and raises PropertyChanged
            string lastProperty;
            vm.PropertyChanged += (sender, e) => { lastProperty = e.PropertyName; };

            lastProperty = null;
            vm.DepartmentName = "DepartmentName_NEW";
            Assert.AreEqual("DepartmentName", lastProperty, "Setting DepartmentName property did not raise correct PropertyChanged event.");
            Assert.AreEqual("DepartmentName_NEW", dep.DepartmentName, "Setting DepartmentName property did not update model.");

            lastProperty = null;
            vm.DepartmentCode = "DepartmentCode_NEW";
            Assert.AreEqual("DepartmentCode", lastProperty, "Setting DepartmentName property did not raise correct PropertyChanged event.");
            Assert.AreEqual("DepartmentCode_NEW", dep.DepartmentCode, "Setting DepartmentCode property did not update model.");
        }

        /// <summary>
        /// Verify getters reflect changes in model
        /// </summary>
        [TestMethod]
        public void ModelChangesFlowToProperties()
        {
            // Test ViewModel returns current value from model
            Department dep = new Department { DepartmentName = "DepartmentName", DepartmentCode = "DepartmentCode" };
            DepartmentViewModel vm = new DepartmentViewModel(dep);

            vm.DepartmentName = "DepartmentName_NEW";
            Assert.AreEqual("DepartmentName_NEW", dep.DepartmentName, "DepartmentName property is not fetching the value from the model.");
            vm.DepartmentCode = "DepartmentCode_NEW";
            Assert.AreEqual("DepartmentCode_NEW", dep.DepartmentCode, "DepartmentCode property is not fetching the value from the model.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            Utilities.CheckNullArgumentException(() => { new DepartmentViewModel(null); }, "department", "ctor");
        }
    }
}
