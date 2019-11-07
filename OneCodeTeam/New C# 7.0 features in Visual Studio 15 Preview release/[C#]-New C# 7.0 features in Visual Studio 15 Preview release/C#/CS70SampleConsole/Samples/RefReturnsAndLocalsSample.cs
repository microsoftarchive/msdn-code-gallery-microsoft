using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS70SampleConsole
{
    class RefReturnsAndLocalsSample
    {
        public static void Run()
        {
            int[] array = { 2, 3, 5, 7, 11, 13, 17 };

            ref int val1 = ref Search(2, array);
            ref int val2 = ref Search(7, array);
            Swap(ref val1, ref val2);

            StringBuilder sb = new StringBuilder();
            array.ToList().ForEach((i) => { sb.Append($"{i} "); });
            Console.WriteLine(sb.ToString());
        }

        // the return value is ref type
        static ref int Search(int targetNumber, int[] array)
        {
            for (int i = 0; i < array.Length; ++i) {
                if (array[i] == targetNumber)
                    return ref array[i];
            }
            throw new Exception("Not found");
        }

        static void Swap(ref int val1, ref int val2)
        {
            int tmp;
            tmp = val1;
            val1 = val2;
            val2 = tmp;
        }
    }
}
