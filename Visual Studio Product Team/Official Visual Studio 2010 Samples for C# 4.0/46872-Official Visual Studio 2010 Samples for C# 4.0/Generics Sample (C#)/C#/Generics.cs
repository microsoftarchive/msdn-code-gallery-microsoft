// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)
//
// Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Generics_CSharp
{
    //Type parameter T in angle brackets.
    public class MyList<T> : IEnumerable<T>
    {
        protected Node head;
        protected Node current = null;

        // Nested type is also generic on T
        protected class Node
        {
            public Node next;
            //T as private member datatype.
            private T data;
            //T used in non-generic constructor.
            public Node(T t)
            {
                next = null;
                data = t;
            }
            public Node Next
            {
                get { return next; }
                set { next = value; }
            }
            //T as return type of property.
            public T Data
            {
                get { return data; }
                set { data = value; }
            }
        }

        public MyList()
        {
            head = null;
        }

        //T as method parameter type.
        public void AddHead(T t)
        {
            Node n = new Node(t);
            n.Next = head;
            head = n;
        }

        // Implement GetEnumerator to return IEnumerator<T> to enable
        // foreach iteration of our list. Note that in C# 2.0 
        // you are not required to implement Current and MoveNext.
        // The compiler will create a class that implements IEnumerator<T>.
        public IEnumerator<T> GetEnumerator()
        {
            Node current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        // We must implement this method because 
        // IEnumerable<T> inherits IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }


    public class SortedList<T> : MyList<T> where T : IComparable<T>
    {
        // A simple, unoptimized sort algorithm that 
        // orders list elements from lowest to highest:
        public void BubbleSort()
        {
            if (null == head || null == head.Next)
                return;

            bool swapped;
            do
            {
                Node previous = null;
                Node current = head;
                swapped = false;

                while (current.next != null)
                {
                    //  Because we need to call this method, the SortedList
                    //  class is constrained on IEnumerable<T>
                    if (current.Data.CompareTo(current.next.Data) > 0)
                    {
                        Node tmp = current.next;
                        current.next = current.next.next;
                        tmp.next = current;

                        if (previous == null)
                        {
                            head = tmp;
                        }
                        else
                        {
                            previous.next = tmp;
                        }
                        previous = tmp;
                        swapped = true;
                    }

                    else
                    {
                        previous = current;
                        current = current.next;
                    }

                }// end while
            } while (swapped);
        }
    }

    // A simple class that implements IComparable<T>
    // using itself as the type argument. This is a
    // common design pattern in objects that are
    // stored in generic lists.
    public class Person : IComparable<Person>
    {
        string name;
        int age;

        public Person(string s, int i)
        {
            name = s;
            age = i;
        }

        // This will cause list elements
        // to be sorted on age values.
        public int CompareTo(Person p)
        {
            return age - p.age;
        }

        public override string ToString()
        {
            return name + ":" + age;
        }

        // Must implement Equals.
        public bool Equals(Person p)
        {
            return (this.age == p.age);
        }
    }

    class Generics
    {
        static void Main(string[] args)
        {
            //Declare and instantiate a new generic SortedList class.
            //Person is the type argument.
            SortedList<Person> list = new SortedList<Person>();

            //Create name and age values to initialize Person objects.
            string[] names = new string[] { "Franscoise", "Bill", "Li", "Sandra", "Gunnar", "Alok", "Hiroyuki", "Maria", "Alessandro", "Raul" };
            int[] ages = new int[] { 45, 19, 28, 23, 18, 9, 108, 72, 30, 35 };

            //Populate the list.
            for (int x = 0; x < names.Length; x++)
            {
                list.AddHead(new Person(names[x], ages[x]));
            }

            Console.WriteLine("Unsorted List:");
            //Print out unsorted list.
            foreach (Person p in list)
            {
                Console.WriteLine(p.ToString());
            }

            //Sort the list.
            list.BubbleSort();

            Console.WriteLine(String.Format("{0}Sorted List:", Environment.NewLine));
            //Print out sorted list.
            foreach (Person p in list)
            {
                Console.WriteLine(p.ToString());
            }

            Console.WriteLine("Done");
        }
    }

}
