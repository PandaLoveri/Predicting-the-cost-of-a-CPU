using System;
using System.Linq;

namespace Normalize
{
    class Normalization
    {
        public delegate double[] Nornmalize(double[] arr);

        /// <summary>
        /// Нормирование выборочной совокупности методом Минимакс
        /// </summary>
        public static Nornmalize MinMax = (arr) => arr.Select(x => (x - DescriptiveStatistics.Min(arr)) / (DescriptiveStatistics.Max(arr) - DescriptiveStatistics.Min(arr))).ToArray();

        /// <summary>
        /// Нормирование выборочной совокупности методом Z-масштабирования
        /// </summary>
        public static Nornmalize Z = (arr) => arr.Select(x => (x - DescriptiveStatistics.Average(arr)) / DescriptiveStatistics.StandartDeviation(arr)).ToArray();

        /// <summary>
        /// Нормирование выборочной совокупности Логарифмическим методом
        /// </summary>
        public static Nornmalize Log = (arr) => arr.Select(x => Math.Log10(x)).ToArray();

        /// <summary>
        /// Нормирование выборочной совокупности методом Квадратного корня
        /// </summary>
        public static Nornmalize Sqrt = (arr) => arr.Select(x => Math.Sqrt(x)).ToArray();

        /// <summary>
        /// Нормирование выборочной совокупности Квадратным методом
        /// </summary>
        public static Nornmalize Sqr = (arr) => arr.Select(x => x * x).ToArray();

        /// <summary>
        /// Нормирование каждого параметра матрицы
        /// </summary>
        public static double[][] GetNormMatrix(double[][] matrix, Nornmalize f)
        {      
            double[][] NormMatrix = new double[matrix.GetLength(0)][];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == 5 || i == 6 || i == 7 || i == 10 || i ==14 || i==15)
                    NormMatrix[i] = Log(matrix[i]);
                else if (i == 11 )
                {
                    NormMatrix[i] = matrix[i];
                }
                else if (i == 12 )
                {
                    NormMatrix[i] = MinMax(matrix[i]);
                }
                else
                    NormMatrix[i] = f(matrix[i]);
            }
            return NormMatrix;
        }
    }
}
