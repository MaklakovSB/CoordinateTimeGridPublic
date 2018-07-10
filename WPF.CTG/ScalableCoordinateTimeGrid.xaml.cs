using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for ScalableCoordinateTimeGrid.xaml
    /// </summary>
    public partial class ScalableCoordinateTimeGrid : UserControl, INotifyPropertyChanged
    {
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

        public ScalableCoordinateTimeGrid()
        {
            DataContext = this;
            InitializeComponent();
            TransformManager.TransformInit(_coordinateViewPort, _scalableCoordinatePlane);

            //var cnv = new Canvas()
            //{
            //    Name = "_coordinateViewPort",
            //    ClipToBounds = true,
            //    Background = new SolidColorBrush(Colors.Black),
            //};

            //var scp = new ScalableCoordinatePlane()
            //{
            //    Name = "_scalableCoordinatePlane",
            //    Height = 2000,
            //    Width = 5000,
            //    GridColor = Colors.White
            //};

            //Canvas.SetTop(scp, 0);
            //Canvas.SetLeft(scp, 0);

            //cnv.Children.Add(scp);
            //Content = cnv;
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
