using System;
using System.Linq;

namespace Normalize
{
    class DescriptiveStatistics
    {
        /// <summary>
        /// Минимум
        /// </summary>
        public static double Min(double[] arr)
        {
            return arr.Min();
        }

        /// <summary>
        /// Максимум
        /// </summary>
        public static double Max(double[] arr)
        {
            return arr.Max();
        }
       
        /// <summary>
        /// Размах
        /// </summary>
        public static double Scope(double[] arr)
        {
            return arr.Max()-arr.Min();
        }

        /// <summary>
        /// Среднее значение
        /// </summary>
        public static double Average(double[] arr)
        {
            return arr.Sum() / arr.Length;
        }

        /// <summary>
        /// Дисперсия
        /// </summary>
        public static double Dispersion(double[] arr)
        {
            double d = 0, aver = Average(arr);
            for (int i = 0; i < arr.Length; i++)
                d += (arr[i] - aver) * (arr[i] - aver);
            return d /= arr.Length - 1;
        }

        /// <summary>
        /// Стандартное отклонение
        /// </summary>
        public static double StandartDeviation(double[] arr)
        {
            return Math.Sqrt(Dispersion(arr));
        }

        /// <summary>
        /// Медиана
        /// </summary>
        public static double Median(double[] array)
        {
            double[] arr=new double[array.Length];
            Array.Copy(array,arr,array.Length);
            Array.Sort(arr);
            int k = arr.Length / 2;
            return arr.Length % 2 == 0 ? (arr[k] + arr[k + 1]) / 2 : arr[k + 1];
        }

        /// <summary>
        /// Стандартная ошибка
        /// </summary>
        public static double StandartError(double[] arr)
        {
            return StandartDeviation(arr) / Math.Sqrt(arr.Length);
        }

        /// <summary>
        /// Асимметрия
        /// </summary>
        public static double Asymmetry(double[] arr)
        {
            double aver =Average(arr);
            double asym = 0;
            foreach (double x in arr)
                asym += Math.Pow(x - aver, 3);
            asym /= Math.Pow(StandartDeviation(arr),3);
            return asym * arr.Length / ((arr.Length - 1) * (arr.Length - 2)); 
        }

        /// <summary>
        /// Эксцесс
        /// </summary>
        public static double Excess(double[] arr)
        {
            double aver = Average(arr);
            int n = arr.Length;
            double exc = 0;
            foreach (double x in arr)
                exc += Math.Pow((x - aver), 4);
            exc /= Math.Pow(StandartDeviation(arr), 4);
            exc *= (n * (n + 1)) / ((n - 1) * (n - 2) * (n - 3));
            exc -= 3 * (n - 1) * (n - 1) / ((n - 2) * (n - 3));
            return exc;
        }

        /// <summary>
        /// Счет суммы всех элементов
        /// </summary>
        public static double Sum(double[] arr)
        {
            return arr.Sum();
        }

        
        /// <summary>
        /// Описательная статистика по параметру
        /// </summary>
        public static string DS(double[] arr)
        {
            return $"Минимум: {Math.Round(Min(arr), 3)}\n" +
                   $"Максимум: {Math.Round(Max(arr), 3)}\n" +
                   $"Интервал: {Math.Round(Scope(arr),3)}\n" +
                   $"Среднее: {Math.Round(Average(arr),3)}\n" +
                   $"Дисперсия: {Math.Round(Dispersion(arr),3)}\n" +
                   $"Станд. отклонение: {Math.Round(StandartDeviation(arr),3)}\n" +
                   $"Медиана: {Math.Round(Median(arr),3)}\n" +
                   $"Станд. ошибка: {Math.Round(StandartError(arr),3)}\n" +
                   $"Асимметрия: {Math.Round(Asymmetry(arr),3)}\n" +
                   $"Эксцесс: {Math.Round(Excess(arr),3)}\n" +
                   $"Счет: {Math.Round(Sum(arr),3)}";
        }
    }
}
