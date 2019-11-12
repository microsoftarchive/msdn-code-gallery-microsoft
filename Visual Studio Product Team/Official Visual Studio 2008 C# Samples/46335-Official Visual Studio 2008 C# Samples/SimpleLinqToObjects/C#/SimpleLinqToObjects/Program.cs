using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleLinqToObjects
{
    class SimpleLinqToObjects
    {

        /// <summary>
        /// This example demonstrates a bare bones LINQ to Objects
        /// program. Note that "using System.Linq" is included in the
        /// using clause. Included in this code are examples of:
        ///     
        ///     * A Collection Initializer
        ///     * A Query Expression
        ///     * Type Inference        
        /// </summary>
        /// <param name="args"></param>
        static void Main()
        {
            // A C# 3.0 Collection Initializer
            List<int> numberList = new List<int> { 1, 2, 3, 4 };

            // The var keyword in this Query Expression demonstrates
            // how to use Type Inference. Place your cursor over the word
            // var to see the type of the identifier called query.
            var query = from i in numberList
                        where i < 4
                        select i;

            // Iterate over the IEnumerable type returned by the query.
            foreach (var number in query)
            {
                Console.WriteLine(number);
            }
            Console.ReadLine();
        }
    }
}
