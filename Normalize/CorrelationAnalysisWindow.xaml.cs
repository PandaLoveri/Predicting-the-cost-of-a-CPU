using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Normalize
{
    /// <summary>
    /// Interaction logic for CorrelationAnalysisWindow.xaml
    /// </summary>
    public partial class CorrelationAnalysisWindow : Window
    {
        public CorrelationAnalysisWindow()
        {
            InitializeComponent();

            Grid gPair = new Grid();
            gPair.Name = "gPair";
            InitializeMatrixCorrelations(gPair);
            spMatixPairCorrelation.Children.Add(gPair);

            Grid gPrivate = new Grid();
            gPrivate.Name = "gPrivate";
            InitializeMatrixCorrelations(gPrivate);
            spMatixPrivateCorrelation.Children.Add(gPrivate);

            Grid gPairStudentCriterion = new Grid();
            gPairStudentCriterion.Name = "gPairStudentCriterion";
            InitializeMatrixCorrelations(gPairStudentCriterion);
            spPairStudentCriterion.Children.Add(gPairStudentCriterion);
                      
            InitializeMatrixCorrelations(gChangesCorrelationCoefficient);

            DrawDiagram(cnvPair, CorrelationAnalysis.R);
            DrawDiagram(cnvPrivate, CorrelationAnalysis.R_Private);
           
            InitializeMultipleCoefficient();            
        }


        /// <summary>
        /// Инициализация матрицы корреляций
        /// </summary>
        public void InitializeMatrixCorrelations(Grid grid)
        {
            grid.Margin = new Thickness(5);
            for (int i = 0; i < Data.countParametrs+1; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            CorrelationAnalysis.GetMatrixR();
            CorrelationAnalysis.GetMatrixR_Private();

            for (int i = 0; i < Data.countParametrs; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(1);
                    grid.Children.Add(border);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j + 1);

                    TextBlock tb = new TextBlock();
                    double r = Math.Abs(Math.Round(CorrelationAnalysis.R[i, j], 2));
                    double r_priv = Math.Abs(Math.Round(CorrelationAnalysis.R_Private[i, j], 2));
                    switch (grid.Name)
                    {
                        case "gPair":
                            tb.Name = $"tbR{i}{j}";
                            tb.Text = $"{Math.Round(CorrelationAnalysis.R[i, j], 2)}";                            
                            if (0.7 <= r && r <= 1)
                                tb.Foreground = Brushes.Green;
                            if (0.5 <= r &&r < 0.7)
                                tb.Foreground = Brushes.Orange;
                            if (0.3 <= r && r < 0.5)
                                tb.Foreground = Brushes.Red;
                            break;
                        case "gPrivate":
                            tb.Name = $"tbPrivateMatrix{i}{j}";
                            tb.Text = $"{Math.Round(CorrelationAnalysis.R_Private[i, j], 2)}";                            
                            if (0.7 <= r_priv && r_priv <= 1)
                                tb.Foreground = Brushes.Green;
                            if (0.5 <= r_priv && r_priv < 0.7)
                                tb.Foreground = Brushes.Orange;
                            if (0.3 <= r_priv && r_priv < 0.5)
                                tb.Foreground = Brushes.Red;
                            break;
                        case "gPairStudentCriterion":
                            tb.Name = $"tbPairStudentCriterion{i}{j}";
                            tb.Text = Math.Round(CorrelationAnalysis.StudentCriterion(i, j), 2) > CorrelationAnalysis.StudentTabularCriterion ? "З" : "Н";
                            break;
                        case "gChangesCorrelationCoefficient":
                            tb.Name = $"tbChangesCorrelationCoefficient{i}{j}";                            
                            tb.Text = Math.Abs(r_priv)>Math.Abs(r) ? "↑" : "↓";
                            if (Math.Abs(r_priv) == Math.Abs(r))
                                tb.Text = "";
                            if (Math.Sign(r) != Math.Sign(r_priv))
                                tb.Text += tb.Text!="" ? " + !" : "!";                            
                            break;
                        default:
                            break;
                    }

                    grid.Children.Add(tb);
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, j + 1);
                }
            }

            for (int i = 0; i < Data.countParametrs; i++)
            {
                TextBlock tb1 = new TextBlock();
                tb1.Text =i==Data.countParametrs-1 ? "Y": $"X{i+1}";
                grid.Children.Add(tb1);
                Grid.SetRow(tb1, Data.countParametrs);
                Grid.SetColumn(tb1, i+1);

                TextBlock tb2 = new TextBlock();
                tb2.Text = i == Data.countParametrs - 1 ? "Y" : $"X{i + 1}";
                grid.Children.Add(tb2);
                Grid.SetRow(tb2, i);
                Grid.SetColumn(tb2, 0);
            }
        }

        /// <summary>
        /// Диаграмма корреляций
        /// </summary>
        public void DrawDiagram(Canvas cnv, DenseMatrix M)
        {
            double x0 =147, y0 = 100, r=100;
            List<List<Double>> points = new List<List<double>>();
            List<List<Double>> points_tb = new List<List<double>>();
            for (int i=0;i<Data.countParametrs;i++)
            {
                double phi =2*Math.PI*i/Data.countParametrs;               
                double x = x0 + r * Math.Cos(phi), y = y0 + r * Math.Sin(phi);
                points.Add(new List<double>() {x,y});
                double x_tb = x0-3 + (r+15) * Math.Cos(phi), y_tb = y0-10 + (r + 15) * Math.Sin(phi);
                points_tb.Add(new List<double>() { x_tb, y_tb });
            }  
            for (int i=0;i<points.Count();i++)
            {
                Rectangle p = new Rectangle();
                p.Height = p.Width = 5;
                p.Fill = Brushes.Black;
                Canvas.SetTop(p, points[i][0]);
                Canvas.SetLeft(p, points[i][1]);
                cnv.Children.Add(p);
                TextBlock tb = new TextBlock();
                tb.Text = i == Data.countParametrs - 1 ? "Y" : $"X{i + 1}"; 
                Canvas.SetTop(tb, points_tb[i][0]);
                Canvas.SetLeft(tb, points_tb[i][1]);
                cnv.Children.Add(tb);
            }
            foreach (List<Double> point in points)
            {
                Rectangle p = new Rectangle();
                p.Height =p.Width= 5;
                p.Fill = Brushes.Black;
                Canvas.SetTop(p, point[0]);
                Canvas.SetLeft(p, point[1]);
                cnv.Children.Add(p);
            }
            for (int i=0;i<Data.countParametrs;i++)
            {
                for (int j=0;j<i;j++)
                {
                    Line l = new Line();
                    l.Y1 = points[i][0];
                    l.Y2 = points[j][0];
                    l.X1 = points[i][1];
                    l.X2 = points[j][1];
                    l.StrokeThickness = 1;
                    if (0.7 <= Math.Round(M[i, j], 2) && Math.Round(M[i, j], 2) <= 1)
                        l.Stroke = Brushes.Green;
                    if (0.5 <= Math.Round(M[i, j], 2) && Math.Round(M[i, j], 2) < 0.7)
                        l.Stroke = Brushes.Orange;
                    if (0.3 <= Math.Round(M[i, j], 2) && Math.Round(M[i, j], 2) < 0.5)
                        l.Stroke = Brushes.Red;
                    cnv.Children.Add(l);
                }
            }            
        }

        /// <summary>
        /// Инициализация поля о множественном коэффициенте
        /// </summary>
        public void InitializeMultipleCoefficient()
        {
            if (CorrelationAnalysis.R == null)
                CorrelationAnalysis.GetMatrixR();

            double r = Math.Round(CorrelationAnalysis.MultipleCorrelationCoefficient(Data.countParametrs-1), 2);
            double indexOfDetermination = Math.Round(Math.Pow(CorrelationAnalysis.MultipleCorrelationCoefficient(Data.countParametrs - 1),2), 2);
            tbMultipleCorrelation.Text = "Значение множественного\nкоэффициента корреляции: " + $"{r}";
            tbFisherCriterion.Text = $"Табличное значение критерия Фишера: {CorrelationAnalysis.FischerTabularCriterion}\n" +
                                     $"Наблюдаемое значение критерия: {CorrelationAnalysis.FisherCriterion(r)}\n\n";
            tbFisherCriterion.Text += CorrelationAnalysis.FisherCriterion(r) > CorrelationAnalysis.FischerTabularCriterion? "Коэффициент множественной корреляции значим" : "Коэффициент множественной корреляции незначим";     
        }

        private void ChStrengthen_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var child in gChangesCorrelationCoefficient.Children)
            {
                if (child is TextBlock)
                {
                    TextBlock tb = child as TextBlock;
                    int i = Grid.GetRow(tb), j = Grid.GetColumn(tb);
                    if (i != Data.countParametrs && j != 0 && i<Data.countParametrs)
                    {
                        j--;
                        double r = Math.Abs(Math.Round(CorrelationAnalysis.R[i, j], 2));
                        double r_priv = Math.Abs(Math.Round(CorrelationAnalysis.R_Private[i, j], 2));
                        if (Math.Abs(r) > Math.Abs(r_priv))
                        {
                            tb.Background = Brushes.Plum;
                        }
                    }
                }
            }
        }

        private void ChStrengthen_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var child in gChangesCorrelationCoefficient.Children)
            {
                if (child is TextBlock)
                {
                    TextBlock tb = child as TextBlock;
                    int i = Grid.GetRow(tb), j = Grid.GetColumn(tb);
                    if (i != Data.countParametrs && j != 0 && i < Data.countParametrs)
                    {
                        j--;
                        double r = Math.Abs(Math.Round(CorrelationAnalysis.R[i, j], 2));
                        double r_priv = Math.Abs(Math.Round(CorrelationAnalysis.R_Private[i, j], 2));
                        if (Math.Abs(r) > Math.Abs(r_priv))
                        {
                            tb.Background = Brushes.AntiqueWhite;
                        }
                    }
                }
            }
        }

        private void ChWeaken_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var child in gChangesCorrelationCoefficient.Children)
            {
                if (child is TextBlock)
                {
                    TextBlock tb = child as TextBlock;
                    int i = Grid.GetRow(tb), j = Grid.GetColumn(tb);
                    if (i != Data.countParametrs && j != 0 && i < Data.countParametrs)
                    {
                        j--;
                        double r = Math.Abs(Math.Round(CorrelationAnalysis.R[i, j], 2));
                        double r_priv = Math.Abs(Math.Round(CorrelationAnalysis.R_Private[i, j], 2));
                        if (Math.Abs(r) < Math.Abs(r_priv))
                        {
                            tb.Background = Brushes.Chartreuse;
                        }
                    }
                }
            }
        }

        private void ChWeaken_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var child in gChangesCorrelationCoefficient.Children)
            {
                if (child is TextBlock)
                {
                    TextBlock tb = child as TextBlock;
                    int i = Grid.GetRow(tb), j = Grid.GetColumn(tb);
                    if (i != Data.countParametrs && j != 0 && i < Data.countParametrs)
                    {
                        j--;
                        double r = Math.Abs(Math.Round(CorrelationAnalysis.R[i, j], 2));
                        double r_priv = Math.Abs(Math.Round(CorrelationAnalysis.R_Private[i, j], 2));
                        if (Math.Abs(r) < Math.Abs(r_priv))
                        {
                            tb.Background = Brushes.AntiqueWhite;
                        }
                    }
                }
            }
        }

    }
}
