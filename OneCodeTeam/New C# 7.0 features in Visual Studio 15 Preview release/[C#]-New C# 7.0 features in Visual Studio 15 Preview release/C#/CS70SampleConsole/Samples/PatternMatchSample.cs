using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class PatternMatchSample
    {
        public static void Run()
        {
            IsExpressionWithPattern();
            SwitchStatementsWithPattern();
        }

        static void IsExpressionWithPattern()
        {
            var content = "Hello pattern";

            // 1. type pattern
            if (content is string str) {
                Console.WriteLine($"Is expression with type pattern, value is \"{str}\"");
            }
            // 2. constant pattern
            if (content is "Hello pattern") {
                Console.WriteLine($"Is expression with constant pattern, constant check successfully");
            }
            // 3. var pattern
            if (content is var j) {
                Console.WriteLine($"Is expression with var pattern, variable identifier change to \"j\" and value is \"{j}\"");
            }
        }

        static void SwitchStatementsWithPattern()
        {
            var content = "Hello pattern";

            // 1. type pattern
            switch (content) {
                case string str: {
                        Console.WriteLine($"Switch expression with type pattern, value is \"{str}\"");
                        break;
                    }
                case null: {
                        break;
                    }
            }
            // 2. constant pattern
            switch (content) {
                case "Hello pattern": {
                        Console.WriteLine($"Switch expression with constant pattern, constant check successfully");
                        break;
                    }
                case null: {
                        break;
                    }
            }
            // 3. var pattern
            switch (content) {
                case var j: {
                        Console.WriteLine($"Switch expression with var pattern, variable identifier change to \"j\" and value is \"{j}\"");
                        break;
                    }
            }
        }
    }
}
