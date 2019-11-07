// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// CSharpServer.cs
// compile with: /target:library
// post-build command: regasm CSharpServer.dll /tlb:CSharpServer.tlb

using System;
using System.Runtime.InteropServices;
namespace CSharpServer
{
   // Since the .NET Framework interface and coclass have to behave as 
   // COM objects, we have to give them guids.
   [Guid("DBE0E8C4-1C61-41f3-B6A4-4E2F353D3D05")]
   public interface IManagedInterface
   {
      int PrintHi(string name);
   }

   [Guid("C6659361-1625-4746-931C-36014B146679")]
   public class InterfaceImplementation : IManagedInterface
   {
      public int PrintHi(string name)
      {
         Console.WriteLine("Hello, {0}!", name);
         return 33;
      }
   }
}

