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
        #region Свойства

        /// <summary>
        /// Цвет сетки.
        /// </summary>
        public Brush MarkingGridBrush => (Brush) new SolidColorBrush(SelectedGridColor);

        /// <summary>
        /// Цвет сетки.
        /// </summary>
        public Color SelectedGridColor
        {
            get
            {
                return _selectedGridColor;
            }
            set
            {
                _selectedGridColor = value;
                OnPropertyChanged(nameof(SelectedGridColor));
                OnPropertyChanged(nameof(MarkingGridBrush));
            }
        }
        private Color _selectedGridColor = Brushes.Chartreuse.Color;

        /// <summary>
        /// Цвет фона.
        /// </summary>
        public Brush BackgroundBrush => (Brush)new SolidColorBrush(SelectedBackgroundColor);

        /// <summary>
        /// Цвет фона.
        /// </summary>
        public Color SelectedBackgroundColor
        {
            get
            {
                return _selectedBackgroundColor;
            }
            set
            {
                _selectedBackgroundColor = value;
                OnPropertyChanged(nameof(SelectedBackgroundColor));
                OnPropertyChanged(nameof(BackgroundBrush));
            }
        }
        private Color _selectedBackgroundColor = Brushes.Black.Color;

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
        /// Показывать горизонтальную линейку.
        /// </summary>
        public Visibility HorizontalRulerPanelVisibility => _isHorizontalRulerPanelVisibilityChecked ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Показывать горизонтальную линейку.
        /// </summary>
        public bool IsHorizontalRulerPanelVisibilityChecked
        {
            get { return _isHorizontalRulerPanelVisibilityChecked; }
            set
            {
                _isHorizontalRulerPanelVisibilityChecked = value;
                OnPropertyChanged(nameof(IsHorizontalRulerPanelVisibilityChecked));
                OnPropertyChanged(nameof(HorizontalRulerPanelVisibility));
            }
        }
        private bool _isHorizontalRulerPanelVisibilityChecked = true;

        /// <summary>
        /// Показывать вертикальную линейку.
        /// </summary>
        public Visibility VerticalRulerPanelVisibility => _isVerticalRulerPanelVisibilityChecked ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Показывать вертикальную линейку.
        /// </summary>
        public bool IsVerticalRulerPanelVisibilityChecked
        {
            get { return _isVerticalRulerPanelVisibilityChecked; }
            set
            {
                _isVerticalRulerPanelVisibilityChecked = value;
                OnPropertyChanged(nameof(IsVerticalRulerPanelVisibilityChecked));
                OnPropertyChanged(nameof(VerticalRulerPanelVisibility));
            }
        }
        private bool _isVerticalRulerPanelVisibilityChecked = true;

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

        #endregion

        #region * Конструктор

        /// <summary>
        /// * Конструктор
        /// </summary>
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion

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
