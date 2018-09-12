using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF.CTG
{
    /// <summary>
    /// Масштабируемая координатная плоскость.
    /// </summary>
    public partial class ScalableCoordinatePlane : Canvas, INotifyPropertyChanged
    {
        #region Константы

        /// <summary>
        /// Зарезервированное имя вертикальных линий разметки.
        /// </summary>
        public const string VerticalLine = "VerticalLine";

        /// <summary>
        /// Зарезервированное имя горизонтальных линий разметки.
        /// </summary>
        public const string HorizontalLine = "HorizontalLine";

        #endregion

        #region Свойства зависимости

        /// <summary>
        /// Оригинальная высота.
        /// </summary>
        public static readonly DependencyProperty OriginalHeightProperty = DependencyProperty.Register(
            nameof(OriginalHeight),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnOriginalHeightPropertyChange));

        /// <summary>
        /// Оригинальная ширина.
        /// </summary>
        public static readonly DependencyProperty OriginalWidthProperty = DependencyProperty.Register(
            nameof(OriginalWidth),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.0, OnOriginalWidthPropertyChange));

        /// <summary>
        /// Цвет кисти разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridBrushProperty = DependencyProperty.Register(
            nameof(MarkingGridBrush),
            typeof(Brush),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(Brushes.Transparent, OnMarkingGridBrushPropertyChange));

        /// <summary>
        /// Толщина линий разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridStrokeThicknessProperty = DependencyProperty.Register(
            nameof(MarkingGridStrokeThickness),
            typeof(double),
            typeof(ScalableCoordinatePlane),
            new PropertyMetadata(0.4, OnMarkingGridStrokeThicknessPropertyChange));

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
        /// Оригинальная ширина.
        /// </summary>
        public double OriginalWidth
        {
            get { return (double)GetValue(OriginalWidthProperty); }
            set { SetValue(OriginalWidthProperty, value); }
        }

        /// <summary>
        /// Оригинальная высота.
        /// </summary>
        public double OriginalHeight
        {
            get { return (double)GetValue(OriginalHeightProperty); }
            set { SetValue(OriginalHeightProperty, value); }
        }

        /// <summary>
        /// Цвет кисти разметочной сетки
        /// </summary>
        public Brush MarkingGridBrush
        {
            get { return (Brush) GetValue(MarkingGridBrushProperty); }
            set { SetValue(MarkingGridBrushProperty, value); }
        }

        /// <summary>
        /// Толщина линий разметочной сетки.
        /// </summary>
        public double MarkingGridStrokeThickness
        {
            get { return (double) GetValue(MarkingGridStrokeThicknessProperty); }
            set { SetValue(MarkingGridStrokeThicknessProperty, value); }
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

        private RectangleGeometry _rectangleGeometry1;
        private RectangleGeometry _rectangleGeometry2;
        private DrawingBrush _drawingBrush;
        private GeometryDrawing _geometryDrawing = new GeometryDrawing();

        private Rect _rect1;// = new Rect { X = 1, Y = 1, Height = 5, Width = 5 };
        private Rect _rect2;// = new Rect { X = 1.05, Y = 1.05, Height = 4.95, Width = 4.95 };
        private Rect _rect3;// = new Rect { X = - 0.05, Y = -0.05, Height = 5, Width = 5 };

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

        /// <summary>
        /// Текущая дельта коэффициента масштабирования по оси X.
        /// </summary>
        public double ScaleDeltaX
        {
            get { return _scaleDeltaX; }
            set
            {
                _scaleDeltaX = value;
                OnPropertyChanged(nameof(ScaleDeltaX));
            }
        }
        private double _scaleDeltaX;

        /// <summary>
        /// Текущая дельта коэффициента масштабирования по оси Y.
        /// </summary>
        public double ScaleDeltaY
        {
            get { return _scaleDeltaY; }
            set
            {
                _scaleDeltaY = value;
                OnPropertyChanged(nameof(ScaleDeltaY));
            }
        }
        private double _scaleDeltaY;

        #endregion

        #region * Конструкторы

        /// <summary>
        /// * Конструктор
        /// </summary>
        public ScalableCoordinatePlane() : base() 
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Инициализация сетки.
        /// </summary>
        private void MarkingGridInitializeBackground()
        {
            var thickness = (double)this.GetValue(MarkingGridStrokeThicknessProperty);

            thickness = thickness / 2;
            var th = thickness / 2;

            _rect1 = new Rect { X = 1, Y = 1, Height = 5, Width = 5 };
            _rect2 = new Rect { X = _rect1.X + thickness, Y = _rect1.Y + thickness, Height = _rect1.Height - thickness, Width = _rect1.Width - thickness };
            _rect3 = new Rect { X = 0, Y = 0, Height = _rect1.Height, Width = _rect1.Width };


        _rectangleGeometry1 = new RectangleGeometry() {Rect = _rect1};
            _rectangleGeometry2 = new RectangleGeometry() {Rect = _rect2};

            var geometryGroup = new GeometryGroup() {FillRule = FillRule.EvenOdd};

            geometryGroup.Children.Add(_rectangleGeometry1);
            geometryGroup.Children.Add(_rectangleGeometry2);

            _geometryDrawing.Brush = MarkingGridBrush;

            _geometryDrawing.Geometry = geometryGroup;

            _drawingBrush = new DrawingBrush()
            {
                TileMode = TileMode.FlipXY,
                Viewport = _rect3,
                ViewportUnits = BrushMappingMode.Absolute,
            };

            _drawingBrush.Drawing = _geometryDrawing;

            Background = _drawingBrush;
        }

        /// <summary>
        /// Применение масштаба по оси X
        /// </summary>
        private void ScaleRateXChange(double newScaleRate)
        {
            var newValue = newScaleRate;

            // Вычисляем текущую дельту коэффициента масштабирования.
            ScaleDeltaX = newValue / _scaleRateX;

            // Сохраняем новый коэффициент масштабирования.
            _scaleRateX = newValue;

            // Масштабируем размеры координатной плоскости.
            Width = _originalWidth * _scaleRateX;

            // Масштабируем содержимое координатной плоскости.
            if (Children.Count > 0)
            {
                foreach (FrameworkElement child in Children)
                {
                    // Масштабируем размеры содержимого.
                    child.Width *= ScaleDeltaX;

                    // Масштабируем координаты содержимого.
                    child.SetCurrentValue(Canvas.LeftProperty, Canvas.GetLeft(child) * ScaleDeltaX);
                }
            }

            var thickness = (double)this.GetValue(MarkingGridStrokeThicknessProperty);

            thickness = thickness / 2;
            var th = thickness / 2;

            // Масштабируем сетку.
            _rect1 = new Rect { X = _rect1.X, Y = _rect1.Y, Height = _rect1.Height, Width = _rect1.Width * ScaleDeltaX };
            _rect2 = new Rect { X = _rect1.X + thickness, Y = _rect1.Y + thickness, Height = _rect1.Height - thickness, Width = _rect1.Width - thickness };
            //_rect3 = new Rect { X = _rect3.X, Y = _rect3.Y, Height = _rect1.Height, Width = _rect1.Width };
            _rect3 = new Rect { X = 0, Y = 0, Height = _rect1.Height, Width = _rect1.Width };

            _rectangleGeometry1.Rect = _rect1;
            _rectangleGeometry2.Rect = _rect2;
            _drawingBrush.Viewport = _rect3;
        }

        /// <summary>
        /// Применение масштаба по оси Y
        /// </summary>
        private void ScaleRateYChange(double newScaleRate)
        {
            var newValue = newScaleRate;

            // Вычисляем текущую дельту коэффициента масштабирования.
            ScaleDeltaY = newValue / _scaleRateY;

            // Сохраняем новый коэффициент масштабирования.
            _scaleRateY = newValue;

            // Масштабируем размеры координатной плоскости.
            Height = _originalHeight * _scaleRateY;

            // Масштабируем содержимое координатной плоскости.
            if (Children.Count > 0)
            {
                foreach (FrameworkElement child in Children)
                {
                    // Масштабируем размеры содержимого.
                    child.Height *= ScaleDeltaY;

                    // Масштабируем координаты содержимого.
                    child.SetCurrentValue(Canvas.TopProperty, Canvas.GetTop(child) * ScaleDeltaY);
                }
            }

            // Используем толщину линий сетки.
            var thickness = (double)this.GetValue(MarkingGridStrokeThicknessProperty);
            thickness = thickness / 2;

            // Масштабируем сетку.
            _rect1 = new Rect { X = _rect1.X, Y = _rect1.Y, Height = _rect1.Height * ScaleDeltaY, Width = _rect1.Width };
            _rect2 = new Rect { X = _rect1.X + thickness, Y = _rect1.Y + thickness, Height = _rect1.Height - thickness, Width = _rect1.Width - thickness };
            _rect3 = new Rect { X = 0, Y = 0, Height = _rect1.Height, Width = _rect1.Width };

            _rectangleGeometry1.Rect = _rect1;
            _rectangleGeometry2.Rect = _rect2;
            _drawingBrush.Viewport = _rect3;
        }

        #endregion

        #region Обработчики событий

        /// <summary>
        /// Загрузка контрола.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            // Инициализация разметочной сетки.
            MarkingGridInitializeBackground();
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
                var newValue = (double) e.NewValue;
                obj.ScaleRateXChange(newValue);
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
                var newValue = (double)e.NewValue;
                obj.ScaleRateYChange(newValue);
            }
        }

        /// <summary>
        /// Изменение цвета сетки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMarkingGridBrushPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                var newValue = (Brush)e.NewValue;
                obj._geometryDrawing.Brush = newValue;
            }
        }

        /// <summary>
        /// Изменение толщины линий сетки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMarkingGridStrokeThicknessPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                if (obj._rectangleGeometry1 != null && obj._rectangleGeometry2 != null && obj._drawingBrush != null)
                {
                    var thickness = (double)e.NewValue;
                    thickness = thickness / 2;

                    var x = obj._rectangleGeometry1.Rect.X;
                    var y = obj._rectangleGeometry1.Rect.Y;
                    var height = obj._rectangleGeometry1.Rect.Height;
                    var width = obj._rectangleGeometry1.Rect.Width;

                    var rect1 = new Rect { X = x, Y = y, Height = height, Width = width };
                    var rect2 = new Rect { X = x + thickness, Y = y + thickness, Height = height - thickness, Width = width - thickness };
                    var rect3 = new Rect { X = 0, Y = 0, Height = height, Width = width };

                    obj._rectangleGeometry1.Rect = rect1;
                    obj._rectangleGeometry2.Rect = rect2;
                    obj._drawingBrush.Viewport = rect3;
                }
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
