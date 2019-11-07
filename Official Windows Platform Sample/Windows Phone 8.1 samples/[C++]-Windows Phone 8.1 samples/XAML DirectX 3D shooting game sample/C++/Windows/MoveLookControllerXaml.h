//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//// *** Windows Version ***

#pragma once

// MoveLookControllerXaml:
// This class derives from the MoveLookController abstract class.
// It implements the constructor to connect the event handlers to the
// CoreWindow object.
// For the Windows specific implementation it includes support for both
// relative mouse movement and XBox Controller input via XInput.
// NOTE: enabling and disabling the cursor glyphs is handled in DirectXPage.xaml.cpp
// because the game is running on a separate thread that does not have access to
// the CoreWindow.  The DirectXPage.xaml.cpp HideCursor and ShowCursor marshall to
// the UI thread where Xaml is running with the CoreWindow.
//
// The following controls are mapped from the Xbox controller using XInput:
//     XINPUT_GAMEPAD_START - is mapped to IsPressComplete in the WaitForInput state and
//         IsPauseRequested in the Active state.
//     Left Thumb stick - is mapped to the move control in Active mode
//     Right Thumb stick - is mapped to the look control in Active mode
//     Right Trigger - is mapped to IsFiring in Active mode or "Fire" button.
//

#include "MoveLookController.h"
#include <XInput.h>

ref class MoveLookControllerXaml : public MoveLookController
{
internal:
    static MoveLookController^ Create(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::CoreDispatcher^ dispatcher
        );

private:
    MoveLookControllerXaml(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::CoreDispatcher^ dispatcher
        );

protected:
    virtual void ResetState() override;
    virtual void UpdatePollingDevices() override;
    virtual bool PollingFireInUse() override { return m_xinputTriggerInUse; };
    virtual void ShowCursor() override;
    virtual void HideCursor() override;

protected private:
    // Cached pointer to a dispatcher to marshal execution back to the Xaml UI thread.
    Windows::UI::Core::CoreDispatcher^ m_dispatcher;

    // Xbox Input related members.
    bool                        m_isControllerConnected;  // Is the Xbox controller connected.
    XINPUT_CAPABILITIES         m_xinputCaps;             // Capabilites of the controller.
    XINPUT_STATE                m_xinputState;            // The current state of the controller.
    bool                        m_xinputStartButtonInUse;
    bool                        m_xinputTriggerInUse;
};
