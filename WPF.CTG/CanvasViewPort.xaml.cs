using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace WPF.CTG
{
    /// <summary>
    /// Interaction logic for CanvasViewPort.xaml
    /// </summary>
    public partial class CanvasViewPort : Canvas, INotifyPropertyChanged
    {
        public CanvasViewPort()
        {
            DataContext = this;
            InitializeComponent();
            //SizeChanged += HandleSizeChanged;
        }

        //public static DependencyProperty RealWidthProperty = DependencyProperty.Register(
        //    nameof(RealWidth), 
        //    typeof(double),
        //    typeof(CanvasViewPort),
        //    new PropertyMetadata(500D));

        //public double RealWidth
        //{
        //    get { return (double)GetValue(RealWidthProperty); }
        //    set { SetValue(RealWidthProperty, value); }
        //}

        //private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    RealWidth = e.NewSize.Width;
        //}

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
