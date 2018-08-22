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
        /// Ссылка на общий контролл.
        /// </summary>
        private CoordinateTimeGrid _coordinateTimeGrid;

        /// <summary>
        /// Ссылка на ViewPort
        /// </summary>
        private Canvas _coordinateViewPort;

        /// <summary>
        /// Ссылка на координатную плоскость
        /// </summary>
        private ScalableCoordinatePlane _scalableCoordinatePlane;

        #endregion

        #region Свойства

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
        private double _canvasLeft;

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
        private double _canvasTop;

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
                OnPropertyChanged(nameof(WidthWithScaling));
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
                OnPropertyChanged(nameof(HeightWithScaling));
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
        /// Верхняя видимая граница координатной плоскости.
        /// </summary>
        public double TopVisibleEdge
        {
            get { return _topVisibleEdge; }
            set
            {
                _topVisibleEdge = value;
                OnPropertyChanged(nameof(TopVisibleEdge));
            }
        }
        private double _topVisibleEdge;

        /// <summary>
        /// Нижняя видимая граница координатной плоскости.
        /// </summary>
        public double BottomVisibleEdge
        {
            get { return _bottomVisibleEdge; }
            set
            {
                _bottomVisibleEdge = value;
                OnPropertyChanged(nameof(BottomVisibleEdge));
            }
        }
        private double _bottomVisibleEdge;

        /// <summary>
        /// Левая видимая граница координатной плоскости.
        /// </summary>
        public double LeftVisibleEdge
        {
            get { return _leftVisibleEdge; }
            set
            {
                _leftVisibleEdge = value;
                OnPropertyChanged(nameof(LeftVisibleEdge));
            }
        }
        private double _leftVisibleEdge;

        /// <summary>
        /// Правая видимая граница координатной плоскости.
        /// </summary>
        public double RightVisibleEdge
        {
            get { return _rightVisibleEdge; }
            set
            {
                _rightVisibleEdge = value;
                OnPropertyChanged(nameof(RightVisibleEdge));
            }
        }
        private double _rightVisibleEdge;

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
        public void TransformInit(CoordinateTimeGrid coordinateTimeGrid, Canvas canvas, ScalableCoordinatePlane scalableCoordinatePlane)
        {
            _coordinateTimeGrid = coordinateTimeGrid;

            _coordinateViewPort = canvas;
            _coordinateViewPort.SizeChanged += sizeChanged;

            _scalableCoordinatePlane = scalableCoordinatePlane;

            _scalableCoordinatePlane.MouseDown += mouseDown;
            _scalableCoordinatePlane.MouseUp += mouseUp;
            _scalableCoordinatePlane.MouseMove += mouseMove;
            _scalableCoordinatePlane.MouseWheel += mouseWheel;

            PropertyChanged += propertyСhanged;

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

                // Получаем верхние левые крайние точки внутреннего и внешнего холста
                // для контроля верхней и левой границы.
                var ptsPMin = _coordinateViewPort.PointToScreen(new Point() { X = 0, Y = 0 });
                var ptsCMin = _scalableCoordinatePlane.PointToScreen(new Point() { X = 0, Y = 0 });

                // Получаем нижние правые крайние точки внутреннего и внешнего холста
                // для контроля нижней и правой границы.
                var ptsPMax = _coordinateViewPort.PointToScreen(new Point() { X = _coordinateViewPort.ActualWidth, Y = _coordinateViewPort.ActualHeight });
                var ptsCMax = _scalableCoordinatePlane.PointToScreen(new Point() { X = _scalableCoordinatePlane.ActualWidth, Y = _scalableCoordinatePlane.ActualHeight });

                // Получим положительные допустимые смещения.
                var maxOffsetToRight = (ptsPMin.X - ptsCMin.X);
                var maxOffsetToDown = (ptsPMin.Y - ptsCMin.Y);
                var maxOffsetToLeft = (ptsCMax.X - ptsPMax.X);
                var maxOffsetToUp = (ptsCMax.Y - ptsPMax.Y);

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

            // 3) Нужно проверить не станет ли координатная плоскость менше размеров ViewPort'а при следующем шаге
            // масштабирования. Если таки станет меньше, то нужно вычислить корректировочный множитель, чтобы
            // координатная плоскость точно вписалась в размеры ViewPort'a. (Актуально при уменьшении масштаба)

            // Получим ширины и высоты ViewPort'а и координатной плоскости с учётом текущего и предстоящего шага масштабирования.
            var widthPlane = _scalableCoordinatePlane.OriginalWidth * ScaleRateX * futureScalingRateStep;
            var heightPlane = _scalableCoordinatePlane.OriginalHeight * ScaleRateY * futureScalingRateStep;

            // Получим актуальные размеры ViewPort'а.
            var widthViewPort = _coordinateViewPort.ActualWidth;
            var heightViewPort = _coordinateViewPort.ActualHeight;

            // Получим разницу размеров между координатной плоскостью и ViewPort'ом.
            var differenceWidth = widthPlane - widthViewPort;
            var differenceHeight = heightPlane - heightViewPort;

            // Если разница отрицательна, то это значит что координатная плоскость меньше чем ViewPort 
            // и нужно масштабировать её под размеры ViewPort'а.
            if (differenceWidth < 0 || differenceHeight < 0)
            {
                double correctedScalingRateStep;

                // Будем ориентироваться на ту ось, по которой размер координатной плоскости меньше.
                if (differenceWidth < differenceHeight)
                {
                    // Расчёт корректировки.
                    correctedScalingRateStep = widthViewPort / widthPlane;
                }
                else
                {
                    // Расчёт корректировки.
                    correctedScalingRateStep = heightViewPort / heightPlane;
                }

                // Корректируем шаг масштаба.
                futureScalingRateStep *= correctedScalingRateStep;
            }

            // 4) Вычислить смещения.

            // Получаем точку положения курсора мыши и принимаем её как опорную для масштабирования координатной плоскости.
            var cursorPos = Mouse.GetPosition(_scalableCoordinatePlane);

            // Вычислим будующее положение точки
            double futureCursorPosX = cursorPos.X * futureScalingRateStep;
            double futureCursorPosY = cursorPos.Y * futureScalingRateStep;

            // Вычислим разницу нового и старого положения.
            var diffPosX = cursorPos.X - futureCursorPosX;
            var diffPosY = cursorPos.Y - futureCursorPosY;

            // Сохраним разницу как будующее смещение для возможных корректировок.
            var futureMoveX = diffPosX;
            var futureMoveY = diffPosY;

            // Вычислим будующее положение координатной плоскости.
            var futureCanvasLeft = CanvasLeft + futureMoveX;
            var futureCanvasTop = CanvasTop + futureMoveY;

            // Вычислим будующие размеры координатной плоскости.
            double futurePlaneWidth = _scalableCoordinatePlane.OriginalWidth * ScaleRateX * futureScalingRateStep;
            double futurePlaneHeight = _scalableCoordinatePlane.OriginalHeight * ScaleRateY * futureScalingRateStep;

            // Вычислим будующую отображаемую часть* координатной плоскости.
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
                    futureMoveX += - futureCanvasLeft;
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
                    // Может
                    //throw new Exception();
                }

                if (futureCanvasTop > 0)
                {
                    futureMoveY += - futureCanvasTop;
                }

                if (visiblePartHeight < _coordinateViewPort.ActualHeight)
                {
                    futureMoveY += _coordinateViewPort.ActualHeight - visiblePartHeight;
                }
            }

            MoveX = 0;
            MoveY = 0;

            // Если не заблокировано масштабирование по оси.
            if (!_coordinateTimeGrid.IsBlockingScaleX)
                MoveX = futureMoveX;

            // Если не заблокировано масштабирование по оси.
            if (!_coordinateTimeGrid.IsBlockingScaleY)
                MoveY = futureMoveY;

            // Применение масштаба.
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
            // отображена во ViewPort'е или же часть смещена за "границу ViewPort'a".

            var scalingRateStep = 1.0;

            // Получим ширину и высоту координатной плоскости.
            var widthPlane = _scalableCoordinatePlane.ActualWidth;
            var heightPlane = _scalableCoordinatePlane.ActualHeight;

            // Получим новую ширину и высоту ViewPort'а.
            var widthViewPort = e.NewSize.Width;
            var heightViewPort = e.NewSize.Height;

            // Если координатная плоскость не вписывается слева или сверху.
            if (CanvasLeft != 0 || CanvasTop != 0)
            {
                // Если координатная плоскость не вписывается слева.
                // Т.е. начало координатной плоскости по оси X левее чем левая граница ViewPort'а.
                if (CanvasLeft < 0)
                {
                    // Не скрытая за левой границей часть координатной плоскости. Пока эта часть больше
                    // чем ширина ViewPort'а, то не нужно координатную плоскость тащить вправо (Увеличивая
                    // отрицательное значение параметра CanvasLeft до нуля.)
                    var visiblePartWidth = widthPlane - Math.Abs(CanvasLeft);

                    // Если эта часть таки меньше чем ширина ViewPort'а.
                    if (visiblePartWidth < widthViewPort)
                    {
                        // Получаем - на сколько меньше.
                        var diffWidth = widthViewPort - visiblePartWidth;

                        // Увеличиваем отрицательное значение приближая его к нулю.
                        CanvasLeft += diffWidth;
                    }
                }
                else if (CanvasLeft > 0)
                {
                    // Не предполагалось что CanvasLeft может стать больше нуля.
                    CanvasLeft = 0;
                }

                // Если координатная плоскость не вписывается сверху т.е. начало координатной плоскости 
                // по оси Y выше чем верхняя граница ViewPort'а.
                if (CanvasTop < 0)
                {
                    // Не скрытая за верхней границей часть координатной плоскости. Пока эта часть больше
                    // чем высота ViewPort'а, то не нужно координатную плоскость тащить вниз (Увеличивая
                    // отрицательное значение параметра CanvasTop до нуля.)
                    var visiblePartHeight = heightPlane - Math.Abs(CanvasTop);

                    // Если эта часть таки меньше чем высота ViewPort'а.
                    if (visiblePartHeight < heightViewPort)
                    {
                        // Получаем - на сколько меньше.
                        var diffHeight = heightViewPort - visiblePartHeight;

                        // Увеличиваем отрицательное значение приближая его к нулю.
                        CanvasTop += diffHeight;
                    }
                }
                else if (CanvasTop > 0)
                {
                    // Не предполагалось что CanvasTop может стать больше нуля.
                    CanvasTop = 0;
                }
            }

            // При растягивании ViewPort'а координатная плоскость могжет стать меньше чем 
            // новые размеры ViewPort'а. Поэтому нужно сравнить новые размеры ViewPort'а с
            // размерами координатной плоскости. И если координатная плоскость - меньше, то
            // высчитать недостающий масштаб и применить его.

            // Получим разницу размеров между внутренним и внешним контролом.
            var differenceWidth = widthPlane - widthViewPort;
            var differenceHeight = heightPlane - heightViewPort;

            // Если разница отоицательна, то это значит что координатная плоскость меньше чем ViewPort 
            // и нужно масштабировать её под размеры ViewPort'а.
            if (differenceWidth < 0 || differenceHeight < 0)
            {
                // Корректировочный шаг масштабирования.
                double correctedScalingRateStep;

                // Будем ориентироваться на ту ось, по которой размер координатной плоскости меньше.
                if (differenceWidth < differenceHeight)
                {
                    // Расчёт корректировки.
                    correctedScalingRateStep = widthViewPort / widthPlane;
                }
                else
                {
                    // Расчёт корректировки.
                    correctedScalingRateStep = heightViewPort / heightPlane;
                }

                // Корректируем шаг масштаба.
                scalingRateStep *= correctedScalingRateStep;

                // Применить масштаб.
                ScalePlane(scalingRateStep);
            }

            // Оповещение о изменившихся свойствах
            // для вызова метода CalculateVisibleEdge
            if (_coordinateViewPort.Width != e.NewSize.Width)
                OnPropertyChanged(nameof(_coordinateViewPort.Width));

            if(_coordinateViewPort.Height != e.NewSize.Height)
                OnPropertyChanged(nameof(_coordinateViewPort.Height));
        }

        /// <summary>
        /// Обработчик событий PropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void propertyСhanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CanvasLeft) | 
                e.PropertyName == nameof(CanvasTop)  | 
                e.PropertyName == "Width" | 
                e.PropertyName == "Height")
            {
                CalculateVisibleEdge();
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

                // Если не заблокировано масштабирование по оси.
                if (!_coordinateTimeGrid.IsBlockingScaleX)
                    ScaleRateX *= _scalingRateStep;

                // Если не заблокировано масштабирование по оси.
                if (!_coordinateTimeGrid.IsBlockingScaleY)
                    ScaleRateY *= _scalingRateStep;
            }
        }

        /// <summary>
        /// Метод перерасчёта видимых краёв координатной плоскости.
        /// </summary>
        private void CalculateVisibleEdge()
        {
            var viewPortHeight = _coordinateViewPort.ActualHeight;
            var viewPortWidth = _coordinateViewPort.ActualWidth;

            TopVisibleEdge = (CanvasTop * -1);
            LeftVisibleEdge = (CanvasLeft * -1);

            BottomVisibleEdge = TopVisibleEdge + viewPortHeight;
            RightVisibleEdge = LeftVisibleEdge + viewPortWidth;
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
