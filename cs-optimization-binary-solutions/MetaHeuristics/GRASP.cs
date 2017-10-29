using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    /// <summary>
    /// GRASP implements Greedy Randomized Adaptive Search Program
    /// </summary>
    public class GRASP : MultiTrajectoryBinarySolver
    {
        protected TerminationEvaluationMethod mLocalSearchTerminationCondition;

        public delegate int[] GreedyConstructSolutionMethod(CostEvaluationMethod evaluate, object constraints);
        public GreedyConstructSolutionMethod mGreedySolutionConstructor;

        protected SingleTrajectoryBinarySolver mLocalSearch;
        public SingleTrajectoryBinarySolver LocalSearch
        {
            get { return mLocalSearch; }
            set { mLocalSearch = value; }
        }

        protected int mDimension;
        protected int[] mMasks;
        public GRASP(int[] masks, int dimension, SingleTrajectoryBinarySolver local_search, TerminationEvaluationMethod local_search_termination_condition, GreedyConstructSolutionMethod solution_constructor)
        {
            mDimension=dimension;

            mLocalSearch = local_search;
            mLocalSearchTerminationCondition = local_search_termination_condition;

            if (mLocalSearch == null || mLocalSearchTerminationCondition == null)
            {
                throw new NullReferenceException();
            }

            mMasks = (int[])masks.Clone();
            mGreedySolutionConstructor = solution_constructor;
            if (mGreedySolutionConstructor == null)
            {
                mGreedySolutionConstructor = (evaluate, constraints) =>
                    {
                        int[] solution = new int[mDimension];
                        for (int i = 0; i < solution.Length; ++i)
                        {
                            solution[i] = -1;
                        }
                        for (int i = 0; i < mDimension; ++i)
                        {
                            int best_feature_value = -1;
                            double min_cost = double.MaxValue;
                            for (int bit_value = 0; bit_value < 2; ++bit_value)
                            {
                                solution[i] = bit_value;
                                
                                double cost = evaluate(solution, constraints); //evaluate partial solution
                                if (min_cost > cost)
                                {
                                    min_cost = cost;
                                    best_feature_value = bit_value;
                                }
                            }
                        }
                        return solution;
                    };
            }
        }


        protected virtual int[] GreedyConstructRandomSolution(CostEvaluationMethod evaluate, object constraints)
        {
            return mGreedySolutionConstructor(evaluate, constraints);
        }

        public override BinarySolution Minimize(CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            double? improvement = null;
            int iteration = 0;

            BinarySolution best_solution = new BinarySolution();

            while (!should_terminate(improvement, iteration))
            {
                int[] x = GreedyConstructRandomSolution(evaluate, constraints);
                
                BinarySolution x_pi_refined = mLocalSearch.Minimize(x, evaluate, mLocalSearchTerminationCondition, constraints);

                if (best_solution.TryUpdateSolution(x_pi_refined.Values, x_pi_refined.Cost, out improvement))
                {
                    OnSolutionUpdated(best_solution, iteration);
                }

                OnStepped(best_solution, iteration);
                iteration++;
            }

            return best_solution;
        }
    }
}
