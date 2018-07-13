using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for ScalableCoordinateTimeGrid.xaml
    /// </summary>
    public partial class CoordinateTimeGrid : UserControl, INotifyPropertyChanged
    {
        #region Свойства зависимости

        /// <summary>
        /// Определяет визуальное состояния элемента отображения отладочной информации.
        /// </summary>
        public static readonly DependencyProperty DebugInfoVisibilityProperty = DependencyProperty.Register(
            nameof(DebugInfoVisibility),
            typeof(Visibility),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(Visibility.Collapsed, OnDebugInfoVisibilityPropertyChange));

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public static readonly DependencyProperty IsBlockingScaleXProperty = DependencyProperty.Register(
            nameof(IsBlockingScaleX),
            typeof(bool),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(false, OnIsBlockingScaleXPropertyChange));

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public static readonly DependencyProperty IsBlockingScaleYProperty = DependencyProperty.Register(
            nameof(IsBlockingScaleY),
            typeof(bool),
            typeof(CoordinateTimeGrid),
            new PropertyMetadata(false, OnIsBlockingScaleYPropertyChange));

        #endregion

        #region Акцессоры свойств зависимости

        /// <summary>
        /// Определяет визуальное состояния элемента отображения отладочной информации.
        /// </summary>
        public Visibility DebugInfoVisibility
        {
            get { return (Visibility)GetValue(DebugInfoVisibilityProperty); }
            set { SetValue(DebugInfoVisibilityProperty, value); }
        }

        /// <summary>
        /// Блокировать масштабирование по оси X.
        /// </summary>
        public bool IsBlockingScaleX
        {
            get { return (bool)GetValue(IsBlockingScaleXProperty); }
            set { SetValue(IsBlockingScaleXProperty, value); }
        }

        /// <summary>
        /// Блокировать масштабирование по оси Y.
        /// </summary>
        public bool IsBlockingScaleY
        {
            get { return (bool)GetValue(IsBlockingScaleYProperty); }
            set { SetValue(IsBlockingScaleYProperty, value); }
        }

        #endregion

        #region Свойства

        /// <summary>
        /// Управляющий трансформациями координатной плоскости.
        /// </summary>
        public TransformManager TransformManager
        {
            get
            {
                if(_transformManager == null)
                    _transformManager = new TransformManager();
                return _transformManager;
            }

            set
            {
                _transformManager = value;
                OnPropertyChanged(nameof(TransformManager));
            }
        }
        private TransformManager _transformManager;

        /// <summary>
        /// Возвращает и устанавливает визуальное состояние вертикальной полосы прокрутки.
        /// </summary>
        public Visibility VerticalScrollBarVisibility
        {
            get { return _verticalScrollBarVisibility; }
            set
            {
                _verticalScrollBarVisibility = value;
                OnPropertyChanged(nameof(VerticalScrollBarVisibility));
            }
        }
        private Visibility _verticalScrollBarVisibility = Visibility.Collapsed;

        /// <summary>
        /// Возвращает и устанавливает визуальное состояние горизонтальной полосы прокрутки.
        /// </summary>
        public Visibility HorizontalScrollBarVisibility
        {
            get { return _horizontalScrollBarVisibility; }
            set
            {
                _horizontalScrollBarVisibility = value;
                OnPropertyChanged(nameof(HorizontalScrollBarVisibility));
            }
        }
        private Visibility _horizontalScrollBarVisibility = Visibility.Collapsed;

        #endregion

        #region * Конструкторы

        public CoordinateTimeGrid()
        {
            DataContext = this;
            InitializeComponent();
            TransformManager.TransformInit(_coordinateViewPort, _scalableCoordinatePlane);

            //// Перенесено в XAML.
            ////var cnv = new Canvas()
            ////{
            ////    Name = "_coordinateViewPort",
            ////    ClipToBounds = true,
            ////    Background = new SolidColorBrush(Colors.Black),
            ////};
            ////
            ////var scp = new ScalableCoordinatePlane()
            ////{
            ////    Name = "_scalableCoordinatePlane",
            ////    Height = 2000,
            ////    Width = 5000,
            ////    GridColor = Colors.White
            ////};
            ////
            ////Canvas.SetTop(scp, 0);
            ////Canvas.SetLeft(scp, 0);
            ////
            ////cnv.Children.Add(scp);
            ////Content = cnv;
        }

        #endregion

        #region Обработчики событий изменения свойств зависимости

        /// <summary>
        /// Изменение визуального состояния элемента отображения отладочной информации.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnDebugInfoVisibilityPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var div = obj._debugInfo.Visibility;
                div = (Visibility)e.NewValue;
            }
        }

        /// <summary>
        /// Изменение визуального состояния элемента отображения отладочной информации.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsBlockingScaleXPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var tm = obj.TransformManager;
                tm.IsBlockingScaleX = (bool)e.NewValue;
            }
        }

        /// <summary>
        /// Изменение визуального состояния элемента отображения отладочной информации.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnIsBlockingScaleYPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as CoordinateTimeGrid;
            if (obj != null)
            {
                var tm = obj.TransformManager;
                tm.IsBlockingScaleY = (bool)e.NewValue;
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
