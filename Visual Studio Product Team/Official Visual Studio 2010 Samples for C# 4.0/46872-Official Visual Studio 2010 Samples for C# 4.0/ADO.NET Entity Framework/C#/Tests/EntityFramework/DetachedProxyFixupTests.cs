// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.EntityFramework
{
    using System.Diagnostics.CodeAnalysis;
    using EmployeeTracker.EntityFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Tests.Model;

    /// <summary>
    /// Tests the fixup behavior of Proxied versions of objects in the model when not attached to an ObjectContext
    /// </summary>
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Context is disposed in test cleanup.")]
    public class DetachedProxyFixupTests : FixupTestsBase
    {
        /// <summary>
        /// Context to use for proxy creation
        /// </summary>
        private EmployeeEntities context;

        /// <summary>
        /// Creates the resources needed for this test
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            this.context = new EmployeeEntities();

            // Unit tests run without a database so we need to switch off LazyLoading
            this.context.ContextOptions.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Releases any resourced used for this test run
        /// </summary>
        [TestCleanup]
        public void Teardown()
        {
            this.context.Dispose();
        }

        /// <summary>
        /// Returns a change tracking proxy derived from T
        /// </summary>
        /// <typeparam name="T">The type to be created</typeparam>
        /// <returns>A new instance of type T</returns>
        protected override T CreateObject<T>()
        {
            return this.context.CreateObject<T>();
        }
    }
}
