// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.ViewModel.Helpers
{
    using System;
    using EmployeeTracker.ViewModel.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for DelegateCommand
    /// </summary>
    [TestClass]
    public class DelegateCommandTests
    {
        /// <summary>
        /// Construct command with no predicate specified for CanExecute
        /// Verify CanExecute is always true and that command executes when null is specified
        /// </summary>
        [TestMethod]
        public void ExecuteNoPredicateWithNull()
        {
            bool called = false;
            DelegateCommand cmd = new DelegateCommand((o) => called = true);
            Assert.IsTrue(cmd.CanExecute(null), "Command should always be able to execute when no predicate is supplied.");
            cmd.Execute(null);
            Assert.IsTrue(called, "Command did not run supplied Action.");
        }

        /// <summary>
        /// Construct command with null predicate
        /// </summary>
        [TestMethod]
        public void ConstructorAcceptsNullPredicate()
        {
            DelegateCommand cmd = new DelegateCommand((o) => { }, null);
            Assert.IsTrue(cmd.CanExecute(null), "Command with null specified for predicate should always be able to execute.");
        }

        /// <summary>
        /// Construct command with no predicate specified for CanExecute
        /// Verify CanExecute is always true and that command executes when an object is specified
        /// </summary>
        [TestMethod]
        public void ExecuteNoPredicateWithArgument()
        {
            bool called = false;
            DelegateCommand cmd = new DelegateCommand((o) => called = true);
            Assert.IsTrue(cmd.CanExecute("x"), "Command should always be able to execute when no predicate is supplied.");
            cmd.Execute("x");
            Assert.IsTrue(called, "Command did not run supplied Action.");
        }

        /// <summary>
        /// Construct command with a 'true' predicate specified for CanExecute
        /// Verify CanExecute and that command executes
        /// </summary>
        [TestMethod]
        public void ExecuteWithPredicate()
        {
            bool called = false;
            DelegateCommand cmd = new DelegateCommand((o) => called = true, (o) => true);
            Assert.IsTrue(cmd.CanExecute(null), "Command should be able to execute when predicate returns true.");
            cmd.Execute(null);
            Assert.IsTrue(called, "Command did not run supplied Action.");
        }

        /// <summary>
        /// Construct command with a 'false' predicate specified for CanExecute
        /// Verify CanExecute and that attempting to execute throws
        /// </summary>
        [TestMethod]
        public void AttemptExecuteWithFalsePredicate()
        {
            bool called = false;
            DelegateCommand cmd = new DelegateCommand((o) => called = true, (o) => false);
            Assert.IsFalse(cmd.CanExecute(null), "Command should not be able to execute when predicate returns false.");

            try
            {
                cmd.Execute(null);
            }
            catch (InvalidOperationException)
            {
            }

            Assert.IsFalse(called, "Command should not have run supplied Action.");
        }

        /// <summary>
        /// Verify NullArgumentExceptions are thrown where null is invalid
        /// </summary>
        [TestMethod]
        public void CheckNullArgumentExceptions()
        {
            Utilities.CheckNullArgumentException(() => { new DelegateCommand(null); }, "execute", "ctor");
        }
    }
}
