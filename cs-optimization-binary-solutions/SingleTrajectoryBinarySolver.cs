using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization
{
    public abstract class SingleTrajectoryBinarySolver : BinarySolver
    {
        public int MaxIterations { get; set; } = 500;
        public int Dimension { get; set; } = 1000;
        public abstract BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null);
        public BinarySolution Minimize(CostEvaluationMethod evaluate, object constraints = null)
        {
            int[] x_0 = new int[Dimension];
            for(int i=0; i < x_0.Length; ++i)
            {
                x_0[i] = RandomEngine.NextBoolean() ? 1 : 0;
            }

            return Minimize(x_0, evaluate, (improvement, iterations) =>
            {
                return iterations >= MaxIterations;
            }, constraints);
        }

        public BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, object constraints = null)
        {
            return Minimize(x_0, evaluate, (improvement, iterations) =>
            {
                return iterations >= MaxIterations;
            }, constraints);
        }
    }
}
