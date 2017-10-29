using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryOptimization.MetaHeuristics
{
    public class TabuSearch : StochasticHillClimber
    {
        protected Dictionary<int, int> mTabuList = new Dictionary<int, int>();
        protected int mTabuDuration;

        public TabuSearch(int[] masks, int tabu_duration)
            : base(masks)
        {
            mTabuDuration = tabu_duration;
        }

        protected virtual void TabuMove(int move_id)
        {
            mTabuList[move_id] = mTabuDuration;
        }

        protected virtual void LowerTabuList()
        {
            List<int> tabu_move_id_list = mTabuList.Keys.ToList();
            for (int i = 0; i < tabu_move_id_list.Count; ++i)
            {
                int tabu_move_id = tabu_move_id_list[i];
                mTabuList[tabu_move_id] -= 1;
                if (mTabuList[tabu_move_id] <= 0)
                {
                    mTabuList.Remove(tabu_move_id);
                }
            }
        }

        public virtual bool IsMoveTabu(int move_id)
        {
            if (mTabuList.ContainsKey(move_id))
            {
                return mTabuList[move_id] > 0;
            }
            return false;
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
                int[] best_x_in_neighborhood = null;
                double best_x_in_neighborhood_fx = 0;
                int move_id = -1;
                for (int i = 0; i < x.Length; ++i)
                {
                    if (!IsMoveTabu(i))
                    {
                        int[] x_pi = GetNeighbor(x, i, constraints);
                        double fx_pi = evaluate(x_pi, constraints);

                        if (fx_pi < fx)
                        {
                            best_x_in_neighborhood = x_pi;
                            best_x_in_neighborhood_fx = fx_pi;
                            move_id = i;
                        }
                    }
                }

                if (best_x_in_neighborhood != null)
                {
                    if (best_solution.TryUpdateSolution(best_x_in_neighborhood, best_x_in_neighborhood_fx, out improvement))
                    {
                        TabuMove(move_id);
                        OnSolutionUpdated(best_solution, iteration);
                    }
                }

                LowerTabuList();

                OnStepped(best_solution, iteration);
                iteration++;
            }

            return best_solution;
        }
    }
}
