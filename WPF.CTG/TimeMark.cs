using System;

namespace WPF.CTG
{
    /// <summary>
    /// Класс определяет маркер времени.
    /// </summary>
    public class TimeMark
    {
        public int Hour => Time.Hour;
        public int Minute => Time.Minute;
        public int Second => Time.Second;

        public string LabelTime => $@"{Hour:D2}:{Minute:D2}:{Second:D2}";

        public DateTime Time { get; set; }

    }
}
