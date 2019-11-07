//Copyright (C) Microsoft Corporation.  All rights reserved.

// tokens.cs
using System;
// The System.Collections namespace is made available:
using System.Collections;

// Declare the Tokens class:
public class Tokens : IEnumerable
{
   private string[] elements;

   Tokens(string source, char[] delimiters)
   {
      // Parse the string into tokens:
      elements = source.Split(delimiters);
   }

   // IEnumerable Interface Implementation:
   //   Declaration of the GetEnumerator() method 
   //   required by IEnumerable
   public IEnumerator GetEnumerator()
   {
      return new TokenEnumerator(this);
   }

   // Inner class implements IEnumerator interface:
   private class TokenEnumerator : IEnumerator
   {
      private int position = -1;
      private Tokens t;

      public TokenEnumerator(Tokens t)
      {
         this.t = t;
      }

      // Declare the MoveNext method required by IEnumerator:
      public bool MoveNext()
      {
         if (position < t.elements.Length - 1)
         {
            position++;
            return true;
         }
         else
         {
            return false;
         }
      }

      // Declare the Reset method required by IEnumerator:
      public void Reset()
      {
         position = -1;
      }

      // Declare the Current property required by IEnumerator:
      public object Current
      {
         get
         {
            return t.elements[position];
         }
      }
   }

   // Test Tokens, TokenEnumerator

   static void Main()
   {
      // Testing Tokens by breaking the string into tokens:
      Tokens f = new Tokens("This is a well-done program.", 
         new char[] {' ','-'});
      foreach (string item in f)
      {
         Console.WriteLine(item);
      }
   }
}

