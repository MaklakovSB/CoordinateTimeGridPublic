using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace СoordinateTimeGridSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Цвет сетки.
        /// </summary>
        public Brush MarkingGridBrush => (Brush) new SolidColorBrush(SelectedColor);

        /// <summary>
        /// Цвет сетки.
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
                OnPropertyChanged(nameof(MarkingGridBrush));
            }
        }
        private Color _selectedColor = Brushes.Chartreuse.Color;

        /// <summary>
        /// Показывать окно отладочной информации.
        /// </summary>
        public Visibility DebugInfoVisibility => _isDebugInfoVisibilityChecked ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Показывать окно отладочной информации.
        /// </summary>
        public bool IsDebugInfoVisibilityChecked
        {
            get { return _isDebugInfoVisibilityChecked; }
            set
            {
                _isDebugInfoVisibilityChecked = value;
                OnPropertyChanged(nameof(IsDebugInfoVisibilityChecked));
                OnPropertyChanged(nameof(DebugInfoVisibility));
            }
        }
        private bool _isDebugInfoVisibilityChecked;

        /// <summary>
        /// Показывать горизонтальную полосу прокрутки.
        /// </summary>
        public Visibility HorizontalScrollBarVisibility => _isHorizontalScrollBarVisibilityChecked ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Показывать горизонтальную полосу прокрутки.
        /// </summary>
        public bool IsHorizontalScrollBarVisibilityChecked
        {
            get { return _isHorizontalScrollBarVisibilityChecked; }
            set
            {
                _isHorizontalScrollBarVisibilityChecked = value;
                OnPropertyChanged(nameof(IsHorizontalScrollBarVisibilityChecked));
                OnPropertyChanged(nameof(HorizontalScrollBarVisibility));
            }
        }
        private bool _isHorizontalScrollBarVisibilityChecked = true;

        /// <summary>
        /// Показывать вертикальную полосу прокрутки.
        /// </summary>
        public Visibility VerticalScrollBarVisibility => _isVerticalScrollBarVisibilityChecked ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Показывать вертикальную полосу прокрутки.
        /// </summary>
        public bool IsVerticalScrollBarVisibilityChecked
        {
            get { return _isVerticalScrollBarVisibilityChecked; }
            set
            {
                _isVerticalScrollBarVisibilityChecked = value;
                OnPropertyChanged(nameof(IsVerticalScrollBarVisibilityChecked));
                OnPropertyChanged(nameof(VerticalScrollBarVisibility));
            }
        }
        private bool _isVerticalScrollBarVisibilityChecked = true;

        /// <summary>
        /// Блокировать масштабирование по оси Х.
        /// </summary>
        public bool IsBlockingScaleX
        {
            get { return _isBlockingScaleX; }
            set
            {
                _isBlockingScaleX = value;
                OnPropertyChanged(nameof(IsBlockingScaleX));
            }
        }
        private bool _isBlockingScaleX;

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public bool IsBlockingScaleY
        {
            get { return _isBlockingScaleY; }
            set
            {
                _isBlockingScaleY = value;
                OnPropertyChanged(nameof(IsBlockingScaleY));
            }
        }
        private bool _isBlockingScaleY;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
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
