//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

// See the ReadMe.html for additional information
namespace LinqToXmlSample
{



    class Program
    {
        static void Main(string [] args) 
        {

            // List all books by Serge and Peter with co-authored books repeated
            XDocument doc = XDocument.Load(SetDataPath() + "bib.xml");
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Peter"));
            var books = b1.Concat(b2);
            foreach (var q in books)
                Console.WriteLine(q);

            Console.ReadLine();
        }

        static public string SetDataPath()
        {
            string path = Environment.CommandLine;
            while (path.StartsWith("\""))
            {
                path = path.Substring(1, path.Length - 2);
            }
            while (path.EndsWith("\"") || path.EndsWith(" "))
            {
                path = path.Substring(0, path.Length - 2);
            }
            path = Path.GetDirectoryName(path);

            return Path.Combine(path, "data\\");
        }
    }
}







