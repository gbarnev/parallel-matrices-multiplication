using System;
using System.IO;
using System.Linq;

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

            PrintMatrix(matrixA);
            Console.WriteLine();
            PrintMatrix(matrixB);
            Console.WriteLine();
            PrintMatrix(MultiplyMatrices(matrixA, matrixB));
            Console.WriteLine();

            Console.ReadLine();
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

        public static double[,] MultiplyMatrices(double[,] matrixA, double[,] matrixB)
        {
            int dimX = matrixA.GetLength(0);
            int dimY = matrixA.GetLength(1);

            if (dimY != matrixB.GetLength(0))
                throw new ArgumentException("Unable to multiply matrices with different dimensions!");

            double[,] matrixC = new double[dimX, dimY];
            for (int i = 0; i < dimX; i++)
            {
                for (int j = 0; j < matrixB.GetLength(1); j++)
                {
                    double k = 0;
                    for (int w = 0; w < dimY; w++)
                    {
                        k += matrixA[i, w] * matrixB[w, j];
                    }
                    matrixC[i, j] = k;
                }
            }
            return matrixC;
        }
    }
}
