using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for ScalableCoordinateTimeGrid.xaml
    /// </summary>
    public partial class CoordinateTimeGrid : UserControl, INotifyPropertyChanged
    {
        #region Свойства зависимости

        /// <summary>
        /// Определяет визуальное состояния элемента отображения отладочной информации.
        /// </summary>
        public static readonly DependencyProperty DebugInfoVisibilityProperty = DependencyProperty.Register(
            nameof(DebugInfoVisibility),
            typeof(Visibility),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public static readonly DependencyProperty IsBlockingScaleXProperty = DependencyProperty.Register(
            nameof(IsBlockingScaleX),
            typeof(bool),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(false));

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public static readonly DependencyProperty IsBlockingScaleYProperty = DependencyProperty.Register(
            nameof(IsBlockingScaleY),
            typeof(bool),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(false));

        /// <summary>
        /// Визуальное состояние вертикальной полосы прокрутки.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(VerticalScrollBarVisibility),
            typeof(Visibility),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Визуальное состояние горизонтальной полосы прокрутки.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(HorizontalScrollBarVisibility),
            typeof(Visibility),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Цвет кисти разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridBrushProperty = DependencyProperty.Register(
            nameof(MarkingGridBrush),
            typeof(Brush),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Толщина линий разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridStrokeThicknessProperty = DependencyProperty.Register(
            nameof(MarkingGridStrokeThickness),
            typeof(double),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(0.4));

        #endregion

        #region Акцессоры свойств зависимости

        /// <summary>
        /// Определяет визуальное состояния элемента отображения отладочной информации.
        /// </summary>
        public Visibility DebugInfoVisibility
        {
            get { return (Visibility)GetValue(DebugInfoVisibilityProperty); }
            set { SetValue(DebugInfoVisibilityProperty, value); }
        }

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public bool IsBlockingScaleX
        {
            get { return (bool)GetValue(IsBlockingScaleXProperty); }
            set { SetValue(IsBlockingScaleXProperty, value); }
        }

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public bool IsBlockingScaleY
        {
            get { return (bool)GetValue(IsBlockingScaleYProperty); }
            set { SetValue(IsBlockingScaleYProperty, value); }
        }

        /// <summary>
        /// Визуальное состояние вертикальной полосы прокрутки.
        /// </summary>
        public Visibility VerticalScrollBarVisibility
        {
            get { return (Visibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Визуальное состояние горизонтальной полосы прокрутки.
        /// </summary>
        public Visibility HorizontalScrollBarVisibility
        {
            get { return (Visibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Кисть разметочной сетки координатной плоскости.
        /// </summary>
        public Brush MarkingGridBrush
        {
            get { return (Brush)GetValue(MarkingGridBrushProperty); }
            set { SetValue(MarkingGridBrushProperty, value); }
        }

        /// <summary>
        /// Толщина линий разметочной сетки.
        /// </summary>
        public double MarkingGridStrokeThickness
        {
            get { return (double)GetValue(MarkingGridStrokeThicknessProperty); }
            set { SetValue(MarkingGridStrokeThicknessProperty, value); }
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Управляющий трансформациями координатной плоскости.
        /// </summary>
        public TransformManager TransformManager
        {
            get
            {
                if(_transformManager == null)
                    _transformManager = new TransformManager();
                return _transformManager;
            }

            set
            {
                _transformManager = value;
                OnPropertyChanged(nameof(TransformManager));
            }
        }
        private TransformManager _transformManager;

        #endregion

        #region * Конструкторы

        public CoordinateTimeGrid()
        {
            DataContext = this;
            InitializeComponent();

            TransformManager.TransformInit(_coordinateViewPort, _scalableCoordinatePlane);
        }

        #endregion

        #region Обработчики событий изменения свойств зависимости
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
