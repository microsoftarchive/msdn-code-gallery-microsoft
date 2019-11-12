using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class LocalFunctionsSample
    {
        public static void Run()
        {
            void print(string content)
            {
                Console.WriteLine($"Call the local function to print \"{content}\"");
            }

            print("Hello Local Function");
        }
    }
}
