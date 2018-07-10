using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System;
using System.Windows.Input;

namespace WPF.CTG
{
    /// <summary>
    /// Масштабируемая координатная плоскость.
    /// </summary>
    public partial class ScalableCoordinatePlane : Canvas, INotifyPropertyChanged
    {
        #region Константы

        /// <summary>
        /// Минимальный предел множителя масштаба
        /// </summary>
        private const double MinScaleFactor = 0.18;

        /// <summary>
        /// Максимальный предел множителя масштаба
        /// </summary>
        private const double MaxScaleFactor = 3.2;

        #endregion

        #region Приватные поля

        /// <summary>
        /// Шаг масштабирования.
        /// </summary>
        private double _scalingRateStep = 1.05;

        /// <summary>
        /// Точка захвата холста мышкой для перемещения.
        /// </summary>
        private Point? _dragStart;

        #endregion

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

        #region Свойства

        /// <summary>
        /// Цвет кисти разметочной сетки
        /// </summary>
        public Color GridColor
        {
            //get { return _coordinateGridColor.Color; }
            //set
            //{
            //    _coordinateGridColor.Color = value;
            //    OnPropertyChanged(nameof(GridColor));
            //}

            get { return (Color)base.GetValue(GridColorProperty); }
            set
            {
                base.SetValue(GridColorProperty, value);
                OnPropertyChanged(nameof(GridColor));
            }
        }

        /// <summary>
        /// Смещение по оси X
        /// </summary>
        public double MoveX
        {
            get { return _translateTransform.X; }
            set
            {
                _translateTransform.X = value;
                OnPropertyChanged(nameof(MoveX));
            }
        }

        /// <summary>
        /// Смещение по оси Y
        /// </summary>
        public double MoveY
        {
            get { return _translateTransform.Y; }
            set
            {
                _translateTransform.Y = value;
                OnPropertyChanged(nameof(MoveY));
            }
        }

        /// <summary>
        /// Кордината X опорной точки масштабирования.
        /// </summary>
        public double ScaleCenterX
        {
            get { return _scaleTransform.CenterX; }
            set
            {
                _scaleTransform.CenterX = value;
                OnPropertyChanged(nameof(ScaleCenterX));
            }
        }

        /// <summary>
        /// Кордината Y опорной точки масштабирования.
        /// </summary>
        public double ScaleCenterY
        {
            get { return _scaleTransform.CenterY; }
            set
            {
                _scaleTransform.CenterY = value;
                OnPropertyChanged(nameof(ScaleCenterY));
            }
        }

        /// <summary>
        /// Коэффициент масштаба по оси X.
        /// </summary>
        public double ScaleRateX
        {
            get { return _scaleTransform.ScaleX; }
            set
            {
                _scaleTransform.ScaleX = value;
                OnPropertyChanged(nameof(ScaleRateX));
            }
        }

        /// <summary>
        /// Коэффициент масштаба по оси Y.
        /// </summary>
        public double ScaleRateY
        {
            get { return _scaleTransform.ScaleY; }
            set
            {
                _scaleTransform.ScaleY = value;
                OnPropertyChanged(nameof(ScaleRateY));
            }
        }


        /// <summary>
        /// Общий накапливаемый коэффициент масштаба.
        /// </summary>
        public double AccumulatedScaleFactor
        {
            get { return _accumulatedScaleFactor; }
            set
            {
                _accumulatedScaleFactor = value;
                OnPropertyChanged(nameof(AccumulatedScaleFactor));
            }
        }
        private double _accumulatedScaleFactor = 1;

        /// <summary>
        /// Позиция дочернего холста относительно левой границы внешнего холста.
        /// </summary>
        public double CanvasLeft
        {
            get { return _canvasLeft; }
            set
            {
                _canvasLeft = value;
                OnPropertyChanged(nameof(CanvasLeft));
            }
        }
        private double _canvasLeft = 0;

        /// <summary>
        /// Позиция дочернего холста относительно верхней границы внешнего холста.
        /// </summary>
        public double CanvasTop
        {
            get { return _canvasTop; }
            set
            {
                _canvasTop = value;
                OnPropertyChanged(nameof(CanvasTop));
            }
        }
        private double _canvasTop = 0;

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

        /// <summary>
        /// Возвращает значение шага масштабирования в зависимости от 
        /// дельты колёсика мыши. Возвращаемые значения будут следующими:
        /// дельта положительна - множитель больше 1
        /// дельта равна нулю - множитель равен 1
        /// дельта отрицательна - множитель меньше 1
        /// </summary>
        /// <param name="delta">Дельта колеса мыши</param>
        /// <returns></returns>
        private double GetScalingRateStep(int delta)
        {
            if (delta > 0)
                return 1.05;

            if (delta < 0)
                return 1.0 / 1.05;

            return _scalingRateStep = 1.0;
        }

