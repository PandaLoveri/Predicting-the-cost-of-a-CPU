using System;
using System.Collections.Generic;
using System.Linq;

namespace Normalize
{
    class X2
    {
        public static int Count  { get; set; }
        public static int CountOfIntervals { get; set; }
        public static double Step { get; set; }
        public static double[] Points { get; set; }
        public static double[] NewX { get; set; }
        public static List<double> EmpiricalFrequencies { get; set; }
        public static List<double> TheoreticalFrequencies { get; set; }

        /// <summary>
        /// Количество интервалов статистического ряда
        /// </summary>
        private static void GetCountOfIntervals(double[] arr)
        {
            Count = Data.Array[0].Length;
            int k;
            k = (int)Math.Floor(1 + 3.32 * Math.Log10(Count)); //формула Стерджесса           
            //if (Count < 100)
            //   k = (int)Math.Floor(1 + 3.32 * Math.Log10(Count)); //формула Стерджесса
            //else
            //    k = (int)Math.Floor(5 * Math.Log10(Count));        //формула Брукса 
            CountOfIntervals = arr.Distinct().Count() < k ? arr.Distinct().Count() : k;
        }

        /// <summary>
        /// Шаг интервального ряда
        /// </summary>
        private static void GetStep(double[] arr)
        {
            GetCountOfIntervals(arr);
            Step = DescriptiveStatistics.Scope(arr) / CountOfIntervals;
        }

        /// <summary>
        /// Точки инетрвального ряда
        /// </summary>
        private static void GetPoints(double[] arr)
        {
            GetStep(arr);
            Points = new double[CountOfIntervals + 1];
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = DescriptiveStatistics.Min(arr) + i*Step;
            }
        }

        /// <summary>
        /// Точки середин интервало
        /// </summary>
        private static void GetNewX(double[] arr)
        {
            GetPoints(arr);
            NewX = new double[CountOfIntervals];            
            for (int i = 0; i < Points.Length - 1; i++)
            {
                NewX[i] = Points[i] + Step/ 2;
            }
        }

        /// <summary>
        /// Инициализация списков
        /// </summary>
        private static List<double> InitializingList()
        {
            List<double> list = new List<double>();
            for (int i = 0; i < CountOfIntervals; i++)
            {
                list.Add(0);
            }
            return list;
        }

        /// <summary>
        /// Эмпирические частоты
        /// </summary>
        private static void GetEmpiricalFrequencies(double[] arr)
        {
            GetNewX(arr);
            EmpiricalFrequencies=InitializingList();
            for (int i = 0; i < Points.Length-1; i++)
            {
                foreach (double x in arr)
                {
                    if (i== Points.Length - 2 && Points[Points.Length - 2] <= x)
                        EmpiricalFrequencies[Points.Length - 2]++;
                    else if (Points[i] <= x && x < Points[i + 1])
                        EmpiricalFrequencies[i]++;
                }
            }   
        }

        /// <summary>
        /// Теоретические частоты
        /// </summary>
        private static void GetTheoreticalFrequencies(double[] arr)
        {
            TheoreticalFrequencies=InitializingList();

            double x_mean = 0;
            for (int i = 0; i < CountOfIntervals; i++)
                x_mean += NewX[i] * EmpiricalFrequencies[i];
            x_mean /= Count;

            double s = 0;
            for (int i = 0; i < CountOfIntervals; i++)
                s += (NewX[i]- x_mean) * (NewX[i] - x_mean) * EmpiricalFrequencies[i];
            s /= Count - 1;
            s = Math.Sqrt(s);

            for (int i = 0; i < CountOfIntervals; i++)
            {
                double u = (NewX[i] - x_mean) / s;
                double f_u = 1 / (Math.Sqrt(2 * Math.PI) * Math.Pow(Math.E, (u * u) / 2));
                TheoreticalFrequencies[i] = Count * Step * f_u / s;
            }
        }

        /// <summary>
        /// Объединение элементов листа
        /// </summary>
        private static void CombiningListElements (int from, int to, int istart, int jstart, ref List<List<int>> list)
        {
            EmpiricalFrequencies[to] += EmpiricalFrequencies[from];
            TheoreticalFrequencies[to] += TheoreticalFrequencies[from];
            EmpiricalFrequencies.RemoveAt(from);
            TheoreticalFrequencies.RemoveAt(from);
                       
            for (int i = istart; i < list.Count(); i++)
                for (int j = jstart; j < list[i].Count; j++)
                    list[i][j]--;
        }

        /// <summary>
        /// Суммирование(объединение) малочастотных интервалов
        /// </summary>
        private static bool SumLowFrequencyIntervals()
        {
            bool flag = true;
            List<List<int>> indexes = new List<List<int>>();
            for (int i = 0; i < EmpiricalFrequencies.Count(); i++)
            {
                List<int> temp = new List<int>();
                while (i< EmpiricalFrequencies.Count() && EmpiricalFrequencies[i] < 5)
                {
                    temp.Add(i++);
                    flag = false;
                }
                if (temp.Count()!=0)
                    indexes.Add(temp);
            }
            
            for (int i = 0; i < indexes.Count(); i++)
            {
                if (indexes[i].Count == 1)
                {
                    int ind = indexes[i][0];
                    if (ind == 0)
                    {
                        CombiningListElements(1, 0, i+1,0,ref indexes);
                    }
                    else if (ind == EmpiricalFrequencies.Count()-1)
                    {
                        CombiningListElements(ind, ind - 1, i+1, 0, ref indexes);
                    }
                    else
                    {
                        if (EmpiricalFrequencies[ind - 1]<EmpiricalFrequencies[ind + 1])
                            CombiningListElements(ind, ind - 1,i+1, 0, ref indexes);
                        else
                            CombiningListElements(ind, ind + 1, i + 1, 0, ref indexes);
                    }
                }
                else
                {
                    for (int j=1;j<indexes[i].Count();j++)
                    {
                        CombiningListElements(indexes[i][j], indexes[i][0], i, j+1, ref indexes);
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Критерий хи-квадрат
        /// </summary>
        public static double GetX2(double[] arr)
        {
            double X2 = 0;
            GetEmpiricalFrequencies(arr);
            GetTheoreticalFrequencies(arr);
            if (CountOfIntervals <= 3)
                return 0;           

            bool flag = SumLowFrequencyIntervals();
            while (!flag)
                flag = SumLowFrequencyIntervals();
            CountOfIntervals = EmpiricalFrequencies.Count();
            if (CountOfIntervals <= 3)
                return 0;

            for (int i = 0; i < CountOfIntervals; i++)
            {
                X2+= Math.Pow(EmpiricalFrequencies[i] - TheoreticalFrequencies[i], 2) / TheoreticalFrequencies[i];
            }
            return X2;
        }
    }
}
