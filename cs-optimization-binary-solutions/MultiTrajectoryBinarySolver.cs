using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization
{
    public abstract class MultiTrajectoryBinarySolver : BinarySolver
    {
        public int MaxIterations { get; set; } = 200;

        public abstract BinarySolution Minimize(CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null);

        public BinarySolution Minimize(CostEvaluationMethod evaluate, object constraint = null)
        {
            return this.Minimize(evaluate, (improvement, iterations) =>
            {
                return iterations >= MaxIterations;
            }, constraint);
        }
    }
}
