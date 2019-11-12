// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

static class Samples {
    static int[] numbers = new [] { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
    static string[] strings = new [] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
    
    class Person {
      public string Name {get; set;}
      public int Level {get; set;}
    }
    
    static Person[] persons = new Person[] {
        new Person {Name="Jesper", Level=3},
        new Person {Name="Lene", Level=3},
        new Person {Name="Jonathan", Level=5},
        new Person {Name="Sagiv", Level=3},
        new Person {Name="Jacqueline", Level=3},
        new Person {Name="Ellen", Level=3},
        new Person {Name="Gustavo", Level=9}
        };
    
    public static void Sample1() {
        // use Where() to filter out elements matching a particular condition       
        var fnums = numbers.Where(n => n < 5);
    
        Console.WriteLine("Numbers < 5");
        foreach(int x in fnums) {
            Console.WriteLine(x);
        }
    }    

    public static void Sample2() {
        // use First() to find the one element matching a particular condition       
        string v = strings.First(s => s[0] == 'o');
    
        Console.WriteLine("string starting with 'o': {0}", v);
    }        
    
    public static void Sample3() {
        // use Select() to convert each element into a new value
        var snums = numbers.Select(n => strings[n]);
        
        Console.WriteLine("Numbers");
        foreach(string s in snums) {
            Console.WriteLine(s);
        }           
    }
    
    public static void Sample4()
    {
        // use Anonymous Type constructors to construct multi-valued results on the fly
        var q = strings.Select(s => new {Head = s.Substring(0,1), Tail = s.Substring(1)});
        foreach(var p in q) {
            Console.WriteLine("Head = {0}, Tail = {1}", p.Head, p.Tail);
        }
    }
    
    public static void Sample5() { 
        // Combine Select() and Where() to make a complete query
        var q = numbers.Where(n => n < 5).Select(n => strings[n]);
        
        Console.WriteLine("Numbers < 5");
        foreach(var x in q) {
            Console.WriteLine(x);
        }       
    }
    
    public static void Sample6() {
        // Sequence operators form first-class queries are not executed until you enumerate them.
        int i = 0;
        var q = numbers.Select(n => ++i);
        // Note, the local variable 'i' is not incremented until each element is evaluated (as a side-effect).
        foreach(var v in q) {
          Console.WriteLine("v = {0}, i = {1}", v, i);          
        }
        Console.WriteLine();
        
        // Methods like ToList() cause the query to be executed immediately, caching the results
        int i2 = 0;
        var q2 = numbers.Select(n => ++i2).ToList();
        // The local variable i2 has already been fully incremented before we iterate the results
        foreach(var v in q2) {
          Console.WriteLine("v = {0}, i2 = {1}", v, i2);
        }        
    }
    
    public static void Sample7() {
        // use GroupBy() to construct group partitions out of similar elements
        var q = strings.GroupBy(s => s[0]); // <- group by first character of each string
        
        foreach(var g in q) {
          Console.WriteLine("Group: {0}", g.Key);
          foreach(string v in g) {
            Console.WriteLine("\tValue: {0}", v);
          }
        }
    }
    
    public static void Sample8() {
        // use GroupBy() and aggregates such as Count(), Min(), Max(), Sum(), Average() to compute values over a partition
        var q = strings.GroupBy(s => s[0]).Select(g => new {FirstChar = g.Key, Count = g.Count()});
        
        foreach(var v in q) {
          Console.WriteLine("There are {0} string(s) starting with the letter {1}", v.Count, v.FirstChar);
        }
    }
    
    public static void Sample9() {
        // use OrderBy()/OrderByDescending() to give order to your resulting sequence
        var q = strings.OrderBy(s => s);  // order the strings by their name
        
        foreach(string s in q) {
          Console.WriteLine(s);
        }
    }
    
    public static void Sample9a() {  
        // use ThenBy()/ThenByDescending() to provide additional ordering detail
        var q = persons.OrderBy(p => p.Level).ThenBy(p => p.Name);
        
        foreach(var p in q) {
          Console.WriteLine("{0}  {1}", p.Level, p.Name);
        }
    }
    
    public static void Sample10() {
        // use query expressions to simplify
        var q = from p in persons 
                orderby p.Level, p.Name
                group p.Name by p.Level into g
                select new {Level = g.Key, Persons = g};
        
        foreach(var g in q) {
            Console.WriteLine("Level: {0}", g.Level);
            foreach(var p in g.Persons) {
                Console.WriteLine("Person: {0}", p);
            }
        }                
    }
}
