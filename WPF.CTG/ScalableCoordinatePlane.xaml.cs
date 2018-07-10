using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.CTG
{
    /// <summary>
    /// Масштабируемая координатная плоскость.
    /// </summary>
    public partial class ScalableCoordinatePlane : Canvas, INotifyPropertyChanged
    {
        #region Свойства зависимости

        /// <summary>
        /// Цвет кисти разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty GridColorProperty = DependencyProperty.Register(
            nameof(GridColor),
            typeof(Color),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(Colors.Transparent, OnGridColorPropertyChange));

        /// <summary>
        /// Смещение координатной плоскости по оси X.
        /// </summary>
        public static readonly DependencyProperty MoveXProperty = DependencyProperty.Register(
            nameof(MoveX),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnMoveXPropertyChange));

        /// <summary>
        /// Смещение координатной плоскости по оси Y.
        /// </summary>
        public static readonly DependencyProperty MoveYProperty = DependencyProperty.Register(
            nameof(MoveY),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnMoveYPropertyChange));

        /// <summary>
        /// Кордината X опорной точки масштабирования координатной плоскости.
        /// </summary>
        public static readonly DependencyProperty ScaleCenterXProperty = DependencyProperty.Register(
            nameof(ScaleCenterX),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnScaleCenterXPropertyChange));

        /// <summary>
        /// Кордината Y опорной точки масштабирования координатной плоскости.
        /// </summary>
        public static readonly DependencyProperty ScaleCenterYProperty = DependencyProperty.Register(
            nameof(ScaleCenterY),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnScaleCenterYPropertyChange));

        /// <summary>
        /// Коэффициент масштаба по оси X.
        /// </summary>
        public static readonly DependencyProperty ScaleRateXProperty = DependencyProperty.Register(
            nameof(ScaleRateX),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(1.0, OnScaleRateXPropertyChange));

        /// <summary>
        /// Коэффициент масштаба по оси Y.
        /// </summary>
        public static readonly DependencyProperty ScaleRateYProperty = DependencyProperty.Register(
            nameof(ScaleRateY),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(1.0, OnScaleRateYPropertyChange));

        #endregion

        #region Акцессоры свойств зависимости

        /// <summary>
        /// Цвет кисти разметочной сетки
        /// </summary>
        public Color GridColor
        {
            get { return (Color) GetValue(GridColorProperty); }
            set { SetValue(GridColorProperty, value); }
        }

        /// <summary>
        /// Смещение по оси X
        /// </summary>
        public double MoveX
        {
            get { return (double)GetValue(MoveXProperty); }
            set { SetValue(MoveXProperty, value); }
        }

        /// <summary>
        /// Смещение по оси Y
        /// </summary>
        public double MoveY
        {
            get { return (double)GetValue(MoveYProperty); }
            set { SetValue(MoveYProperty, value); }
        }

        /// <summary>
        /// Кордината X опорной точки масштабирования.
        /// </summary>
        public double ScaleCenterX
        {
            get { return (double)GetValue(ScaleCenterXProperty); }
            set { SetValue(ScaleCenterXProperty, value); }
        }

        /// <summary>
        /// Кордината Y опорной точки масштабирования.
        /// </summary>
        public double ScaleCenterY
        {
            get { return (double)GetValue(ScaleCenterYProperty); }
            set { SetValue(ScaleCenterYProperty, value); }
        }

        /// <summary>
        /// Коэффициент масштаба по оси X.
        /// </summary>
        public double ScaleRateX
        {
            get { return (double)GetValue(ScaleRateXProperty); }
            set { SetValue(ScaleRateXProperty, value); }
        }

        /// <summary>
        /// Коэффициент масштаба по оси Y.
        /// </summary>
        public double ScaleRateY
        {
            get { return (double) GetValue(ScaleRateYProperty); }
            set { SetValue(ScaleRateYProperty, value); }
        }

        #endregion

        #region Приватные поля

        /// <summary>
        /// Геометрия разметочной сетки.
        /// </summary>
        private GeometryDrawing _markingGridGeometry;

        /// <summary>
        /// Кисть разметочной сетки.
        /// </summary>
        private SolidColorBrush _coordinateGridColor;

        /// <summary>
        /// Трансформация смещения.
        /// </summary>
        private TranslateTransform _translateTransform;

        /// <summary>
        /// Трансформация масштабирования.
        /// </summary>
        private ScaleTransform _scaleTransform;

        /// <summary>
        /// Родительский канвас
        /// </summary>        
        private Canvas _coordinateViewPort;

        /// <summary>
        /// This
        /// </summary>
        private Canvas _scalableCoordinatePlane;

        #endregion

        #region * Конструкторы

        /// <summary>
        /// * Конструктор
        /// </summary>
        public ScalableCoordinatePlane()
        {
            DataContext = this;
            InitializeComponent();
            MarkingGridInitialize();
            МodifierInitialize();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Инициализация сетки на заднем фоне координатной плоскости.
        /// </summary>
        private void MarkingGridInitialize()
        {
            var rG1 = new RectangleGeometry();
            rG1.Rect = new Rect(1, 1, 10, 10);

            var rG2 = new RectangleGeometry();
            rG2.Rect = new Rect(1.2, 1.2, 9.8, 9.8);

            var gG = new GeometryGroup() { FillRule = FillRule.EvenOdd };
            gG.Children.Add(rG1);
            gG.Children.Add(rG2);

            _markingGridGeometry = new GeometryDrawing() { Brush = _coordinateGridColor = new SolidColorBrush() };
            _markingGridGeometry.Geometry = gG;

            var dB = new DrawingBrush()
            {
                TileMode = TileMode.FlipXY,
                Viewport = new Rect(0, 0, 10, 10),
                ViewportUnits = BrushMappingMode.Absolute
            };

            dB.Drawing = _markingGridGeometry;
            this.Background = dB;
        }

        /// <summary>
        /// Инициализация трансформаций.
        /// </summary>
        private void МodifierInitialize()
        {
            var tg = new TransformGroup();
            tg.Children.Add(_translateTransform = new TranslateTransform());
            tg.Children.Add(_scaleTransform = new ScaleTransform());
            RenderTransform = tg;
        }

        #endregion

        #region Обработчики событий изменения свойств зависимости

        /// <summary>
        /// Изменение цвета кисти разметочной сетки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnGridColorPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                var scb = (SolidColorBrush)obj._markingGridGeometry.Brush;
                scb.Color = (Color)e.NewValue;
            }
        }

        /// <summary>
        /// Изменение смещения по оси X
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMoveXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._translateTransform.X = (double) e.NewValue;
            }
        }

        /// <summary>
        /// Изменение смещения по оси Y
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMoveYPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._translateTransform.Y = (double) e.NewValue;
            }
        }

        /// <summary>
        /// Изменение кординаты X опорной точки масштабирования.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScaleCenterXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._scaleTransform.CenterX = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Изменение кординаты Y опорной точки масштабирования.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScaleCenterYPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._scaleTransform.CenterY = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Изменение коэффициента масштаба по оси X.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScaleRateXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._scaleTransform.ScaleX = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Изменение коэффициента масштаба по оси Y.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScaleRateYPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._scaleTransform.ScaleY = (double)e.NewValue;
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
