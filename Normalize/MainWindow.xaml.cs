using System.Windows;

namespace Normalize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static double[][] Matrix;
        public static double[][] NormMatrix;
        int count_column;

        public MainWindow()
        {
            InitializeComponent();
            btnDescriptiveStatistics.IsEnabled = false;
            btnCheckingForNormality.IsEnabled = false;
            btnCorrelationAnalysis.IsEnabled = false;
            btnRegressionAnalysis.IsEnabled = false;
        }
        
        #region Описательная статистика
        private void BtnDescriptiveStatistics_Click(object sender, RoutedEventArgs e)
        {
            DescriptiveStatisticsWindow descriptiveStatistics=new DescriptiveStatisticsWindow();
            descriptiveStatistics.Show();
            btnCheckingForNormality.IsEnabled = true;
        }
        #endregion

        #region Проверка нормальности распределения 
        private void BtnCheckingForNormality_Click(object sender, RoutedEventArgs e)
        {
            CheckingForNormalityWindow checkingForNormality = new CheckingForNormalityWindow();
            checkingForNormality.Show();
            btnCorrelationAnalysis.IsEnabled = true;
        }
        #endregion

        #region Коррелиационный  анализ 
        private void BtnCorrelationAnalysis_Click(object sender, RoutedEventArgs e)
        {
            CorrelationAnalysisWindow correlationAnalysisWindow = new CorrelationAnalysisWindow();
            correlationAnalysisWindow.Show();
            btnRegressionAnalysis.IsEnabled = true;
        }
        #endregion

        #region Регрессионный анализ 
        private void BtnRegressionAnalysis_Click(object sender, RoutedEventArgs e)
        {
            RegressionAnalysisWindow regressionAnalysisWindow = new RegressionAnalysisWindow();
            regressionAnalysisWindow.Show();
        }
        #endregion

        #region Загрузка данных
        private void miDownloadDataClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = "csv";
            openFileDialog.Filter = "All files|*.csv";

            if (openFileDialog.ShowDialog() == true && openFileDialog.SafeFileName != "")
            {
                string filepath = openFileDialog.FileName;
                Data.GetMatrix(filepath);
                Matrix = Data.Array;
                count_column = Data.parametrs.Count;
                NormMatrix = Normalization.GetNormMatrix(Matrix, Normalization.Sqr);
                btnDescriptiveStatistics.IsEnabled = true;
            }
        }
        #endregion

        #region О программе 
        private void miInformationClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Название продукта:\n«Прогнозирование стоимости центрального процессора»" +
                            "\n\nРазработчик: Якупова Ильсия Фаизовна (гр. 17КБ(с)РЗПО)" +
                            "\nРуководитель: Влацкая Ирина Валерьевна (зав. каф. КБМОИС, к.т.н., доцент)" +
                            "\n\nВерсия: 0.0.1" + "\nГод издания: 2020", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion
    }
}
