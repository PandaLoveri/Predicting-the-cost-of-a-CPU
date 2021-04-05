using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Normalize
{
    /// <summary>
    /// Interaction logic for DescriptiveStatisticsWindow.xaml
    /// </summary>
    public partial class DescriptiveStatisticsWindow : Window
    {
        public DescriptiveStatisticsWindow()
        {
            InitializeComponent();

            for (int i = 0; i < MainWindow.Matrix.GetLength(0); i++)
            {
                tbxData.Text += (i!=MainWindow.Matrix.GetLength(0)-1) ?$"X{i + 1}\t" : "Y";
                tbxNormData.Text += (i != MainWindow.Matrix.GetLength(0) - 1) ? $"X{i + 1}\t" : "Y";
            }
            for (int i = 0; i < MainWindow.Matrix[0].Length; i++)
            {
                tbxData.Text += "\n";
                tbxNormData.Text += "\n";
                for (int j = 0; j < MainWindow.Matrix.GetLength(0); j++)
                {
                    tbxData.Text += MainWindow.Matrix[j][i] + "\t";
                    tbxNormData.Text += Math.Round(MainWindow.NormMatrix[j][i],3) + "\t";
                }
            }

            ParametersDS(gDataDS, MainWindow.Matrix);
            ParametersDS(gNormDataDS, MainWindow.NormMatrix);

        }


        public static void ParametersDS(Grid grid, double[][] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                TextBlock tbHeader = new TextBlock();
                tbHeader.Text = (i != matrix.GetLength(0) - 1) ? $"X{i + 1}" : "Y";
                grid.Children.Add(tbHeader);

                if (i < 8)
                {
                    Grid.SetRow(tbHeader, 1);
                    Grid.SetColumn(tbHeader, i);
                }
                else
                {
                    Grid.SetRow(tbHeader, 3);
                    Grid.SetColumn(tbHeader, i - 8);
                }

                TextBlock tb = new TextBlock();
                tb.Text = DescriptiveStatistics.DS(matrix[i]);
                grid.Children.Add(tb);
                Border border = new Border();
                border.BorderBrush = Brushes.SteelBlue;
                border.Margin = new Thickness(0,0,15,0);
                border.BorderThickness = new Thickness(1);
                grid.Children.Add(border);

                if (i < 8)
                {
                    Grid.SetRow(tb, 2);
                    Grid.SetColumn(tb, i);
                    Grid.SetRow(border, 2);
                    Grid.SetColumn(border, i);
                }
                else
                {
                    Grid.SetRow(tb, 4);
                    Grid.SetColumn(tb, i - 8);
                    Grid.SetRow(border, 4);
                    Grid.SetColumn(border, i - 8);
                }
            }
        }
    }
}
