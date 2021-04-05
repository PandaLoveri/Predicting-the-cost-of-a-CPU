using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;

namespace Normalize
{
    public struct RemoteVariable
    {
        public int index;
        public int param_index;
    };

    class RegressionAnalysis
    {
        public static List<int> ParamList=new List<int>();
        public static int n = MainWindow.NormMatrix[0].Length, k;
        public static DenseMatrix Y;
        private static double Q_e, Q_R;
        public static double s_2;
        private static List<double> t_exp=new List<double>();
        public static List<bool> CoefficientSignificance=new List<bool>();
        public static List<List<double>> Intervals=new List<List<double>>();
        public static List<List<double>> IntervalsEquation = new List<List<double>>();
        private static int ind_min;
        private static Stack<RemoteVariable> ind_add=new Stack<RemoteVariable>();
        public static bool IsNextStepPossible=true;

        public static double[] F_crit = new double[15]
       {
            3.06,
            2.66,
            2.43,
            2.27,
            2.16,
            2.16,
            2.01,
            2.01,
            2.01,
            1.83,
            1.83,
            1.83,
            1.83,
            1.83,
            1.83
       };

        public static double[] t_crit = new double[15]
        {
            1.9768110,
            1.9769315,
            1.9770537,
            1.9771777,
            1.9773035,
            1.9774312,
            1.9775608,
            1.9776923,
            1.9778258,
            1.9779613,
            1.9780988,
            1.9782385,
            1.9783804,
            1.9785245,
            1.9786708
        };
        
        /// <summary>
        /// Инициализация списка параметров, входящих в уравнение регрессии
        /// </summary>
        private static void InitializePararmList()
        {
           for (int i=0;i<Data.countParametrs-1;i++)
            {
                ParamList.Add(i);
            }
        }

        /// <summary>
        /// Получение матрицы X
        /// </summary>
        private static DenseMatrix GetX()
        {
            k = ParamList.Count;
            if (k == 0)
            {
                InitializePararmList();
                k = ParamList.Count;
                GetY();
            }
            DenseMatrix X = new DenseMatrix(n, k + 1);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < k + 1; j++)
                    X[i, j] = (j == 0) ? 1.0 : MainWindow.NormMatrix[ParamList[j- 1]][i];
            return X;
        }

        /// <summary>
        /// Получение матрицы Y
        /// </summary>
        private static void GetY()
        {
            Y = new DenseMatrix(n, 1);
            for (int i = 0; i < n; i++)
                Y[i, 0] = MainWindow.NormMatrix[Data.countParametrs-1][i];
        }
    
        /// <summary>
        /// Матрица (Xt*X)inverse
        /// </summary>
        public static DenseMatrix Get_Xt_X_inverse()
        {
            DenseMatrix X = GetX();
            DenseMatrix Xt = (DenseMatrix)X.Transpose();
            DenseMatrix Xt_X = (DenseMatrix)Xt.Multiply(X);
            return (DenseMatrix)Xt_X.Inverse();           
        }
                          
        /// <summary>
        /// Коэффициенты уравнения регрессии
        /// </summary>
        public static DenseMatrix GetCoefficient()
        {
            DenseMatrix X = GetX();
            DenseMatrix Xt = (DenseMatrix)X.Transpose();
            DenseMatrix Xt_X_inverse = Get_Xt_X_inverse();
            DenseMatrix Xt_X_inverse_Xt= (DenseMatrix)Xt_X_inverse.Multiply(Xt);
            return (DenseMatrix)Xt_X_inverse_Xt.Multiply(Y);
        }

        /// <summary>
        /// Матрица NewY
        /// </summary>
        public static DenseMatrix Get_NewY()
        {
            DenseMatrix X = GetX();
            DenseMatrix B = GetCoefficient();
            return (DenseMatrix)X.Multiply(B);
        }


