using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    public class MultiStartHillClimber : MultiTrajectoryBinarySolver
    {
        public delegate int[] CreateRandomSolutionMethod(int index, object constraints);
        public CreateRandomSolutionMethod mSolutionGenerator;

        protected TerminationEvaluationMethod mLocalSearchTerminationCondition;

        protected SingleTrajectoryBinarySolver mLocalSearch;
        public SingleTrajectoryBinarySolver LocalSearch
        {
            get { return mLocalSearch; }
            set { mLocalSearch = value; }
        }

        protected int[] mMasks;
        public MultiStartHillClimber(int[] masks, SingleTrajectoryBinarySolver local_search, TerminationEvaluationMethod local_search_should_terminate, CreateRandomSolutionMethod generator)
        {
            mLocalSearchTerminationCondition = local_search_should_terminate;
            mLocalSearch = local_search;
            mSolutionGenerator = generator;

            if (mSolutionGenerator == null || mLocalSearch == null || mLocalSearchTerminationCondition==null)
            {
                throw new NullReferenceException();
            }

            mMasks = (int[])masks.Clone();
        }

        public int[] CreateRandomSolution(int index, object constraints)
        {
            return mSolutionGenerator(index, constraints);
        }

        public override BinarySolution Minimize(CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            double? improvement = null;
            int iteration = 0;
            
            BinarySolution best_solution = new BinarySolution();

            while (!should_terminate(improvement, iteration))
            {
                int[] x_pi = CreateRandomSolution(iteration, constraints);
                double fx_pi = evaluate(x_pi, constraints);

                BinarySolution x_pi_refined = mLocalSearch.Minimize(x_pi, evaluate, mLocalSearchTerminationCondition, constraints);

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
