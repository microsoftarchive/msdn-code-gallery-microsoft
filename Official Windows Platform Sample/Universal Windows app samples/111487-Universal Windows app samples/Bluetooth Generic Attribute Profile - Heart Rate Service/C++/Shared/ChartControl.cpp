#include "pch.h"
#include "ChartControl.h"

using namespace SDKSample::BluetoothGattHeartRate;

using namespace std;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Shapes;

ChartControl::ChartControl()
{
    // Create a vector of colors, used to draw the chart background
    colors = ref new Vector<Color>();

    // Start from a base color of light blue
    Color baseColor;
    baseColor.A = 0xFF;
    baseColor.R = 0x99;
    baseColor.G = 0xFF;
    baseColor.B = 0xFF;

    unsigned char gradient = 0x99 / DEFAULT_GRADIENT_COUNT;
    
    // Create background colors that move toward dark blue
    for (int i = 0; i < DEFAULT_GRADIENT_COUNT; i++)
    {
        baseColor.R -= gradient;
        baseColor.G -= gradient;

        colors->Append(baseColor);
    }

    this->DataPointCount = DEFAULT_DATAPOINT_COUNT;

    Color foreColor;
    foreColor.R = 0xDD;
    foreColor.G = 0xFF;
    foreColor.B = 0xDD;

    chartColor = ref new SolidColorBrush(foreColor);

    this->SizeChanged += ref new SizeChangedEventHandler(this, &SDKSample::BluetoothGattHeartRate::ChartControl::OnSizeChanged);

    DrawBackground();
}

ChartControl::~ChartControl()
{
}

void ChartControl::CreateRenderingOptions()
{
    renderingOptions = nullptr;
    if (dataSet != nullptr)
    {
        renderingOptions = ref new RenderingOptions();
#pragma push_macro("min")
#pragma push_macro("max")
#undef min
#undef max
        renderingOptions->MinValue = numeric_limits<float64>::max();
        renderingOptions->MaxValue = numeric_limits<float64>::min();
#pragma pop_macro("min")
#pragma pop_macro("max")

        uint32 startIndex = (dataSet->Size < DataPointCount) ? 0 : dataSet->Size - DataPointCount;

        for (auto i = startIndex; i < dataSet->Size; i++)
        {
            renderingOptions->MinValue = min(dataSet->GetAt(i)->HeartRateValue, renderingOptions->MinValue);
            renderingOptions->MaxValue = max(dataSet->GetAt(i)->HeartRateValue, renderingOptions->MaxValue);
        }
    }
}

void ChartControl::PlotChart(IVectorView<HeartRateMeasurement^>^ data)
{
    // First set the data points that we are going to render
    // The functions will use this data to plot the chart
    dataSet = data;

    // Remove previous rendering
    this->Children->Clear();

    CreateRenderingOptions();

    // Preprocess the data for rendering
    FillOffsetVector();

    // Render the actual chart in natural Z order

    DrawBackground();

    DrawYAxis();

    DrawChart();
}

void ChartControl::FillOffsetVector()
{
    offsetVector = nullptr;

    if ((dataSet != nullptr) && (dataSet->Size > 0))
    {
        offsetVector = ref new Vector<DataPoint^>();

        auto valueDiff = renderingOptions->MaxValue - renderingOptions->MinValue;

        // Add a 10% buffer to the extreme values on the chart for framing
        auto diffBuffer = (valueDiff > 0) ? (valueDiff * 0.1) : 2;
        renderingOptions->MaxValueBuffered = renderingOptions->MaxValue + diffBuffer;
        renderingOptions->MinValueBuffered = renderingOptions->MinValue - diffBuffer;
        renderingOptions->MinValueBuffered = (renderingOptions->MinValueBuffered > 0) ? 
            renderingOptions->MinValueBuffered : 0;

        valueDiff = renderingOptions->MaxValueBuffered - renderingOptions->MinValueBuffered;

        // Calculate the number of data points used
        auto pointsDisplayed = (dataSet->Size > DataPointCount) ? DataPointCount : dataSet->Size;

        // Add a 5% horizontal buffer to the displayed values, for framing
        auto bufferWidth = ActualWidth * 0.05;
        auto tickOffset = (ActualWidth - (bufferWidth * 2)) / pointsDisplayed;
        auto currentOffset = bufferWidth;

        for (auto i = dataSet->Size - pointsDisplayed; i < dataSet->Size; i++)
        {
            auto currentDiff = renderingOptions->MaxValueBuffered - dataSet->GetAt(i)->HeartRateValue;

            offsetVector->Append(ref new DataPoint(
                currentOffset,
                (currentDiff / valueDiff) * ActualHeight,
                dataSet->GetAt(i)->HeartRateValue));

            currentOffset += tickOffset;
        }
    }
}

