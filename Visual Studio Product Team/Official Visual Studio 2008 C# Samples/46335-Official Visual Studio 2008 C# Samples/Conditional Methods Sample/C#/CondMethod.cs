//Copyright (C) Microsoft Corporation.  All rights reserved.

// CondMethod.cs
// compile with: /target:library /d:DEBUG
using System; 
using System.Diagnostics;
namespace TraceFunctions 
{
   public class Trace 
   { 
       [Conditional("DEBUG")] 
       public static void Message(string traceMessage) 
       { 
           Console.WriteLine("[TRACE] - " + traceMessage); 
       } 
   } 
}

