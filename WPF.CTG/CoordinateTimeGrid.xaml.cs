using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using WPF.CTG.Converters;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for CoordinateTimeGrid.xaml
    /// </summary>
    //[ContentProperty("Children")]
    public partial class CoordinateTimeGrid : UserControl, INotifyPropertyChanged
    {
        #region Константы
        #endregion

        #region Статические члены класса
        #endregion

        #region Приватные поля

        //private CoordinatePlane _coordinatePlane;

        #endregion

        #region Свойства зависимости
        #endregion

        #region События свойств зависимости
        #endregion

        #region Свойства

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

        /// <summary>
        /// 
        /// </summary>
        public double MaximumScrlY => _coordinatePlane.ActualHeight * _coordinatePlane.ScaleRateY; // - _canvasParent.ActualHeight;

        /// <summary>
        /// 
        /// </summary>
        public double MaximumScrlX => _coordinatePlane.ActualWidth * _coordinatePlane.ScaleRateX;// - _canvasParent.ActualWidth;

        /// <summary>
        /// Коллекция для хранения нумерации делений шкалы.
        /// </summary>
        public ObservableCollection<TimeMark> ScaleNumbering
        {
            get
            {
                if (_scaleNumbering == null)
                {
                    _scaleNumbering = new ObservableCollection<TimeMark>();
                    ScaleInit();
                    OnPropertyChanged(nameof(ScaleNumbering));
                }
                return _scaleNumbering;
            }
        }
        private ObservableCollection<TimeMark> _scaleNumbering;

        #endregion

        #region * Конструкторы

        /// <summary>
        /// * Конструктор
        /// </summary>
        public CoordinateTimeGrid()
        {
            DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Инициализация отметок шкалы
        /// </summary>
        private void ScaleInit()
        {
            var tm0 = new TimeMark() { Time = DateTime.Parse(@"2018-05-19 00:00:00") };
            for (int i = 0; i < 50; i++) /* (24 * 60 / 5) */
            {
                var tmi = new TimeMark() { Time = tm0.Time.AddMinutes(i * 5) };
                _scaleNumbering.Add(tmi);
            }
        }

        /// <summary>
        /// Обработчик изменения размера внешнего холста.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _canvasParent_SizeChanged(object sender, SizeChangedEventArgs e)
        {

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
