using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Normalize
{
    /// <summary>
    /// Interaction logic for RegressionAnalysisWindow.xaml
    /// </summary>
    public partial class RegressionAnalysisWindow : Window
    {
        public RegressionAnalysisWindow()
        {
            InitializeComponent();
            InitializeViewOfTheEquation();
            InitializeY();
            InitializeSignificanceOfTheEquation();
            InitializeSignificanceOfTheCoefficient();            
            InitializeIntervalEstimateEquation();
            InitializeEvaluatingAdequacyModel();

            InitializeGrid(gEnterData);
            InitializeGrid(gNormData);
            
            btnPreviousStep.IsEnabled = false;
            if (RegressionAnalysis.SearchForAMinorVariable() == -1 || RegressionAnalysis.k<2)
                btnNextStep.IsEnabled = false;
            else
                btnNextStep.IsEnabled = true;
        }

        /// <summary>
        /// Нижний индекс
        /// </summary>
        private Run SubscriptRun(string text, int font_size)
        {
            Run run = new Run(text);
            run.FontSize = font_size;
            run.BaselineAlignment = BaselineAlignment.Subscript;
            return run;
        }
        
        /// <summary>
        /// Инициализация вида уравнения регрессии
        /// </summary>
        private void InitializeViewOfTheEquation()
        {            
            tbGeneralView.Text = "";
            tbGeneralView.Inlines.Add("Общий вид уравнения регрессии: y = b");
            tbGeneralView.Inlines.Add(SubscriptRun("0", 10));

            tbRegressionEquation.Text = "";
            DenseMatrix B = RegressionAnalysis.GetCoefficient();
            tbRegressionEquation.Inlines.Add($"Уравнение регрессии: y = { Math.Round(B[0, 0], 2)}");

            tbxPointEstimate.Text = "";
            tbxPointEstimate.Inlines.Add("b");
            tbxPointEstimate.Inlines.Add(SubscriptRun("0", 10));
            tbxPointEstimate.Inlines.Add($" = {Math.Round(B[0, 0], 4)}\n");

            for (int i = 0; i < RegressionAnalysis.ParamList.Count; i++)
            {
                tbGeneralView.Inlines.Add(" + b");
                tbGeneralView.Inlines.Add(SubscriptRun($"{RegressionAnalysis.ParamList[i] + 1}", 10));
                tbGeneralView.Inlines.Add("x");
                tbGeneralView.Inlines.Add(SubscriptRun($"{RegressionAnalysis.ParamList[i] + 1}", 10));

                if (B[i + 1, 0]<0)
                    tbRegressionEquation.Inlines.Add(" - ");
                else
                    tbRegressionEquation.Inlines.Add(" + ");
                tbRegressionEquation.Inlines.Add($"{Math.Abs(Math.Round(B[i + 1, 0], 4))}x");
                tbRegressionEquation.Inlines.Add(SubscriptRun($"{RegressionAnalysis.ParamList[i] + 1}", 10));

                tbxPointEstimate.Inlines.Add("b");
                tbxPointEstimate.Inlines.Add(SubscriptRun($"{RegressionAnalysis.ParamList[i] + 1}", 10));
                tbxPointEstimate.Inlines.Add($" = {Math.Round(B[i + 1, 0], 4)}\n");
            }            
        }

        /// <summary>
        /// Инициализация Y
        /// </summary>
        private void InitializeY()
        {
            tbxY.Text = "";
            tbxY_.Text = "";
            tbxDifference.Text = "";
            DenseMatrix NewY = RegressionAnalysis.Get_NewY();
            for (int i = 0; i < RegressionAnalysis.Y.RowCount; i++)
            {
                tbxY.Inlines.Add("y");
                tbxY.Inlines.Add(SubscriptRun($"{i + 1}", 10));
                tbxY.Inlines.Add($" = {Math.Round(RegressionAnalysis.Y[i, 0], 4)}\n");

                tbxY_.Inlines.Add("y^");
                tbxY_.Inlines.Add(SubscriptRun($"{i + 1}", 10));
                tbxY_.Inlines.Add($" = {Math.Round(NewY[i, 0], 4)}\n");

                tbxDifference.Inlines.Add("y");
                tbxDifference.Inlines.Add(SubscriptRun($"{i + 1}", 10));
                tbxDifference.Inlines.Add(" - y^");
                tbxDifference.Inlines.Add(SubscriptRun($"{i + 1}", 10));
                tbxDifference.Inlines.Add($" = {Math.Round(RegressionAnalysis.Y[i, 0] - NewY[i, 0], 4)}\n");                
            }
        }

        /// <summary>
        /// Оценка значимости коэффициентов уравнения регрессии
        /// </summary>
        private void InitializeSignificanceOfTheCoefficient()
        {
            tbxIntervalEstimate.Text = "";
            tbxSignificanceOfTheCoefficient.Text = "";

            RegressionAnalysis.EstimateSignificanceOfTheCoefficient();
            for (int i = 0; i < RegressionAnalysis.CoefficientSignificance.Count; i++)
            {
                tbxIntervalEstimate.Inlines.Add($"{RegressionAnalysis.Intervals[i][0]} {(char)'\u2A7D'} b");
                if (i == 0)
                    tbxIntervalEstimate.Inlines.Add(SubscriptRun($"{i}", 10));
                else
                    tbxIntervalEstimate.Inlines.Add(SubscriptRun($"{RegressionAnalysis.ParamList[i-1]+1}", 10));
                tbxIntervalEstimate.Inlines.Add($" {(char)'\u2A7D'} {RegressionAnalysis.Intervals[i][1]}\n");

                tbxSignificanceOfTheCoefficient.Inlines.Add("b");
                if (i==0)
                    tbxSignificanceOfTheCoefficient.Inlines.Add(SubscriptRun($"{i}", 10));
                else
                    tbxSignificanceOfTheCoefficient.Inlines.Add(SubscriptRun($"{RegressionAnalysis.ParamList[i-1]+1}", 10));
                if (RegressionAnalysis.CoefficientSignificance[i])
                    tbxSignificanceOfTheCoefficient.Inlines.Add(" значим\n");
                else
                    tbxSignificanceOfTheCoefficient.Inlines.Add(" не значим\n");
            }
        }

        /// <summary>
        /// Интервальная оценка уравнения регрессии
        /// </summary>
        private void InitializeIntervalEstimateEquation()
        {
            tbxIntervalEstimateEquation.Text = "";
            RegressionAnalysis.IntervalEstimateEquation();
            for (int i = 0; i < RegressionAnalysis.IntervalsEquation.Count; i++)
            {
                tbxIntervalEstimateEquation.Inlines.Add($"{Math.Round(RegressionAnalysis.IntervalsEquation[i][0], 4)} {(char)'\u2A7D'} Y~");
                tbxIntervalEstimateEquation.Inlines.Add(SubscriptRun($"{i+1}", 10));
                tbxIntervalEstimateEquation.Inlines.Add($" {(char)'\u2A7D'} {Math.Round(RegressionAnalysis.IntervalsEquation[i][1], 4)}\n");
            }
        }


        /// <summary>
        /// Оценка значимости уравнения регрессии
        /// </summary>
        private void InitializeSignificanceOfTheEquation()
        {
            double f_exp = RegressionAnalysis.F_exp();
            tbSignificanceOfTheEquation.Text = "";
            tbSignificanceOfTheEquation.Inlines.Add("F");
            tbSignificanceOfTheEquation.Inlines.Add(SubscriptRun($"кр", 10));
            tbSignificanceOfTheEquation.Inlines.Add($"(0.05;{RegressionAnalysis.k+1};{RegressionAnalysis.n- RegressionAnalysis.k-1}) = {Math.Round(RegressionAnalysis.F_crit[RegressionAnalysis.k - 1], 2)} F");
            tbSignificanceOfTheEquation.Inlines.Add(SubscriptRun($"набл", 10));
            tbSignificanceOfTheEquation.Inlines.Add($" = {Math.Round(f_exp, 2)}\t");
            if (f_exp > RegressionAnalysis.F_crit[RegressionAnalysis.k - 1])
                tbSignificanceOfTheEquation.Inlines.Add("Уравнение регрессии значимо");
            else
                tbSignificanceOfTheEquation.Inlines.Add("Уравнение регрессии не значимо");            
        }

        /// <summary>
        /// Оценка адекватности уравнения регрессии
        /// </summary>
        private void InitializeEvaluatingAdequacyModel()
        {
            double R_2 = RegressionAnalysis.CoefficientOfDetermination();
            tbCoefficientOfDetermination.Text = $"Значение коэффициента детерминации: {Math.Round(R_2,2)}\t\t";
            if (R_2 >= 0.8)
                tbCoefficientOfDetermination.Text += "Построенная модель адеквантна";
            else if (R_2 >= 0.5)
                tbCoefficientOfDetermination.Text += "Построенная модель является приемлемой";
            else
                tbCoefficientOfDetermination.Text += "Построенная модель неадеквантна";
        }

        /// <summary>
        /// Обновление
        /// </summary>
        private void Refresh()
        {           
            //Очистка всех TextBlock
            tbGeneralView.Text = null;
            tbxY.Text = null;
            tbxY_.Text = null;
            tbxDifference.Text = null;
            tbRegressionEquation.Text = null;
            tbxPointEstimate.Text = null;
            tbxIntervalEstimate.Text = null;
            tbxSignificanceOfTheCoefficient.Text = null;
            tbxIntervalEstimateEquation.Text = null;
            tbSignificanceOfTheEquation.Text = null;
            tbCoefficientOfDetermination.Text = null;
            //Обновление
            InitializeViewOfTheEquation();
            InitializeY();
            InitializeSignificanceOfTheEquation();
            InitializeSignificanceOfTheCoefficient();
            InitializeIntervalEstimateEquation();
            InitializeEvaluatingAdequacyModel();

            if (RegressionAnalysis.SearchForAMinorVariable() == -1 || RegressionAnalysis.k < 2)
                btnNextStep.IsEnabled = false;
            else
                btnNextStep.IsEnabled = true;
            if (RegressionAnalysis.k == Data.countParametrs - 1)
                btnPreviousStep.IsEnabled = false;
        }

        /// <summary>
        /// Обработчик события вызова предыдущего шага регрессионного анализа
        /// </summary>
        private void BtnPreviousStep_Click(object sender, RoutedEventArgs e)
        {
            RegressionAnalysis.AddParameter();
            Refresh();
        }

        /// <summary>
        /// Обработчик события вызова следующего шага регрессионного анализа
        /// </summary>
        private void BtnNextStep_Click(object sender, RoutedEventArgs e)
        {
            RegressionAnalysis.DeleteParameter();
            Refresh();
            btnPreviousStep.IsEnabled = true;
        }

        /// <summary>
        /// Инициализация grid для прогнозирования
        /// </summary>
        private void InitializeGrid(Grid grid)
        {
            grid.Margin = new Thickness(5);
            for (int i = 0; i < 5; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20)});
            }
            for (int i = 0; i < 6; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });
            }
                        
            for (int i=0;i<15;i++)
            {
                TextBlock tb = new TextBlock();
                tb.Text = $"X{i+1}";
                Grid.SetColumn(tb, i/5*2);
                Grid.SetRow(tb, i % 5);
                grid.Children.Add(tb);

                TextBox tbx = new TextBox();
                tbx.Name = grid.Name=="gNormData" ? $"tbxXNorm{i}" : $"tbxX{i}";
                Grid.SetColumn(tbx, ((i / 5) * 2) + 1);
                Grid.SetRow(tbx, i % 5);
                grid.Children.Add(tbx);
            }          
        }

        /// <summary>
        /// Обработчик события вызова прогнозирования
        /// </summary>
        private void BtnRegress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<double> temp = new List<double>();
                foreach (object child in gEnterData.Children)
                {
                    if (child is TextBox)
                    {
                        TextBox tb = child as TextBox;
                        int i = int.Parse(tb.Name.Substring(4));
                        if (RegressionAnalysis.ParamList.Contains(i))
                        {
                            if (tb.Text == null) throw new Exception($"Введите значение параметра X{i}!");
                            double x = double.Parse(tb.Text);

                            if (i == 5 || i == 6 || i == 7 || i == 10 || i == 14 || i == 15)
                                temp.Add(Math.Log10(x));
                            else if (i == 11)
                            {
                                temp.Add(x);
                            }
                            else if (i == 12)
                            {
                                temp.Add((x - DescriptiveStatistics.Min(MainWindow.Matrix[i])) / (DescriptiveStatistics.Max(MainWindow.Matrix[i]) - DescriptiveStatistics.Min(MainWindow.Matrix[i])));
                            }
                            else
                                temp.Add(x * x);
                        }
                    }
                }

                DenseMatrix X0 = new DenseMatrix(RegressionAnalysis.k + 1, 1);
                X0[0, 0] = 1;
                int j = 0;
                foreach (object child in gNormData.Children)
                {
                    if (child is TextBox)
                    {
                        TextBox tb = child as TextBox;
                        int i = int.Parse(tb.Name.Substring(8));
                        if (RegressionAnalysis.ParamList.Contains(i))
                        {
                            X0[j + 1, 0] = temp[j];
                            tb.Text = $"{Math.Round(X0[j + 1, 0], 4)} ";
                            j++;
                        }
                        else
                            tb.Text = null;
                    }
                }
                   
                DenseMatrix Xt_X_inverse = RegressionAnalysis.Get_Xt_X_inverse();
                DenseMatrix Matrix = (DenseMatrix)((DenseMatrix)X0.TransposeThisAndMultiply(Xt_X_inverse)).Multiply(X0);
                double incurracy = RegressionAnalysis.t_crit[RegressionAnalysis.k - 1] * Math.Sqrt(RegressionAnalysis.s_2 * (Matrix[0, 0] + 1));
                
                DenseMatrix B = RegressionAnalysis.GetCoefficient();
                double NormPrognoz = ((DenseMatrix)X0.TransposeThisAndMultiply(B))[0,0];
                double leftNormPrognoz = NormPrognoz - incurracy, rightNormPrognoz = NormPrognoz + incurracy;
                tblNormPrognoz.Text = $" Полученное значение: Y^ = {Math.Round(NormPrognoz, 4)}";
                tblNormPrognoz.Text += $"\n Интервал предсказания: {Math.Round(leftNormPrognoz, 4)} {(char)'\u2A7D'} Y~ {(char)'\u2A7D'} {Math.Round(rightNormPrognoz, 4)}\n";

                double Prognoz = Math.Pow(10,NormPrognoz);
                tblPrognoz.Text = $" Полученное значение: Y^ = {Math.Round(Prognoz, 4)}";
                double leftPrognoz = Math.Pow(10, leftNormPrognoz), rightPrognoz = Math.Pow(10, rightNormPrognoz);
                tblPrognoz.Text += $"\n Интервал предсказания: {Math.Round(leftPrognoz, 4)} {(char)'\u2A7D'} Y~ {(char)'\u2A7D'} {Math.Round(rightPrognoz, 4)}\n";

            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

       
    }
}
