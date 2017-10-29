using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization
{
    public abstract class SingleTrajectoryBinarySolver : BinarySolver
    {
        public abstract BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null);
    }
}
