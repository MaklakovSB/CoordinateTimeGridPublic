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
        }

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
