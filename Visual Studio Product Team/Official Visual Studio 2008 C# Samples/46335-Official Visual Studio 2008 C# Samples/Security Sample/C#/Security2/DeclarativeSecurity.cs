//Copyright (C) Microsoft Corporation.  All rights reserved.

// DeclarativeSecurity.cs
using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

class NativeMethods
{
    // This is a call to unmanaged code. Executing this method requires 
    // the UnmanagedCode security permission. Without this permission,
    // an attempt to call this method will throw a SecurityException:
    [DllImport("msvcrt.dll")]
    public static extern int puts(string str);
    [DllImport("msvcrt.dll")]
    internal static extern int _flushall();
}

class MainClass
{
    // The security permission attached to this method will deny the
    // UnmanagedCode permission from the current set of permissions for
    // the duration of the call to this method:
    // Even though the CallUnmanagedCodeWithoutPermission method is
    // called from a stack frame that already calls
    // Assert for unmanaged code, you still cannot call native code.
    // Because this function is attached with the Deny permission for
    // unmanaged code, the permission gets overwritten.
    [SecurityPermission(SecurityAction.Deny, Flags = 
       SecurityPermissionFlag.UnmanagedCode)]
    private static void CallUnmanagedCodeWithoutPermission()
    {
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

    // The security permission attached to this method will force a 
    // runtime check for the unmanaged code permission whenever
    // this method is called. If the caller does not have unmanaged code
    // permission, then the call will generate a Security Exception.
    // Even though the CallUnmanagedCodeWithPermission method is called
    // from a stack frame that already calls
    // Deny for unmanaged code, it will not prevent you from calling
    // native code. Because this method is attached with the Assert
    // permission for unmanaged code, the permission gets overwritten.
    [SecurityPermission(SecurityAction.Assert, Flags = 
       SecurityPermissionFlag.UnmanagedCode)]
    private static void CallUnmanagedCodeWithPermission()
    {
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
        SecurityPermission perm = new
            SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        // The method itself is attached with the security permission 
        // Deny for unmanaged code, which will override
        // the Assert permission in this stack frame.
        perm.Assert();
        CallUnmanagedCodeWithoutPermission();

        // The method itself is attached with the security permission
        // Assert for unmanaged code, which will override the Deny 
        // permission in this stack frame.
        perm.Deny();
        CallUnmanagedCodeWithPermission();
    }
}

