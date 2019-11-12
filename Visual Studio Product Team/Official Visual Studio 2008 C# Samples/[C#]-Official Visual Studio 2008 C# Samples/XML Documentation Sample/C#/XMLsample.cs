//Copyright (C) Microsoft Corporation.  All rights reserved.

// XMLsample.cs
// compile with: /doc:XMLsample.xml
using System;

/// <summary>
/// Class level summary documentation goes here.</summary>
/// <remarks>
/// Longer comments can be associated with a type or member 
/// through the remarks tag</remarks>
public class SomeClass
{
   /// <summary>
   /// Store for the name property</summary>
   private string myName = null;

   /// <summary>
   /// The class constructor. </summary>
   public SomeClass()
   {
       // TODO: Add Constructor Logic here
   }
   
   /// <summary>
   /// Name property </summary>
   /// <value>
   /// A value tag is used to describe the property value</value>
   public string Name
   {
      get 
      {
         if ( myName == null )
         {
            throw new Exception("Name is null");
         }
             
         return myName;
      }
   }

   /// <summary>
   /// Description for SomeMethod.</summary>
   /// <param name="s"> Parameter description for s goes here</param>
   /// <seealso cref="String">
   /// You can use the cref attribute on any tag to reference a type or member 
   /// and the compiler will check that the reference exists. </seealso>
   public void SomeMethod(string s)
   {
   }

   /// <summary>
   /// Some other method. </summary>
   /// <returns>
   /// Return results are described through the returns tag.</returns>
   /// <seealso cref="SomeMethod(string)">
   /// Notice the use of the cref attribute to reference a specific method </seealso>
   public int SomeOtherMethod()
   {
      return 0;
   }

   /// <summary>
   /// The entry point for the application.
   /// </summary>
   /// <param name="args"> A list of command line arguments</param>
   public static int Main(String[] args)
   {
      // TODO: Add code to start application here

       return 0;
   }
}

