using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;

namespace WPF.CTG
{
    public class TransformManager : DependencyObject, INotifyPropertyChanged
    {
        #region Константы

        /// <summary>
        /// Минимальный предел множителя масштаба
        /// </summary>
        private const double MinScaleFactor = 0.59;

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

        public static DependencyProperty RealWidthProperty =
            DependencyProperty.Register("RealWidth", typeof(double),
                typeof(TransformManager),
                new PropertyMetadata(0.0));

        public double RealWidth
        {
            get { return (double)GetValue(RealWidthProperty); }
            set { SetValue(RealWidthProperty, value); }
        }

        #region Свойства

        /// <summary>
        /// Ссылка на родительский канвас-рамку
        /// </summary>
        private Canvas _coordinateViewPort;

        //private CanvasViewPort _coordinateViewPort;

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
                if (value > MaxScaleFactor)
                    value = MaxScaleFactor;

                if (value < MinScaleFactor)
                    value = MinScaleFactor;

                _scaleRateX = value;
                OnPropertyChanged(nameof(ScaleRateX));
                OnPropertyChanged(nameof(HeightWithScaling));
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
                if (value > MaxScaleFactor)
                    value = MaxScaleFactor;

                if (value < MinScaleFactor)
                    value = MinScaleFactor;

                _scaleRateY = value;
                OnPropertyChanged(nameof(ScaleRateY));
                OnPropertyChanged(nameof(WidthWithScaling));
            }
        }
        private double _scaleRateY = 1.0;

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public bool IsBlockingScaleX { get; set; }

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public bool IsBlockingScaleY { get; set; }

        /// <summary>
        /// Ширина координатной плоскости с учётом масштаба.
        /// </summary>
        public double WidthWithScaling => _scalableCoordinatePlane.Width * ScaleRateX;

        /// <summary>
        /// Высота координатной плоскости с учётом масштаба.
        /// </summary>
        public double HeightWithScaling => _scalableCoordinatePlane.Height * ScaleRateY;

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
            _coordinateViewPort.SizeChanged += sizeChanged;

            _scalableCoordinatePlane = scalableCoordinatePlane;

            _scalableCoordinatePlane.MouseDown += mouseDown;
            _scalableCoordinatePlane.MouseUp += mouseUp;
            _scalableCoordinatePlane.MouseMove += mouseMove;
            _scalableCoordinatePlane.MouseWheel += mouseWheel;

            OnPropertyChanged(nameof(WidthWithScaling));
            OnPropertyChanged(nameof(HeightWithScaling));

            //OnPropertyChanged(nameof(_coordinateViewPort.ActualWidth));
            //OnPropertyChanged(nameof(MaximumWidthScroll));
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
                Point currentPosition = Mouse.GetPosition(element);

                // Находим дельты сдвига мыши отностительно точки захвата.
                var deltaX = currentPosition.X - _dragStart.Value.X;
                var deltaY = currentPosition.Y - _dragStart.Value.Y;

                deltaX = deltaX / ScaleRateX;
                deltaY = deltaY / ScaleRateY;


                // Получаем верхние левые крайние точки внутреннего и внешнего холста
                // для контроля верхней и левой границы.
                var ptsPMin = _coordinateViewPort.PointToScreen(new Point() { X = 0, Y = 0 });
                var ptsCMin = _scalableCoordinatePlane.PointToScreen(new Point() { X = 0, Y = 0 });

                // Получаем нижние правые крайние точки внутреннего и внешнего холста
                // для контроля нижней и правой границы.
                var ptsPMax = _coordinateViewPort.PointToScreen(new Point() { X = _coordinateViewPort.ActualWidth, Y = _coordinateViewPort.ActualHeight });
                var ptsCMax = _scalableCoordinatePlane.PointToScreen(new Point() { X = _scalableCoordinatePlane.ActualWidth, Y = _scalableCoordinatePlane.ActualHeight });

                // Получим положительные допустимые смещения.
                var maxOffsetToRight = (ptsPMin.X + 1 - ptsCMin.X);
                var maxOffsetToDown = (ptsPMin.Y + 1 - ptsCMin.Y);
                var maxOffsetToLeft = (ptsCMax.X - ptsPMax.X + 1);
                var maxOffsetToUp = (ptsCMax.Y - ptsPMax.Y + 1);

                // Смещение вправо
                if (deltaX > 0 && maxOffsetToRight > 0)
                {
                    if (deltaX > maxOffsetToRight)
                    {
                        deltaX = maxOffsetToRight;
                    }

                    CanvasLeft = CanvasLeft + deltaX;
                }
                else
                // Смещение влево
                if (deltaX < 0 && maxOffsetToLeft > 0)
                {
                    if (Math.Abs(deltaX) > maxOffsetToLeft)
                    {
                        deltaX = maxOffsetToLeft * -1;
                    }

                    CanvasLeft = CanvasLeft + deltaX;
                }

                // Смещение вниз
                if (deltaY > 0 && maxOffsetToDown > 0)
                {
                    if (deltaY > maxOffsetToDown)
                    {
                        deltaY = maxOffsetToDown;
                    }

                    CanvasTop = CanvasTop + deltaY;
                }
                else
                // Смещение вверх
                if (deltaY < 0 && maxOffsetToUp > 0)
                {
                    if (Math.Abs(deltaY) > maxOffsetToUp)
                    {
                        deltaY = maxOffsetToUp * -1;
                    }

                    CanvasTop = CanvasTop + deltaY;
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
            OnPropertyChanged(nameof(WidthWithScaling));

            // Получим дельту.
            var delta = e.Delta;

            // Ограничение масштаба.
            // Если уже применённый коэффициент масштаба больше или равен максимальному при положительной дельте либо
            // если уже применённый коэффициент масштаба меньше или равен минимальному при отрицательной дельте, то отлуп.
            if (((ScaleRateX >= MaxScaleFactor && delta > 0)
                || (ScaleRateX <= MinScaleFactor && delta < 0)) 
                || ((ScaleRateY >= MaxScaleFactor && delta > 0)
                || (ScaleRateY <= MinScaleFactor && delta < 0)))
                return;

            // Получим предстоящий шаг масштабирования в зависимости от дельты.
            var scalingRateStep = GetScalingRateStep(delta);

            // Получим показатель уменьшения масштаба.
            var decreaseScale = scalingRateStep < 1;

            // Если предстоит уменьшение масштаба, то нужно проверить не станет ли координатная плоскость менше
            // ViewPort'а либо по ширине либо по высоте.
            if (decreaseScale)
            {
                // Получим ширины и высоты ViewPort'а и координатной плоскости с учётом текущего 
                // масштаба и предстоящего шага масштабирования.
                var widthPlane = _scalableCoordinatePlane.ActualWidth * ScaleRateX * scalingRateStep;
                var heightPlane = _scalableCoordinatePlane.ActualHeight * ScaleRateY * scalingRateStep;
                var widthViewPort = _coordinateViewPort.ActualWidth;
                var heightViewPort = _coordinateViewPort.ActualHeight;

                // Получим разницу размеров между внутренним и внешним контролом.
                var differenceWidth = widthPlane - widthViewPort;
                var differenceHeight = heightPlane - heightViewPort;

                // Если разница менше чем -2, то это наш случай - координатная плоскость при следующем шаге
                // масштабирования станет меньше чем ViewPort и нужно скорретировать предстоящий шаг масштаба.
                if (differenceWidth < -2 || differenceHeight < -2)
                {
                    var correctedScalingRateStep = 0.0;

                    // Будем ориентироваться на ту ось, по которой размер координатной плоскости меньше.
                    // Если widthViewPort это ширина вьюпорта, то "widthViewPort - 2" - ширина его внутренней области.
                    if (differenceWidth < differenceHeight)
                    {
                        correctedScalingRateStep = (widthViewPort - 2) / widthPlane;
                    }
                    else
                    {
                        correctedScalingRateStep = (heightViewPort - 2) / heightPlane;
                    }

                    // Корректируем шаг масштаба.
                    scalingRateStep *= correctedScalingRateStep;
                }
            }

            // Расчёт и применение масштаба.
            ScalePlane(scalingRateStep);

            // Компенсационное смещение координатной плоскости в случае если образовалась брешь.
            CompensationMove();
        }

        /// <summary>
        /// Обработчик события изменения размера ViewPort
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            var elementCanvas = sender as Canvas;
            if (elementCanvas == null)
                return;

            RealWidth = e.NewSize.Width;

            // Если растянули ViewPort, то координатная плоскость могла
            // стать меньше чем ViewPort.Поэтому нужно сравнить размеры ViewPort'а с размерами
            // координатной плоскости с учётом текущего масштаба. И если координатная плоскость - меньше, то
            // высчитать недостающий масштаб и применить его, а потом сделать компенсационный сдвиг. Если же
            // координатная плоскость не меньше ViewPort'а, то просто сделать компенсационный сдвиг - достаточно.

            var scalingRateStep = 1.0;

            // Получим ширины и высоты ViewPort'а и координатной плоскости с учётом текущего 
            // масштаба и предстоящего шага масштабирования.
            var widthPlane = _scalableCoordinatePlane.ActualWidth * ScaleRateX;
            var heightPlane = _scalableCoordinatePlane.ActualHeight * ScaleRateY;
            var widthViewPort = _coordinateViewPort.ActualWidth;
            var heightViewPort = _coordinateViewPort.ActualHeight;

            // Получим разницу размеров между внутренним и внешним контролом.
            var differenceWidth = widthPlane - widthViewPort;
            var differenceHeight = heightPlane - heightViewPort;

            // Если разница менше чем -2, то это наш случай - координатная плоскость меньше чем ViewPort 
            // и нужно масштабировать её.
            if (differenceWidth < -2 || differenceHeight < -2)
            {
                var correctedScalingRateStep = 0.0;

                // Будем ориентироваться на ту ось, по которой размер координатной плоскости меньше.
                // Если widthViewPort это ширина вьюпорта, то "widthViewPort - 2" - ширина его внутренней области.
                if (differenceWidth < differenceHeight)
                {
                    correctedScalingRateStep = (widthViewPort - 2) / widthPlane;
                }
                else
                {
                    correctedScalingRateStep = (heightViewPort - 2) / heightPlane;
                }

                // Корректируем шаг масштаба.
                scalingRateStep *= correctedScalingRateStep;

                // Применить масштаб.
                ScalePlane(scalingRateStep);
            }

            // Компенсационное смещение координатной плоскости в случае если образовалась брешь.
            CompensationMove();
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
                return 1.04;

            if (delta < 0)
                return 0.96;

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

        /// <summary>
        /// Метод масштабирования.
        /// </summary>
        private void ScalePlane(double scalingRateStep, double? scaleCenterX = null, double? scaleCenterY = null)
        {
            // Если дельта равна нулю то множитель масштаба равен единице.
            if (scalingRateStep != 1)
            {
                // Получаем точку положения курсора мыши и устанавливаем её как опорную для масштабирования канваса.
                var position = Mouse.GetPosition(_scalableCoordinatePlane);
                ScaleCenterX = scaleCenterX ?? position.X;
                ScaleCenterY = scaleCenterY ?? position.Y;

                // Рсчёт масштаба.
                _scalingRateStep = scalingRateStep;

                if(!IsBlockingScaleX)
                    ScaleRateX *= _scalingRateStep;

                if(!IsBlockingScaleY)
                    ScaleRateY *= _scalingRateStep;

                // Дело в том что положение мыши меняется во время преобразования относительно канваса.
                // И следующий шаг прокрутки колеса по инерции происходит с опорной точкой которая оказывается за 
                // пределами канваса, в результате канвас улетает из под мыши далеко за пределы видимости, 
                // но Mouse.GetPosition позволяет получить точную точку после очередного преобразования холста.
                var cursorpos = Mouse.GetPosition(_scalableCoordinatePlane);
                var discrepancyX = (scaleCenterX ?? cursorpos.X) - position.X;
                var discrepancyY = (scaleCenterY ?? cursorpos.Y) - position.Y;

                // Компенсируем сдвиг канваса после масштабирования.
                MoveX += discrepancyX;
                MoveY += discrepancyY;
            }
        }

        /// <summary>
        /// Компенсационное смещение координатной плоскости в случае если образовалась брешь.
        /// </summary>
        private void CompensationMove()
        {
            // Получаем контрольные точки.
            var extremePoints = GetCanvasControlPoints();

            // Определим условия нарушение границ - образования бреши.
            var left = extremePoints.ExternalCanvasMinimum.X < extremePoints.InternalCanvasMinimum.X;
            var top = extremePoints.ExternalCanvasMinimum.Y < extremePoints.InternalCanvasMinimum.Y;
            var right = extremePoints.ExternalCanvasMaximum.X > extremePoints.InternalCanvasMaximum.X;
            var bottom = extremePoints.ExternalCanvasMaximum.Y > extremePoints.InternalCanvasMaximum.Y;

            if (left)
            {
                MoveX += (extremePoints.InternalCanvasMinimum.X - (extremePoints.ExternalCanvasMinimum.X + 1)) /
                         ScaleRateX * -1;
            }
            else if (right)
            {
                MoveX += (extremePoints.InternalCanvasMaximum.X - (extremePoints.ExternalCanvasMaximum.X -1)) /
                         ScaleRateX * -1;
            }

            if (top)
            {
                MoveY += (extremePoints.InternalCanvasMinimum.Y - (extremePoints.ExternalCanvasMinimum.Y + 1)) /
                         ScaleRateY * -1;
            }
            else if (bottom)
            {
                MoveY += (extremePoints.InternalCanvasMaximum.Y - (extremePoints.ExternalCanvasMaximum.Y -1)) /
                         ScaleRateY * -1;
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
