using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class DeconstructionSample
    {
        public static void Run()
        {
            // deconstruct for class
            var point = new My3DPoint(2, 3, 5);
            (int x, int y, int z) = point;
            Console.WriteLine($"Deconstruct on \"My3DPoint\" instance. x: {x}, y: {y}, z: {z}");

            // deconstruct for function tuple return value
            (string a, string b, string c) = SplitFullName("Black Big Smith");
            Console.WriteLine($"Deconstruct on \"SplitFullName\" return value is a: {a}, b: {b}, c: {c}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>Return value is named tuple</returns>
        static (string FirstName, string MiddleName, string LastName) SplitFullName(string fullName)
        {
            var names = fullName.Split(' ', '\t');
            switch (names.Length) {
                case 2: {
                        return (FirstName: names[0], MiddleName: string.Empty, LastName: names[1]);
                    }
                case 3: {
                        return (FirstName: names[0], MiddleName: names[1], LastName: names[2]);
                    }
                default: {
                        return (FirstName: fullName, MiddleName: string.Empty, LastName: string.Empty);
                    }
            }
        }
    }

    class My3DPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public My3DPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Only class declaring "Deconstruct" method will support deconstruct
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Deconstruct(out int x, out int y, out int z)
        {
            x = X;
            y = Y;
            z = Z;
        }
    }
}
