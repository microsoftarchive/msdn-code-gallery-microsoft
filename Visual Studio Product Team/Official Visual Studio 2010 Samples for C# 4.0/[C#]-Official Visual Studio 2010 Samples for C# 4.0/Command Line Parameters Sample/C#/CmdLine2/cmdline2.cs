// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// cmdline2.cs
// arguments: John Paul Mary
using System;

public class CommandLine2
{
   public static void Main(string[] args)
   {
       Console.WriteLine("Number of command line parameters = {0}",
          args.Length);
       foreach(string s in args)
       {
          Console.WriteLine(s);
       }
   }
}

