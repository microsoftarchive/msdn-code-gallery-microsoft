//Copyright (C) Microsoft Corporation.  All rights reserved.

// conversion.cs
using System;

struct RomanNumeral
{
    public RomanNumeral(int value) 
    { 
       this.value = value; 
    }
    // Declare a conversion from an int to a RomanNumeral. Note the
    // the use of the operator keyword. This is a conversion 
    // operator named RomanNumeral:
    static public implicit operator RomanNumeral(int value) 
    {
       // Note that because RomanNumeral is declared as a struct, 
       // calling new on the struct merely calls the constructor 
       // rather than allocating an object on the heap:
       return new RomanNumeral(value);
    }
    // Declare an explicit conversion from a RomanNumeral to an int:
    static public explicit operator int(RomanNumeral roman)
    {
       return roman.value;
    }
    // Declare an implicit conversion from a RomanNumeral to 
    // a string:
    static public implicit operator string(RomanNumeral roman)
    {
       return("Conversion not yet implemented");
    }
    private int value;
}

class Test
{
    static public void Main()
    {
        RomanNumeral numeral;

        numeral = 10;

// Call the explicit conversion from numeral to int. Because it is
// an explicit conversion, a cast must be used:
        Console.WriteLine((int)numeral);

// Call the implicit conversion to string. Because there is no
// cast, the implicit conversion to string is the only
// conversion that is considered:
        Console.WriteLine(numeral);
 
// Call the explicit conversion from numeral to int and 
// then the explicit conversion from int to short:
        short s = (short)numeral;

        Console.WriteLine(s);
    }
}

