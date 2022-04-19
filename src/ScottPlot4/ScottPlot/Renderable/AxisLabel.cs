using ScottPlot.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;

namespace ScottPlot.Renderable
{
    public class AxisLabel : PropertyNotifier, IRenderable, INotifyPropertyChanged
    {
        private bool isVisible = true;
        /// <summary>
        /// Controls whether this axis occupies space and is displayed
        /// </summary>
        public bool IsVisible { get=> isVisible; set { isVisible = value; OnPropertyChanged(); } }

        /// <summary>
        /// Edge of the data area this axis represents
        /// </summary>
        public Edge Edge;

        private string label = null;
        /// <summary>
        /// Axis title
        /// </summary>
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }

        private Drawing.Font font;
        /// <summary>
        /// Font options for the axis title
        /// </summary>
        public Drawing.Font Font
        {
            get => font;
            set
            {
                if (font != null)
                    font.PropertyChanged -= Internal_PropertyChanged;
                font = value;
                if (font != null)
                    font.PropertyChanged += Internal_PropertyChanged;
                OnPropertyChanged();
            }
        }
        private void Internal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(sender));
        }

        private Bitmap imageLabel = null;
        /// <summary>
        /// Set this field to display a bitmap instead of a text axis label
        /// </summary>
        public Bitmap ImageLabel { get => imageLabel; set { imageLabel = value; OnPropertyChanged(); } }

        private float imagePaddingToDataArea = 5;
        /// <summary>
        /// Padding (in pixels) between the image and the edge of the data area
        /// </summary>
        public float ImagePaddingToDataArea { get => imagePaddingToDataArea; set { if (imagePaddingToDataArea == value) return; imagePaddingToDataArea = value; OnPropertyChanged(); } }

        private float imagePaddingToFigureEdge = 5;
        /// <summary>
        /// Padding (in pixels) between the image and the edge of the figure
        /// </summary>
        public float ImagePaddingToFigureEdge { get => imagePaddingToFigureEdge; set { if (imagePaddingToFigureEdge == value) return; imagePaddingToFigureEdge = value; OnPropertyChanged(); } }

        /// <summary>
        /// Total amount (in pixels) to pad the image when measuring axis size
        /// </summary>
        public float ImagePadding => ImagePaddingToDataArea + ImagePaddingToFigureEdge;

        private float pixelSizePadding;
        /// <summary>
        /// Amount of padding (in pixels) to surround the contents of this axis
        /// </summary>
        public float PixelSizePadding { get => pixelSizePadding; set { if (pixelSizePadding == value) return; pixelSizePadding = value; OnPropertyChanged(); } }

        private float pixelOffset;
        /// <summary>
        /// Distance to offset this axis to account for multiple axes
        /// </summary>
        public float PixelOffset { get => pixelOffset; set { if (pixelOffset == value) return; pixelOffset = value; OnPropertyChanged(); } }

        private float pixelSize;
        /// <summary>
        /// Exact size (in pixels) of the contents of this this axis
        /// </summary>
        public float PixelSize { get => pixelSize; set { if (pixelSize == value) return; pixelSize = value; OnPropertyChanged(); } }

        /// <summary>
        /// Return the size of the contents of this axis.
        /// Returned dimensions are screen-accurate (even if this axis is rotated).
        /// </summary>
        /// <returns></returns>
        public SizeF Measure()
        {
            if (ImageLabel != null)
            {
                return (Edge == Edge.Bottom || Edge == Edge.Top)
                    ? new SizeF(ImageLabel.Width, ImageLabel.Height + ImagePadding)
                    : new SizeF(ImageLabel.Height, ImageLabel.Width + ImagePadding);
            }
            else
            {
                return GDI.MeasureString(Label, Font);
            }
        }

        public AxisLabel()
        {
            Font = new Drawing.Font() { Size = 16 };
        }

        public void Render(PlotDimensions dims, Bitmap bmp, bool lowQuality = false)
        {
            if (IsVisible == false || (string.IsNullOrWhiteSpace(Label) && ImageLabel == null))
                return;

            using var gfx = GDI.Graphics(bmp, dims, lowQuality, false);
            (float x, float y) = GetAxisCenter(dims);

            if (ImageLabel is not null)
            {
                RenderImageLabel(gfx, dims, x, y);
                return;
            }

            if (Font.Rotation != 0)
            {
                RenderTextLabelRotated(gfx, dims, x, y);
                return;
            }

            RenderTextLabel(gfx, dims, x, y);
        }

        private void RenderImageLabel(Graphics gfx, PlotDimensions dims, float x, float y)
        {
            // TODO: use ImagePadding instead of fractional padding

            float xOffset = Edge switch
            {
                Edge.Left => ImagePaddingToFigureEdge,
                Edge.Right => -ImageLabel.Width - ImagePaddingToFigureEdge,
                Edge.Bottom => -ImageLabel.Width,
                Edge.Top => -ImageLabel.Width,
                _ => throw new NotImplementedException()
            };

            float yOffset = Edge switch
            {
                Edge.Left => -ImageLabel.Height,
                Edge.Right => -ImageLabel.Height,
                Edge.Bottom => -ImageLabel.Height - ImagePaddingToFigureEdge,
                Edge.Top => 0 + ImagePaddingToFigureEdge,
                _ => throw new NotImplementedException()
            };

            gfx.TranslateTransform(x, y);
            gfx.DrawImage(ImageLabel, xOffset, yOffset);
            GDI.ResetTransformPreservingScale(gfx, dims);
        }

        private void RenderTextLabel(Graphics gfx, PlotDimensions dims, float x, float y)
        {
            float padding = (Edge == Edge.Bottom) ? -PixelSizePadding : PixelSizePadding;

            int rotation = Edge switch
            {
                Edge.Left => -90,
                Edge.Right => 90,
                Edge.Bottom => 0,
                Edge.Top => 0,
                _ => throw new NotImplementedException()
            };

            using var font = GDI.Font(Font);
            using var brush = GDI.Brush(Font.Color);
            using var sf = GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower);
            sf.LineAlignment = Edge switch
            {
                Edge.Left => StringAlignment.Near,
                Edge.Right => StringAlignment.Near,
                Edge.Bottom => StringAlignment.Far,
                Edge.Top => StringAlignment.Near,
                _ => throw new NotImplementedException()
            };

            gfx.TranslateTransform(x, y);
            gfx.RotateTransform(rotation);
            gfx.DrawString(Label, font, brush, 0, padding, sf);
            GDI.ResetTransformPreservingScale(gfx, dims);
        }

        private void RenderTextLabelRotated(Graphics gfx, PlotDimensions dims, float x, float y)
        {
            using var font = GDI.Font(Font);
            using var brush = GDI.Brush(Font.Color);

            gfx.TranslateTransform(x, y);

            if (Edge == Edge.Right)
            {
                if (Font.Rotation != 90)
                    throw new NotImplementedException("right axis label rotation must be 0 or 90");

                using var sf = GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower);
                gfx.RotateTransform(-Font.Rotation);
                gfx.DrawString(Label, font, brush, 0, 0, sf);
            }
            else if (Edge == Edge.Left)
            {
                if (Font.Rotation != 90)
                    throw new NotImplementedException("left axis label rotation must be 0 or 90");

                using var sf = GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper);
                gfx.RotateTransform(-Font.Rotation);
                gfx.DrawString(Label, font, brush, 0, 0, sf);
            }
            else if (Edge == Edge.Bottom)
            {
                if (Font.Rotation != 180)
                    throw new NotImplementedException("bottom axis label rotation must be 0 or 180");

                using var sf = GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Upper);
                gfx.RotateTransform(-Font.Rotation);
                gfx.DrawString(Label, font, brush, 0, 0, sf);
            }
            else if (Edge == Edge.Top)
            {
                if (Font.Rotation != 180)
                    throw new NotImplementedException("top axis label rotation must be 0 or 180");

                using var sf = GDI.StringFormat(HorizontalAlignment.Center, VerticalAlignment.Lower);
                gfx.RotateTransform(-Font.Rotation);
                gfx.DrawString(Label, font, brush, 0, 0, sf);
            }
            else
            {
                throw new NotImplementedException(Edge.ToString());
            }

            GDI.ResetTransformPreservingScale(gfx, dims);
        }

        /// <summary>
        /// Return the point and rotation representing the center of the base of this axis
        /// </summary>
        private (float x, float y) GetAxisCenter(PlotDimensions dims)
        {
            float x = Edge switch
            {
                Edge.Left => dims.DataOffsetX - PixelOffset - PixelSize,
                Edge.Right => dims.DataOffsetX + dims.DataWidth + PixelOffset + PixelSize,
                Edge.Bottom => dims.DataOffsetX + dims.DataWidth / 2,
                Edge.Top => dims.DataOffsetX + dims.DataWidth / 2,
                _ => throw new NotImplementedException()
            };

            float y = Edge switch
            {
                Edge.Left => dims.DataOffsetY + dims.DataHeight / 2,
                Edge.Right => dims.DataOffsetY + dims.DataHeight / 2,
                Edge.Bottom => dims.DataOffsetY + dims.DataHeight + PixelOffset + PixelSize,
                Edge.Top => dims.DataOffsetY - PixelOffset - PixelSize,
                _ => throw new NotImplementedException()
            };

            return (x, y);
        }
    }
}
