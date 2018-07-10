using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;

namespace WPF.CTG
{
    public class TransformManager: INotifyPropertyChanged
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

        #region Свойства

        /// <summary>
        /// Ссылка на родительский канвас-рамку
        /// </summary>
        private Canvas _coordinateViewPort;

        /// <summary>
        /// Ссылка на координатную плоскость
        /// </summary>
        private ScalableCoordinatePlane _scalableCoordinatePlane;

        /// <summary>
        /// Координата X опорной точки масштабирования координатной плоскости. ZoomCenterX
        /// </summary>
        public double ScaleCenterX
        {
            get { return _scaleCenterX; }
            set
            {
                _scaleCenterX = value;
                OnPropertyChanged(nameof(ScaleCenterX));
            }
        }
        private double _scaleCenterX;

        /// <summary>
        /// Координата Y опорной точки масштабирования координатной плоскости. ZoomCenterY
        /// </summary>
        public double ScaleCenterY
        {
            get { return _scaleCenterY; }
            set
            {
                _scaleCenterY = value;
                OnPropertyChanged(nameof(ScaleCenterY));
            }
        }
        private double _scaleCenterY;

        /// <summary>
        /// Корректировочное смещение дочернего холста по оси X.
        /// </summary>
        public double MoveX
        {
            get { return _moveX; }
            set
            {
                _moveX = value;
                OnPropertyChanged(nameof(MoveX));
            }
        }
        private double _moveX = 1.05;

        /// <summary>
        /// Корректировочное смещение дочернего холста по оси Y.
        /// </summary>
        public double MoveY
        {
            get { return _moveY; }
            set
            {
                _moveY = value;
                OnPropertyChanged(nameof(MoveY));
            }
        }
        private double _moveY = 1.05;

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

        /// <summary>
        /// Накапливаемый коэффициент масштаба по оси X.
        /// </summary>
        public double ScaleRateX
        {
            get { return _scaleRateX; }
            set
            {
                _scaleRateX = value;
                OnPropertyChanged(nameof(ScaleRateX));
            }
        }
        private double _scaleRateX = 1.0;

        /// <summary>
        /// Накапливаемый коэффициент масштаба по оси Y.
        /// </summary>
        public double ScaleRateY
        {
            get { return _scaleRateY; }
            set
            {
                _scaleRateY = value;
                OnPropertyChanged(nameof(ScaleRateY));
            }
        }
        private double _scaleRateY = 1.0;

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

        #endregion

        #region * Конструктор

        /// <summary>
        /// * Конструктор
        /// </summary>
        public TransformManager()
        {}

        #endregion

        #region Инициализация

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="scalableCoordinatePlane"></param>
        public void TransformInit(Canvas canvas, ScalableCoordinatePlane scalableCoordinatePlane)
        {
            _coordinateViewPort = canvas;
            _scalableCoordinatePlane = scalableCoordinatePlane;

            _scalableCoordinatePlane.MouseDown += mouseDown;
            _scalableCoordinatePlane.MouseUp += mouseUp;
            _scalableCoordinatePlane.MouseMove += mouseMove;
            _scalableCoordinatePlane.MouseWheel += mouseWheel;
        }

        #endregion

        #region Обработчики событий

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

        #region Методы

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
