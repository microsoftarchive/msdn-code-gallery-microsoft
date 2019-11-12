//Copyright (C) Microsoft Corporation.  All rights reserved.

// ImperativeSecurity.cs
using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

class NativeMethods
{
    // This is a call to unmanaged code. Executing this method requires 
    // the UnmanagedCode security permission. Without this permission
    // an attempt to call this method will throw a SecurityException:
    [DllImport("msvcrt.dll")]
    public static extern int puts(string str);
    [DllImport("msvcrt.dll")]
    internal static extern int _flushall();
}

class MainClass
{
    private static void CallUnmanagedCodeWithoutPermission()
    {
        // Create a security permission object to describe the
        // UnmanagedCode permission:
        SecurityPermission perm = 
           new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        // Deny the UnmanagedCode from our current set of permissions.
        // Any method that is called on this thread until this method 
        // returns will be denied access to unmanaged code.
        // Even though the CallUnmanagedCodeWithPermission method
        // is called from a stack frame that already
        // calls Assert for unmanaged code, you still cannot call native
        // code. Because you use Deny here, the permission gets 
        // overwritten.
        perm.Deny();

        try
        {
            Console.WriteLine("Attempting to call unmanaged code without permission.");
            NativeMethods.puts("Hello World!");
            NativeMethods._flushall();
            Console.WriteLine("Called unmanaged code without permission. Whoops!");
        }
        catch (SecurityException)
        {
            Console.WriteLine("Caught Security Exception attempting to call unmanaged code.");
        }
    }

    private static void CallUnmanagedCodeWithPermission()
    {
        // Create a security permission object to describe the
        // UnmanagedCode permission:
        SecurityPermission perm = 
           new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        // Check that you have permission to access unmanaged code.
        // If you don't have permission to access unmanaged code, then
        // this call will throw a SecurityException.
        // Even though the CallUnmanagedCodeWithPermission method
        // is called from a stack frame that already
        // calls Assert for unmanaged code, you still cannot call native
        // code. Because you use Deny here, the permission gets 
        // overwritten.
        perm.Assert();

        try
        {
            Console.WriteLine("Attempting to call unmanaged code with permission.");
            NativeMethods.puts("Hello World!");
            NativeMethods._flushall();
            Console.WriteLine("Called unmanaged code with permission.");
        }
        catch (SecurityException)
        {
            Console.WriteLine("Caught Security Exception attempting to call unmanaged code. Whoops!");
        }
    }

    public static void Main() 
    {
        // The method itself will call the security permission Deny 
        // for unmanaged code, which will override the Assert permission
        // in this stack frame.
        SecurityPermission perm = new 
            SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
        perm.Assert();
        CallUnmanagedCodeWithoutPermission();

        // The method itself will call the security permission Assert
        // for unmanaged code, which will override the Deny permission in
        // this stack frame.
        perm.Deny();
        CallUnmanagedCodeWithPermission();
    }
}

