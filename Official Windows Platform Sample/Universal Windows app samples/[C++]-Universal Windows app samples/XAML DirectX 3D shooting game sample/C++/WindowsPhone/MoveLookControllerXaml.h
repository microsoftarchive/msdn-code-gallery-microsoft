//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

//// *** Windows Phone Version ***

#pragma once

// MoveLookControllerXaml:
// This class derives from the MoveLookController abstract class.
// It implements the constructor method to connect the event handlers to the
// CoreWindow object.
// For the Windows Phone specific implementation it only connects pointer events,
// and a hardware back button event handler.  It does not include any
// support for polled devices.
//

#include "MoveLookController.h"

ref class MoveLookControllerXaml : public MoveLookController
{
internal:
    static MoveLookController^ Create(
        _In_ Windows::UI::Core::CoreWindow^ window,
        _In_ Windows::UI::Core::CoreDispatcher^ dispatcher
        );

private:
    MoveLookControllerXaml(
        _In_ Windows::UI::Core::CoreWindow^ window
        );

    void OnHardwareBackButtonPressed(
        _In_ Platform::Object^ sender,
        Windows::Phone::UI::Input::BackPressedEventArgs ^args
        );
};
