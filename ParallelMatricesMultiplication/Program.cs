using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelMatricesMultiplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var listArgs = args.ToList();

            double[,] matrixA;
            double[,] matrixB;

            var fileIdx = listArgs.IndexOf("-i");
            if (fileIdx > -1)
            {
                string filePath = listArgs[fileIdx + 1];
                (matrixA, matrixB) = ParseMatricesFromFile(filePath);
            }
            else
            {
                int m = int.Parse(listArgs[listArgs.IndexOf("-m") + 1]);
                int n = int.Parse(listArgs[listArgs.IndexOf("-n") + 1]);
                int k = int.Parse(listArgs[listArgs.IndexOf("-k") + 1]);
                Random rand = new Random();
                matrixA = new double[m, n];
                matrixB = new double[n, k];
                FillRandomMatrix(rand, matrixA);
                FillRandomMatrix(rand, matrixB);
            }
            var mult = new ParallelMatrixMultiplication(matrixA, matrixB, 8);
            mult.ComputeAsync().Wait();
        }

        public static (double[,] MatrixA, double[,] MatrixB) ParseMatricesFromFile(string pathToFile)
        {
            string[] input = File.ReadAllLines(pathToFile);
            var dimensions = input[0].Split(' ');
            int m = int.Parse(dimensions[0]);
            int n = int.Parse(dimensions[1]);
            int k = int.Parse(dimensions[2]);

            var matrixA = new double[m, n];
            var matrixB = new double[n, k];

            for (int i = 0; i < m; i++)
            {
                var cols = input[i + 1].Split(' ');
                for (int j = 0; j < n; j++)
                {
                    matrixA[i, j] = int.Parse(cols[j]);
                }
            }

            for (int i = 0; i < n; i++)
            {
                var cols = input[i + m + 1].Split(' ');
                for (int j = 0; j < k; j++)
                {
                    matrixB[i, j] = int.Parse(cols[j]);
                }
            }

            return (matrixA, matrixB);
        }

        public static void FillRandomMatrix(Random random, double[,] matrix, int maxElem = 10)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = random.NextDouble() * maxElem;
                }
            }
        }

        public static void PrintMatrix(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j]:0.##} ");
                }
                Console.WriteLine();
            }
        }

    }

}
