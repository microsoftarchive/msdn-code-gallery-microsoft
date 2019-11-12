// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// compose.cs
using System;

delegate void MyDelegate(string s);

class MyClass
{
    public static void Hello(string s)
    {
        Console.WriteLine("  Hello, {0}!", s);
    }

    public static void Goodbye(string s)
    {
        Console.WriteLine("  Goodbye, {0}!", s);
    }

    public static void Main()
    {
        MyDelegate a, b, c, d;

        // Create the delegate object a that references 
        // the method Hello:
        a = new MyDelegate(Hello);
        // Create the delegate object b that references 
        // the method Goodbye:
        b = new MyDelegate(Goodbye);
        // The two delegates, a and b, are composed to form c, 
        // which calls both methods in order:
        c = a + b;
        // Remove a from the composed delegate, leaving d, 
        // which calls only the method Goodbye:
        d = c - a;

        Console.WriteLine("Invoking delegate a:");
        a("A");
        Console.WriteLine("Invoking delegate b:");
        b("B");
        Console.WriteLine("Invoking delegate c:");
        c("C");
        Console.WriteLine("Invoking delegate d:");
        d("D");
    }
}

