using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelMatricesMultiplication
{
    public class ParallelMatrixMultiplication
    {
        private class MatrixPart
        {
            public MatrixPart(int startIdx, int endIdx)
            {
                StartIdx = startIdx;
                EndIdx = endIdx;
            }
            public int StartIdx { get; set; }
            public int EndIdx { get; set; }
        }

        private readonly int dimX;
        private readonly int dimY;
        private readonly int tCnt;
        private readonly double[,] matrixA;
        private readonly double[,] matrixB;
        private double[,] resMatrix;

        public ParallelMatrixMultiplication(double[,] matrixA, double[,] matrixB, int threadCount)
        {
            dimX = matrixA.GetLength(0);
            dimY = matrixA.GetLength(1);

            if (dimY != matrixB.GetLength(0))
                throw new ArgumentException("Unable to multiply matrices with different dimensions!");

            this.tCnt = threadCount;
            this.matrixA = matrixA;
            this.matrixB = matrixB;
        }

        public double[,] Compute()
        {
            this.resMatrix = new double[dimX, dimY];
            int tRowsLen = dimX / tCnt;
            int tRowsLenRem = dimX % tCnt;
            Thread[] threads = new Thread[tCnt];
            for (int i = 0; i < tCnt; i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart(MatrixMultiply));
                int startIdx = i * tRowsLen;
                int endIdx = i < tCnt - 1 ? startIdx + tRowsLen : startIdx + tRowsLen + tRowsLenRem;
                threads[i].Start(new MatrixPart(startIdx, endIdx));
            }

            for (int i = 0; i < tCnt; i++)
                threads[i].Join();

            return resMatrix;
        }

        public async Task<double[,]> ComputeAsync()
        {
            this.resMatrix = new double[dimX, dimY];
            int tRowsLen = dimX / tCnt;
            int tRowsLenRem = dimX % tCnt;
            Task[] tasks = new Task[tCnt];

            for (int i = 0; i < tCnt; i++)
            {
                int startIdx = i * tRowsLen;
                int endIdx = i < tCnt - 1 ? startIdx + tRowsLen : startIdx + tRowsLen + tRowsLenRem;
                tasks[i] = Task.Factory.StartNew(MatrixMultiply, new MatrixPart(startIdx, endIdx), TaskCreationOptions.LongRunning);
            }

            await Task.WhenAll(tasks);

            return this.resMatrix;
        }

        private void MatrixMultiply(object matrixPart)
        {
            var part = (MatrixPart)matrixPart;
            Console.WriteLine($"Thread '{Thread.CurrentThread.ManagedThreadId}' started computing.");

            for (int i = part.StartIdx; i < part.EndIdx; i++)
            {
                for (int j = 0; j < matrixB.GetLength(1); j++)
                {
                    double k = 0;
                    for (int w = 0; w < dimY; w++)
                    {
                        k += matrixA[i, w] * matrixB[w, j];
                    }
                    resMatrix[i, j] = k;
                }
            }
            Console.WriteLine($"Thread '{Thread.CurrentThread.ManagedThreadId}' finished computing.");
        }
    }
}
