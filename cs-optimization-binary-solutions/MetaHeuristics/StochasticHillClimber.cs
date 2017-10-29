using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    public class StochasticHillClimber : SingleTrajectoryBinarySolver
    {
        public delegate int[] CreateNeighborMethod(int[] x, int index, object constraints);
        protected CreateNeighborMethod mSolutionGenerator;

        public int[] mMasks;

        public StochasticHillClimber(int[] masks, CreateNeighborMethod generator = null)
        {
            mMasks = (int[])masks.Clone();

            mSolutionGenerator=generator;

            if(mSolutionGenerator==null)
            {
                mSolutionGenerator=(x, index, constraints)=>
                    {
                        int[] x_p = (int[])x.Clone();
                        for (int i = 0; i < mMasks.Length; ++i)
                        {
                            x_p[(index+i) % x_p.Length]= mMasks[i]==1 ? x[(index+i)]=1-x[(index+i) % x.Length] : x[(index+i) % x.Length];
                        }
                        return x_p;
                    };
            }
        }

        public int[] GetNeighbor(int[] x, int index, object constraints)
        {
            return mSolutionGenerator(x, index, constraints);
        }

        public override BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            double? improvement = null;
            int iteration = 0;

            double fx_0 = evaluate(x_0, constraints);
            BinarySolution best_solution = new BinarySolution(x_0, fx_0);

            int dimension = x_0.Length;

            while (!should_terminate(improvement, iteration))
            {
                int[] best_x_in_neighborhood = null;
                double best_x_in_neighborhood_fx = double.MaxValue;
                for (int i = 0; i < dimension; ++i)
                {
                    int[] x_pi = GetNeighbor(best_solution.Values, i, constraints);
                    double fx_pi = evaluate(x_pi, constraints);

                    if (fx_pi < best_x_in_neighborhood_fx)
                    {
                        best_x_in_neighborhood = x_pi;
                        best_x_in_neighborhood_fx = fx_pi;
                    }
                }

                if (best_x_in_neighborhood != null)
                {
                    if (best_solution.TryUpdateSolution(best_x_in_neighborhood, best_x_in_neighborhood_fx, out improvement))
                    {
                        OnSolutionUpdated(best_solution, iteration);
                    }
                }

                OnStepped(best_solution, iteration);
                iteration++;
            }

            return best_solution;
        }
    }
}
