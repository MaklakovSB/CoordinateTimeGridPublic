using System.Windows;

namespace WPF.CTG
{
    /// <summary>
    /// Класс определяет крайние точки для контроля границ внешнего и внутреннего холста.
    /// </summary>
    public class ExtremePoints
    {
        /// <summary>
        /// Крайняя вехняя левая точка на экране внешнего холста.
        /// </summary>
        public Point ExternalCanvasMinimum { get; set; }

        /// <summary>
        /// Крайняя правая нижняя точка на экране внешнего холста.
        /// </summary>
        public Point ExternalCanvasMaximum { get; set; }

        /// <summary>
        /// Крайняя вехняя левая точка на экране внутреннего холста.
        /// </summary>
        public Point InternalCanvasMinimum { get; set; }

        /// <summary>
        /// Крайняя правая нижняя точка на экране внутреннего холста.
        /// </summary>
        public Point InternalCanvasMaximum { get; set; }
    }
}
