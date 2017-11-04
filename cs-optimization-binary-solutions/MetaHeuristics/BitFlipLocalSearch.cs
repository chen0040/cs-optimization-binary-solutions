using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryOptimization.MetaHeuristics
{
    public class BitFlipLocalSearch : SingleTrajectoryBinarySolver
    {
        public override BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            BinarySolution x = new BinarySolution(x_0, evaluate(x_0, constraints));
            int iterations = 0;
            for(int i=0; i < x_0.Length; ++i)
            {
                BinarySolution xPrime = (BinarySolution)x.Clone();
                xPrime.FlipBit(i);
                xPrime.Cost = evaluate(x.Values, constraints);
                double improvement = x.Cost - xPrime.Cost;
                if(xPrime.IsBetterThan(x))
                {
                    x = xPrime;
                    break;
                }
                iterations++;
                if(should_terminate(improvement, iterations))
                {
                    break;
                }
            }
            return x;
        }
    }
}
