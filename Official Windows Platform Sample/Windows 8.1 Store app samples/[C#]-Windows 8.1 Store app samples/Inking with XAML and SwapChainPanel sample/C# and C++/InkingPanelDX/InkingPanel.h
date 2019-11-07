//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "DirectXPanelBase.h"

#pragma warning (disable: 4451) // warns about possible invalid marshaling of objects across
                                // contexts, however we don't need to worry about this because
                                // the classes in the Inking API are INoMarshal

namespace InkingPanelDX
{
    // Delegate for the RecognitionResultUpdated event.
    [Windows::Foundation::Metadata::WebHostHidden]
    public delegate void InkAnswerUpdatedEventHandler(
        Platform::Object^ sender, 
        Platform::String^ answer);

    // Processes and renders mouse and pen input on a delegate thread.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class InkingPanel sealed : public DirectXPanelBase
    {
    public:
        InkingPanel();

        event InkAnswerUpdatedEventHandler^ InkAnswerUpdated;

        void ProcessInputOnDelegateThread();
        void Clear();

    protected private:
        void OnClear();
        void OnPointerPressed(
            Platform::Object^ sender,
            Windows::UI::Core::PointerEventArgs^ e);
        void OnPointerMoved(
            Platform::Object^ sender,
            Windows::UI::Core::PointerEventArgs^ e);
        void OnPointerReleased(
            Platform::Object^ sender,
            Windows::UI::Core::PointerEventArgs^ e);
        void OnRecognize();
        void OnRender();

        void CreateStrokeGeometry(
            Windows::UI::Input::Inking::InkStroke^ stroke, 
            ID2D1PathGeometry** geometry);
        void RenderStrokes();

        bool IsErase(Windows::UI::Core::PointerEventArgs^ e);
        bool IsInk(Windows::UI::Core::PointerEventArgs^ e);

#pragma region DirectXPanelBase overrides
        virtual void CreateDeviceIndependentResources() override;
        virtual void CreateDeviceResources() override;
        virtual void CreateSizeDependentResources() override;
        virtual void ReleaseSizeDependentResources() override;

        virtual void OnSizeChanged(
            Platform::Object^ sender, 
            Windows::UI::Xaml::SizeChangedEventArgs^ e) override;
        virtual void OnCompositionScaleChanged(
            Windows::UI::Xaml::Controls::SwapChainPanel ^sender, 
            Platform::Object ^args) override;
        virtual void OnSuspending(
            Platform::Object^ sender, 
            Windows::ApplicationModel::SuspendingEventArgs^ e) override;

        virtual void Render() override;
        virtual void SetSwapChain() override;
#pragma endregion

    protected private:
        Windows::UI::Core::CoreIndependentInputSource^ m_coreInput;
        Windows::UI::Input::Inking::InkManager^ m_inkManager;

        D2D1_COLOR_F m_backgroundColor;
        D2D1_COLOR_F m_strokeColor;
        float m_strokeSize;

        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_currentBuffer;
        Microsoft::WRL::ComPtr<ID3D11Texture2D> m_previousBuffer;
        Microsoft::WRL::ComPtr<ID2D1StrokeStyle> m_inkStrokeStyle;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_strokeBrush;

        // Stores the id of the 'active' pointer, -1 if none.
        // Set by OnPointerPressed, checked by OnPointerMoved, and reset by OnPointerReleased.
        int m_activePointerId;

#ifdef _DEBUG
    protected private:
        DWORD m_coreInputThreadId;
#endif
    };
}

#pragma warning (default: 4451)