using ScottPlot.Plottable;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScottPlot.Demo.WPF.WpfDemos
{
    /// <summary>
    /// Interaction logic for WpfConfig.xaml
    /// </summary>
    public partial class MVVMTest : Window, INotifyPropertyChanged
    {
        public MVVMTest()
        {
            InitializeComponent();
            PlotStyles = CreateStyleList();
            SelectedPlotStyle = ScottPlot.Style.Default;
            this.DataContext = this;
            this.Loaded += MVVMTest_Loaded;
        }

        private void MVVMTest_Loaded(object sender, RoutedEventArgs e)
        {

            Axes.Add(Renderable.Edge.Left, 2);
            Axes.Add(Renderable.Edge.Left, 3);
            Axes.Add(Renderable.Edge.Top, 2);
            Axes.Add(Renderable.Edge.Right, 2);

            int pointCount = 51;
            double[] x = DataGen.Consecutive(pointCount);
            double[] sin = DataGen.Sin(pointCount);

            SignalPlotXY sp1 = new SignalPlotXY();
            sp1.Xs = x;
            sp1.Ys = sin;
            sp1.MinRenderIndex = 0;
            sp1.MaxRenderIndex = sp1.Ys.Length - 1;
            sp1.Label = "sin SignalPlot";
            Plottables.Add(sp1);

            double[] cos = DataGen.Cos(pointCount);
            ScatterPlot sp3 = new ScatterPlot(x, cos);
            sp3.YAxisIndex = 2;
            sp3.Label = "cos ScatterPlot";
            Plottables.Add(sp3);

            Plottables.Add(new VSpan() { DragEnabled = true, Y1 = 2, Y2 = 10 });
            Plottables.Add(new HSpan() { DragEnabled = true, X1 = 2, X2 = 10 });
            Plottables.Add(new VLine() { DragEnabled = true, X = 1 });
            Plottables.Add(new HLine() { DragEnabled = true, Y = 1 });
        }

        private PlottableCollection plottables = new PlottableCollection();
        public PlottableCollection Plottables
        {
            get => plottables;
            set { plottables = value; }
        }

        private object selectedObject;
        public object SelectedObject
        {
            get => selectedObject;
            set {
                selectedObject = value;
                OnPropertyChanged(); 
            }
        }

        private AxisCollection axes = new AxisCollection();
        public AxisCollection Axes
        {
            get => axes;
            set { axes = value; }
        }

        private List<Styles.IStyle> CreateStyleList()
        {
            List<Styles.IStyle> plotStyles = new List<Styles.IStyle>();
            plotStyles.AddRange(ScottPlot.Style.GetStyles());
            return plotStyles;
        }
        private List<Styles.IStyle> plotStyles;
        public List<Styles.IStyle> PlotStyles
        {
            get
            {
                return plotStyles;
            }
            set
            {
                if (plotStyles == value)
                    return;
                plotStyles = value;
                OnPropertyChanged();
            }
        }

        private Styles.IStyle selectedPlotStyle;
        public Styles.IStyle SelectedPlotStyle
        {
            get
            {
                return selectedPlotStyle;
            }
            set
            {
                selectedPlotStyle = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;




    }
}
