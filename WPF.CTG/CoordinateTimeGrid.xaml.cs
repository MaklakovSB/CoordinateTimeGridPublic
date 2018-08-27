using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for ScalableCoordinateTimeGrid.xaml
    /// </summary>
    [ContentProperty("Children")]
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

        /// <summary>
        /// Оригинальная ширина координатной плоскости.
        /// </summary>
        public static readonly DependencyProperty OriginalPlaneWidthProperty = DependencyProperty.Register(
            nameof(OriginalPlaneWidth),
            typeof(double),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(0.0));

        /// <summary>
        /// Оригинальная высота координатной плоскости.
        /// </summary>
        public static readonly DependencyProperty OriginalPlaneHeightProperty = DependencyProperty.Register(
            nameof(OriginalPlaneHeight),
            typeof(double),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(0.0));

        /// <summary>
        /// Attached property.
        /// Координата по оси X.
        /// </summary>
        public static readonly DependencyProperty XProperty = DependencyProperty.RegisterAttached(
            "X",
            typeof(double),
            typeof(CoordinateTimeGrid),
            new FrameworkPropertyMetadata(0.0, OnXPropertyChange)
        );

        /// <summary>
        /// Attached property.
        /// Координата по оси Y.
        /// </summary>
        public static readonly DependencyProperty YProperty = DependencyProperty.RegisterAttached(
            "Y",
            typeof(double),
            typeof(CoordinateTimeGrid),
            new FrameworkPropertyMetadata(0.0, OnYPropertyChange)
        );

        /// <summary>
        /// Attached property.
        /// Z-индекс.
        /// </summary>
        public static readonly DependencyProperty ZIndexProperty = DependencyProperty.RegisterAttached(
            "ZIndex",
            typeof(int),
            typeof(CoordinateTimeGrid),
            new FrameworkPropertyMetadata(0, OnZIndexPropertyChange)
        );

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

        /// <summary>
        /// Оригинальная ширина координатной плоскости.
        /// </summary>
        public double OriginalPlaneWidth
        {
            get { return (double)GetValue(OriginalPlaneWidthProperty); }
            set { SetValue(OriginalPlaneWidthProperty, value); }
        }

        /// <summary>
        /// Оригинальная высота координатной плоскости.
        /// </summary>
        public double OriginalPlaneHeight
        {
            get { return (double)GetValue(OriginalPlaneHeightProperty); }
            set { SetValue(OriginalPlaneHeightProperty, value); }
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

        /// <summary>
        /// Коллекция дочерних элементов.
        /// </summary>
        public UIElementCollection Children => _scalableCoordinatePlane.Children;

        #endregion

        #region * Конструкторы

        /// <summary>
        /// * Конструктор
        /// </summary>
        public CoordinateTimeGrid()
        {
            DataContext = this;
            InitializeComponent();

            TransformManager.TransformInit(this, _coordinateViewPort, _scalableCoordinatePlane);
        }

        #endregion

        #region Методы прикрепляемых свойств зависимости

        /// <summary>
        /// Установить значение координаты по оси X.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetX(UIElement element, double value)
        {
            element.SetValue(XProperty, value);
        }

        /// <summary>
        /// Получить значение координаты по оси X.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double GetX(UIElement element)
        {
            return (double)element.GetValue(XProperty);
        }

        /// <summary>
        /// Установить значение координаты по оси Y.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetY(UIElement element, double value)
        {
            element.SetValue(YProperty, value);
        }

        /// <summary>
        /// Получить значение координаты по оси Y.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static double GetY(UIElement element)
        {
            return (double)element.GetValue(YProperty);
        }

        /// <summary>
        /// Установить значение Z-индекса.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetZIndex(UIElement element, int value)
        {
            element.SetValue(ZIndexProperty, value);
        }

        /// <summary>
        /// Получить значение Z-индекса.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static int GetZIndex(UIElement element)
        {
            return (int)element.GetValue(ZIndexProperty);
        }

        /// <summary>
        /// Обработчик изменения координаты по оси X.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //d.SetCurrentValue(Canvas.LeftProperty, (double)e.NewValue);
            Canvas.SetLeft((UIElement)d, (double)e.NewValue);
        }

        /// <summary>
        /// Обработчик изменения координаты по оси Y.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnYPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //d.SetCurrentValue(Canvas.TopProperty, (double)e.NewValue);
            Canvas.SetTop((UIElement)d, (double)e.NewValue);
        }

        /// <summary>
        /// Обработчик изменения Z-индекса.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnZIndexPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel.SetZIndex((UIElement)d, (int)e.NewValue);
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
