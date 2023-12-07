using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessQueen.Src
{
    class GenericAlgorithm
    {
        private int N; // Kích thước của bảng
        private int PopulationSize; // Kích thước quần thể
        private double CrossoverProbability; // Xác suất lai ghép
        private double MutationProbability; // Xác suất đột biến
        private int MaxGenerations; // Số lượng thế hệ tối đa
        private int NumParallelTasks; // Số lượng nhiệm vụ song song
        private Random random;

        public GenericAlgorithm(int n, int populationSize, double crossoverProbability, double mutationProbability, int maxGenerations, int numParallelTasks)
        {
            N = n;
            PopulationSize = populationSize;
            CrossoverProbability = crossoverProbability;
            MutationProbability = mutationProbability;
            MaxGenerations = maxGenerations;
            NumParallelTasks = numParallelTasks;
            random = new Random();
        }

        public void Run()
        {
            Parallel.For(0, NumParallelTasks, i =>
            {
                SolveNQueensParallelGA();
            });
        }

        private void SolveNQueensParallelGA()
        {
            int[][] population = InitializePopulation();

            Parallel.For(0, PopulationSize, (i, state) =>
            {
                int[] parent1 = SelectParent(population);
                int[] parent2 = SelectParent(population);

                int[] child = Crossover(parent1, parent2);

                if (random.NextDouble() < MutationProbability)
                    Mutate(child);

                population[i] = child;

                if (IsSolutionFound(population))
                {
                    Console.WriteLine("Solution found in generation " + MaxGenerations);
                    PrintBoard(GetBestIndividual(population));
                    state.Stop();
                }
            });
        }

        private int[][] InitializePopulation()
        {
            int[][] population = new int[PopulationSize][];
            Parallel.For(0, PopulationSize, i =>
            {
                population[i] = GenerateRandomIndividual();
            });
            return population;
        }

        private int[] GenerateRandomIndividual()
        {
            int[] individual = new int[N];
            for (int i = 0; i < N; i++)
            {
                individual[i] = random.Next(N);
            }
            return individual;
        }

        private int[] SelectParent(int[][] population)
        {
            int index1 = random.Next(population.Length);
            int index2 = random.Next(population.Length);
            return Fitness(population[index1]) < Fitness(population[index2]) ? population[index1] : population[index2];
        }

        private int Fitness(int[] individual)
        {
            int conflicts = 0;
            for (int i = 0; i < N - 1; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    if (individual[i] == individual[j] || Math.Abs(i - j) == Math.Abs(individual[i] - individual[j]))
                    {
                        conflicts++;
                    }
                }
            }
            return conflicts;
        }

        private int[] Crossover(int[] parent1, int[] parent2)
        {
            int[] child = new int[N];
            int crossoverPoint = random.Next(N);
            for (int i = 0; i < crossoverPoint; i++)
            {
                child[i] = parent1[i];
            }
            for (int i = crossoverPoint; i < N; i++)
            {
                child[i] = parent2[i];
            }
            return child;
        }

        private void Mutate(int[] individual)
        {
            int mutationPoint = random.Next(N);
            int mutationValue = random.Next(N);
            individual[mutationPoint] = mutationValue;
        }

        private bool IsSolutionFound(int[][] population)
        {
            foreach (var individual in population)
            {
                if (Fitness(individual) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private int[] GetBestIndividual(int[][] population)
        {
            int[] bestIndividual = population[0];
            int bestFitness = Fitness(bestIndividual);

            for (int i = 1; i < population.Length; i++)
            {
                int currentFitness = Fitness(population[i]);
                if (currentFitness < bestFitness)
                {
                    bestFitness = currentFitness;
                    bestIndividual = population[i];
                }
            }

            return bestIndividual;
        }

        private string PrintBoard(int[] board)
        {
            int squareSize = Math.Min(700 / N, 700 / N);

            var result = new StringBuilder();
            result.AppendLine("<div style='display: flex; flex-wrap: wrap; width: " + (squareSize * N) + "px; height: " + (squareSize * N) + "px;'>");

            for (int row = 0; row < N; row++)
            {
                for (int col = 0; col < N; col++)
                {
                    result.AppendLine(RenderSquare(col, row, board[row], squareSize));
                }
            }

            result.AppendLine("</div>");

            return result.ToString();
        }

        string RenderSquare(int col, int row, int isQueen, int size)
        {
            string squareColor = (row + col) % 2 == 0 ? "white" : "black";
            string queenClassName = isQueen == 1 ? "red-queen" : "";

            // Tính toán kích thước cho quân Hậu dựa trên kích thước của ô vuông
            int queenSize = size; // Kích thước của quân Hậu, có thể điều chỉnh tùy ý
            
            return $"<div class='square {queenClassName}' style='width: {size - 2}px; height: {size - 2}px; background-color: {squareColor}; text-align: center;'>" +
                   $"<div style='width: {queenSize}px; height: {queenSize}px; margin: auto; line-height: {queenSize}px;font-size: {queenSize * 0.6}px;'>{ (isQueen == 1 ? '♕' : ' ') }</div></div>";
        }
    }
}