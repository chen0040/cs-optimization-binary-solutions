using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization
{
    public class RandomSearch : SingleTrajectoryBinarySolver
    {
        protected int mSearchSpaceSize = -1;
        public delegate int[] CreateRandomSolutionMethod(object constraints);
        protected CreateRandomSolutionMethod mSolutionGenerator;

        public RandomSearch(CreateRandomSolutionMethod generator, int search_space_size =-1)
        {
            mSearchSpaceSize = search_space_size;
            mSolutionGenerator = generator;
        }

        public int[] CreateRandomSolution(CreateRandomSolutionMethod generator, object constraints)
        {
            return generator(constraints);
        }

        public override BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            double? improvement = null;
            int iteration = 0;

            if (mSearchSpaceSize == -1)
            {
                mSearchSpaceSize = x_0.Length;
            }

            double fx_0 = evaluate(x_0, constraints);
            BinarySolution best_solution = new BinarySolution(x_0, fx_0);

            while (!should_terminate(improvement, iteration))
            {
                int[] best_x_in_neighborhood = null;
                double best_x_in_neighborhood_fx = double.MaxValue; ;
                for (int i = 0; i < mSearchSpaceSize; ++i)
                {
                    int[] x_pi = CreateRandomSolution(mSolutionGenerator, constraints);
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