        /// <summary>
        /// Получить контрольные точки холстов.
        /// </summary>
        /// <returns></returns>
        private ExtremePoints GetCanvasControlPoints()
        {
            return new ExtremePoints()
            {
                ExternalCanvasMinimum = _coordinateViewPort.PointToScreen(new Point() { X = 0, Y = 0 }),
                ExternalCanvasMaximum = _coordinateViewPort.PointToScreen(new Point() { X = _coordinateViewPort.ActualWidth, Y = _coordinateViewPort.ActualHeight }),
                InternalCanvasMinimum = _scalableCoordinatePlane.PointToScreen(new Point() { X = 0, Y = 0 }),
                InternalCanvasMaximum = _scalableCoordinatePlane.PointToScreen(new Point() { X = _scalableCoordinatePlane.ActualWidth, Y = _scalableCoordinatePlane.ActualHeight })
            };
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

        #region Обработчики событий

        /// <summary>
        /// Обработчик загрузки контрола.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loaded(object sender, RoutedEventArgs e)
        {
            _coordinateViewPort = (Canvas)this.Parent;
            _scalableCoordinatePlane = this;
        }

        /// <summary>
        /// Обработчик нажатия клавиши мыши. 
        /// Фиксация стартовой точки - начала перемещения холста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;

            if (e.RightButton == MouseButtonState.Pressed)
            {
                _dragStart = e.GetPosition(element);
                element.CaptureMouse();
            }
        }

