using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BinaryOptimization.MetaHeuristics
{
    public class LearningClassifierSystem
    {
        public const char MaskChar = '#';

        public delegate int TargetFunctionMethod(string input);

        public double TestModel(TargetFunctionMethod target_function, int dimension, List<LearningClassifier> model, int num_trials = 50)
        {
            int correct = 0;
            for (int i = 0; i < num_trials; ++i)
            {
                string input = GenerateRandomInput(dimension);
                List<LearningClassifier> match_set = (from c in model where c.Match(input) select c).ToList();
                Dictionary<int, double> predition_array = GeneratePrediction(match_set);
                int predicted_action = SelectAction(predition_array, false);
                if (predicted_action == target_function(input))
                {
                    correct += 1;
                }
            }

            return (double)correct / num_trials;
        }

        public int Predict(List<LearningClassifier> model, string input)
        {
            List<LearningClassifier> match_set = (from c in model where c.Match(input) select c).ToList();
            Dictionary<int, double> predition_array = GeneratePrediction(match_set);
            int predicted_action = SelectAction(predition_array, false);
            return predicted_action;
        }

        public static int Negate(int bit)
        {
            return bit==1 ? 0 : 1;
        }

        public static void RunTest()
        {
            HashSet<int> all_actions = new HashSet<int>() { 0, 1 };
            int max_generations = 5000;
            int pop_size = 200;
            int ga_freq = 25;
            int dimension=6;
            LearningClassifierSystem system = new LearningClassifierSystem();
            system.Execute((input) =>
                {
                    int[] ints = new int[dimension];
                    for (int i = 0; i < dimension; ++i)
                    {
                        ints[i] = (input[i] == '1' ? 1 : 0);
                    }
                    int x0 = ints[0];
                    int x1 = ints[1];
                    int x2 = ints[2];
                    int x3 = ints[3];
                    int x4 = ints[4];
                    int x5 = ints[5];

                    return Negate(x0) * Negate(x1) * x2 + Negate(x0) * x1 * x3 + x0 * Negate(x1) * x4 + x0 * x1 * x5;

                },
                dimension,
                pop_size, max_generations, all_actions, ga_freq);
        }

        public void Execute(TargetFunctionMethod target_function, int dimension, int pop_size, int max_generations, HashSet<int> all_actions, int ga_freq)
        {
            List<LearningClassifier> model = TrainModel(target_function, dimension, pop_size, max_generations, all_actions, ga_freq);
            double accuracy = TestModel(target_function, dimension, model);
            Console.WriteLine("Classification Accuracy: {0}", accuracy);
        }

        public List<LearningClassifier> TrainModel(TargetFunctionMethod target_function, int dimension, int pop_size, int max_generations, HashSet<int> all_actions, int ga_freq)
        {
            List<LearningClassifier> pop = new List<LearningClassifier>();
            for (int generation = 0; generation < max_generations; ++generation)
            {
                bool explore = generation % 2 == 0;
                string input = GenerateRandomInput(dimension);
                List<LearningClassifier> match_set = GenerateMatchSet(pop, input, all_actions, generation);
                Dictionary<int, double> predictions = GeneratePrediction(match_set);
                int predicted_action = SelectAction(predictions, explore);
                double reward = 0;
                if (predicted_action == target_function(input))
                {
                    reward = 1000.0;
                }
                if (explore)
                {
                    LearningClassifier[] action_set = (from m in match_set where m.Action == predicted_action select m).ToArray();
                    UpdateSet(action_set, reward);
                    UpdateFitness(action_set);
                    if (CanRunGA(action_set, generation, ga_freq))
                    {
                        foreach (LearningClassifier c in action_set)
                        {
                            c.LastTime = generation;
                        }
                        RunGA(all_actions, pop, action_set, input, generation, pop_size);
                    }
                }
                else
                {
                    double error = System.Math.Abs(predictions[predicted_action] - reward);
                    int correct = (reward == 1000.0 ? 1 : 0);

                }
            }

            return pop;
        }

        protected LearningClassifier BinaryTournament(LearningClassifier[] action_set)
        {
            int index1 = RandomEngine.NextInt(action_set.Length);
            int index2 = index1;
            do
            {
                index2 = RandomEngine.NextInt(action_set.Length);
            } while (index1 == index2);

            if (action_set[index1].Fitness > action_set[index2].Fitness)
            {
                return action_set[index1];
            }
            return action_set[index2];
        }

        protected void RunGA(HashSet<int> all_actions, List<LearningClassifier> pop, LearningClassifier[] action_set, string input, int generation, int pop_size, double crossover_rate=0.8)
        {
            LearningClassifier p1 = BinaryTournament(action_set);
            LearningClassifier p2 = BinaryTournament(action_set);
            LearningClassifier c1, c2;
            if (RandomEngine.NextDouble() < crossover_rate)
            {
                Crossover(p1, p2, out c1, out c2);
            }
            else
            {
                c1 = p1.Clone();
                c2 = p2.Clone();

                c1.Num = 1.0;
                c2.Num = 1.0;
                c1.Exp = 0;
                c2.Exp = 0;
            }

            Mutate(c1, all_actions, input);
            Mutate(c2, all_actions, input);
            InsertIntoPop(c1, pop);
            InsertIntoPop(c2, pop);

            while (pop.Sum(c => c.Num) > pop_size)
            {
                DeleteFromPop(pop, pop_size);
            }
        }

        protected bool CanRunGA(LearningClassifier[] action_set, int generation, int ga_freq)
        {
            if (action_set.Length <= 2) return false;
            double total = action_set.Sum(c => (c.LastTime * c.Num));
            double sum = action_set.Sum(c => c.Num);
            if (generation - total / sum > ga_freq) return true;
            return false;
        }

        protected void Mutate(LearningClassifier cl, HashSet<int> all_actions, string input, double mutation_rate = 0.04)
        {
            int dimension = cl.Condition.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dimension; ++i)
            {
                if (RandomEngine.NextDouble() < mutation_rate)
                {
                    sb.Append((cl.Condition[i] == MaskChar) ? input[i] : MaskChar);
                }
                else
                {
                    sb.Append(cl.Condition[i]);
                }
            }

            cl.Condition = sb.ToString();

            if (RandomEngine.NextDouble() < mutation_rate)
            {
                List<int> subset = new List<int>();
                foreach (int action in all_actions)
                {
                    if (action != cl.Action)
                    {
                        subset.Add(action);
                    }
                }
                cl.Action = subset[RandomEngine.NextInt(subset.Count)];
            }
        }

        protected string UniformCrossover(string parent1, string parent2)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parent1.Length; ++i)
            {
                sb.AppendFormat("{0}", RandomEngine.NextBoolean() ? parent1[i] : parent2[i]);
            }
            return sb.ToString();
        }

        protected void Crossover(LearningClassifier p1, LearningClassifier p2, out LearningClassifier c1, out LearningClassifier c2)
        {
            string c1_condition = UniformCrossover(p1.Condition, p2.Condition);
            string c2_condition = UniformCrossover(p1.Condition, p2.Condition);

            c1 = new LearningClassifier(c1_condition, p1.Action, p1.LastTime);
            c2 = new LearningClassifier(c2_condition, p2.Action, p2.LastTime);

            c1.Num = 1.0;
            c2.Num = 1.0;
            c1.Exp = 0.0;
            c2.Exp = 0.0;

            c1.Pred = c2.Pred = (p1.Pred + p2.Pred) / 2.0;
            c1.Error = c2.Error = 0.25 * (p1.Error + p2.Error) / 2.0;
            c1.Fitness = c2.Fitness = 0.1 * (p1.Fitness + p2.Fitness) / 2.0;

        }

        protected void InsertIntoPop(LearningClassifier cla, List<LearningClassifier> pop)
        {
            foreach (LearningClassifier c in pop)
            {
                if (cla.Condition == c.Condition && cla.Action == c.Action)
                {
                    c.Num += 1;
                }
            }
            pop.Add(cla);
        }

        protected void UpdateSet(LearningClassifier[] action_set, double reward, double beta = 0.2)
        {
            double sum = action_set.Sum(c => c.Num);
            foreach (LearningClassifier c in action_set)
            {
                c.Exp += 1.0;
                if (c.Exp < 1.0 / beta)
                {
                    c.Error = c.Error * (c.Exp - 1.0) + System.Math.Abs(reward - c.Pred) / c.Exp;
                    c.Pred = (c.Pred * (c.Exp - 1.0) + reward) / c.Exp;
                    c.SetSize = (c.SetSize * (c.Exp - 1.0) + sum) / c.Exp;
                }
                else
                {
                    c.Error += beta * (System.Math.Abs(reward - c.Pred) - c.Error);
                    c.Pred += beta * (reward - c.Pred);
                    c.SetSize += beta * (sum - c.SetSize);
                }
            }
        }

        protected void UpdateFitness(LearningClassifier[] action_set, double min_error=10.0, double learning_rate=0.2, double alpha=0.1, double v=-5.0)
        {
            double sum = 0.0;
            double[] acc = new double[action_set.Length];
            for (int i = 0; i < action_set.Length; ++i)
            {
                LearningClassifier c=action_set[i];
                acc[i] = (c.Error < min_error) ? 1.0 : alpha * System.Math.Pow(c.Error / min_error, v);
                sum += acc[i] * c.Num;
            }
            for (int i = 0; i < action_set.Length; ++i)
            {
                LearningClassifier c = action_set[i];
                c.Fitness += learning_rate * (acc[i] * c.Num / sum - c.Fitness);
            }
        }

        protected string GenerateRandomInput(int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sb.AppendFormat("{0}", RandomEngine.NextBoolean() ? "1" : "0");
            }
            return sb.ToString();
        }


        public LearningClassifier CreateRandomClassifier(string input, List<int> actions, int generation, double rate = 1.0 / 3.0)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; ++i)
            {
                sb.AppendFormat("{0}", RandomEngine.NextDouble() < rate ? MaskChar : input[i]);
            }
            string condition = sb.ToString();
            int action = actions[RandomEngine.NextInt(actions.Count)];
            return new LearningClassifier(condition, action, generation);
        }

        public int[] GetActions(IEnumerable<LearningClassifier> pop)
        {
            HashSet<int> actions = new HashSet<int>();
            foreach (LearningClassifier c in pop)
            {
                actions.Add(c.Action);
            }
            return actions.ToArray();
        }

        public List<LearningClassifier> GenerateMatchSet(List<LearningClassifier> pop, string input, HashSet<int> all_actions, int generation)
        {
            List<LearningClassifier> match_set = new List<LearningClassifier>();
            HashSet<int> actions = new HashSet<int>();
            foreach (LearningClassifier c in pop)
            {
                if (c.Match(input))
                {
                    match_set.Add(c);
                    actions.Add(c.Action);
                }
            }

            List<int> remaining_actions = new List<int>();
            foreach (int a in all_actions)
            {
                if (!actions.Contains(a))
                {
                    remaining_actions.Add(a);
                }
            }

            while (remaining_actions.Count > 0)
            {
                LearningClassifier c = CreateRandomClassifier(input, remaining_actions, generation);
                match_set.Add(c);
                pop.Add(c);
                actions.Add(c.Action);
                remaining_actions.Remove(c.Action);
            }

            return match_set;
        }

        protected int SelectAction(Dictionary<int, double> predictions, bool explore = false)
        {
            int[] keys = predictions.Keys.ToArray();
            if (explore) return keys[RandomEngine.NextInt(keys.Length)];
            keys = keys.OrderByDescending(k => predictions[k]).ToArray();
            return keys.First();
        }

        public Dictionary<int, double> GeneratePrediction(List<LearningClassifier> match_set)
        {
            Dictionary<int, double> sum = new Dictionary<int, double>();
            Dictionary<int, double> count = new Dictionary<int, double>();
            Dictionary<int, double> weights = new Dictionary<int, double>();

            foreach (LearningClassifier c in match_set)
            {
                int action = c.Action;
                sum.Inc(action, c.Pred * c.Fitness);
                count.Inc(action, c.Fitness);
            }

            foreach (int action in count.Keys)
            {
                if (count[action] > 0)
                {
                    weights[action] = sum[action] / count[action];
                }
            }

            return weights;
        }

        protected void DeleteFromPop(List<LearningClassifier> pop, int pop_size, double delete_threshold = 20.0)
        {
            double total = pop.Sum(s => s.Num);
            if (total <= pop_size) return;

            double[] dvote = new double[pop.Count];
            double vote_sum = 0.0;
            for (int i = 0; i < pop.Count; ++i)
            {
                dvote[i] = CalcDeletionVote(pop, pop[i], delete_threshold);
                vote_sum += dvote[i];
            }

            int index = -1;
            vote_sum = 0;
            double point = vote_sum * RandomEngine.NextDouble();
            for (int i = 0; i < pop.Count; ++i)
            {
                vote_sum += dvote[i];
                if (point <= vote_sum)
                {
                    index = i;
                    break;
                }
            }

            if (pop[index].Num > 1)
            {
                pop[index].Num -= 1;
            }
            else
            {
                pop.RemoveAt(index);
            }
        }

        protected double CalcDeletionVote(List<LearningClassifier> pop, LearningClassifier c, double delete_threshold, double f_threshold = 1.0)
        {
            double vote = c.SetSize * c.Num;
            double total = pop.Sum(s => s.Num);
            double avg_fitness = pop.Sum(s => s.Fitness) / total;
            double derated = c.Fitness / c.Num;
            if (c.Exp > delete_threshold && derated < (f_threshold * avg_fitness))
            {
                return vote * (avg_fitness / derated);
            }
            return vote;
        }
    }
}
