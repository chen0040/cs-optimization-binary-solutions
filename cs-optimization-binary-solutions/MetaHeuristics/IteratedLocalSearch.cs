using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BinaryOptimization.MetaHeuristics
{
    public class IteratedLocalSearch : SingleTrajectoryBinarySolver
    {
        public delegate int[] CreateRandomNeighborhoodMethod(int[] x, int index, object constraints);
        public CreateRandomNeighborhoodMethod mSolutionGenerator;

        protected TerminationEvaluationMethod mLocalSearchShouldTerminate;

        protected SingleTrajectoryBinarySolver mLocalSearch;
        public SingleTrajectoryBinarySolver LocalSearch
        {
            get { return mLocalSearch; }
            set { mLocalSearch = value; }
        }

        public void DefineNeighborhood(SingleTrajectoryBinarySolver neighborhood, TerminationEvaluationMethod termination_condition)
        {
            mLocalSearch = neighborhood;
            mLocalSearchShouldTerminate = termination_condition;
        }


        protected int[] mMasks;
        public IteratedLocalSearch(int[] masks, TerminationEvaluationMethod local_search_should_terminate, CreateRandomNeighborhoodMethod generator = null)
        {
            mLocalSearchShouldTerminate = local_search_should_terminate;

            mMasks = (int[])masks.Clone();
            mSolutionGenerator = generator;
            if (mSolutionGenerator == null)
            {
                mSolutionGenerator = (x, index, constraints) =>
                {
                    int[] x_p = (int[])x.Clone();
                    for (int i = 0; i < mMasks.Length; ++i)
                    {
                        x_p[(index + i) % x_p.Length] = mMasks[i] == 1 ? x[(index + i)] = 1 - x[(index + i) % x.Length] : x[(index + i) % x.Length];
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
            int[] x = (int[])x_0.Clone();

            double fx = evaluate(x, constraints);
            BinarySolution best_solution = new BinarySolution(x, fx);

            while (!should_terminate(improvement, iteration))
            {
                int r_index = (int)(RandomEngine.NextDouble() * x.Length);
                int[] x_pi = GetNeighbor(x, r_index, constraints);
                double fx_pi = evaluate(x_pi, constraints);

                BinarySolution x_pi_refined = mLocalSearch.Minimize(x_pi, evaluate, mLocalSearchShouldTerminate, constraints);

                if (best_solution.TryUpdateSolution(x_pi_refined.Values, x_pi_refined.Cost, out improvement))
                {
                    OnSolutionUpdated(best_solution, iteration);
                }

                x = best_solution.Values;
                fx = best_solution.Cost;

                OnStepped(best_solution, iteration);
                iteration++;
            }

            return best_solution;
        }
    }
}
