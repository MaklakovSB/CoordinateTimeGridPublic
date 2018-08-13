using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        /// Инициальная высота.
        /// </summary>
        public static readonly DependencyProperty OriginalHeightProperty = DependencyProperty.Register(
            nameof(OriginalHeight),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnOriginalHeightPropertyChange));

        /// <summary>
        /// Инициальная ширина.
        /// </summary>
        public static readonly DependencyProperty OriginalWidthProperty = DependencyProperty.Register(
            nameof(OriginalWidth),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnOriginalWidthPropertyChange));


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

        /// <summary>
        /// Коэффициент масштаба по оси Y и X.
        /// </summary>
        public static readonly DependencyProperty ScaleRateProperty = DependencyProperty.Register(
            nameof(ScaleRate),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(1.0, OnScaleRatePropertyChange));

        #endregion

        #region Акцессоры свойств зависимости

        /// <summary>
        /// 
        /// </summary>
        public double OriginalWidth
        {
            get { return (double)GetValue(OriginalWidthProperty); }
            set { SetValue(OriginalWidthProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double OriginalHeight
        {
            get { return (double)GetValue(OriginalHeightProperty); }
            set { SetValue(OriginalHeightProperty, value); }
        }

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

        /// <summary>
        /// Коэффициент масштаба по оси Y и X.
        /// </summary>
        public double ScaleRate
        {
            get { return (double)GetValue(ScaleRateYProperty); }
            set { SetValue(ScaleRateYProperty, value); }
        }

        #endregion

        #region Приватные поля

        private double _originalHeight;
        private double _originalWidth;

        private double _scaleRateX = 1;
        private double _scaleRateY = 1;
        private double _scaleRate = 1;

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

        #region Свойства

        /// <summary>
        /// Ширина координатной плоскости.
        /// </summary>
        public new double Width
        {
            get
            {
                return base.Width;
            }
            private set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Высота координатной плоскости.
        /// </summary>
        public new double Height
        {
            get
            {
                return base.Height;
            }
            private set
            {
                base.Height = value;
            }
        }

        #endregion

        #region * Конструкторы

        /// <summary>
        /// * Конструктор
        /// </summary>
        public ScalableCoordinatePlane() : base() 
        {
            DataContext = this;
            //base.Height = OriginalHeight;
            //base.Width = OriginalWidth;
            
            InitializeComponent();
            //MarkingGridInitialize();
            //МodifierInitialize();
        }

        #endregion

        #region Методы

        //private int size = 10; // do something less hardcoded

        //protected override void OnRender(DrawingContext dc)
        //{
        //    Pen pen = new Pen(Brushes.AliceBlue, 0.1);

        //    // vertical lines
        //    double pos = 0;
        //    int count = 0;
        //    do
        //    {
        //        dc.DrawLine(pen, new Point(pos * ScaleRateX, 0), new Point(pos * ScaleRateX, DesiredSize.Height));
        //        pos += size * ScaleRateX;
        //        count++;
        //    }
        //    while (pos * ScaleRateX < DesiredSize.Width);

        //    //string title = count.ToString();

        //    // horizontal lines
        //    pos = 0;
        //    count = 0;
        //    do
        //    {
        //        dc.DrawLine(pen, new Point(0, pos), new Point(DesiredSize.Width, pos));
        //        pos += size * ScaleRateY;
        //        count++;
        //    }
        //    while (pos < DesiredSize.Height);

        //    // display the grid size (debug mode only!)
        //    //title += "x" + count;
        //    //dc.DrawText(new FormattedText(title, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 20, Brushes.White), new Point(0, 0));
        //}

        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    return availableSize;
        //}

        /// <summary>
        /// Инициализация сетки на заднем фоне координатной плоскости.
        /// </summary>
        private void MarkingGridInitialize()
        {
            ////var rG1 = new RectangleGeometry();
            ////rG1.Rect = new Rect(1, 1, 10, 10);

            ////var rG2 = new RectangleGeometry();
            ////rG2.Rect = new Rect(1.2, 1.2, 9.8, 9.8);

            //var P1 = new Path() {Stroke = _coordinateGridColor};

            //var lG1 = new Line()
            //{
                 
            //};

           

            //var lG2 = new LineGeometry()
            //{
            //    StartPoint = new Point() { X = 0, Y = 10 },
            //    EndPoint = new Point() { X = 320, Y = 10 },
            //    StrokeContein
            //};




            //var gG = new GeometryGroup() { FillRule = FillRule.EvenOdd };
            //gG.Children.Add(lG1);
            //gG.Children.Add(lG2);

            //_markingGridGeometry = new GeometryDrawing() { Brush = _coordinateGridColor = new SolidColorBrush() {Color = new Color() {A=255, R=255,G=255,B=255} } };
            //_markingGridGeometry.Geometry = gG;

            //var dB = new DrawingBrush()
            //{
            //    TileMode = TileMode.FlipXY,
            //    Viewport = new Rect(0, 0, 10, 10),
            //    ViewportUnits = BrushMappingMode.Absolute
            //};

            //dB.Drawing = _markingGridGeometry;
            //this.Background = dB;
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
        /// Изменение инициальной высоты после инициализации не допустимо.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnOriginalHeightPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._originalHeight = (double)e.NewValue;
                obj.Height = obj._originalHeight * obj._scaleRateY;
                //obj.OnPropertyChanged(nameof(Height));
            }
        }

        /// <summary>
        /// Изменение инициальной ширины после инициализации не допустимо.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnOriginalWidthPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj._originalWidth = (double)e.NewValue;
                obj.Width = obj._originalWidth * obj._scaleRateX;
                //obj.OnPropertyChanged(nameof(Width));

            }
        }

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
                //var scb = (SolidColorBrush)obj._markingGridGeometry.Brush;
                //scb.Color = (Color)e.NewValue;
                obj.GridColor = (Color)e.NewValue;
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
                //obj._translateTransform.X = (double) e.NewValue;
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
                //obj._translateTransform.Y = (double) e.NewValue;
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
                //obj._scaleTransform.CenterX = (double)e.NewValue;
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
                //obj._scaleTransform.CenterY = (double)e.NewValue;
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
                obj._scaleRateX = (double)e.NewValue;
                obj.Width = obj._originalWidth * obj._scaleRateX;
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
                obj._scaleRateY = (double)e.NewValue;
                obj.Height = obj._originalHeight * obj._scaleRateY;
            }
        }

        
        /// <summary>
        /// Изменение коэффициента масштаба по оси Y.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScaleRatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                var newValue = (double) e.NewValue;
                obj._scaleRate = newValue;

                obj._scaleRateY = newValue;
                obj._scaleRateX = newValue;

                obj.Height = obj._originalHeight * obj._scaleRateY;
                obj.Width = obj._originalWidth * obj._scaleRateX;
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
