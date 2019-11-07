#pragma once
#include <agile.h>

#include <agile.h>

enum class ActivationEntryPoint
{
    Unknown,
    MEPlaybackNative
};

ref class FrameworkView sealed : public Windows::ApplicationModel::Core::IFrameworkView
{
public:
    FrameworkView();

    virtual void Initialize(_In_ Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
    virtual void SetWindow(_In_ Windows::UI::Core::CoreWindow^ window);
    virtual void Load(Platform::String^ entryPoint);
    virtual void Run();
    virtual void Uninitialize();

private:
    ActivationEntryPoint m_activationEntryPoint;
    Platform::Agile<Windows::UI::Core::CoreWindow> m_window;
    Platform::Agile<Windows::ApplicationModel::Core::CoreApplicationView> m_applicationView;
};

ref class FrameworkViewSource sealed : Windows::ApplicationModel::Core::IFrameworkViewSource 
{
public:
    FrameworkViewSource() {}
    virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView()
    {
        return ref new FrameworkView();
    }
};
