//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;

class NullableBoxing
{
    static void Main()
    {
        int? a;
        object oa;

        // Assigns a to Nullable<int> (value = default(int), hasValue = false).
        a = null;

        // Assigns oa to null (because x==null), not boxed "int?".
        oa = a;

        Console.WriteLine("Testing 'a' and 'boxed a' for null...");
        // Nullable variables can be compared to null.
        if (a == null)
        {
            Console.WriteLine("  a == null");
        }

        // Boxed nullable variables can be compared to null
        // because boxing a nullable where HasValue==false
        // sets the reference to null.
        if (oa == null)
        {
            Console.WriteLine("  oa == null");
        }

        Console.WriteLine("Unboxing a nullable type...");
        int? b = 10;
        object ob = b;

        // Boxed nullable types can be unboxed
        int? unBoxedB = (int?)ob;
        Console.WriteLine("  b={0}, unBoxedB={0}", b, unBoxedB);

        // Unboxing a nullable type set to null works if
        // unboxed into a nullable type.
        int? unBoxedA = (int?)oa;
        if (oa == null && unBoxedA == null)
        {
            Console.WriteLine("  a and unBoxedA are null");
        }

        Console.WriteLine("Attempting to unbox into non-nullable type...");
        // Unboxing a nullable type set to null throws an
        // exception if unboxed into a non-nullable type.
        try
        {
            int unBoxedA2 = (int)oa;
        }
        catch (Exception e)
        {
            Console.WriteLine("  {0}", e.Message);
        }
    }

}
