/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComboBox.UnitTest
{
    internal static class Utilities
    {
        public delegate void ThrowingFunction();

        public static bool HasFunctionThrown<ExceptionType>(ThrowingFunction function) 
            where ExceptionType : Exception
        {
            bool hasThrown = false;
            try
            {
                function();
            }
            catch (ExceptionType)
            {
                hasThrown = true;
            }
            catch (System.Reflection.TargetInvocationException ti)
            {
                hasThrown = (null != ti.InnerException as ExceptionType);
                if (!hasThrown)
                {
                    throw;
                }
            }

            return hasThrown;
        }

        public static void SameArray<Element_T>(Element_T[] expected, Element_T[] actual)
        {
            // If one array is null, then also the other must be null.
            if ((null == expected) || (null == actual))
            {
                Assert.IsNull(expected);
                Assert.IsNull(actual);
                return;
            }

            // The arrays are equal only if they contain the same number of elements.
            Assert.AreEqual<int>(expected.Length, actual.Length);

            // Now check that all the elements are the same.
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual<Element_T>(expected[i], actual[i]);
            }
        }
    }
}
