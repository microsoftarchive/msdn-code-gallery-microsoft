// Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Tests.Model
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the fixup behavior of Pure POCO versions of objects in the model
    /// </summary>
    [TestClass]
    public class BaseModelTypeFixupTests : FixupTestsBase
    {
        /// <summary>
        /// Returns an instance of T created from the default constructor
        /// </summary>
        /// <typeparam name="T">The type to be created</typeparam>
        /// <returns>A new instance of type T</returns>
        protected override T CreateObject<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
