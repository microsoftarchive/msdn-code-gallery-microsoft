// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleVariance
{
    class Animal { }
    class Cat: Animal { }

    class Program
    {
        // To understand what the new CoVariance and ContraVariance code does for you
        // Try deleting or adding the words out and in from the following 2 lines of code:
        delegate T Func1<out T>();
        delegate void Action1<in T>(T a);
        
        static void Main(string[] args)
        {
            Func1<Cat> cat = () => new Cat();
            Func1<Animal> animal = cat;

            Action1<Animal> act1 = (ani) => { Console.WriteLine(ani); };
            Action1<Cat> cat1 = act1;

            Console.WriteLine(animal());
            cat1(new Cat());
        }

        
    }
}
