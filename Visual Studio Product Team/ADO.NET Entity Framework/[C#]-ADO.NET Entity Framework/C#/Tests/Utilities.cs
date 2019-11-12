// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace Tests
{
    using System;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Static helper methods for unit testing
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Verifies that the supplied Action throws an ArgumentNullException
        /// Performs an Assert.Fail is the exception is not thrown
        /// </summary>
        /// <param name="call">The action that should throw</param>
        /// <param name="parameter">The parameter that should be identified in the exception</param>
        /// <param name="method">Method name for logging purposes</param>
        public static void CheckNullArgumentException(Action call, string parameter, string method)
        {
            if (call == null)
            {
                throw new ArgumentNullException("call");
            }

            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            try
            {
                call();
                Assert.Fail(string.Format(CultureInfo.InvariantCulture, "Supplying null '{0}' argument to '{1}' did not throw.", parameter, method));
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual(parameter, ex.ParamName, string.Format(CultureInfo.InvariantCulture, "'ArgumentNullException.ParamName' wrong when supplying null '{0}' argument to '{1}'.", parameter, method));
            }
        }
    }
}
