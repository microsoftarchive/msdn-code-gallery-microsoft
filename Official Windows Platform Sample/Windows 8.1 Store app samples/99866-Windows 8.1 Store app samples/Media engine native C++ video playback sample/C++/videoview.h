#pragma once

#include "MEPlayer.h"

ref class VideoView
{
internal:
    VideoView();
    
    void Initialize(
        _In_ Platform::Agile<Windows::UI::Core::CoreWindow> window,
        _In_ Platform::Agile<Windows::ApplicationModel::Core::CoreApplicationView> applicationView
        );

    // Declare Event Handlers
    void OnWindowSizeChanged(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
        );

    void OnPointerPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender, 
        _In_ Windows::UI::Core::PointerEventArgs^ args
        );

    void OnKeyPressed(
        _In_ Windows::UI::Core::CoreWindow^ sender,
        _In_ Windows::UI::Core::KeyEventArgs^ args
        );

	void OnResuming(
        _In_ Platform::Object^ sender, 
        _In_ Platform::Object^ args);

    void OnSuspending(
        _In_ Platform::Object^ sender, 
        _In_ Windows::ApplicationModel::SuspendingEventArgs^ args);

    void Run();	

private:

    // Basics
    Platform::Agile<Windows::UI::Core::CoreWindow> m_window;
    Platform::Agile<Windows::ApplicationModel::Core::CoreApplicationView> m_applicationView;

    // MediaEngine Player
    MEPlayer^ m_player;
};