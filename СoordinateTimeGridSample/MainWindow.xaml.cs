using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using WPF.CTG;

namespace СoordinateTimeGridSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Цвет разметки
        /// </summary>
        public Color ColorGr
        {
            get
            {
                return _colorGr;
            }
            set
            {
                _colorGr = value; 
                OnPropertyChanged(nameof(ColorGr));
            }
        }
        private Color _colorGr;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            ColorGr = Colors.Blue;
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
