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
        private const double MinScaleFactor = 0.035;

        /// <summary>
        /// Максимальный предел множителя масштаба
        /// </summary>
        private const double MaxScaleFactor = 3.5;

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

        /// <summary>
        /// Ссылка на родительский канвас-рамку
        /// </summary>
        private Canvas _coordinateViewPort;

        /// <summary>
        /// Ссылка на координатную плоскость
        /// </summary>
        private ScalableCoordinatePlane _scalableCoordinatePlane;

        #endregion

        #region Свойства

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
        private double _moveX;

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
        private double _moveY;

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
                //if (value > MaxScaleFactor)
                //    value = MaxScaleFactor;

                //if (value < MinScaleFactor)
                //    value = MinScaleFactor;

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
                //if (value > MaxScaleFactor)
                //    value = MaxScaleFactor;

                //if (value < MinScaleFactor)
                //    value = MinScaleFactor;

                _scaleRateY = value;
                OnPropertyChanged(nameof(ScaleRateY));
                OnPropertyChanged(nameof(WidthWithScaling));
            }
        }
        private double _scaleRateY = 1.0;

        /// <summary>
        /// Накапливаемый коэффициент масштаба по оси Y и X.
        /// </summary>
        public double ScaleRate
        {
            get { return _scaleRate; }
            set
            {
                //if (value > MaxScaleFactor)
                //    value = MaxScaleFactor;

                //if (value < MinScaleFactor)
                //    value = MinScaleFactor;

                _scaleRate = value;
                OnPropertyChanged(nameof(ScaleRate));
                OnPropertyChanged(nameof(WidthWithScaling));
                OnPropertyChanged(nameof(HeightWithScaling));
            }
        }
        private double _scaleRate = 1.0;

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public bool IsBlockingScaleX { get; set; }

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public bool IsBlockingScaleY { get; set; }

        /// <summary>
        /// Ширина координатной плоскости.
        /// </summary>
        public double WidthWithScaling => _scalableCoordinatePlane.ActualWidth;

        /// <summary>
        /// Высота координатной плоскости.
        /// </summary>
        public double HeightWithScaling => _scalableCoordinatePlane.ActualHeight;

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

                // с применением масштаба к дельте число может стать экспонентциальным.
                //deltaX = deltaX * ScaleRateX;
                //deltaY = deltaY * ScaleRateY;


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

            // 1) Получим дельту.
            var delta = e.Delta;

            // 2) Получим предстоящий шаг масштабирования в зависимости от дельты.
            var futureScalingRateStep = GetScalingRateStep(delta);

            // 3) Нужно проверить не станет ли координатная плоскость менше чем клиентская область
            // ViewPort'а. Если при следующем шаге масштабирования она станет меньше, то нужно вычислить
            // корректировочный множитель так чтоб координатная плоскость точно вписалась в клиентскую
            // область ViewPort'a. (Актуально при уменьшении масштаба)

            // Получим ширины и высоты ViewPort'а и координатной плоскости с учётом предстоящего шага масштабирования.
            var widthPlane = _scalableCoordinatePlane.OriginalWidth * ScaleRate * futureScalingRateStep;
            var heightPlane = _scalableCoordinatePlane.OriginalHeight * ScaleRate * futureScalingRateStep;
            var widthViewPort = _coordinateViewPort.ActualWidth;
            var heightViewPort = _coordinateViewPort.ActualHeight;

            // Получим разницу размеров между внутренним и внешним контролом.
            var differenceWidth = widthPlane - widthViewPort;
            var differenceHeight = heightPlane - heightViewPort;

            // Если разница менше чем 0, то это наш случай - координатная плоскость при следующем шаге
            // масштабирования станет меньше чем ViewPort и нужно скорретировать предстоящий шаг масштаба.
            if (differenceWidth < 0 || differenceHeight < 0)
            {
                double correctedScalingRateStep;

                // Будем ориентироваться на ту ось, по которой размер координатной плоскости меньше.
                if (differenceWidth < differenceHeight)
                {
                    correctedScalingRateStep = widthViewPort / widthPlane;
                }
                else
                {
                    correctedScalingRateStep = heightViewPort / heightPlane;
                }

                // Корректируем шаг масштаба.
                futureScalingRateStep *= correctedScalingRateStep;
            } // конец (3)

            // 4) Вычислить смещения.

            // Получаем точку положения курсора мыши и принимаем её как опорную для масштабирования координатной плоскости.
            var cursorPos = Mouse.GetPosition(_scalableCoordinatePlane);

            // Вычислим будующее положение точки
            var futureCursorPosX = cursorPos.X * futureScalingRateStep;
            var futureCursorPosY = cursorPos.Y * futureScalingRateStep;

            // Вычислим разницу нового и старого положения.
            var diffOpsX = cursorPos.X - futureCursorPosX;
            var diffPosY = cursorPos.Y - futureCursorPosY;

            // Сохраним разницу как будующее смещение для возможных корректировок.
            var futureMoveX = diffOpsX;
            var futureMoveY = diffPosY;

            // Вычислим будующее положение координатной плоскости.
            var futureCanvasLeft = CanvasLeft + futureMoveX;
            var futureCanvasTop = CanvasTop + futureMoveY;

            // Вычислим будующие размеры координатной плоскости.
            var futurePlaneWidth = _scalableCoordinatePlane.OriginalWidth * ScaleRate * futureScalingRateStep;
            var futurePlaneHeight = _scalableCoordinatePlane.OriginalHeight * ScaleRate * futureScalingRateStep;

            // Вычислим будующую отображаемую часть координатной плоскости.
            var visiblePartWidth = futurePlaneWidth + futureCanvasLeft;
            var visiblePartHeight = futurePlaneHeight + futureCanvasTop;

            // Проверим не получится ли брешь между координатной плоскостью и ViewPort'ом по ширине.
            if (futureCanvasLeft > 0 || visiblePartWidth < _coordinateViewPort.ActualWidth)
            {
                // Не может быть брешь с обеих сторон одновременно.
                if (futureCanvasLeft > 0 && visiblePartWidth < _coordinateViewPort.ActualWidth)
                {
                    // Может
                    //throw new Exception();
                }

                if (futureCanvasLeft > 0)
                {
                    futureMoveX +=- futureCanvasLeft;
                }

                if (visiblePartWidth < _coordinateViewPort.ActualWidth)
                {
                    futureMoveX += _coordinateViewPort.ActualWidth - visiblePartWidth;
                }

            }

            // Проверим не получится ли брешь между координатной плоскостью и ViewPort'ом по высоте.
            if (futureCanvasTop > 0 || visiblePartHeight < _coordinateViewPort.ActualHeight)
            {
                // Не может быть брешь с обеих сторон одновременно.
                if (futureCanvasTop > 0 && visiblePartHeight < _coordinateViewPort.ActualHeight)
                {
                    //throw new Exception();
                }

                if (futureCanvasTop > 0)
                {
                    futureMoveY +=- futureCanvasTop;
                }

                if (visiblePartHeight < _coordinateViewPort.ActualHeight)
                {
                    futureMoveY += _coordinateViewPort.ActualHeight - visiblePartHeight;
                }
            }// Конец (4)

            MoveX = futureMoveX;
            MoveY = futureMoveY;

            // Расчёт и применение масштаба.
            ScalePlane(futureScalingRateStep);

            // Компенсируем сдвиг канваса после масштабирования.
            CanvasLeft += MoveX;
            CanvasTop += MoveY;
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

            // 1) Вначале нужно знать вся ли координатная плоскость с текущим масштабом полностью 
            // отображена в клиентской области ViewPort'a
            // или же часть смещена за "границу ViewPort'a".

            var scalingRateStep = 1.0;

            // Получим ширины и высоты ViewPort'а и координатной плоскости.
            var widthPlane = _scalableCoordinatePlane.ActualWidth;
            var heightPlane = _scalableCoordinatePlane.ActualHeight;
            var widthViewPort = e.NewSize.Width;
            var heightViewPort = e.NewSize.Height;

            // Если есть не отображённая часть слева либо брешь слева.''
            if (CanvasLeft != 0 || CanvasTop != 0)
            {
                if (CanvasLeft < 0)
                {
                    var visiblePartWidth = widthPlane - Math.Abs(CanvasLeft);

                    if (visiblePartWidth < widthViewPort)
                    {
                        var diffWidth = widthViewPort - visiblePartWidth;

                        if (diffWidth < 0)
                        {
                            throw new Exception();
                        }

                        CanvasLeft += diffWidth;
                    }
                }
                else if (CanvasLeft > 0)
                {
                    CanvasLeft = 0;
                }

                if (CanvasTop < 0)
                {
                    var visiblePartHeight = heightPlane - Math.Abs(CanvasTop);

                    if (visiblePartHeight < heightViewPort)
                    {
                        var diffHeight = heightViewPort - visiblePartHeight;

                        if (diffHeight < 0)
                        {
                            throw new Exception();
                        }

                        if (CanvasTop < 0)
                        {
                            CanvasTop += diffHeight;
                        }
                        else if (CanvasTop > 0)
                        {
                            CanvasTop -= diffHeight;
                        }
                    }
                }
                else if (CanvasTop > 0)
                {
                    CanvasTop = 0;
                }
            }

            // Если растянули ViewPort, то координатная плоскость могла
            // стать меньше чем ViewPort. Поэтому нужно сравнить размеры ViewPort'а с размерами
            // координатной плоскости. И если координатная плоскость - меньше, то
            // высчитать недостающий масштаб и применить его, а потом сделать компенсационный сдвиг. Если же
            // координатная плоскость не меньше ViewPort'а, то просто сделать компенсационный сдвиг - достаточно.

            // Получим разницу размеров между внутренним и внешним контролом.
            var differenceWidth = widthPlane - widthViewPort;
            var differenceHeight = heightPlane - heightViewPort;

            // Если разница менше чем 0, то это наш случай - координатная плоскость меньше чем ViewPort 
            // и нужно масштабировать её.
            if (differenceWidth < 0 || differenceHeight < 0)
            {
                double correctedScalingRateStep;

                // Будем ориентироваться на ту ось, по которой размер координатной плоскости меньше.
                if (differenceWidth < differenceHeight)
                {
                    correctedScalingRateStep = widthViewPort / widthPlane;
                }
                else
                {
                    correctedScalingRateStep = heightViewPort / heightPlane;
                }

                // Корректируем шаг масштаба.
                scalingRateStep *= correctedScalingRateStep;

                // Применить масштаб.
                ScalePlane(scalingRateStep);
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
                return 0.95;

            return _scalingRateStep = 1.0;
        }

        /// <summary>
        /// Метод масштабирования.
        /// </summary>
        private void ScalePlane(double scalingRateStep)
        {
            // Если дельта равна нулю то множитель масштаба равен единице.
            if (scalingRateStep != 1)
            {
                // Рсчёт масштаба.
                _scalingRateStep = scalingRateStep;
                ScaleRate *= _scalingRateStep;
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