        /// <summary>
        /// F-критерий
        /// </summary>
        public static double F_exp()
        {
            DenseMatrix NewY = Get_NewY();
            Q_R = ((DenseMatrix)NewY.TransposeThisAndMultiply(NewY))[0,0];
            Q_e = 0;
            for (int i=0;i< n;i++)
            {
                Q_e += Math.Pow(Y[i, 0] - NewY[i, 0], 2);
            }
            return (Q_R * (n - k - 1)) / (Q_e * (k + 1));
        }

        /// <summary>
        /// Значимость отдельных коэффициентов уравнения регрессии
        /// </summary>
        public static void EstimateSignificanceOfTheCoefficient()
        {
            DenseMatrix B = GetCoefficient();
            DenseMatrix Xt_X_inverse = Get_Xt_X_inverse();

            s_2 = Q_e / (n - k - 1);
            
            for (int j=0;j<k+1;j++)
            {
                double s_bj = Math.Sqrt(s_2* Xt_X_inverse[j,j]);
                t_exp.Add(Math.Abs(B[j, 0]) / s_bj);
                CoefficientSignificance.Add( t_exp[j]>t_crit[k-1] ?true:false);

                List<double> temp = new List<double>();
                temp.Add(Math.Round(B[j, 0] - t_crit[k-1] * s_bj, 4));
                temp.Add(Math.Round(B[j, 0] + t_crit[k-1] * s_bj, 4));
                Intervals.Add(temp);
            }
        }

        /// <summary>
        /// Интервальная оценка уравнения регрессии
        /// </summary>
        public static void IntervalEstimateEquation()
        {
            DenseMatrix X = GetX();
            DenseMatrix B = GetCoefficient();
            DenseMatrix Xt_X_inverse = Get_Xt_X_inverse();
            for (int i = 0; i < n; i++)
            {
                DenseMatrix X0 = new DenseMatrix(k + 1, 1);
                double y0 = 0;
                for (int j = 0; j < k + 1; j++)
                {
                    X0[j, 0] = X[i, j];
                    y0 += X[j, 0] * B[j, 0];
                }
                DenseMatrix Matrix = (DenseMatrix)((DenseMatrix)X0.TransposeThisAndMultiply(Xt_X_inverse)).Multiply(X0);
                double sigma = t_crit[k - 1] * Math.Sqrt(s_2 * Matrix[0, 0]);
                List<double> temp = new List<double>();
                temp.Add(y0 - sigma);
                temp.Add(y0 + sigma);
                IntervalsEquation.Add(temp);
            }
        }

        /// <summary>
        /// Коэффициент детерминации
        /// </summary>
        public static double CoefficientOfDetermination()
        {
            return Math.Pow(CorrelationAnalysis.MultipleCorrelationCoefficient(Data.countParametrs - 1), 2);
        }

        /// <summary>
        /// Очистка листов
        /// </summary>
        private static void ClearLists()
        {
            t_exp.Clear();
            CoefficientSignificance.Clear();
            Intervals.Clear();
            IntervalsEquation.Clear();
        }

        /// <summary>
        /// Поиск незначительной переменной с минимальным по абсолютной величине значением t_exp 
        /// </summary>
        public static int SearchForAMinorVariable()
        {            
            ind_min = -1;
            double min = Double.MaxValue;
            for (int i = 1; i < CoefficientSignificance.Count; i++)
            {
                if (CoefficientSignificance[i] == false && t_exp[i] < min)
                {
                    min = t_exp[i];
                    ind_min = i;
                }
            }            
            return ind_min;
        }


        /// <summary>
        /// Исключение одной из незначительных переменных из уравнение регрессии
        /// </summary>
        public static void DeleteParameter()
        {
            ind_add.Push(new RemoteVariable { index=ind_min-1, param_index=ParamList[ind_min - 1] });
            ParamList.RemoveAt(ind_min- 1);
            ClearLists();
        }

        /// <summary>
        /// Добавление исключенной переменной в уравнение регрессии
        /// </summary>
        public static void AddParameter()
        {
            RemoteVariable temp = ind_add.Pop();
            ParamList.Insert(temp.index, temp.param_index);
            ClearLists();
        }
    }
}
