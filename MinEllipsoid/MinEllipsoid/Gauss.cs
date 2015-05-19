using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinEllipsoid
{
    static class Gauss
    {
        static private double[,] ElemMatr(int n)
        {
            //building identity matrix
            double[,] matrix = new double[n, n];
            for (int i = 0; i < n; i++)
                matrix[i, i] = 1;
            return matrix;
        }
        static private double[,] PermutMatr(int n, int k, int l)
        {
            //building permutation matrix where k and l elements on diagonal are (OMFG!1) permutated!!
            double[,] matrix = ElemMatr(n);
            matrix[k, k] = 0; matrix[l, l] = 0;
            matrix[k, l] = 1; matrix[l, k] = 1;
            return matrix;
        }
        static private double[,] FrobMatr(double[,] system, int k)
        {
            //Frobenius matrix for k step
            //just another thing you must go and RTFM
            int n = system.GetLength(0);
            double[,] frob = ElemMatr(n);
            frob[k,k] = 1.0 / system[k,k];
            for (int i = k + 1; i < n; i++)
                frob[i,k] = -system[i,k] / system[k,k];
            return frob;
        }
        static private double[,] mult(double[,] A, double[,] B)
        {
            //multiplication of two compatible matrix
            int n = A.GetLength(0); int m = B.GetLength(1); int k = B.GetLength(0);
            double[,] res = new double[n,m];
            for(int i = 0; i < n; i++)
                for(int j = 0; j < m; j++)
                    for(int l = 0; l < k; l++)
                        res[i, j] += A[i, l] * B[l, j];
            return res;
        }
        static private double[,] ForwGauss(double[,] system)
        {
            //forward steps of Gauss method
            int n = system.GetLength(0);
            double[,] matrix = (double[,])system.Clone();


            for (int i = 0; i < n; i++)
            {
                //making up-triangle system with permutation and Frobenius matrixes
                int k = i;
                double max = Math.Abs(matrix[i,i]);
                for (int j = i; j < n; j++)
                    if (Math.Abs(matrix[j,i]) > max)
                    {
                        max = Math.Abs(matrix[j,i]);
                        k = j;
                    }
                if (k != i)
                {
                    double[,] permMatrix = PermutMatr(n, k, i);
                    matrix = mult(permMatrix, matrix);
                }
                double[,] frobMatrix = FrobMatr(matrix, i);
                matrix = mult(frobMatrix, matrix);
            }
            return matrix;
        }
        static private double[] BackGauss(double[,] system)
        {
            //back steps of Gauss method
            int n = system.GetLength(0);
            double[,] matrix = (double[,])system.Clone();
            double[] r = new double[n];
            //calculating final answer from up-triangle matrix
            r[n - 1] = matrix[n-1,n];
            for (int i = n - 2; i >= 0; i--)
            {
                r[i] = matrix[i,n];
                for (int j = i + 1; j < n; j++)
                    r[i] -= matrix[i,j] * r[j];
            }
            return r;
        }
        static public double[] GaussSolve(double[,] system)
        {
            //full procedure with forward and back steps (if vector of free constants is in system already)
            double[,] matrix = ForwGauss(system);
            double[] r = BackGauss(matrix);
            return r;
        }
        static public double[] GaussSolve(double[,] system, double[] free)
        {
            //full procedure with forward and back steps (if vector of free constants and system are separeted)
            //just let them unite in one system and use previous procedure
            int n = system.GetLength(0);
            double[,] A = new double[n, n + 1];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    A[i, j] = system[i, j];
            for (int i = 0; i < n; i++)
                A[i, n] = free[i];
            return GaussSolve(A);
        }
    }
}
