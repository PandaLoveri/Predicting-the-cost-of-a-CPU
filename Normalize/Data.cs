using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Normalize
{
    class Data
    {
        public static List<string> parametrs = new List<string>();
        public static int countParametrs { get; set; }
        public static double[][] Array { get; set; }

        public static void GetMatrix(string path)
        {
            string[] data = File.ReadAllLines(path, Encoding.Default);
            string[] str = data[0].Split(';');
            countParametrs = str.Length;

            foreach (string s in str) if (s != "") parametrs.Add(s);

            Array = new double[parametrs.Count][];
            for (int i = 0; i < parametrs.Count; i++)
            {
                double[] temp = new double[data.Length - 1];
                for (int j = 1; j < data.Length; j++)
                {
                    str = data[j].Split(';');
                    temp[j - 1] = double.Parse(str[i]);
                }
                Array[i] = temp;
            }
        }
    }
}
