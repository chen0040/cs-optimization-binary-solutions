using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinaryOptimization.MetaHeuristics;

namespace BinaryOptimization
{
    class GeneticAlgorithm_UT
    {
        public static void RunMain(string[] args)
        {
            int popSize = 100;
            int dimension = 1000;
            GeneticAlgorithm method = new GeneticAlgorithm(popSize, dimension);
            method.MaxIterations = 500;

            method.SolutionUpdated += (best_solution, step) =>
            {
                Console.WriteLine("Step {0}: Fitness = {1}", step, best_solution.Cost);
            };

            BinarySolution finalSolution = method.Minimize((solution, constraints) =>
            {
                int num1Bits = 0;
                for(int i=0; i < solution.Length; ++i)
                {
                    num1Bits += solution[i];
                }
                return num1Bits; // try to minimize the number of 1 bits in the solution
            });
            Console.WriteLine("final solution: {0}", finalSolution);
        }
    }
}
