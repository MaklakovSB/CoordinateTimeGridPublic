using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for RulerPanel.xaml
    /// </summary>
    public partial class RulerPanel : Canvas, INotifyPropertyChanged
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
        /// Цвет кисти разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridBrushProperty = DependencyProperty.Register(
            nameof(MarkingGridBrush),
            typeof(Brush),
            typeof(RulerPanel),
            new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Толщина линий разметочной сетки.
        /// </summary>
        public static readonly DependencyProperty MarkingGridStrokeThicknessProperty = DependencyProperty.Register(
            nameof(MarkingGridStrokeThickness),
            typeof(double),
            typeof(RulerPanel),
            new PropertyMetadata(0.4));

        /// <summary>
        /// Коэффициент масштаба по оси X.
        /// </summary>
        public static readonly DependencyProperty ScaleRateXProperty = DependencyProperty.Register(
            nameof(ScaleRateX),
            typeof(double),
            typeof(RulerPanel),
            new PropertyMetadata(1.0, OnScaleRateXPropertyChange));

        /// <summary>
        /// Коэффициент масштаба по оси Y.
        /// </summary>
        public static readonly DependencyProperty ScaleRateYProperty = DependencyProperty.Register(
            nameof(ScaleRateY),
            typeof(double),
            typeof(RulerPanel),
            new PropertyMetadata(1.0, OnScaleRateYPropertyChange));

        #endregion

        #region Акцессоры свойств зависимости

        /// <summary>
        /// Цвет кисти разметочной сетки
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
            get { return (double)GetValue(ScaleRateYProperty); }
            set { SetValue(ScaleRateYProperty, value); }
        }

        /////// <summary>
        /////// Верхняя видимая граница координатной плоскости.
        /////// </summary>
        ////public double TopVisibleEdge
        ////{
        ////    get { return (double)GetValue(TopVisibleEdgeProperty); }
        ////    set { SetValue(TopVisibleEdgeProperty, value); }
        ////}

        /////// <summary>
        /////// Нижняя видимая граница координатной плоскости.
        /////// </summary>
        ////public double BottomVisibleEdge
        ////{
        ////    get { return (double)GetValue(BottomVisibleEdgeProperty); }
        ////    set { SetValue(BottomVisibleEdgeProperty, value); }
        ////}

        /////// <summary>
        /////// Левая видимая граница координатной плоскости.
        /////// </summary>
        ////public double LeftVisibleEdge
        ////{
        ////    get { return (double)GetValue(LeftVisibleEdgeProperty); }
        ////    set { SetValue(LeftVisibleEdgeProperty, value); }
        ////}

        /////// <summary>
        /////// Правая видимая граница координатной плоскости.
        /////// </summary>
        ////public double RightVisibleEdge
        ////{
        ////    get { return (double)GetValue(RightVisibleEdgeProperty); }
        ////    set { SetValue(RightVisibleEdgeProperty, value); }
        ////}

        #endregion

        #region Приватные поля

        private double _originalHeight;
        private double _originalWidth;

        private double _scaleRateX = 1;
        private double _scaleRateY = 1;

        #endregion

        #region Свойства

        /// <summary>
        /// Ориентация линейки.
        /// </summary>
        public Orientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                OnPropertyChanged(nameof(Orientation));
            }
        }
        private Orientation _orientation;

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
        public RulerPanel() : base() 
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Инициализация сетки на заднем фоне координатной плоскости.
        /// </summary>
        private void MarkingGridInitialize(double originalWidth, double originalHeight)
        {
            var vertCount = originalWidth / 10;
            var horizCount = originalHeight / 10;

            for (var x = 0; x < vertCount; x++)
            {
                var vLine = new Line()
                {
                    Name = nameof(VerticalLine),
                    X1 = x * 10,
                    X2 = x * 10,
                };

                //Binding bindVisibleTop = new Binding();
                //bindVisibleTop.Source = this;
                //bindVisibleTop.Path = new PropertyPath(nameof(TopVisibleEdge));
                //bindVisibleTop.Mode = BindingMode.OneWay;
                //vLine.SetBinding(Line.Y1Property, bindVisibleTop);

                //Binding bindVisibleBottom = new Binding();
                //bindVisibleBottom.Source = this;
                //bindVisibleBottom.Path = new PropertyPath(nameof(BottomVisibleEdge));
                //bindVisibleBottom.Mode = BindingMode.OneWay;
                //vLine.SetBinding(Line.Y2Property, bindVisibleBottom);

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
                    Name = nameof(HorizontalLine),
                    Y1 = y * 10,
                    Y2 = y * 10,
                };

                //Binding bindVisibleLeft = new Binding();
                //bindVisibleLeft.Source = this;
                //bindVisibleLeft.Path = new PropertyPath(nameof(LeftVisibleEdge));
                //bindVisibleLeft.Mode = BindingMode.OneWay;
                //hLine.SetBinding(Line.X1Property, bindVisibleLeft);

                //Binding bindVisibleRight = new Binding();
                //bindVisibleRight.Source = this;
                //bindVisibleRight.Path = new PropertyPath(nameof(RightVisibleEdge));
                //bindVisibleRight.Mode = BindingMode.OneWay;
                //hLine.SetBinding(Line.X2Property, bindVisibleRight);

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

            // Масштабируем содержимое координатной плоскости.
            if (Children.Count > 0)
            {
                foreach (FrameworkElement child in Children)
                {
                    if (child.Name == nameof(VerticalLine))
                    {
                        // Масштабируем координаты вертикальных линий разметки.
                        var vertLine = (Line)child;
                        vertLine.X1 *= ScaleDeltaX;
                        vertLine.X2 *= ScaleDeltaX;
                    }
                    else
                    {
                        // Масштабируем размеры содержимого.
                        child.Width *= ScaleDeltaX;

                        // Масштабируем координаты содержимого.
                        child.SetCurrentValue(Canvas.LeftProperty, Canvas.GetLeft(child) * ScaleDeltaX);
                    }
                }
            }
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

            // Масштабируем содержимое координатной плоскости.
            if (Children.Count > 0)
            {
                foreach (FrameworkElement child in Children)
                {
                    if (child.Name == nameof(HorizontalLine))
                    {
                        // Масштабируем координаты горизонтальных линий разметки.
                        var horizontLine = (Line)child;
                        horizontLine.Y1 *= ScaleDeltaY;
                        horizontLine.Y2 *= ScaleDeltaY;
                    }
                    else
                    {
                        // Масштабируем размеры содержимого.
                        child.Height *= ScaleDeltaY;

                        // Масштабируем координаты содержимого.
                        child.SetCurrentValue(Canvas.TopProperty, Canvas.GetTop(child) * ScaleDeltaY);
                    }
                }
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
            //MarkingGridInitialize();
        }

        #endregion

        #region Обработчики событий изменения свойств зависимости

        /// <summary>
        /// Изменение коэффициента масштаба по оси X.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnScaleRateXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as RulerPanel;
            if (obj != null)
            {
                var newValue = (double)e.NewValue;
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
            var obj = d as RulerPanel;
            if (obj != null)
            {
                var newValue = (double)e.NewValue;
                obj.ScaleRateYChange(newValue);
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
