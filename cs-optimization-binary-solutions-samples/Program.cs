using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneticAlgorithm_UT.RunMain(args);
            MemeticAlgorithm_UT.RunMain(args);
            StochasticHillClimber_UT.RunMain(args);
            IteratedLocalSearch_UT.RunMain(args);
        }
    }
}
