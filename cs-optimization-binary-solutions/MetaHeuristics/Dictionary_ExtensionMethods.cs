using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    public static class Dictionary_ExtensionMethods
    {
        public static void Inc<T>(this Dictionary<T, double> data, T key, double inc_value)
        {
            if (data.ContainsKey(key))
            {
                data[key] += inc_value;
            }
            else
            {
                data[key] = inc_value;
            }
        }
    }
}
