#pragma once
#include "pch.h"

#include "HeartRateService.h"

template<>
class std::equal_to<Windows::UI::Color>
{
public:
    bool operator() (const Windows::UI::Color& color1, const Windows::UI::Color& color2) {
        return ((color1.A == color2.A) && 
            (color1.R == color2.R) && 
            (color1.G == color2.G) && 
            (color1.B == color2.B));
    }
};

namespace SDKSample
{
    namespace BluetoothGattHeartRate 
    {
        public ref class DataPoint sealed
        {
        public:
            property float64 OffsetX 
            { 
                float64 get() 
                {
                    return offsetX;
                }
                void set(float64 value) 
                {
                    offsetX = value;
                }
            }
    
            property float64 OffsetY 
            { 
                float64 get() 
                {
                    return offsetY;
                }
                void set(float64 value) 
                {
                     offsetY = value;
                }
            }

            property float64 Value 
            { 
                float64 get() 
                { 
                    return value;
                }
                void set(float64 value) 
                {
                    value = value;
                }
            }

            DataPoint(float64 offsetX, float64 offsetY, float64 value)
            {
                this->offsetX = offsetX;
                this->offsetY = offsetY;
                this->value = value;
            }

        private:
            float64 offsetX;
            float64 offsetY;
            float64 value;
        };

        public ref class RenderingOptions sealed
        {
        public:
            property float64 MinValue 
            { 
                float64 get() 
                {
                    return minValue;
                }
                void set(float64 value) 
                {
                    minValue = value;
                }
            }

            property float64 MaxValue 
            { 
                float64 get() 
                {
                    return maxValue;
                }
                void set(float64 value) 
                {
                     maxValue = value;
                }
            }

            property float64 MinValueBuffered
            { 
                float64 get() 
                {
                    return minValueBuffered;
                }
                void set(float64 value) 
                {
                    minValueBuffered = value;
                }
            }

            property float64 MaxValueBuffered 
            { 
                float64 get() 
                {
                    return maxValueBuffered;
                }
                void set(float64 value)
                {
                    maxValueBuffered = value;
                }
            }
            
        private:
            float64 minValue;
            float64 maxValue;
            float64 minValueBuffered;
            float64 maxValueBuffered;
        };

        public ref class ChartControl sealed
            : public Windows::UI::Xaml::Controls::Canvas
        {
        public:
            // Number of data points the chart displays
            property uint32 DataPointCount 
            { 
                uint32 get() 
                {
                    return datapointCount; 
                }
                void set(uint32 value) 
                {
                    datapointCount = value;
                }
            }

            ChartControl();
            virtual ~ChartControl();

            void PlotChart(Windows::Foundation::Collections::IVectorView<HeartRateMeasurement^>^ data);

        private:
            void CreateRenderingOptions();
            void FillOffsetVector();
            void DrawBackground();
            void DrawYAxis();
            void DrawChart();
            void OnSizeChanged(Platform::Object ^sender, Windows::UI::Xaml::SizeChangedEventArgs ^e);

            Windows::UI::Xaml::Controls::TextBlock^ CreateTextBlock(float64 val);

            // Constants 
            static const uint32 DEFAULT_DATAPOINT_COUNT = 20;
            static const uint32 DEFAULT_GRADIENT_COUNT = 5;

            // Private members
            uint32 datapointCount;
            Windows::Foundation::Collections::IVectorView<HeartRateMeasurement^>^ dataSet;
            RenderingOptions^ renderingOptions;
            Platform::Collections::Vector<DataPoint^>^ offsetVector;

            Windows::UI::Xaml::Media::Brush^ chartColor;
            Platform::Collections::Vector<Windows::UI::Color>^ colors;
        };
    };
};
