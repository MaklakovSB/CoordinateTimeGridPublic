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
            new PropertyMetadata(Visibility.Collapsed, OnDebugInfoVisibilityPropertyChange));

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public static readonly DependencyProperty IsBlockingScaleXProperty = DependencyProperty.Register(
            nameof(IsBlockingScaleX),
            typeof(bool),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(false, OnIsBlockingScaleXPropertyChange));

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public static readonly DependencyProperty IsBlockingScaleYProperty = DependencyProperty.Register(
            nameof(IsBlockingScaleY),
            typeof(bool),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(false, OnIsBlockingScaleYPropertyChange));

        /// <summary>
        /// Визуальное состояние вертикальной полосы прокрутки.
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(VerticalScrollBarVisibility),
            typeof(Visibility),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Visibility.Collapsed, OnVerticalScrollBarVisibilityPropertyChange));

        /// <summary>
        /// Визуальное состояние горизонтальной полосы прокрутки.
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register(
            nameof(HorizontalScrollBarVisibility),
            typeof(Visibility),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Visibility.Collapsed, OnHorizontalScrollBarVisibilityPropertyChange));

        /// <summary>
        /// Цвет кисти разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridBrushProperty = DependencyProperty.Register(
            nameof(MarkingGridBrush),
            typeof(Brush),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Brushes.Transparent, OnMarkingGridBrushPropertyChange));

        /// <summary>
        /// Толщина линий разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridStrokeThicknessProperty = DependencyProperty.Register(
            nameof(MarkingGridStrokeThickness),
            typeof(double),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(0.4, OnMarkingGridStrokeThicknessPropertyChange));

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

        /// <summary>
        /// Изменение визуального состояния элемента отображения отладочной информации.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnDebugInfoVisibilityPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var div = obj._debugInfo;
                div.SetValue(VisibilityProperty, (Visibility)e.NewValue);
            }
        }

        /// <summary>
        /// Изменение визуального состояния элемента отображения отладочной информации.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsBlockingScaleXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var tm = obj.TransformManager;
                tm.SetValue(IsBlockingScaleXProperty, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Изменение визуального состояния элемента отображения отладочной информации.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsBlockingScaleYPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var tm = obj.TransformManager;
                tm.SetValue(IsBlockingScaleYProperty, (bool)e.NewValue);
            }
        }

        /// <summary>
        /// Визуального состояния вертикальной полосы прокрутки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnVerticalScrollBarVisibilityPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var vsb = obj._verticalScrollBar;
                vsb.SetValue(VisibilityProperty, (Visibility)e.NewValue);

            }
        }

        /// <summary>
        /// Визуального состояния горизонтальной полосы прокрутки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnHorizontalScrollBarVisibilityPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var hsb = obj._horizontalScrollBar;
                hsb.SetValue(VisibilityProperty, (Visibility)e.NewValue);
            }
        }

        /// <summary>
        /// Изменение цвета кисти разметочной сетки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMarkingGridBrushPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                obj.SetValue(MarkingGridBrushProperty, e.NewValue);
            }
        }

        /// <summary>
        /// Изменение толщины линий разметочной сетки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMarkingGridStrokeThicknessPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                obj.SetValue(MarkingGridStrokeThicknessProperty, e.NewValue);
            }
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
