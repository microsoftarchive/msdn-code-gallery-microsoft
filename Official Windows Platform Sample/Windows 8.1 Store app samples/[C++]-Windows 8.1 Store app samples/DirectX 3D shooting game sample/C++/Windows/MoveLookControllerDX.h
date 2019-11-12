//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//// *** Windows Version ***

#pragma once

// MoveLookControllerDX:
// This class derives from the MoveLookController abstract class.
// It implements the constructor method to connect the event handlers to the
// CoreWindow object.
// For the Windows specific implementation it includes support for both
// relative mouse movement and XBox Controller input via XInput.
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

ref class MoveLookControllerDX : public MoveLookController
{
internal:
    static MoveLookController^ Create(
        _In_ Windows::UI::Core::CoreWindow^ window
        );

private:
    MoveLookControllerDX(
        _In_ Windows::UI::Core::CoreWindow^ window
        );

protected:
    virtual void ResetState() override;
    virtual void ShowCursor() override;
    virtual void HideCursor() override;
    virtual void UpdatePollingDevices() override;
    virtual bool PollingFireInUse() override { return m_xinputTriggerInUse; };

protected private:
    // Xbox Input related members.
    bool                        m_isControllerConnected;  // Is the Xbox controller connected.
    XINPUT_CAPABILITIES         m_xinputCaps;             // Capabilites of the controller.
    XINPUT_STATE                m_xinputState;            // The current state of the controller.
    bool                        m_xinputStartButtonInUse;
    bool                        m_xinputTriggerInUse;
};
