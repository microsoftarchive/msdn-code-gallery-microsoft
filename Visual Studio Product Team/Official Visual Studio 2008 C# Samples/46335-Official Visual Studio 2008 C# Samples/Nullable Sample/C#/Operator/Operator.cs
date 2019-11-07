//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;

class NullableOperator
{
    static int? GetNullableInt()
    {
        return null;
    }

    static string GetStringValue()
    {
        return null;
    }

    static void Main()
    {
        // ?? operator example.
        int? x = null;

        // y = x, unless x is null, in which case y = -1.
        int y = x ?? -1;
        Console.WriteLine("y == " + y);                          

        // Assign i to return value of method, unless
        // return value is null, in which case assign
        // default value of int to i.
        int i = GetNullableInt() ?? default(int);
        Console.WriteLine("i == " + i);                          

        // ?? also works with reference types. 
        string s = GetStringValue();
        // Display contents of s, unless s is null, 
        // in which case display "Unspecified".
        Console.WriteLine("s = {0}", s ?? "null");
    }
}
