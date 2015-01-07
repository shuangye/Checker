using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Threading.Helpers
{
    static class ArrayExtensions
    {
        public static T[] Shuffle<T>(this T[] array)
        {
            Random random = new Random(Environment.TickCount);
            for (int i = array.Length - 1; i > 0; --i )
            {
                int j = random.Next() % (i + 1);
                var temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
            return array;
        }

        public static int[] Negative(this int[] array)
        {
            for (int i = 0; i < array.Length; ++i)
                array[i] = -array[i];
            return array;
        }
    }
}
