using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace СoordinateTimeGridSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// Тест цвета разметочной сетки.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_coordinateTimeGrid.MarkingGridBrush != Brushes.CornflowerBlue)
            {
                _coordinateTimeGrid.MarkingGridBrush = Brushes.CornflowerBlue;
            }
            else
            {
                _coordinateTimeGrid.MarkingGridBrush = Brushes.Goldenrod;
            }
        }

        #region Реализация интерфейсов

        /// <summary>
        /// Реализация интерфейса INotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}
