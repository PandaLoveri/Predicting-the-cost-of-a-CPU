using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Normalize
{
    /// <summary>
    /// Interaction logic for CheckingForNormalityWindow.xaml
    /// </summary>
    public partial class CheckingForNormalityWindow : Window
    {
        public CheckingForNormalityWindow()
        {
            InitializeComponent();
            InitializeGrid();

            tbTickLegend.Text = $"{(char)'\u2713'} Распределение нормальное";
            tbCrossLegend.Text= $"{(char)'\u2717'} Распределение ненормальное";
            tbNOLegend.Text= $"{(char)'\u2300'} Невозможно оценить нормальность";
        }

        private void InitializeGrid()
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            for (int i = 0; i < 4; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
            }
            for (int i = 0; i < 17; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() {  Width= new GridLength(60) });
            }

           
            for (int i=0;i<4;i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.SteelBlue;
                    border.BorderThickness = new Thickness(1);
                    grid.Children.Add(border);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                }
            }

            TextBlock tb1 = new TextBlock();
            tb1.Text = "К(пр)";
            Grid.SetColumn(tb1, 0);
            Grid.SetRow(tb1, 1);
            grid.Children.Add(tb1);

            TextBlock tb2 = new TextBlock();
            tb2.Text = "К(кр)";
            Grid.SetColumn(tb2, 0);
            Grid.SetRow(tb2, 2);
            grid.Children.Add(tb2);

            TextBlock tb3 = new TextBlock();
            tb3.Text = "Норм.";
            Grid.SetColumn(tb3, 0);
            Grid.SetRow(tb3, 3);
            grid.Children.Add(tb3);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 1; j < 17; j++)
                {
                    TextBlock tb = new TextBlock();
                    TextBlock tb_res = new TextBlock();
                    TextBlock tb_crit = new TextBlock();

                    if (i == 0 && j == 16) tb.Text = $"Y"; 
                    else if (i == 0) tb.Text = $"X{j}";
                    else if (i == 1)
                    {
                        tb.Name = $"tbX_exp{j}";                       
                        tb_res.Name= $"tbX_res{j}";
                        Check(j - 1, tb, tb_crit, tb_res);

                        Grid.SetColumn(tb_crit, j);
                        Grid.SetRow(tb_crit, 2);
                        grid.Children.Add(tb_crit);

                        Grid.SetColumn(tb_res, j);
                        Grid.SetRow(tb_res, 3);
                        grid.Children.Add(tb_res);
                    }
                   
                    Grid.SetColumn(tb, j);
                    Grid.SetRow(tb, i);
                    grid.Children.Add(tb);
                    
                }
            }

            spMain.Children.Add(grid);
        }

        private void Check(int param_num, TextBlock tb, TextBlock tb_crit, TextBlock tb_res)
        {
            double[] arr = MainWindow.NormMatrix[param_num];
            double x_exper = X2.GetX2(arr);
            if (x_exper == 0)
            {
                tb.Text = $"{(char)'\u2014'}";
                tb_crit.Text = $"{(char)'\u2014'}";
                tb_res.Text = $"{(char)'\u2300'}";               
            }
            else
            {
                tb.Text = $"{Math.Round(x_exper,2)}";
                tb_res.Text = x_exper < X_crit[X2.CountOfIntervals - 1] ? $"{(char)'\u2713'}" : $"{(char)'\u2717'}";
                tb_crit.Text = X_crit[X2.CountOfIntervals - 1-3].ToString();
            }     
        }

        private double[] X_crit = new double[]
        {
            3.8,
            6.0,
            7.8,
            9.5,
            11.1,
            12.6,
            14.1,
            15.5,
            16.9,
            18.3,
            19.7
        };
    }
}
