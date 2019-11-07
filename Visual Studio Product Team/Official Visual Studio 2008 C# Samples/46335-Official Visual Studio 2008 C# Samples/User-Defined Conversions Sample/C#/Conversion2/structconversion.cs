//Copyright (C) Microsoft Corporation.  All rights reserved.

// structconversion.cs
using System;

struct RomanNumeral
{
    public RomanNumeral(int value) 
    {
        this.value = value; 
    }
    static public implicit operator RomanNumeral(int value)
    {
        return new RomanNumeral(value);
    }
    static public implicit operator RomanNumeral(BinaryNumeral binary)
    {
        return new RomanNumeral((int)binary);
    }
    static public explicit operator int(RomanNumeral roman)
    {
         return roman.value;
    }
    static public implicit operator string(RomanNumeral roman) 
    {
        return("Conversion not yet implemented");
    }
    private int value;
}

struct BinaryNumeral
{
    public BinaryNumeral(int value) 
    {
        this.value = value;
    }
    static public implicit operator BinaryNumeral(int value)
    {
        return new BinaryNumeral(value);
    }
    static public implicit operator string(BinaryNumeral binary)
    {
        return("Conversion not yet implemented");
    }
    static public explicit operator int(BinaryNumeral binary)
    {
        return(binary.value);
    }

    private int value;
}

class Test
{
    static public void Main()
    {
        RomanNumeral roman;
        roman = 10;
        BinaryNumeral binary;
        // Perform a conversion from a RomanNumeral to a
        // BinaryNumeral:
        binary = (BinaryNumeral)(int)roman;
        // Performs a conversion from a BinaryNumeral to a RomanNumeral.
        // No cast is required:
        roman = binary;
        Console.WriteLine((int)binary);
        Console.WriteLine(binary);
    }
}

