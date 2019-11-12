#include "pch.h"

#include "FrameworkView.h"
#include "VideoView.h"

using namespace Windows::ApplicationModel::Core;
using namespace Windows::UI::Core;
using namespace Windows::ApplicationModel::Activation;

FrameworkView::FrameworkView()
{
    m_activationEntryPoint = ActivationEntryPoint::Unknown;
}

void FrameworkView::Initialize(
    _In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView
    )
{
    m_applicationView = applicationView;
}

// this method is called after Initialize
void FrameworkView::SetWindow(
    _In_ Windows::UI::Core::CoreWindow^ window
    )
{
    m_window = window;
}

void FrameworkView::Load(Platform::String^ entryPoint )
{
    if (entryPoint == "MEPlaybackNative.App")
    {
        m_activationEntryPoint = ActivationEntryPoint::MEPlaybackNative;
    }
}

// this method is called after Load
void FrameworkView::Run()
{
    if (m_activationEntryPoint == ActivationEntryPoint::MEPlaybackNative)
    {
        auto view = ref new VideoView();
        view->Initialize(m_window, m_applicationView);
        view->Run();

        // Must delete the view explicitly in order to break a circular dependency
        // between View and CoreWindow. View holds on to a CoreWindow reference most
        // typically for window activation, while CoreWindow refers back to View when
        // event handlers are hooked up. Without breaking this circular dependency,
        // neither View nor CoreWindow object gets to clean up. It's also important
        // to note that a 'delete' call on a ref class instance simply means calling
        // into a class destructor in order to explicitly break a cycle. It doesn't
        // actually deallocate any memory.
        delete view;
    }
    else
    {
        MEDIA::ThrowIfFailed(E_UNEXPECTED);
    }
}

void FrameworkView::Uninitialize()
{
}