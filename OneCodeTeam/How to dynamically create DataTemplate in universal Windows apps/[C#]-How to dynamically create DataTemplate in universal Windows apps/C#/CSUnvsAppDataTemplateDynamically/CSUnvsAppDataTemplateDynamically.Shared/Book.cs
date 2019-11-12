/****************************** Module Header ******************************\
 * Module Name:    Book.cs
 * Project:        CSUnvsAppDataTemplateDynamically
 * Copyright (c) Microsoft Corporation.
 * 
 * This class is used to initialize data in Book collection
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;

namespace CSUnvsAppDataTemplateDynamically
{
    public class Book
    {

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }

        public static List<Book> GetBooks()
        {
            List<Book> Books = new List<Book>();

            Books.Add(new Book() { Author = "Allen", Price = 29.99M, Name = "War" });
            Books.Add(new Book() { Author = "Carter", Price = 35.00M, Name = "Fighting Like A Man" });
            Books.Add(new Book() { Author = "Rose", Price = 39.99M, Name = "Tomorrow" });
            Books.Add(new Book() { Author = "Daisy", Price = 99.00M, Name = "Last Three Days" });
            Books.Add(new Book() { Author = "Mary", Price = 10.00M, Name = "Fire Wall" });
            Books.Add(new Book() { Author = "Ray", Price = 19.99M, Name = "Lie" });
            Books.Add(new Book() { Author = "Sherry", Price = 45.50M, Name = "Three Wolves" });
            Books.Add(new Book() { Author = "Lisa", Price = 36.99M, Name = "Beauty" });
            Books.Add(new Book() { Author = "Judy", Price = 12.00M, Name = "The One" });
            Books.Add(new Book() { Author = "Jack", Price = 88.99M, Name = "Chosen by God" });
            Books.Add(new Book() { Author = "May", Price = 130.00M, Name = "The Magic" });
            Books.Add(new Book() { Author = "Vivian", Price = 299.99M, Name = "Who is the Murder" });

            return Books;
        }
    }
}
