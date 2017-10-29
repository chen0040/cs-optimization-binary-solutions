using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryOptimization.ProblemModels;

namespace BinaryOptimization.ComponentModels
{
    public interface ISolution
    {
        ISolution Clone();
    }
}
