// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// TraceTest.cs
// compile with: /reference:CondMethod.dll
// arguments: A B C
using System; 
using TraceFunctions; 

public class TraceClient 
{
   public static void Main(string[] args) 
   { 
      Trace.Message("Main Starting"); 
   
      if (args.Length == 0) 
      { 
          Console.WriteLine("No arguments have been passed"); 
      } 
      else 
      { 
          for( int i=0; i < args.Length; i++)    
          { 
              Console.WriteLine("Arg[{0}] is [{1}]",i,args[i]); 
          } 
      } 

       Trace.Message("Main Ending"); 
   } 
}

