using ScottPlot.Plottable;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ScottPlot
{
    public partial class WpfPlot
    {
        #region Plottables

        public PlottableCollection PlottablesSource
        {
            get { return (PlottableCollection)GetValue(PlottablesSourceProperty); }
            set { SetValue(PlottablesSourceProperty, value); }
        }
        public static readonly DependencyProperty PlottablesSourceProperty = DependencyProperty.Register(nameof(PlottablesSource), typeof(PlottableCollection), typeof(WpfPlot), new PropertyMetadata((d, e) =>
        {
            if (d is WpfPlot chart)
            {
                if (e.OldValue is PlottableCollection olddatasources)
                {
                    olddatasources.CollectionChanged += chart.Plottables_CollectionChanged; ;
                    foreach (var seriesItem in olddatasources)
                    {
                        seriesItem.PropertyChanged -= chart.PlottablesItem_PropertyChanged;
                    }
                }
                if (e.NewValue is PlottableCollection newdatasources)
                {
                    chart.Plot.Settings.Plottables = newdatasources;
                    newdatasources.CollectionChanged += chart.Plottables_CollectionChanged;
                    foreach (var seriesItem in newdatasources)
                    {
                        seriesItem.PropertyChanged += chart.PlottablesItem_PropertyChanged;
                    }
                }
            }
        }));

        private void PlottablesItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!Plot.IsRendering)
                RefreshRequest();
        }

        private void Plottables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Plottables_Add(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    Plottables_Remove(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    Plottables_Replace(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Plottables_Reset(e);
                    break;
                default:
                    break;
            }
            RefreshRequest();
        }

        private void Plottables_Add(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (IPlottable item in e.NewItems)
            {
                item.PropertyChanged += PlottablesItem_PropertyChanged;
            }
        }

        private void Plottables_Remove(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (IPlottable item in e.OldItems)
            {
                item.PropertyChanged -= PlottablesItem_PropertyChanged;
            }
        }

        private void Plottables_Reset(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (IPlottable item in Plot.GetPlottables())
            {
                item.PropertyChanged -= PlottablesItem_PropertyChanged;
            }
        }

        private void Plottables_Replace(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (IPlottable item in e.OldItems)
            {
                item.PropertyChanged -= PlottablesItem_PropertyChanged;
            }
            foreach (IPlottable item in e.NewItems)
            {
                item.PropertyChanged += PlottablesItem_PropertyChanged;
            }
        }

        #endregion

        #region Axes

        public AxisCollection AxisSource
        {
            get { return (AxisCollection)GetValue(AxisSourceProperty); }
            set { SetValue(AxisSourceProperty, value); }
        }

        public static readonly DependencyProperty AxisSourceProperty = DependencyProperty.Register(nameof(AxisSource), typeof(AxisCollection), typeof(WpfPlot), new PropertyMetadata((d, e) =>
        {
            if (d is WpfPlot chart)
            {
                if (e.OldValue is AxisCollection olddatasources)
                {
                    olddatasources.CollectionChanged += chart.Axes_CollectionChanged; ;
                    foreach (var seriesItem in olddatasources)
                    {
                        seriesItem.PropertyChanged -= chart.AxesItem_PropertyChanged;
                    }
                }
                if (e.NewValue is AxisCollection newdatasources)
                {
                    chart.Plot.Settings.Axes = newdatasources;
                    newdatasources.CollectionChanged += chart.Axes_CollectionChanged;
                    foreach (var seriesItem in newdatasources)
                    {
                        seriesItem.PropertyChanged += chart.AxesItem_PropertyChanged;
                    }
                }
            }
        }));

        private void AxesItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!Plot.IsRendering)
                RefreshRequest();
        }

        private void Axes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Axes_Add(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    Axes_Remove(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    Axes_Replace(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    Axes_Move(e);
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Axes_Reset(e);
                    break;
                default:
                    break;
            }
            RefreshRequest();
        }

        private void Axes_Add(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Axis item in e.NewItems)
            {
                item.PropertyChanged += AxesItem_PropertyChanged;
            }
        }

        private void Axes_Remove(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Axis item in e.OldItems)
            {
                item.PropertyChanged -= AxesItem_PropertyChanged;
            }
        }

        private void Axes_Move(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Axes_Reset(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Axes_Replace(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Axis item in e.OldItems)
            {
                item.PropertyChanged -= AxesItem_PropertyChanged;
            }
            foreach (Axis item in e.NewItems)
            {
                item.PropertyChanged += AxesItem_PropertyChanged;
            }
        }

        #endregion

        #region Styling

        public Drawing.Palette Palette
        {
            get { return (Drawing.Palette)GetValue(PaletteProperty); }
            set { SetValue(PaletteProperty, value); RefreshRequest(); }
        }
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(nameof(Palette), typeof(Drawing.Palette), typeof(WpfPlot), new PropertyMetadata((d, e) =>
        {
            if (d is WpfPlot chart)
            {
                if (e.NewValue is Drawing.Palette newdatasources)
                {
                    chart.Plot.Settings.PlottablePalette = newdatasources;
                }
            }
        }));

        public Styles.IStyle PlotStyle
        {
            get { return (Styles.IStyle)GetValue(PlotStyleProperty); }
            set { SetValue(PlotStyleProperty, value); RefreshRequest(); }
        }
        public static readonly DependencyProperty PlotStyleProperty = DependencyProperty.Register(nameof(PlotStyle), typeof(Styles.IStyle), typeof(WpfPlot), new PropertyMetadata((d, e) =>
        {
            if (d is WpfPlot chart)
            {
                if (e.NewValue is Styles.IStyle newdatasources)
                {
                    chart.Plot.PlotStyle = newdatasources;
                }
            }
        }));
        #endregion

        #region Selection

        public object SelectedObject
        {
            get { return GetValue(SelectedObjectProperty); }
            set
            {
                object oldValue = GetValue(SelectedObjectProperty);
                SetValue(SelectedObjectProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(nameof(SelectedObject), typeof(object), typeof(WpfPlot), new PropertyMetadata((d, e) =>
        {
            if (d is WpfPlot chart)
            {
            }
        }));

        private const double MouseCaptureDistance = 5;
        private void OnMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (PlottablesSource == null) return;
            (float mouseX, float mouseY) = this.GetMousePixel();

            foreach (Axis item in AxisSource)
            {
                if (item.IsUnderMouse(mouseX, mouseY))
                {
                    SelectedObject = item;
                    return;
                }
            }

            foreach (ISelectable item in PlottablesSource.ByType(typeof(ISelectable)))
            {
                (double mouseCoordX, double mouseCoordY) = Plot.GetCoordinate(mouseX, mouseY, 0, 0);
                double xUnitsPerPx = Plot.Settings.GetXAxis(0).Dims.UnitsPerPx;
                double yUnitsPerPx = Plot.Settings.GetYAxis(0).Dims.UnitsPerPx;

                double snapWidth = xUnitsPerPx * MouseCaptureDistance;
                double snapHeight = yUnitsPerPx * MouseCaptureDistance;
                if (item.IsUnderMouse(mouseCoordX, mouseCoordY, snapWidth, snapHeight))
                {
                    SelectedObject = item;
                    return;
                }
            }

            foreach (ISelectable2DSeries item in PlottablesSource.ByType(typeof(ISelectable2DSeries)))
            {
                if (item == null) continue;
                (double mouseCoordX, double mouseCoordY) = Plot.GetCoordinate(mouseX, mouseY, item.XAxisIndex, item.YAxisIndex);
                double xUnitsPerPx = Plot.Settings.GetXAxis(item.XAxisIndex).Dims.UnitsPerPx;
                double yUnitsPerPx = Plot.Settings.GetYAxis(item.YAxisIndex).Dims.UnitsPerPx;

                double snapWidth = xUnitsPerPx * MouseCaptureDistance;
                double snapHeight = yUnitsPerPx * MouseCaptureDistance;

                if (item.IsUnderMouse(mouseCoordX, mouseCoordY, snapWidth, snapHeight))
                {
                    SelectedObject = item;
                    return;
                }
            }

            SelectedObject = null;
        }

        #endregion

    }
}
