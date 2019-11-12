// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

// tokens2.cs
using System;
using System.Collections;

public class Tokens: IEnumerable
{
   private string[] elements;

   Tokens(string source, char[] delimiters)
   {
      elements = source.Split(delimiters);
   }

   // IEnumerable Interface Implementation:

   public TokenEnumerator GetEnumerator() // non-IEnumerable version
   {
      return new TokenEnumerator(this);
   }

   IEnumerator IEnumerable.GetEnumerator() // IEnumerable version
   {
      return (IEnumerator) new TokenEnumerator(this);
   }

   // Inner class implements IEnumerator interface:

   public class TokenEnumerator: IEnumerator
   {
      private int position = -1;
      private Tokens t;

      public TokenEnumerator(Tokens t)
      {
         this.t = t;
      }

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

      public void Reset()
      {
         position = -1;
      }

      public string Current // non-IEnumerator version: type-safe
      {
         get
         {
            return t.elements[position];
         }
      }

      object IEnumerator.Current // IEnumerator version: returns object
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
      Tokens f = new Tokens("This is a well-done program.", 
         new char [] {' ','-'});
      foreach (string item in f) // try changing string to int
      {
         Console.WriteLine(item);
      }
   }
}

