//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace PartialClassesExample
{
    class PartialClassesMain
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("One argument required.");
                return;
            }

            // CharValues is a partial class -- two of its methods
            // are defined in CharTypesPublic.cs and two are defined
            // in CharTypesPrivate.cs. 
            int aCount = CharValues.CountAlphabeticChars(args[0]);
            int nCount = CharValues.CountNumericChars(args[0]);
            
            Console.Write("The input argument contains {0} alphabetic and {1} numeric characters", aCount, nCount);
        }
    }
}

