using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BinaryOptimization.MetaHeuristics
{
    public class MemeticAlgorithm :  MultiTrajectoryBinarySolver
    {
        public delegate int[] CreateSolutionMethod(int dimension, object constraints);
        protected CreateSolutionMethod mSolutionGenerator;
        protected int mPopSize;
        protected int mDimension;
        protected double mCrossoverRate = 0.9;
        protected double mMutationRate = 0.1;
        protected SingleTrajectoryBinarySolver mLocalSearch = null;
        protected TerminationEvaluationMethod mLocalSearchTerminationCondition = null;
        public int MaxLocalSearchIterations { get; set; } = 100;

        public MemeticAlgorithm(int pop_size, int dimension, SingleTrajectoryBinarySolver local_search)
        {
            mDimension = dimension;
            mPopSize = pop_size;
            mMutationRate = 1.0 / dimension;
            mSolutionGenerator = (dimen, constraints) =>
            {
                int[] solution = new int[dimen];
                for (int i = 0; i < dimen; ++i)
                {
                    solution[i] = RandomEngine.NextBoolean() ? 1 : 0;
                }
                return solution;
            };

            mLocalSearch = local_search;
            mLocalSearchTerminationCondition = (improvement, iterations)=>
            {
                return iterations >= MaxLocalSearchIterations;
            };
            
        }

        public MemeticAlgorithm(int pop_size, int dimension)
        {
            mDimension = dimension;
            mPopSize = pop_size;
            mMutationRate = 1.0 / dimension;
            mSolutionGenerator = (dimen, constraints) =>
            {
                int[] solution = new int[dimen];
                for (int i = 0; i < dimen; ++i)
                {
                    solution[i] = RandomEngine.NextBoolean() ? 1 : 0;
                }
                return solution;
            };
            int[] masks = new int[dimension];
            for(int i=0; i < masks.Length; ++i)
            {
                masks[i] = RandomEngine.NextBoolean() ? 1 : 0;
            }
            mLocalSearchTerminationCondition = (improvement, iterations) =>
            {
                return iterations >= MaxLocalSearchIterations;
            };
            mLocalSearch = new BitFlipLocalSearch();
            

        }


        public MemeticAlgorithm(int pop_size, int dimension, CreateSolutionMethod solution_generator, SingleTrajectoryBinarySolver local_search, TerminationEvaluationMethod local_search_termination_condition)
        {
            mDimension = dimension;
            mPopSize = pop_size;
            mMutationRate = 1.0 / dimension;
            mSolutionGenerator = solution_generator;
            mLocalSearch = local_search;
            mLocalSearchTerminationCondition = local_search_termination_condition;

            if (mSolutionGenerator == null || mLocalSearch == null || mLocalSearchTerminationCondition == null)
            {
                throw new NullReferenceException();
            }
        }

        protected BinarySolution[] SelectParents(BinarySolution[] pop)
        {
            BinarySolution[] parents = new BinarySolution[2];
            for (int i = 0; i < 2; ++i)
            {
                int index1 = RandomEngine.NextInt(pop.Length);
                int index2 = index1;
                do
                {
                    index2 = RandomEngine.NextInt(pop.Length);
                } while (index1 == index2);
                if (pop[index1].Cost < pop[index2].Cost)
                {
                    parents[i] = pop[index1];
                }
                else
                {
                    parents[i] = pop[index2];
                }
            }

            return parents;
        }

        protected BinarySolution CloneParent(BinarySolution[] pop)
        {
            int index1 = RandomEngine.NextInt(pop.Length);
            int index2 = index1;
            do
            {
                index2 = RandomEngine.NextInt(pop.Length);
            } while (index1 == index2);
            if (pop[index1].Cost < pop[index2].Cost)
            {
                return pop[index1].Clone() as BinarySolution;
            }
            else
            {
                return pop[index2].Clone() as BinarySolution;
            }
        }

        public BinarySolution Crossover(BinarySolution[] parents)
        {
            int[] x = new int[mDimension];
            int r = RandomEngine.NextInt(mDimension);
            for (int i = 0; i < r; ++i)
            {
                x[i] = parents[0][i];
            }
            for (int i = r; i < mDimension; ++i)
            {
                x[i] = parents[1][i];
            }
            return new BinarySolution(x, double.MaxValue);
        }

        public void Mutate(BinarySolution s)
        {
            for (int i = 0; i < s.Length; ++i)
            {
                if (RandomEngine.NextDouble() < mMutationRate)
                {
                    s.FlipBit(i);
                }
            }
        }

        public override BinarySolution Minimize(CostEvaluationMethod evaluate, TerminationEvaluationMethod should_terminate, object constraints = null)
        {
            double? improvement = null;
            int iteration = 0;

            BinarySolution[] pop = new BinarySolution[mPopSize];
            BinarySolution[] children = new BinarySolution[mPopSize];
            BinarySolution[] union = new BinarySolution[mPopSize * 2];

            double min_cost = double.MaxValue;
            int best_index = 0;
            for (int i = 0; i < mPopSize; ++i)
            {
                int[] x = mSolutionGenerator(mDimension, constraints);
                double fx = evaluate(x, constraints);
                pop[i] = new BinarySolution(x, fx);
                if (min_cost > fx)
                {
                    min_cost = fx;
                    best_index = i;
                }
            }

            BinarySolution best_solution = pop[best_index].Clone() as BinarySolution;


            while (!should_terminate(improvement, iteration))
            {
                for (int i = 0; i < mPopSize; ++i)
                {
                    if (RandomEngine.NextDouble() < mCrossoverRate)
                    {
                        BinarySolution[] parents = SelectParents(pop);
                        children[i] = Crossover(parents);
                    }
                    else
                    {
                        children[i] = CloneParent(pop);
                    }
                    Mutate(children[i]);

                    children[i] = mLocalSearch.Minimize(children[i].Values, evaluate, mLocalSearchTerminationCondition, constraints);
                }

                for (int i = 0; i < mPopSize; ++i)
                {
                    union[i] = pop[i];
                }
                for (int i = 0; i < mPopSize; ++i)
                {
                    union[i + mPopSize] = children[i];
                }

                union = union.OrderBy(s => s.Cost).ToArray();

                for (int i = 0; i < mPopSize; ++i)
                {
                    pop[i] = union[i];
                }

                if (best_solution.TryUpdateSolution(pop[0].Values, pop[0].Cost, out improvement))
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
