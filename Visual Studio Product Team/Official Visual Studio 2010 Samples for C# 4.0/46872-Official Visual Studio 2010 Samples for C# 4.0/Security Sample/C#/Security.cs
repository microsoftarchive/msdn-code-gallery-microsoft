// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

public class MainClass
{
    public static void Main() 
    {
        //Create File IO Read permission
        FileIOPermission FileIOReadPermission = new FileIOPermission(PermissionState.None);
        FileIOReadPermission.AllLocalFiles = FileIOPermissionAccess.Read;

        //Create Base Permission Set
        PermissionSet BasePermissionSet = new PermissionSet(PermissionState.None); // PermissionState.Unrestricted for full trust
        BasePermissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

        PermissionSet grantset = BasePermissionSet.Copy();
        grantset.AddPermission(FileIOReadPermission);

        //Write Sample source file to read
        System.IO.File.WriteAllText("TEST.TXT", "File Content");

        //-------- Calling Method in Full Trust -------- 
        try
        {
            Console.WriteLine("App Domain Name: " + AppDomain.CurrentDomain.FriendlyName);
            ReadFileMethod();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //-------- Create the AppDomain with FileIO Read Permission -------- 
        AppDomain sandbox = AppDomain.CreateDomain("Sandboxed AppDomain With FileIO.Read permission", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation, grantset, null);
        try
        {
            Console.WriteLine("App Domain Name: " + AppDomain.CurrentDomain.FriendlyName);
            sandbox.DoCallBack(new CrossAppDomainDelegate(ReadFileMethod));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        

        //-------- Create the AppDomain without FileIO Read Permission -------- 
        //Expect Security Exception to be thrown
        PermissionSet grantset2 = BasePermissionSet.Copy();
        AppDomain sandbox2 = AppDomain.CreateDomain("Sandboxed AppDomain Without FileIO.Read permission", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation, grantset2, null);
        try
        {
            Console.WriteLine("App Domain Name: " + AppDomain.CurrentDomain.FriendlyName);
            sandbox2.DoCallBack(new CrossAppDomainDelegate(ReadFileMethod));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("");
        Console.WriteLine("Press any key to end.");
        Console.ReadKey();
    }

    static public void ReadFileMethod()
    {
        string S = System.IO.File.ReadAllText("TEST.TXT");
        Console.WriteLine("File Content: " + S);
        Console.WriteLine("");
    }

}



    
