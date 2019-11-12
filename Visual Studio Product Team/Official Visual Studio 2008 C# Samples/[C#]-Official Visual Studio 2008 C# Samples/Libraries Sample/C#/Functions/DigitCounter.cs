//Copyright (C) Microsoft Corporation.  All rights reserved.

// DigitCounter.cs
// compile with: /target:library
using System; 

// Declare the same namespace as the one in Factorial.cs. This simply 
// allows types to be added to the same namespace.
namespace Functions 
{
    public class DigitCount 
    {
        // The NumberOfDigits static method calculates the number of
        // digit characters in the passed string:
        public static int NumberOfDigits(string theString) 
        {
            int count = 0; 
            for ( int i = 0; i < theString.Length; i++ ) 
            {
                if ( Char.IsDigit(theString[i]) ) 
                {
                    count++; 
                }
            }
            return count;
        }
    }
}