void ChartControl::DrawBackground()
{
    auto tickOffsetY = this->ActualHeight / DEFAULT_GRADIENT_COUNT;
    auto currentOffsetY = 0.0;
    for (int i = 0; i < DEFAULT_GRADIENT_COUNT; i++)
    {
        Rectangle^ rect = ref new Rectangle();
        rect->Fill = ref new SolidColorBrush(colors->GetAt(i));
        rect->Width = this->ActualWidth;
        rect->Height = tickOffsetY;

        this->Children->Append(rect);
        SetLeft(rect, 0);
        SetTop(rect, currentOffsetY);
        currentOffsetY += tickOffsetY;
    }
}

TextBlock^ ChartControl::CreateTextBlock(float64 val)
{
    const int FONT_SIZE = 22;

    TextBlock^ textBlock = ref new TextBlock();
    textBlock->FontSize = FONT_SIZE;
    textBlock->Foreground = ref new SolidColorBrush(Colors::Blue);
    textBlock->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
    wstringstream wss;
    wss << fixed << setprecision(1) << val;
    textBlock->Text = ref new String(wss.str().c_str());

    return textBlock;
}


void ChartControl::DrawYAxis()
{
    if ((dataSet != nullptr) && (dataSet->Size > 0))
    {   
        const int RIGHT_TEXT_MARGIN = 9;
        const int BOTTOM_TEXT_MARGIN = 24;
        const int STRIPE_COUNT = DEFAULT_GRADIENT_COUNT;

        TextBlock^ text = CreateTextBlock(renderingOptions->MaxValueBuffered);
        this->Children->Append(text);
        SetTop(text, 2);

        auto percent = (renderingOptions->MaxValueBuffered - renderingOptions->MinValueBuffered) * 
            (1.0 / (STRIPE_COUNT));

        for (int i = 1; i < STRIPE_COUNT; i++)
        {
            auto percentVal = renderingOptions->MaxValueBuffered - (percent * i);

            text = CreateTextBlock(percentVal);
            this->Children->Append(text);
            SetTop(text, (i * (ActualHeight / STRIPE_COUNT)) - RIGHT_TEXT_MARGIN);
        }
        
        text = CreateTextBlock(renderingOptions->MinValueBuffered);
        this->Children->Append(text);
        SetTop(text, ActualHeight - BOTTOM_TEXT_MARGIN);
    }
}

void ChartControl::DrawChart()
{
    if ((dataSet != nullptr) && (dataSet->Size > 0))
    {
        auto path = ref new Path();
        path->Stroke = chartColor;
        path->StrokeThickness = 15;
        path->StrokeLineJoin = PenLineJoin::Round;
        path->StrokeStartLineCap = PenLineCap::Round;
        path->StrokeEndLineCap = PenLineCap::Round;

        auto geometry = ref new PathGeometry();

        auto figure = ref new PathFigure();
        figure->IsClosed = false;
        figure->StartPoint = Point((float)offsetVector->GetAt(0)->OffsetX, (float)offsetVector->GetAt(0)->OffsetY);
                
        for (uint32 i = 0; i < offsetVector->Size; i++)
        {
            auto segment = ref new LineSegment();
            segment->Point = Point((float)offsetVector->GetAt(i)->OffsetX, (float)offsetVector->GetAt(i)->OffsetY);
            figure->Segments->Append(segment);
        }
        geometry->Figures->Append(figure);
        path->Data = geometry;
        Children->Append(path);
    }
}

void ChartControl::OnSizeChanged(Object ^sender, SizeChangedEventArgs ^e)
{
    PlotChart(dataSet);
}
