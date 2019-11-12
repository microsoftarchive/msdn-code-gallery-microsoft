using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class TupleSample
    {
        public static void Run()
        {
            var tupleName = SplitFullName("Black Smith");
            Console.WriteLine($"FirstName: {tupleName.FirstName}, MiddleName: {tupleName.MiddleName}, LastName: {tupleName.LastName}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns>Return value is named tuple</returns>
        static (string FirstName, string MiddleName, string LastName) SplitFullName(string fullName)
        {
            var names = fullName.Split(' ', '\t');
            switch(names.Length) {
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
}
