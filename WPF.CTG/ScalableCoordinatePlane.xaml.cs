using System.ComponentModel;
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
        public const string verticalLine = "verticalLine";

        /// <summary>
        /// Зарезервированное имя горизонтальных линий разметки.
        /// </summary>
        public const string horizontalLine = "horizontalLine";

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
        private double _scaleRate = 1;

        /// <summary>
        /// Кисть разметочной сетки.
        /// </summary>
        private SolidColorBrush _coordinateGridColor;

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

        /// <summary>
        /// Текущая дельта коэффициента масштабирования.
        /// </summary>
        public double ScaleDelta
        {
            get { return _scaleDelta; }
            set
            {
                _scaleDelta = value;
                OnPropertyChanged(nameof(ScaleDelta));
            }
        }
        private double _scaleDelta;

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
        /// Инициализация сетки на заднем фоне координатной плоскости.
        /// </summary>
        private void MarkingGridInitialize()
        {
            var vertCount = OriginalWidth / 10;
            var horizCount = OriginalHeight / 10;

            for (var x = 0; x < vertCount; x++)
            {
                var vLine = new Line()
                {
                    Name = nameof(verticalLine),
                    X1 = x * 10,
                    X2 = x * 10,
                    Y1 = 0,
                    Y2 = ActualHeight,
                    //StrokeThickness = 0.4,
                };

                Binding bindActualHeight = new Binding();
                bindActualHeight.Source = this;
                bindActualHeight.Path = new PropertyPath(nameof(ActualHeight));
                bindActualHeight.Mode = BindingMode.OneWay;
                vLine.SetBinding(Line.Y2Property, bindActualHeight);

                Binding bindMarkingGridBrush = new Binding();
                bindMarkingGridBrush.Source = this;
                bindMarkingGridBrush.Path = new PropertyPath(nameof(MarkingGridBrush));
                bindMarkingGridBrush.Mode = BindingMode.OneWay;
                vLine.SetBinding(Shape.StrokeProperty, bindMarkingGridBrush);

                Binding bindMarkingGridStrokeThickness = new Binding();
                bindMarkingGridStrokeThickness.Source = this;
                bindMarkingGridStrokeThickness.Path = new PropertyPath(nameof(MarkingGridStrokeThickness));
                bindMarkingGridStrokeThickness.Mode = BindingMode.OneWay;
                vLine.SetBinding(Shape.StrokeThicknessProperty, bindMarkingGridStrokeThickness);


                Children.Add(vLine);
            }

            for (var y = 0; y < horizCount; y++)
            {
                var hLine = new Line()
                {
                    Name = nameof(horizontalLine),
                    X1 = 0,
                    X2 = ActualWidth,
                    Y1 = y * 10,
                    Y2 = y * 10,
                    //StrokeThickness = 0.2,
                    //SnapsToDevicePixels = true,
                    //Stroke = MarkingGridBrush,
                };

                Binding bindActualWidth = new Binding();
                bindActualWidth.Source = this;
                bindActualWidth.Path = new PropertyPath(nameof(ActualWidth));
                bindActualWidth.Mode = BindingMode.OneWay;
                hLine.SetBinding(Line.X2Property, bindActualWidth);

                Binding bindMarkingGridBrush = new Binding();
                bindMarkingGridBrush.Source = this;
                bindMarkingGridBrush.Path = new PropertyPath(nameof(MarkingGridBrush));
                bindMarkingGridBrush.Mode = BindingMode.OneWay;
                hLine.SetBinding(Shape.StrokeProperty, bindMarkingGridBrush);

                Binding bindMarkingGridStrokeThickness = new Binding();
                bindMarkingGridStrokeThickness.Source = this;
                bindMarkingGridStrokeThickness.Path = new PropertyPath(nameof(MarkingGridStrokeThickness));
                bindMarkingGridStrokeThickness.Mode = BindingMode.OneWay;
                hLine.SetBinding(Shape.StrokeThicknessProperty, bindMarkingGridStrokeThickness);

                Children.Add(hLine);
            }
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
            MarkingGridInitialize();
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
        /// Изменение цвета кисти разметочной сетки.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnMarkingGridBrushPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ScalableCoordinatePlane;
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
            var obj = d as ScalableCoordinatePlane;
            if (obj != null)
            {
                obj.SetValue(MarkingGridStrokeThicknessProperty, e.NewValue);
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

                // Вычисляем текущую дельту коэффициента масштабирования.
                obj.ScaleDelta = newValue / obj._scaleRate;

                // Сохраняем новый коэффициент масштабирования.
                obj._scaleRate = newValue;
                obj._scaleRateY = newValue;
                obj._scaleRateX = newValue;

                // Масштабируем размеры координатной плоскости.
                obj.Height = obj._originalHeight * obj._scaleRateY;
                obj.Width = obj._originalWidth * obj._scaleRateX;

                // Масштабируем содержимое координатной плоскости.
                if (obj.Children.Count > 0)
                {
                    foreach (FrameworkElement child in obj.Children)
                    {
                        child.Width *= obj.ScaleDelta;
                        child.Height *= obj.ScaleDelta;

                        child.SetCurrentValue(Canvas.LeftProperty, Canvas.GetLeft(child) * obj.ScaleDelta);
                        child.SetCurrentValue(Canvas.TopProperty, Canvas.GetTop(child) * obj.ScaleDelta);

                        // Масштабируем координаты вертикальных линий разметки.
                        if (child.Name == nameof(verticalLine))
                        {
                            var vertLine = (Line)child;
                            vertLine.X1 *= obj.ScaleDelta;
                            vertLine.X2 *= obj.ScaleDelta;
                        }

                        // Масштабируем координаты горизонтальных линий разметки.
                        if (child.Name == nameof(horizontalLine))
                        {
                            var vertLine = (Line)child;
                            vertLine.Y1 *= obj.ScaleDelta;
                            vertLine.Y2 *= obj.ScaleDelta;
                        }
                    }
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
