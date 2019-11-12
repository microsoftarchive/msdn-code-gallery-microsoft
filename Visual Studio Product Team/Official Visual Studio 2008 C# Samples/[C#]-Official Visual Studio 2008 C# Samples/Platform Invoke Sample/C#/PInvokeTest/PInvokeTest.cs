//Copyright (C) Microsoft Corporation.  All rights reserved.

// PInvokeTest.cs
using System;
using System.Runtime.InteropServices;

class PlatformInvokeTest
{
    [DllImport("msvcrt.dll")]
    public static extern int puts(string c);
    [DllImport("msvcrt.dll")]
    internal static extern int _flushall();

    public static void Main() 
    {
        puts("Test");
        _flushall();
    }
}