        /// <summary>
        /// Обработчик отпускания клавиши мыши.
        /// Обнуление стартовой точки - начала перемещения холста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            var element = (UIElement)sender;
            if (e.RightButton == MouseButtonState.Released)
            {
                _dragStart = null;
                element.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Обработчик перемещения мыши.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseMove(object sender, MouseEventArgs e)
        {
            if (_dragStart != null && e.RightButton == MouseButtonState.Pressed)
            {
                var element = (UIElement)sender;
                if (element == null || _coordinateViewPort == null)
                    return;

                // Получим текущее положение мыши
                Point cursorpos = Mouse.GetPosition(element);

                // Находим дельты сдвига мыши отностительно точки захвата
                double deltaX = cursorpos.X - _dragStart.Value.X;
                double deltaY = cursorpos.Y - _dragStart.Value.Y;

                // Получаем координаты нового положения холста
                var canvasLeft = CanvasLeft + deltaX * AccumulatedScaleFactor;
                var canvasTop = CanvasTop + deltaY * AccumulatedScaleFactor;

                // Получаем верхние левые крайние точки внутреннего и внешнего холста
                // для контроля верхней и левой границы.
                var ptsPMin = _coordinateViewPort.PointToScreen(new Point() { X = 0, Y = 0 });
                var ptsCMin = _scalableCoordinatePlane.PointToScreen(new Point() { X = 0, Y = 0 });

                // Получаем нижние правые крайние точки внутреннего и внешнего холста
                // для контроля нижней и правой границы.
                var ptsPMax = _coordinateViewPort.PointToScreen(new Point() { X = _coordinateViewPort.ActualWidth, Y = _coordinateViewPort.ActualHeight });
                var ptsCMax = _scalableCoordinatePlane.PointToScreen(new Point() { X = _scalableCoordinatePlane.ActualWidth, Y = _scalableCoordinatePlane.ActualHeight });

                if (deltaX > 0) // Смещение вправо
                {
                    var scrDeltaX = ptsPMin.X - ptsCMin.X;
                    if (scrDeltaX > 0)
                    {
                        if (scrDeltaX >= Math.Abs(deltaX))
                        {
                            CanvasLeft = canvasLeft;
                        }
                        else // Если дельта по модулю больше допустимой
                        {
                            deltaX = scrDeltaX;
                            canvasLeft = CanvasLeft + deltaX * AccumulatedScaleFactor;
                            CanvasLeft = canvasLeft;
                        }
                    }
                }
                else if (deltaX < 0) // Смещение влево
                {
                    var scrDeltaX = ptsCMax.X - ptsPMax.X;
                    if (scrDeltaX > 0)
                    {
                        if (scrDeltaX >= Math.Abs(deltaX))
                        {
                            CanvasLeft = canvasLeft;
                        }
                        else // Если дельта по модулю больше допустимой
                        {
                            deltaX = scrDeltaX * -1;
                            canvasLeft = CanvasLeft + deltaX * AccumulatedScaleFactor;
                            CanvasLeft = canvasLeft;
                        }
                    }
                }

                if (deltaY > 0) // Смещение вниз
                {
                    var scrDeltaY = ptsPMin.Y - ptsCMin.Y;
                    if (scrDeltaY > 0)
                    {
                        if (scrDeltaY >= Math.Abs(deltaY))
                        {
                            CanvasTop = canvasTop;
                        }
                        else // Если дельта по модулю больше допустимой
                        {
                            deltaY = scrDeltaY;
                            canvasTop = CanvasTop + deltaY * AccumulatedScaleFactor;
                            CanvasTop = canvasTop;
                        }
                    }
                }
                else if (deltaY < 0) // Смещение вверх
                {
                    var scrDeltaY = ptsCMax.Y - ptsPMax.Y;
                    if (scrDeltaY > 0)
                    {
                        if (scrDeltaY >= Math.Abs(deltaY))
                        {
                            CanvasTop = canvasTop;
                        }
                        else // Если дельта по модулю больше допустимой
                        {
                            deltaY = scrDeltaY * -1;
                            canvasTop = CanvasTop + deltaY * AccumulatedScaleFactor;
                            CanvasTop = canvasTop;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик события вращение колеса мыши.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mouseWheel(object sender, MouseWheelEventArgs e)
        {
            var elementCanvas = sender as Canvas;
            if (elementCanvas == null)
                return;

            var delta = e.Delta;

            // Ограничение масштаба
            if ((AccumulatedScaleFactor > MaxScaleFactor && delta > 0)
                || (AccumulatedScaleFactor < MinScaleFactor && delta < 0))
                return;

            // Получим коэффициент масштабирования в зависимости от дельты
            var scalingRateStep = GetScalingRateStep(delta);

            // Получаем контрольные точки.
            var extremePoints = GetCanvasControlPoints();

            // Условия нарушение границ
            var left = extremePoints.ExternalCanvasMinimum.X <= extremePoints.InternalCanvasMinimum.X;   // ptsPMin.X <= ptsCMin.X;
            var top = extremePoints.ExternalCanvasMinimum.Y <= extremePoints.InternalCanvasMinimum.Y;    //ptsPMin.Y <= ptsCMin.Y;
            var right = extremePoints.ExternalCanvasMaximum.X >= extremePoints.InternalCanvasMaximum.X;  //ptsPMax.X >= ptsCMax.X;
            var bottom = extremePoints.ExternalCanvasMaximum.Y >= extremePoints.InternalCanvasMaximum.Y; //ptsPMax.Y >= ptsCMax.Y;
            var decreaseScale = scalingRateStep < 1;

            // Отсечём варинты нарушения противоположных границ
            // при попытке уменьшения масштаба. Это признак предела
            // адеватного масштабирования.
            if (left && right && decreaseScale)
                return;

            if (top && bottom && decreaseScale)
                return;

            // Если дельта равна нулю то множитель масштаба равен единице
            if (scalingRateStep != 1)
            {
                // Получаем точку положения курсора мыши и устанавливаем её как опорную для масштабирования канваса
                var position = e.GetPosition(elementCanvas);
                ScaleCenterX = position.X;
                ScaleCenterY = position.Y;

                // Рсчёт масштаба
                _scalingRateStep = scalingRateStep;
                ScaleRateX *= _scalingRateStep;
                ScaleRateY *= _scalingRateStep;
                AccumulatedScaleFactor *= _scalingRateStep;

                // Дело в том что положение мыши меняется во время преобразования относительно канваса.
                // И следующий шаг прокрутки колеса по инерции происходит с опорной точкой которая оказывается за 
                // пределами канваса, в результате канвас улетает из под мыши далеко за пределы видимости, 
                // но Mouse.GetPosition позволяет получить точную точку после очередного преобразования холста.
                Point cursorpos = Mouse.GetPosition(elementCanvas);

                var discrepancyX = cursorpos.X - position.X;
                var discrepancyY = cursorpos.Y - position.Y;

                // Компенсируем сдвиг канваса после масштабирования
                MoveX += discrepancyX;
                MoveY += discrepancyY;

                // Получаем контрольные точки.
                extremePoints = GetCanvasControlPoints();

                // Определим условия нарушение границ.
                left = extremePoints.ExternalCanvasMinimum.X < extremePoints.InternalCanvasMinimum.X;
                top = extremePoints.ExternalCanvasMinimum.Y < extremePoints.InternalCanvasMinimum.Y;
                right = extremePoints.ExternalCanvasMaximum.X > extremePoints.InternalCanvasMaximum.X;
                bottom = extremePoints.ExternalCanvasMaximum.Y > extremePoints.InternalCanvasMaximum.Y;

                if (left)
                {
                    MoveX += (extremePoints.InternalCanvasMinimum.X - extremePoints.ExternalCanvasMinimum.X) / AccumulatedScaleFactor * -1;
                }
                else if (right)
                {
                    MoveX += (extremePoints.InternalCanvasMaximum.X - extremePoints.ExternalCanvasMaximum.X) / AccumulatedScaleFactor * -1;
                }

                if (top)
                {
                    MoveY += (extremePoints.InternalCanvasMinimum.Y - extremePoints.ExternalCanvasMinimum.Y) / AccumulatedScaleFactor * -1;
                }
                else if (bottom)
                {
                    MoveY += (extremePoints.InternalCanvasMaximum.Y - extremePoints.ExternalCanvasMaximum.Y) / AccumulatedScaleFactor * -1;
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
