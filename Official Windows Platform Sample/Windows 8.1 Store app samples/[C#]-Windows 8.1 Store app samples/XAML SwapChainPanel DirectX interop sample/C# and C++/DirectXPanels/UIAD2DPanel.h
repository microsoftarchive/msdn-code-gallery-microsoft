//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once
#include "pch.h"
#include "DirectXPanelBase.h"
#include "StepTimer.h"
#include "ShaderStructures.h"

using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Automation::Peers;

namespace DirectXPanels
{

#pragma region Panel and Panel Item Peers
    ref class UIAD2DPanel;

    // A UI Automation peer for a DirectX content element drawn inside a UIAD2DPanel.    
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class UIAD2DPanelItemPeer sealed : AutomationPeer
    {
    public:
        UIAD2DPanelItemPeer(UIAD2DPanel^ owner);

    protected:
        // Implement standard AutomationPeer methods to ensure the element is reachable via UI Automation.
        virtual Windows::Foundation::Rect GetBoundingRectangleCore() override;        
        virtual Windows::Foundation::Point GetClickablePointCore() override;
        virtual bool IsContentElementCore() override    { return true; }
        virtual bool IsControlElementCore() override    { return true; }
        virtual bool IsEnabledCore() override           { return true; }

        virtual AutomationControlType GetAutomationControlTypeCore() override
        {
            return AutomationControlType::Custom;
        }

        virtual Platform::String^ GetLocalizedControlTypeCore() override
        {
            return "UIAPanelItem";
        }

        virtual Platform::String^ GetClassNameCore() override
        {
            return "UIAD2DPanelItemPeer";
        }

        virtual Platform::String^ GetNameCore() override
        {
            return "UIAD2DPanelItem";
        }

    private protected:        
        UIAD2DPanel^                    m_owner;
    };

    // A UI Automation peer for UIAD2DPanel. It exposes both XAML children and custom DirectX content as child peers.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class UIAD2DPanelPeer sealed : FrameworkElementAutomationPeer
    {
    public:
        UIAD2DPanelPeer(UIAD2DPanel^ owner);

    protected:
        virtual IVector<AutomationPeer^>^ GetChildrenCore() override;
        virtual AutomationPeer^ GetPeerFromPointCore(Windows::Foundation::Point point) override;

        virtual Object^ GetPatternCore(PatternInterface patternInterface) override
        {
            if (patternInterface == PatternInterface::Grid)
            {
                return this;
            }
            return nullptr;
        }                

        virtual AutomationControlType GetAutomationControlTypeCore() override
        {
            return AutomationControlType::Pane;
        }

        virtual Platform::String^ GetLocalizedControlTypeCore() override
        {
            return "UIAD2DPanel";
        }

        virtual Platform::String^ GetClassNameCore() override
        {
            return "UIAD2DPanelPeer";
        }
    private protected:
        UIAD2DPanelItemPeer^            m_childPeer;

    };
#pragma endregion

    // Hosts a DirectX rendering surface that draws a rectangle using Direct2D.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class UIAD2DPanel sealed : public DirectXPanels::DirectXPanelBase
    {
    public:
        UIAD2DPanel();

    protected:
        // Returns a custom automation peer.
        virtual Windows::UI::Xaml::Automation::Peers::AutomationPeer^ OnCreateAutomationPeer() override
        {            
            return ref new UIAD2DPanelPeer(this);
        }

    internal:
        // Returns the bounds of the content element in global screen-space coordinates.
        property Windows::Foundation::Rect ContentGlobalBoundingRect
        {
            Windows::Foundation::Rect get()
            {
                // Transform child DirectX content bounds into screen-space coordinates.
                Windows::Foundation::Rect localBounds = Windows::Foundation::Rect(
                    m_contentRect.left,
                    m_contentRect.top,
                    m_contentRect.right - m_contentRect.left,
                    m_contentRect.bottom - m_contentRect.top);

                Windows::UI::Xaml::Media::GeneralTransform^ rootTransform = TransformToVisual(nullptr);

                return rootTransform->TransformBounds(localBounds);
            }
        }

    private protected:
        virtual void Render() override;
        virtual void CreateDeviceResources() override;

        virtual void OnDeviceLost() override;
        virtual void OnSizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e) override;
        virtual void OnCompositionScaleChanged(Windows::UI::Xaml::Controls::SwapChainPanel ^sender, Platform::Object ^args) override;
        virtual void OnResuming(Platform::Object^ sender, Platform::Object^ args) override;        

        D2D1_RECT_F                                             m_contentRect;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>            m_strokeBrush;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>            m_fillBrush;        
    };    
}
