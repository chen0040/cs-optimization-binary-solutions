using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    /// <summary>
    /// VNS implements Variable Neighborhood Search
    /// </summary>
    public class VNS : SingleTrajectoryBinarySolver
    {
        protected LocalSearchChain mNeighborhoods = new LocalSearchChain();

        public void AddNeighborhood(SingleTrajectoryBinarySolver local_search, SingleTrajectoryBinarySolver.TerminationEvaluationMethod termination_condition)
        {
            local_search.Stepped += (solution, iteration) =>
                {
                    OnStepped(solution, iteration);
                };
            local_search.SolutionUpdated += (solution, iteration) =>
                {
                    OnSolutionUpdated(solution, iteration);
                };
            mNeighborhoods.Add(local_search, termination_condition);
        }

        public override BinarySolution Minimize(int[] x_0, CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            double? improvement = null;
            int iteration = 0;

            int[] x = (int[])x_0.Clone();
            double fx = evaluate(x, constraints);

            BinarySolution best_solution = new BinarySolution(x, fx);

            int neighborhood_count = mNeighborhoods.Count;

            BinarySolution current_best = null;

            while (!should_terminate(improvement, iteration))
            {
                for (int l = 0; l < neighborhood_count; ++l)
                {
                    SingleTrajectoryBinarySolver local_search = mNeighborhoods.GetLocalSearchAt(l);
                    SingleTrajectoryBinarySolver.TerminationEvaluationMethod termination_condition = mNeighborhoods.GetTerminationConditionAt(l);

                    current_best = local_search.Minimize(x, evaluate, termination_condition, constraints);

                    x = current_best.Values;
                    fx = current_best.Cost;

                    if (best_solution.TryUpdateSolution(x, fx, out improvement))
                    {
                        OnSolutionUpdated(best_solution, iteration);
                    }

                    OnStepped(best_solution, iteration);
                    iteration++;
                }
            }

            return best_solution;

        }
    }
}
