//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace GameController
{
    public value struct State
    {
        uint32 controllerId;
        uint32 packetNumber;

        bool connected;
        bool a;
        bool b;
        bool x;
        bool y;
        bool dpad_up;
        bool dpad_down;
        bool dpad_left;
        bool dpad_right;
        bool left_thumb;
        bool right_thumb;
        bool left_shoulder;
        bool right_shoulder;
        bool start;
        bool back;

        BYTE LeftTrigger;
        BYTE RightTrigger;
        SHORT LeftThumbX;
        SHORT LeftThumbY;
        SHORT RightThumbX;
        SHORT RightThumbY;
    };

    public ref class Controller sealed
    {
        ~Controller();

        uint32                  m_index;
        bool                    m_isControllerConnected;  // Do we have a controller connected
        XINPUT_CAPABILITIES     m_xinputCaps;             // Capabilites of the controller
        XINPUT_STATE            m_xinputState;            // The current state of the controller
        uint64                  m_lastEnumTime;           // Last time a new controller connection was checked

    public:
        Controller(uint32 index);

        void SetState(uint16 leftSpeed, uint16 rightSpeed);
        State GetState();
    };
}
