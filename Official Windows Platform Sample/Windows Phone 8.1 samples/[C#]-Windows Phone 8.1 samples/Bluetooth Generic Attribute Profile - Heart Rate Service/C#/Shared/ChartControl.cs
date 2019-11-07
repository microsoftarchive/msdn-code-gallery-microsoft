using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace BluetoothGattHeartRate
{
    public sealed class ChartControl : Canvas
    {
        // Constants
        private const int DefaultDatapoints = 20;
        private const int DefaultGradients = 5;

        // Private members
        private HeartRateMeasurement[] dataSet = null;
        private RenderingOptions renderingOptions = null;
        private List<DataPoint> offsetList = null;
        
        Brush chartColor;
        List<Color> colors = new List<Color> 
        { 
            Color.FromArgb(0xFF, 0x99, 0xFF, 0xFF),
            Color.FromArgb(0xFF, 0x88, 0xEE, 0xFF),
            Color.FromArgb(0xFF, 0x77, 0xDD, 0xFF),
            Color.FromArgb(0xFF, 0x66, 0xCC, 0xFF),
            Color.FromArgb(0xFF, 0x55, 0xBB, 0xFF)
        };

        // Number of data points the chart displays
        public int DataPointCount { get; set; }
        public ChartControl()
        {
            this.DataPointCount = DefaultDatapoints;

            this.chartColor = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0xDD, 0xFF, 0xDD));

            DrawBackground();
        }

        private void CreateRenderingOptions()
        {
            renderingOptions = null;
            if (dataSet != null)
            {
                renderingOptions = new RenderingOptions();
                renderingOptions.MinValue = double.MaxValue;
                renderingOptions.MaxValue = double.MinValue;

                int startIndex = (dataSet.Length < DataPointCount) ? 0 : dataSet.Length - DataPointCount;

                for (int i = startIndex; i < dataSet.Length; i++)
                {
                    renderingOptions.MinValue = Math.Min(dataSet[i].HeartRateValue, renderingOptions.MinValue);
                    renderingOptions.MaxValue = Math.Max(dataSet[i].HeartRateValue, renderingOptions.MaxValue);
                }
            }
        }

        public void PlotChart(HeartRateMeasurement[] data)
        {
            // First set the data points that we are going to render
            // The functions will use this data to plot the chart
            dataSet = data;

            // Remove previous rendering
            this.Children.Clear();

            CreateRenderingOptions();

            // Preprocess the data for rendering
            FillOffsetList();

            // Render the actual chart in natural Z order

            DrawBackground();

            DrawYAxis();

            DrawChart();

        }

        private void FillOffsetList()
        {
            offsetList = null;

            if ((dataSet != null) && (dataSet.Length > 0))
            {
                offsetList = new List<DataPoint>();

                var valueDiff = renderingOptions.MaxValue - renderingOptions.MinValue;

                // Add a 10% buffer to the extreme value on the chart for framing
                var diffBuffer = (valueDiff > 0) ? (valueDiff * 0.1) : 2;
                renderingOptions.MaxValueBuffered = renderingOptions.MaxValue + diffBuffer;
                renderingOptions.MinValueBuffered = renderingOptions.MinValue - diffBuffer;
                renderingOptions.MinValueBuffered = (renderingOptions.MinValueBuffered > 0) ? 
                    renderingOptions.MinValueBuffered : 0;

                valueDiff = renderingOptions.MaxValueBuffered - renderingOptions.MinValueBuffered;

                // Calculate the number of data points used
                var pointsDisplayed = (dataSet.Length > DataPointCount) ? DataPointCount : dataSet.Length;

                // Add a 5% horizontal buffer to the displayed values, for framing
                var bufferWidth = ActualWidth * 0.05;
                var tickOffset = (ActualWidth - (bufferWidth * 2)) / pointsDisplayed;
                var currentOffset = bufferWidth;

                for (int i = dataSet.Length - pointsDisplayed; i < dataSet.Length; i++)
                {
                    var currentDiff = renderingOptions.MaxValueBuffered - dataSet[i].HeartRateValue;

                    offsetList.Add(new DataPoint
                    {
                        OffsetX = currentOffset,
                        OffsetY = (currentDiff / valueDiff) * ActualHeight,
                        Value = dataSet[i].HeartRateValue
                    });
                    currentOffset += tickOffset;
                }
            }           
        }

        private void DrawBackground()
        {
            var tickOffsetY = this.ActualHeight / colors.Count;
            var currentOffsetY = 0.0;
            for (int i = 0; i < DefaultGradients; i++)
            {
                Rectangle rect = new Rectangle()
                {
                    Fill = new SolidColorBrush(colors[i]),
                    Width = this.ActualWidth,
                    Height = tickOffsetY
                };
                this.Children.Add(rect);
                SetLeft(rect, 0);
                SetTop(rect, currentOffsetY);
                currentOffsetY += tickOffsetY;
            }
        }

        private void DrawYAxis()
        {
            if ((dataSet != null) && (dataSet.Length > 0))
            {
                const int RightTextMargin = 9;
                const int BottomTextMargin = 24;
                const int NoOfStripes = DefaultGradients;
                const int FontSize = 22;

                TextBlock text = new TextBlock();
                text.FontSize = FontSize;
                text.Foreground = new SolidColorBrush(Colors.Blue);
                text.Text = renderingOptions.MaxValueBuffered.ToString("#.#");
                this.Children.Add(text);
                text.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;

                var percent = (renderingOptions.MaxValueBuffered - renderingOptions.MinValueBuffered) * 
                    (1.0 / (NoOfStripes));
                SetTop(text, 2);

                for (int i = 1; i < NoOfStripes; i++)
                {
                    var percentVal = renderingOptions.MaxValueBuffered - (percent * i);

                    text = new TextBlock();
                    text.FontSize = FontSize;
                    text.Foreground = new SolidColorBrush(Colors.Blue);
                    text.Text = percentVal.ToString("#.#");
                    text.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                    this.Children.Add(text);
                    SetTop(text, (i * (ActualHeight / NoOfStripes)) - RightTextMargin);
                }

                text = new TextBlock();
                text.FontSize = FontSize;
                text.Foreground = new SolidColorBrush(Colors.Blue);
                text.Text = renderingOptions.MinValueBuffered.ToString("#.#");
                text.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                this.Children.Add(text);
                SetTop(text, ActualHeight - BottomTextMargin);
            }
        }

        private void DrawChart()
        {
            if ((dataSet != null) && (dataSet.Length > 0))
            {
                var path = new Windows.UI.Xaml.Shapes.Path();
                path.Stroke = chartColor;
                path.StrokeThickness = 15;
                path.StrokeLineJoin = PenLineJoin.Round;
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeEndLineCap = PenLineCap.Round;

                var geometry = new PathGeometry();

                var figure = new PathFigure();
                figure.IsClosed = false;
                figure.StartPoint = new Point(offsetList[0].OffsetX, offsetList[0].OffsetY);
                
                for (int i = 0; i < offsetList.Count; i++)
                {
                    var segment = new LineSegment();
                    segment.Point = new Point(offsetList[i].OffsetX, offsetList[i].OffsetY);
                    figure.Segments.Add(segment);
                }
                geometry.Figures.Add(figure);
                path.Data = geometry;
                Children.Add(path);
            }
        }
    }

    public class RenderingOptions
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValueBuffered { get; set; }
        public double MaxValueBuffered { get; set; }
    }

    public class DataPoint
    {
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public double Value { get; set; }
    }
}
