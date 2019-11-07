//Copyright (C) Microsoft Corporation.  All rights reserved.

// SuppressSecurity.cs
using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

class NativeMethods
{
    // This is a call to unmanaged code. Executing this method requires 
    // the UnmanagedCode security permission. Without this permission,
    // an attempt to call this method will throw a SecurityException:
    /* NOTE: The SuppressUnmanagedCodeSecurityAttribute disables the
       check for the UnmanagedCode permission at runtime. Be Careful! */
    [SuppressUnmanagedCodeSecurityAttribute()]
    [DllImport("msvcrt.dll")]
    internal static extern int puts(string str);
    [SuppressUnmanagedCodeSecurityAttribute()]
    [DllImport("msvcrt.dll")]
    internal static extern int _flushall();
}

class MainClass
{
    // The security permission attached to this method will remove the
    // UnmanagedCode permission from the current set of permissions for
    // the duration of the call to this method.
    // Even though the CallUnmanagedCodeWithoutPermission method is
    // called from a stack frame that already calls
    // Assert for unmanaged code, you still cannot call native code.
    // Because this method is attached with the Deny permission for
    // unmanaged code, the permission gets overwritten. However, because
    // you are using SuppressUnmanagedCodeSecurityAttribute here, you can
    // still call the unmanaged methods successfully.
    // The code should use other security checks to ensure that you don't
    // incur a security hole.
    [SecurityPermission(SecurityAction.Deny, Flags = 
       SecurityPermissionFlag.UnmanagedCode)]
    private static void CallUnmanagedCodeWithoutPermission()
    {
        try
        {
            // The UnmanagedCode security check is disbled on the call
            // below. However, the unmanaged call only displays UI. The 
            // security will be ensured by only allowing the unmanaged 
            // call if there is a UI permission.
            UIPermission uiPermission = 
               new UIPermission(PermissionState.Unrestricted);
            uiPermission.Demand();

            Console.WriteLine("Attempting to call unmanaged code without UnmanagedCode permission.");
            NativeMethods.puts("Hello World!");
            NativeMethods._flushall();
            Console.WriteLine("Called unmanaged code without UnmanagedCode permission.");
        }
        catch (SecurityException)
        {
            Console.WriteLine("Caught Security Exception attempting to call unmanaged code.");
        }
    }

    // The security permission attached to this method will add the 
    // UnmanagedCode permission to the current set of permissions for the
    // duration of the call to this method.
    // Even though the CallUnmanagedCodeWithPermission method is called
    // from a stack frame that already calls
    // Deny for unmanaged code, it will not prevent you from calling
    // native code. Because this method is attached with the Assert
    // permission for unmanaged code, the permission gets overwritten.
    // Because you are using SuppressUnmanagedCodeSecurityAttribute here,
    // you can call the unmanaged methods successfully.
    // The SuppressUnmanagedCodeSecurityAttribute will let you succeed, 
    // even if you don't have a permission.
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

       // The method itself is attached with the security permission Deny
       // for unmanaged code, which will override the Assert permission in
       // this stack frame. However, because you are using 
       // SuppressUnmanagedCodeSecurityAttribute, you can still call the
       // unmanaged methods successfully.
       // The code should use other security checks to ensure that you 
       // don't incur a security hole.
       perm.Assert();
       CallUnmanagedCodeWithoutPermission();

       // The method itself is attached with the security permission
       // Assert for unmanaged code, which will override the Deny 
       // permission in this stack frame. Because you are using
       // SuppressUnmanagedCodeSecurityAttribute, you can call the
       // unmanaged methods successfully.
       // The SuppressUnmanagedCodeSecurityAttribute will let you succeed,
       // even if you don't have a permission.
       perm.Deny();
       CallUnmanagedCodeWithPermission();
    }
}

