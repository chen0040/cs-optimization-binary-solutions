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

        public int MaxLocalSearchIterations { get; set; } = 20;

        protected SingleTrajectoryBinarySolver mLocalSearch = new BitFlipLocalSearch();
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

        public IteratedLocalSearch(int dimension)
        {
            mMasks = new int[dimension];

            for(int i=0; i < mMasks.Length; ++i)
            {
                mMasks[i] = RandomEngine.NextBoolean() ? 1 : 0;
            }

            mLocalSearchShouldTerminate = (improvement, iterations) =>
            {
                return iterations >= MaxLocalSearchIterations;
            };

            
            mSolutionGenerator = (x, index, constraints) =>
            {
                int[] x_p = (int[])x.Clone();
                for (int i = 0; i < mMasks.Length; ++i)
                {
                    int j = (index + i) % x.Length;
                    x_p[(index + i) % x_p.Length] = mMasks[i] == 1 ? x[j] = 1 - x[j] : x[j];
                }
                return x_p;
            };
        }

        public IteratedLocalSearch(int[] masks)
        {
            mLocalSearchShouldTerminate = (improvement, iterations) =>
            {
                return iterations >= MaxLocalSearchIterations;
            };

            mMasks = (int[])masks.Clone();
            mSolutionGenerator = (x, index, constraints) =>
                {
                    int[] x_p = (int[])x.Clone();
                    for (int i = 0; i < mMasks.Length; ++i)
                    {
                        int j = (index + i) % x.Length;
                        x_p[(index + i) % x_p.Length] = mMasks[i] == 1 ? x[j] = 1 - x[j] : x[j];
                    }
                    return x_p;
                };
        }

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
                        int j = (index + i) % x.Length;
                        x_p[(index + i) % x_p.Length] = mMasks[i] == 1 ? x[j] = 1 - x[j] : x[j];
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
