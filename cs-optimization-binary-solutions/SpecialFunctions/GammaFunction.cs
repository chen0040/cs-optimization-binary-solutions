using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryOptimization.Helpers;

namespace BinaryOptimization.SpecialFunctions
{
    public class GammaFunction
    {
        public static double GetGamma(double x)
        {
            return System.Math.Exp(Gamma.Log(x));
        }
    }
}
