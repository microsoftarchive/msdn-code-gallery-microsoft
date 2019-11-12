using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class DigitSeperatorSample
    {
        public static void Run()
        {
            // integer
            int i = 1000;
            int seperatedInteger = 1_000;
            Console.WriteLine($"Seperate integer: {i == seperatedInteger}");

            // float
            float f = 1.234f;
            float seperatedFloat = 1.2_34f;
            Console.WriteLine($"Seperate float: {f == seperatedFloat}");

            // hex
            var x = 0xFF00AA;
            var seperatedX = 0xFF_00_AA;
            Console.WriteLine($"Seperate hex: {x == seperatedX}");

            // binary
            var b = 0b100010100001;
            var seperatedB = 0b1000_1010_0001;
            Console.WriteLine($"Seperate binary: {b == seperatedB}");
        }
    }
}
