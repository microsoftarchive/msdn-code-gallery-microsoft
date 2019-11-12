//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
#include "pch.h"
#include "DirectXPanelBase.h"

namespace DirectXPanels
{
    // Event args for the RecognitionResultUpdated event.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class RecognitionResultUpdatedEventArgs : public Windows::UI::Xaml::RoutedEventArgs
    {
    public:
        // Handwriting recognition results.
        property Platform::Array<Platform::String^>^ Results
        {
            Platform::Array<Platform::String^>^ get() { return m_results; }
        }

    internal:
        RecognitionResultUpdatedEventArgs(Platform::Array<Platform::String^>^ results)
        {
            m_results = results;
        }

    private protected:
        Platform::Array<Platform::String^>^ m_results;
    };

    // Delegate for the RecognitionResultUpdated event.
    [Windows::Foundation::Metadata::WebHostHidden]
    public delegate void RecognitionResultUpdatedEventHandler(Platform::Object^ sender, DirectXPanels::RecognitionResultUpdatedEventArgs^ s);

    // Hosts a DirectX rendering surface that supports various inking and drawing operations, in addition to raising handwriting recognition events.

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class DrawingPanel sealed : public DirectXPanels::DirectXPanelBase
    {
    public:
        DrawingPanel();

#pragma region DependencyProperties        
        static void RegisterDependencyProperties();

        static property Windows::UI::Xaml::DependencyProperty^ BrushColorProperty
        {
            Windows::UI::Xaml::DependencyProperty^ get() { return m_brushColorProperty; }
        }
        property Windows::UI::Color BrushColor
        {
            // Additionally keep a local value for thread-safe property access.
            Windows::UI::Color get() { return m_brushColor; }
            void set(Windows::UI::Color value) 
            { 
                m_brushColor = value;
                SetValue(BrushColorProperty, value); 
            }
        }

        static property Windows::UI::Xaml::DependencyProperty^ BrushFitsToCurveProperty
        {
            Windows::UI::Xaml::DependencyProperty^ get() { return m_brushFitsToCurveProperty; }
        }
        property bool BrushFitsToCurve
        {
            bool get() { return safe_cast<bool>(GetValue(BrushFitsToCurveProperty)); }
            void set(bool value) { SetValue(BrushFitsToCurveProperty, value); }
        }

        static property Windows::UI::Xaml::DependencyProperty^ BrushSizeProperty
        {
            Windows::UI::Xaml::DependencyProperty^ get() { return m_brushSizeProperty; }
        }
        property Windows::Foundation::Size BrushSize
        {
            // Additionally keep a local value for thread-safe property access.
            Windows::Foundation::Size get() { return m_brushSize; }
            void set(Windows::Foundation::Size value)
            {
                m_brushSize = value;
                SetValue(BrushSizeProperty, value);
            }
        }

        static property Windows::UI::Xaml::DependencyProperty^ BrushIsEraserProperty
        {
            Windows::UI::Xaml::DependencyProperty^ get() { return m_brushIsEraserProperty; }
        }
        property bool BrushIsEraser
        {
            // Keep a local value for thread-safe property reads
            bool get() { return m_brushIsEraser; }
            void set(bool value)
            {
                m_brushIsEraser = value;
                SetValue(BrushIsEraserProperty, value);
            }
        }
#pragma endregion 

        property bool HasContent
        {
            bool get() { return (m_inkManager->GetStrokes()->Size > 0); }
        }

        // Raised when handwriting recognition results have been updated.
        event RecognitionResultUpdatedEventHandler^ RecognitionResultsUpdated;

        void StartProcessingInput();
        void StopProcessingInput();        

        void Update();

        Windows::Foundation::IAsyncAction^ SaveStrokesToStreamAsync(Windows::Storage::Streams::IRandomAccessStream^ stream);
        Windows::Foundation::IAsyncAction^ LoadStrokesFromStreamAsync(Windows::Storage::Streams::IRandomAccessStream^ stream);
        void BeginStrokesReplayFromStream(Windows::Storage::Streams::IRandomAccessStream^ stream, int intervalInMilliseconds);
        void StopStrokesReplay();        

    private protected:
        enum DrawingState {
            Uninitialized = 0,
            None,
            Inking,
            Erasing,
            Replaying
        };

        virtual void Render() override;
        virtual void CreateDeviceResources() override;
        virtual void CreateSizeDependentResources() override;        

        virtual void OnDeviceLost() override;
        virtual void OnSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e) override;
        virtual void OnCompositionScaleChanged(Windows::UI::Xaml::Controls::SwapChainPanel ^sender, Platform::Object ^args) override;
        virtual void OnResuming(Platform::Object^ sender, Platform::Object^ args) override;

        void OnPointerPressed(Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e);
        void OnPointerMoved(Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e);
        void OnPointerReleased(Platform::Object^ sender, Windows::UI::Core::PointerEventArgs^ e);

        void RenderActiveStroke(Windows::UI::Input::PointerPoint^ newPoint);
        void RenderCompletedStrokes(unsigned int strokeCount);
        inline void RenderCompletedStrokes() { RenderCompletedStrokes(m_inkManager->GetStrokes()->Size); }

        void ConvertStrokeToGeometry(Windows::UI::Input::Inking::InkStroke^ stroke, unsigned int segmentCount, ID2D1PathGeometry** geometry);
        inline void ConvertStrokeToGeometry(Windows::UI::Input::Inking::InkStroke^ stroke, ID2D1PathGeometry** geometry) { ConvertStrokeToGeometry(stroke, stroke->GetRenderingSegments()->Size, geometry); }

        // DependencyProperties
        static void BrushColorValueChanged(Windows::UI::Xaml::DependencyObject^ sender, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e);
        static void BrushFitsToCurveValueChanged(Windows::UI::Xaml::DependencyObject^ sender, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e);
        static void BrushSizeValueChanged(Windows::UI::Xaml::DependencyObject^ sender, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e);
        static void BrushIsEraserValueChanged(Windows::UI::Xaml::DependencyObject^ sender, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e);

        static Windows::UI::Xaml::DependencyProperty^                       m_brushColorProperty;
        static Windows::UI::Xaml::DependencyProperty^                       m_brushFitsToCurveProperty;
        static Windows::UI::Xaml::DependencyProperty^                       m_brushSizeProperty;
        Windows::Foundation::Size											m_brushSize;
        static Windows::UI::Xaml::DependencyProperty^                       m_brushIsEraserProperty;
        bool																m_brushIsEraser;
        bool                                                                m_brushFitsToCurve;
        Windows::UI::Color                                                  m_brushColor;

        DrawingState                                                        m_drawingState;

        Windows::UI::Core::CoreIndependentInputSource^						m_coreInput;
        Windows::Foundation::IAsyncAction^									m_inputLoopWorker;

        Microsoft::WRL::ComPtr<ID3D11Texture2D>                             m_currentBuffer;
        Microsoft::WRL::ComPtr<ID3D11Texture2D>                             m_previousBuffer;

        Microsoft::WRL::ComPtr<ID2D1StrokeStyle>                            m_inkStrokeStyle;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>                        m_strokeBrush;

        Windows::UI::Input::Inking::InkManager^								m_inkManager;
        Windows::UI::Input::Inking::InkDrawingAttributes^					m_inkDrawingAttributes;
        Windows::Foundation::Point                                          m_previousPoint;
        unsigned int                                                        m_activePointerId;

        Windows::System::Threading::ThreadPoolTimer^                        m_replayTimer;
        unsigned int                                                        m_currentStrokeIndex;
        unsigned int                                                        m_currentStrokeSegmentIndex;
    };
}