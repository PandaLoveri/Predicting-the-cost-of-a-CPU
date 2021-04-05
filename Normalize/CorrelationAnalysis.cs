using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Normalize
{
    class CorrelationAnalysis
    {
        public static DenseMatrix R;
        public static DenseMatrix R_Private;
        public static Double StudentTabularCriterion=1.9766922;
        public static Double FischerTabularCriterion = 234.52;

        /// <summary>
        /// Парный коэффициент корреляции
        /// </summary>
        private static double PairCorrelationCoefficient(double[] x, double[] y)
        {
            double s_x = DescriptiveStatistics.StandartDeviation(x);
            double s_y = DescriptiveStatistics.StandartDeviation(y);
            double xy = 0, x2=0, y2 = 0;
            int n = x.Length;
            for (int i=0;i<n;i++)
            {
                xy += x[i] * y[i];
                x2 += x[i] * x[i];
                y2 += y[i] * y[i];
            }
            return (n*xy-DescriptiveStatistics.Sum(x)*DescriptiveStatistics.Sum(y))/Math.Sqrt(Math.Abs(n*x2-Math.Pow(DescriptiveStatistics.Sum(x),2))* Math.Abs(n * y2 - Math.Pow(DescriptiveStatistics.Sum(y), 2)));
        }

        /// <summary>
        /// Матрица парных корреляций R
        /// </summary>
        public static void GetMatrixR()
        {
           R = new DenseMatrix(Data.countParametrs, Data.countParametrs);
           for (int i = 0; i < Data.countParametrs; i++)
            for (int j = 0; j <=i; j++)
                    R[i, j] = R[j, i] = PairCorrelationCoefficient(MainWindow.NormMatrix[i], MainWindow.NormMatrix[j]);
        }

        /// <summary>
        ///Минор матрицы
        /// </summary>
        private static double Minor(DenseMatrix A, int i_del, int j_del)
        {
            int n = A.ColumnCount-1;
            DenseMatrix M = new DenseMatrix(n, n);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i < i_del && j < j_del) M[i, j] = A[i, j];
                    if (i < i_del && j >= j_del) M[i, j] = A[i, j + 1];
                    if (i >= i_del && j < j_del) M[i, j] = A[i + 1, j];
                    if (i >= i_del && j >= j_del) M[i, j] = A[i + 1, j + 1];
                }
            }  
            return M.Determinant();
        }

        /// <summary>
        ///Алгебраическое дополнение
        /// </summary>
        private static double Cofactor(DenseMatrix A, int i, int j)
        {
            return Math.Pow(-1, i + j) * Minor(A, i, j);
        }

        /// <summary>
        /// Частный коэффициент корреляции
        /// </summary>
        private static double PrivateCorrelationCoefficient(int i, int j)
        {
            if (R == null) throw new Exception("Вычислите парные коэффициенты!");
            return Cofactor(R, i, j) / Math.Sqrt(Cofactor(R, i, i) * Cofactor(R, j, j));
        }

        /// <summary>
        /// Матрица частных корреляций R_Private
        /// </summary>
        public static void GetMatrixR_Private()
        {
            R_Private = new DenseMatrix(Data.countParametrs, Data.countParametrs);
            for (int i = 0; i < Data.countParametrs; i++)
                for (int j = 0; j <= i; j++)
                    R_Private[i, j] = R_Private[j, i] = PrivateCorrelationCoefficient(i, j);
        }

        /// <summary>
        /// Множественный коэффициент корреляции
        /// </summary>
        public static double MultipleCorrelationCoefficient(int i)
        {
            if (R == null) throw new Exception("Вычислите парные коэффициенты!");
            return Math.Sqrt(1-R.Determinant()/Cofactor(R, i, i));
        }

        /// <summary>
        /// Критерий Стьюдента
        /// </summary>
        public static double StudentCriterion(int i, int j)
        {
            if (R == null) throw new Exception("Вычислите парные коэффициенты!");
            return Math.Abs(R[i, j]) * Math.Sqrt((MainWindow.NormMatrix[0].Length - 2) / (1 - Math.Pow(R[i, j],2)));
        }

        /// <summary>
        /// Критерий Фишера
        /// </summary>
        public static double FisherCriterion(double R)
        {
            return R * R * MainWindow.NormMatrix[0].Length / (1 - R * R);
        }
    }
}
