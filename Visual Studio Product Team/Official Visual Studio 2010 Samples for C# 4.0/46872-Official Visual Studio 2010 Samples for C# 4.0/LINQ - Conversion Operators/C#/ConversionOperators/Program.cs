using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace ConversionOperators
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            //Comment or uncomment the method calls below to run or not

              samples.Linq54(); // This sample uses ToArray to immediately evaluate a sequence into an array

            //samples.Linq55(); // This sample uses ToList to immediately evaluate a sequence into a List<T>

            //samples.Linq56(); // This sample uses ToDictionary to immediately  evaluate a  sequence  and a 
                                // related key expression into a dictionary

            //samples.Linq57(); // This sample uses OfType to return only the elements of the array that are 
                                // of type double
        }

        class LinqSamples
        {
            [Category("Conversion Operators")]
            [Description("This sample uses ToArray to immediately evaluate a sequence into an array.")]
            public void Linq54()
            {
                double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };

                var sortedDoubles =
                    from d in doubles
                    orderby d descending
                    select d;
                var doublesArray = sortedDoubles.ToArray();

                Console.WriteLine("Every other double from highest to lowest:");
                for (int d = 0; d < doublesArray.Length; d += 2)
                {
                    Console.WriteLine(doublesArray[d]);
                }
            }

            [Category("Conversion Operators")]
            [Description("This sample uses ToList to immediately evaluate a sequence into a List<T>.")]
            public void Linq55()
            {
                string[] words = { "cherry", "apple", "blueberry" };

                var sortedWords =
                    from w in words
                    orderby w
                    select w;
                var wordList = sortedWords.ToList();

                Console.WriteLine("The sorted word list:");
                foreach (var w in wordList)
                {
                    Console.WriteLine(w);
                }
            }

            [Category("Conversion Operators")]
            [Description("This sample uses ToDictionary to immediately evaluate a sequence and a " +
                        "related key expression into a dictionary.")]
            public void Linq56()
            {
                var scoreRecords = new[] { new {Name = "Alice", Score = 50},
                                        new {Name = "Bob"  , Score = 40},
                                        new {Name = "Cathy", Score = 45}
                                    };

                var scoreRecordsDict = scoreRecords.ToDictionary(sr => sr.Name);

                Console.WriteLine("Bob's score: {0}", scoreRecordsDict["Bob"]);
            }

            [Category("Conversion Operators")]
            [Description("This sample uses OfType to return only the elements of the array that are of type double.")]
            public void Linq57()
            {
                object[] numbers = { null, 1.0, "two", 3, "four", 5, "six", 7.0 };

                var doubles = numbers.OfType<double>();

                Console.WriteLine("Numbers stored as doubles:");
                foreach (var d in doubles)
                {
                    Console.WriteLine(d);
                }
            }
        }
    }
}
