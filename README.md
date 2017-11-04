# cs-optimization-binary-solutions

Local search optimization for binary-coded solutions implemented in C#

# Install

```bash
Install-Package cs-optimization-binary-solutions -Version 1.0.1
```

# Features

The following meta-heuristic algorithms are provided for binary optimization (Optimization in which the solutions are binary-coded):

* Genetic Algorithm
* Memetic Algorithm
* GRASP
* Multi-start Hill Climbing
* Tabu Search
* Variable Neighbhorhood Search 
* Iterated Local Search 
* Random Search 

# Usage

The code below shows how to use Genetic Algorithm to solve an optimization problem that looks for the binary-coded solution with minimum number of 1 bits:

```cs 
int popSize = 100;
int dimension = 1000; // solution has 1000 bits
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
```


The code below shows how to use Memetic Algorithm to solve an optimization problem that looks for the binary-coded solution with minimum number of 1 bits:

```cs 
int popSize = 100;
int dimension = 1000; // solution has 1000 bits
MemeticAlgorithm method = new MemeticAlgorithm(popSize, dimension);
method.MaxIterations = 10;
method.MaxLocalSearchIterations = 1000;

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
```

The code below shows how to use Stochastic Hill Climber to solve an optimization problem that looks for the binary-coded solution with minimum number of 1 bits:

```cs 
int dimension = 1000; // solution has 1000 bits
StochasticHillClimber method = new StochasticHillClimber(dimension);
method.MaxIterations = 100;

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
```

The code below shows how to use Iterated Local Search to solve an optimization problem that looks for the binary-coded solution with minimum number of 1 bits:

```cs 
int dimension = 1000; // solution has 1000 bits
IteratedLocalSearch method = new IteratedLocalSearch(dimension);
method.MaxIterations = 1000;

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
```


