using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class OutVariablesSample
    {
        public static void Run()
        {
            // declare out variables in function call, and use those variables in the same scope
            if (GetXY(out int i, out int j)) {
                Console.WriteLine($"x: {i}, y: {j}");
            }
        }

        static bool GetXY(out int x, out int y)
        {
            x = 0;
            y = 1;
            return true;
        }
    }
}
