using System;
using System.ComponentModel;
using System.Drawing;

namespace ScottPlot.Drawing
{
    public class Font:PropertyNotifier, INotifyPropertyChanged
    {
        private float size = 12;
        public float Size { get => size; set { size = value; OnPropertyChanged(); } }
        private Color color = Color.Black;
        public Color Color { get => color; set { color = value; OnPropertyChanged(); } }
        private Alignment alignment = Alignment.UpperLeft;
        public Alignment Alignment { get => alignment; set { alignment = value; OnPropertyChanged(); } }
        private bool bold = false;
        public bool Bold { get => bold; set { bold = value; OnPropertyChanged(); } }
        private float rotation = 0;
        public float Rotation { get => rotation; set { rotation = value; OnPropertyChanged(); } }

        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                _Name = InstalledFont.ValidFontName(value); // ensure only valid font names can be assigned
                OnPropertyChanged();
            }
        }

        public Font() => Name = InstalledFont.Sans();
    }
}
